using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using MauiMimikakiApp.CustomViews;
using TakeMauiEasy;
using MauiMimikakiApp.Models;
using MauiMimikakiApp.Drawables;
using Path = Microsoft.Maui.Controls.Shapes.Path;
using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;

namespace MauiMimikakiApp.ViewModels;

internal partial class MimikakiViewModel : ObservableObject
{

    [ObservableProperty] MimiRegionDrawable topRegionDrawable;
    [ObservableProperty] MimiRegionDrawable centerRegionDrawable;
    [ObservableProperty] MimiRegionDrawable bottomRegionDrawable;

    static int dx = 2;
    static int dy = 2;

    readonly PositionTracker _tracker;
    //readonly MimiModel _mimi;

    readonly MimiRegion _topRegion;
    readonly MimiRegion _centerRegion;
    readonly MimiRegion _bottomRegion;

    readonly double _displayRatio;

    internal Action<PositionTrackerState> OnMoveOnMimi;

    internal MimikakiViewModel(PositionTracker tracker, Geometry mimiTop, Geometry mimiCenter, Geometry mimiBottom, double displayRatio)
    {
        _tracker = tracker;
        _displayRatio = displayRatio;

        var topRegionPath = new Path(mimiTop);
        var centerRegionPath = new Path(mimiCenter);
        var bottomRegionPath = new Path(mimiBottom);

        var topInternalRegion = new InternalRegion(topRegionPath.GetPath(), dx, dy);
        var centerInternalRegion = new InternalRegion(centerRegionPath.GetPath(), dx, dy);
        var bottomInternalRegion = new InternalRegion(bottomRegionPath.GetPath(), dx, dy);
        
        //_mimi = new(topInternalRegion, centerInternalRegion, bottomInternalRegion);

        _topRegion = new(topInternalRegion);
        _centerRegion = new(centerInternalRegion);
        _bottomRegion = new(bottomInternalRegion);

        TopRegionDrawable = new MimiRegionDrawable(_topRegion);
        CenterRegionDrawable = new MimiRegionDrawable(_centerRegion);
        BottomRegionDrawable = new MimiRegionDrawable(_bottomRegion);
    
        //InvokeTrackerProcess();
        //DoSomething();
    }

    // async void DoSomething()
    // {
    //     while (true)
    //     {
    //         foreach (var hair in _mimi.Top.Hairs)
    //         {
    //             hair.Displace(new Point(5, 5));
    //             await Task.Delay(100);

    //             StrongReferenceMessenger.Default.Send(new DrawMessage("draw"));
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
        IEnumerable<ITrackerListener> listenersOfHair = 
            new[] { _topRegion.Hairs, _centerRegion.Hairs, _bottomRegion.Hairs}
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listenersOfDirt = 
            new[] { _topRegion.Dirts, _centerRegion.Dirts, _bottomRegion.Dirts }
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listeners = listenersOfHair.Concat(listenersOfDirt);

        while (true)
        {
            var current  = _tracker.CurrentState;
            var position = ScaleForDisplayRatio(current.Position);
            var velocity = ScaleForDisplayRatio(current.Velocity);
            double dt = (double)updateInterval/1000;

            StrongReferenceMessenger.Default.Send(new TrackerUpdateMessage(current));

            if (_topRegion.Contains(position) ||
                _centerRegion.Contains(position) ||
                _bottomRegion.Contains(position))
                    OnMoveOnMimi?.Invoke(current);

            foreach (var listener in listeners)
                listener.OnMove(position, velocity, dt);

            // generate dirt
            TryGenerateDirt();    

            StrongReferenceMessenger.Default.Send(new MimiViewInvalidateMessage("draw"));

            // dirt removed action
            CheckDirtRemoved(_topRegion);
            CheckDirtRemoved(_centerRegion);
            CheckDirtRemoved(_bottomRegion);    

            await Task.Delay(updateInterval);
        }
    }

    void TryGenerateDirt()
    {
        Random rnd = new Random();
        if (rnd.NextDouble() > 0.8)
            _topRegion.GenerateMimiDirt();

        if (rnd.NextDouble() > 0.9)
            _centerRegion.GenerateMimiDirt();

        if (rnd.NextDouble() > 0.95)
            _bottomRegion.GenerateMimiDirt();
    }

    void CheckDirtRemoved(MimiRegion region)
    {
        List<MimiDirt> removed = new();

        foreach (var dirt in region.Dirts)
        {
            if (dirt.IsRemoved) removed.Add(dirt);
        }

        foreach (var dirt in removed)
        {
            // create floating dirt view (shape)
            Rectangle rect = new Rectangle {Fill = Colors.Magenta, WidthRequest = dirt.Size, HeightRequest = dirt.Size};

            Point position = dirt.Position;

            var x0 = position.X;
            var y0 = position.Y;

            rect.TranslationX = x0 - dirt.Size/2;
            rect.TranslationY = y0 - dirt.Size/2;

            StrongReferenceMessenger.Default.Send(new FloatingDirtGenerateMessage(rect));
            
            region.RemoveMimiDirt(dirt);
        }
    }

    Point ScaleForDisplayRatio(Point point)
    {
        return new Point(point.X/_displayRatio, point.Y/_displayRatio);
    }
}

