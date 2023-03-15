using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using MauiMimikakiApp.CustomViews;
using TakeMauiEasy;
using MauiMimikakiApp.Models;
using MauiMimikakiApp.Drawables;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace MauiMimikakiApp.ViewModels;

public partial class TrackableMimiViewModel : ObservableObject
{

    [ObservableProperty] MimiRegionDrawable topRegionDrawable;
    [ObservableProperty] MimiRegionDrawable centerRegionDrawable;
    [ObservableProperty] MimiRegionDrawable bottomRegionDrawable;

    //Dictionary<string, Geometry> _geometryDict;

    // static int dx = 5;
    // static int dy = 5;

    readonly MimiModel _mimi;

    public Action<PositionTrackerState> OnMoveOnMimi;

    public TrackableMimiViewModel(Geometry mimiTop, Geometry mimiCenter, Geometry mimiBottom)
    {
        var topRegionPath = new Path(mimiTop);
        var centerRegionPath = new Path(mimiCenter);
        var bottomRegionPath = new Path(mimiBottom);

        var topInternalRegion = new InternalRegion(topRegionPath.GetPath());
        var centerInternalRegion = new InternalRegion(centerRegionPath.GetPath());
        var bottomInternalRegion = new InternalRegion(bottomRegionPath.GetPath());
        
        _mimi = new(topInternalRegion, centerInternalRegion, bottomInternalRegion);

        TopRegionDrawable = new MimiRegionDrawable(_mimi.Top);
        CenterRegionDrawable = new MimiRegionDrawable(_mimi.Center);
        BottomRegionDrawable = new MimiRegionDrawable(_mimi.Bottom);
    
    }

    
    internal void InvokeTrackerProcess(int updateInterval = 100)
    {
        if (updateInterval < 0) throw new Exception("Invalid update interval is given. The value should be positive.");

        RunTrackerProcess(updateInterval);
    }

    async void RunTrackerProcess(int updateInterval)
    {
        while (true)
        {
            //OnMoveOnMimi?.Invoke(_tracker.CurrentState);

            // OnMoveOnMimiTop

            await Task.Delay(updateInterval);
        }
    }
}

