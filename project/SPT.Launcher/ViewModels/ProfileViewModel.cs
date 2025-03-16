using SPT.Launcher.Helpers;
using SPT.Launcher.MiniCommon;
using SPT.Launcher.Models;
using SPT.Launcher.Models.Launcher;
using SPT.Launcher.Controllers;
using Avalonia;
using ReactiveUI;
using System.Threading.Tasks;
using SPT.Launcher.Attributes;
using SPT.Launcher.ViewModels.Dialogs;
using Avalonia.Threading;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;
using SPT.Launcher.Models.SPT;
using System.Linq;
using SPT.Launcher.Models.Fika;
using Splat.ModeDetection;

namespace SPT.Launcher.ViewModels
{
    [RequireLoggedIn]
    public class ProfileViewModel : ViewModelBase
    {
        private string _CurrentEdition;
        public string CurrentEdition
        {
            get => _CurrentEdition;
            set => this.RaiseAndSetIfChanged(ref _CurrentEdition, value);
        }

        private bool _WipeProfileOnStart;
        public bool WipeProfileOnStart
        {
            get => _WipeProfileOnStart;
            set => this.RaiseAndSetIfChanged(ref _WipeProfileOnStart, value);
        }

        private bool _ProfileWipePending;
        public bool ProfileWipePending
        {
            get => _ProfileWipePending;
            set => this.RaiseAndSetIfChanged(ref _ProfileWipePending, value);
        }

        private int _PlayersOnline;
        public int PlayersOnline
        {
            get => _PlayersOnline;
            set => this.RaiseAndSetIfChanged(ref _PlayersOnline, value);
        }

        private string _dediAvailabilityText;
        public string DediAvailabilityText
        {
            get => _dediAvailabilityText;
            set => this.RaiseAndSetIfChanged(ref _dediAvailabilityText, value);
        }

        private string _dediAvailabilityColor;
        public string DediAvailabilityColor
        {
            get => _dediAvailabilityColor;
            set => this.RaiseAndSetIfChanged(ref _dediAvailabilityColor, value);
        }

        public string CurrentId { get; set; }

        public ProfileInfo ProfileInfo { get; set; } = AccountManager.SelectedProfileInfo;

        public ImageHelper SideImage { get; } = new ImageHelper();

        public ModInfoCollection ModInfoCollection { get; set; } = new ModInfoCollection();

        private readonly GameStarter _gameStarter = new GameStarter(new GameStarterFrontend());

        private readonly ProcessMonitor _monitor;

        public enum EDedicatedStatus
        {
            READY = 1,
            IN_RAID = 2,
        }

        public ProfileViewModel(IScreen Host) : base(Host)
        {
            // cache and load side image if profile has a side
            if (AccountManager.SelectedProfileInfo != null && AccountManager.SelectedProfileInfo.Side != null)
            {
                ImageRequest.CacheSideImage(AccountManager.SelectedProfileInfo.Side);
                SideImage.Path = AccountManager.SelectedProfileInfo.SideImage;
                SideImage.Touch();
            }

            _monitor = new ProcessMonitor("EscapeFromTarkov", 1000, aliveCallback: GameAliveCallBack, exitCallback: GameExitCallback);

            CurrentEdition = AccountManager.SelectedAccount.edition;

            CurrentId = AccountManager.SelectedAccount.id;

            // Initialize dedicated server status
            FikaDedicatedData dediData = FikaController.GetDedicatedData();
            DediAvailabilityText = dediData.Available == Models.Fika.EDedicatedStatus.READY ? "Available" : (dediData.Available == Models.Fika.EDedicatedStatus.IN_RAID ? "In Raid" : "Unavailable");
            DediAvailabilityColor = dediData.Available == Models.Fika.EDedicatedStatus.READY ? "Green" : (dediData.Available == Models.Fika.EDedicatedStatus.IN_RAID ? "Orange" : "Red");

            // Initialize player count
            try
            {
                PlayersOnline = FikaController.GetOnlinePlayers().Length;
            }
            catch
            {
                PlayersOnline = 0;
            }
        }

