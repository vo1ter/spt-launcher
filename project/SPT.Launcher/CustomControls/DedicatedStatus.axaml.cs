using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MyApp;
using SPT.Launcher.Controllers;
using SPT.Launcher.Models.Fika;

namespace SPT.Launcher.CustomControls;

public partial class DedicatedStatus : UserControl
{
    public static readonly StyledProperty<string> DediAvailabilityTextProperty = AvaloniaProperty.Register<OnlinePlayers, string>(
        "DediAvailabilityText");

    public string DediAvailabilityText
    {
        get => GetValue(DediAvailabilityTextProperty);
        set => SetValue(DediAvailabilityTextProperty, value);
    }

    public static readonly StyledProperty<string> DediAvailabilityColorProperty = AvaloniaProperty.Register<OnlinePlayers, string>(
        "DediAvailabilityColor");

    public string DediAvailabilityColor
    {
        get => GetValue(DediAvailabilityColorProperty);
        set => SetValue(DediAvailabilityColorProperty, value);
    }
    public DedicatedStatus()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}