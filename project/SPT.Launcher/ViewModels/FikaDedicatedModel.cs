using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;
using SPT.Launcher;
using SPT.Launcher.Controllers;
using SPT.Launcher.CustomControls;
using SPT.Launcher.MiniCommon;
using SPT.Launcher.Models.Fika;
using SPT.Launcher.ViewModels;

namespace MyApp
{
    public class FikaDedicatedModel : ViewModelBase
    {

        public string DediStatus { get; set; }
        public string DediAvailability { get; set; }

        public FikaDedicatedModel(IScreen Host) : base(Host)
        {
            DediStatus = "test";
            DediAvailability = "test2";

            
        }
    }
}