        private async Task GameVersionCheck()
        {
            string compatibleGameVersion = ServerManager.GetCompatibleGameVersion();

            if (compatibleGameVersion == "") return;

            // get the product version of the exe
            string gameVersion = FileVersionInfo.GetVersionInfo(Path.Join(LauncherSettingsProvider.Instance.GamePath, "EscapeFromTarkov.exe")).FileVersion;

            if (gameVersion == null) return;

            // if the compatible version isn't the same as the game version show a warning dialog
            if(compatibleGameVersion != gameVersion)
            {
                WarningDialogViewModel warning = new WarningDialogViewModel(null, string.Format(LocalizationProvider.Instance.game_version_mismatch_format_2, gameVersion, compatibleGameVersion), LocalizationProvider.Instance.i_understand);
                Dispatcher.UIThread.InvokeAsync(async() =>
                {
                    await ShowDialog(warning);
                });
            }
        }

        public void OpenModsInfoCommand() =>
            NavigateTo(new ModInfoViewModel(HostScreen, ModInfoCollection));

        public void LogoutCommand()
        {
            AccountManager.Logout();

            NavigateTo(new ConnectServerViewModel(HostScreen, true));
        }

        public void ChangeWindowState(Avalonia.Controls.WindowState? State, bool Close = false)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    if (Close)
                    {
                        desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose;
                        desktop.Shutdown();
                    }
                    else
                    {
                        desktop.MainWindow.WindowState = State ?? Avalonia.Controls.WindowState.Normal;
                    }
                }
            });
        }

        public async Task StartGameCommand()
        {
            LauncherSettingsProvider.Instance.AllowSettings = false;

            AccountStatus status = await AccountManager.LoginAsync(AccountManager.SelectedAccount.username, AccountManager.SelectedAccount.password);

            LauncherSettingsProvider.Instance.AllowSettings = true;

            switch (status)
            {
                case AccountStatus.NoConnection:
                    NavigateTo(new ConnectServerViewModel(HostScreen));
                    return;
            }

            LauncherSettingsProvider.Instance.GameRunning = true;

            if (WipeProfileOnStart)
            {
                var wipeStatus = await WipeProfile(AccountManager.SelectedAccount.edition);

                if (wipeStatus != AccountStatus.OK)
                {
                    LauncherSettingsProvider.Instance.GameRunning = false;
                    return;
                }

                WipeProfileOnStart = false;
            }

            GameStarterResult gameStartResult = await _gameStarter.LaunchGame(ServerManager.SelectedServer, AccountManager.SelectedAccount, LauncherSettingsProvider.Instance.GamePath);

            if (gameStartResult.Succeeded)
            {
                _monitor.Start();

                switch (LauncherSettingsProvider.Instance.LauncherStartGameAction)
                {
                    case LauncherAction.MinimizeAction:
                        {
                            ChangeWindowState(Avalonia.Controls.WindowState.Minimized);
                            break;
                        }
                    case LauncherAction.ExitAction:
                        {
                            ChangeWindowState(null, true);
                            break;
                        }
                }
            }
            else
            {
                SendNotification("", gameStartResult.Message, Avalonia.Controls.Notifications.NotificationType.Error);
                LauncherSettingsProvider.Instance.GameRunning = false;
            }
        }

        private async Task<AccountStatus> WipeProfile(string edition)
        {
            AccountStatus status = await AccountManager.WipeAsync(edition);

            switch (status)
            {
                case AccountStatus.OK:
                    {
                        ProfileWipePending = true;
                        CurrentEdition = AccountManager.SelectedAccount.edition;
                        SendNotification("", LocalizationProvider.Instance.account_updated);
                        break;
                    }
                case AccountStatus.NoConnection:
                    {
                        NavigateTo(new ConnectServerViewModel(HostScreen));
                        break;
                    }
                default:
                    {
                        SendNotification("", LocalizationProvider.Instance.edit_account_update_error);
                        break;
                    }
            }

            return status;
        }

        public async Task ChangeEditionCommand()
        {
            var result = await ShowDialog(new ChangeEditionDialogViewModel(null));

            if(result != null && result is SPTEdition edition)
            {
                await WipeProfile(edition.Name);
            }
        }

        public async Task CopyCommand(object parameter)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && parameter is string text)
            {
                if (desktop?.MainWindow?.Clipboard == null)
                {
                    return;
                }
                
                await desktop.MainWindow.Clipboard.SetTextAsync(text);
                SendNotification("", $"{text} {LocalizationProvider.Instance.copied}", Avalonia.Controls.Notifications.NotificationType.Success);
            }
        }

        public async Task RemoveProfileCommand()
        {
            ConfirmationDialogViewModel confirmation = new ConfirmationDialogViewModel(null, string.Format(LocalizationProvider.Instance.profile_remove_question_format_1, AccountManager.SelectedAccount.username));

            var result = await ShowDialog(confirmation);

            if (result is bool b && !b) return;

            AccountStatus status = await AccountManager.RemoveAsync();

            switch(status)
            {
                case AccountStatus.OK:
                    {
                        SendNotification("", LocalizationProvider.Instance.profile_removed);

                        LauncherSettingsProvider.Instance.Server.AutoLoginCreds = null;

                        LauncherSettingsProvider.Instance.SaveSettings();

                        NavigateTo(new ConnectServerViewModel(HostScreen));
                        break;
                    }
                case AccountStatus.UpdateFailed:
                    {
                        SendNotification("", LocalizationProvider.Instance.profile_removal_failed);
                        break;
                    }
                case AccountStatus.NoConnection:
                    {
                        SendNotification("", LocalizationProvider.Instance.no_servers_available);
                        NavigateTo(new ConnectServerViewModel(HostScreen));
                        break;
                    }
            }
        }

        public async Task UpdateOnlinePlayersCommand()
        {
            FikaPlayer[] players = FikaController.GetOnlinePlayers();

            // Update the PlayersOnline property with the count
            PlayersOnline = players.Length;

            if (players.Length == 0)
            {
                SendNotification("Fika", "No players online");
                return;
            }

            SendNotification("Fika", $"{PlayersOnline} {(PlayersOnline == 1 ? "player" : "players")} online");

            // Debug output for each player (optional)
            foreach (var player in players)
            {
                Debug.WriteLine(player.nickname);
            }
        }
        private void UpdateProfileInfo()
        {
            AccountManager.UpdateProfileInfo();
            ImageRequest.CacheSideImage(AccountManager.SelectedProfileInfo.Side);
            ProfileInfo.UpdateDisplayedProfile(AccountManager.SelectedProfileInfo);
            if (ProfileInfo.SideImage != SideImage.Path)
            {
                SideImage.Path = ProfileInfo.SideImage;
                SideImage.Touch();
            }
        }


        //pull profile every x seconds
        private int aliveCallBackCountdown = 60;
        private void GameAliveCallBack(ProcessMonitor monitor)
        {
            aliveCallBackCountdown--;

            if (aliveCallBackCountdown <= 0)
            {
                aliveCallBackCountdown = 60;
                UpdateProfileInfo();
            }
        }

        private void GameExitCallback(ProcessMonitor monitor)
        {
            monitor.Stop();

            LauncherSettingsProvider.Instance.GameRunning = false;

            //Make sure the call to MainWindow happens on the UI thread.
            switch (LauncherSettingsProvider.Instance.LauncherStartGameAction)
            {
                case LauncherAction.MinimizeAction:
                    {
                        ChangeWindowState(Avalonia.Controls.WindowState.Normal);
                        ProfileWipePending = false;

                        break;
                    }
            }

            UpdateProfileInfo();
        }
    }
}
