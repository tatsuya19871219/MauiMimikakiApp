using CommunityToolkit.Mvvm.ComponentModel;
using MauiMimikakiApp.Models;

namespace MauiMimikakiApp.Drawables;

public partial class MimiRegionViewBox : ObservableObject
{
    [ObservableProperty] double _regionWidthRequest;
    [ObservableProperty] double _regionHeightRequest;
    [ObservableProperty] double _regionOffsetX;
    [ObservableProperty] double _regionOffsetY;

    internal MimiRegionViewBox(MimiRegion mimiRegion, double padding = 0)
    {
        var bounds = mimiRegion.Bounds;

        RegionOffsetX = bounds.Left - padding;
        RegionOffsetY = bounds.Top - padding;

        RegionWidthRequest = bounds.Width + 2*padding;
        RegionHeightRequest = bounds.Height + 2*padding;
    }
}