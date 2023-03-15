using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using MauiMimikakiApp.CustomViews;
using TakeMauiEasy;
using MauiMimikakiApp.Models;
using MauiMimikakiApp.Drawables;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace MauiMimikakiApp.ViewModels;

internal partial class TrackableMimiViewModel : ObservableObject
{

    [ObservableProperty] MimiRegionDrawable topRegionDrawable;
    [ObservableProperty] MimiRegionDrawable centerRegionDrawable;
    [ObservableProperty] MimiRegionDrawable bottomRegionDrawable;

    //Dictionary<string, Geometry> _geometryDict;

    static int dx = 2;
    static int dy = 2;

    readonly MimiModel _mimi;

    internal Action<PositionTrackerState> OnMoveOnMimi;

    internal TrackableMimiViewModel(Geometry mimiTop, Geometry mimiCenter, Geometry mimiBottom)
    {
        var topRegionPath = new Path(mimiTop);
        var centerRegionPath = new Path(mimiCenter);
        var bottomRegionPath = new Path(mimiBottom);

        var topInternalRegion = new InternalRegion(topRegionPath.GetPath(), dx, dy);
        var centerInternalRegion = new InternalRegion(centerRegionPath.GetPath(), dx, dy);
        var bottomInternalRegion = new InternalRegion(bottomRegionPath.GetPath(), dx, dy);
        
        _mimi = new(topInternalRegion, centerInternalRegion, bottomInternalRegion);

        TopRegionDrawable = new MimiRegionDrawable(_mimi.Top);
        CenterRegionDrawable = new MimiRegionDrawable(_mimi.Center);
        BottomRegionDrawable = new MimiRegionDrawable(_mimi.Bottom);
    
        //DoSomething();
    }

    // async void DoSomething()
    // {
    //     while(true)
    //     {
    //         foreach(var hair in _mimi.Top.Hairs)
    //         {
    //             hair.Displace(new Point(5, 5));
    //             await Task.Delay(100);

    //             //TopRegion.Invalidate();
    //         }

    //     }
    // }

    
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

