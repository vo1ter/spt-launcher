using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SPT.Launcher.Controllers;
using SPT.Launcher.Models.Fika;

namespace SPT.Launcher.CustomControls;

public partial class DedicatedStatus : UserControl
{
    public static readonly StyledProperty<string> DediAvailabilityTextProperty = AvaloniaProperty.Register<DedicatedStatus, string>(
        "DediAvailabilityText");

    public string DediAvailabilityText
    {
        get => GetValue(DediAvailabilityTextProperty);
        set => SetValue(DediAvailabilityTextProperty, value);
    }

    public static readonly StyledProperty<string> DediAvailabilityColorProperty = AvaloniaProperty.Register<DedicatedStatus, string>(
        "DediAvailabilityColor");

    public string DediAvailabilityColor
    {
        get => GetValue(DediAvailabilityColorProperty);
        set => SetValue(DediAvailabilityColorProperty, value);
    }

    public static readonly StyledProperty<ICommand> UpdateDedicatedStatusCommandProperty =
        AvaloniaProperty.Register<DedicatedStatus, ICommand>(nameof(UpdateDedicatedStatusCommand));

    public ICommand UpdateDedicatedStatusCommand
    {
        get => GetValue(UpdateDedicatedStatusCommandProperty);
        set => SetValue(UpdateDedicatedStatusCommandProperty, value);
    }

    public DedicatedStatus()
    {
        InitializeComponent();


        // Initialize with current status if there are no bindings
        if (string.IsNullOrEmpty(DediAvailabilityText))
        {
            var dediData = FikaController.GetDedicatedData();
            bool isDediAvailable = dediData.Available != null;
            DediAvailabilityText = isDediAvailable ? "Available" : "Unavailable";
            DediAvailabilityColor = isDediAvailable ? "Green" : "Red";
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}