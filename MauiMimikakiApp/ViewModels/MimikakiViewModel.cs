using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using MauiMimikakiApp.CustomViews;
using TakeMauiEasy;
using MauiMimikakiApp.Models;
using MauiMimikakiApp.Drawables;
using Path = Microsoft.Maui.Controls.Shapes.Path;
using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;
using System.Windows.Input;

namespace MauiMimikakiApp.ViewModels;

internal partial class MimikakiViewModel : ObservableObject
{
    [ObservableProperty] MimiRegionDrawable _outerRegionDrawable;
    [ObservableProperty] MimiRegionDrawable _innerRegionDrawable;
    [ObservableProperty] MimiRegionDrawable _holeRegionDrawable;

    [ObservableProperty] double _viewWidth;
    [ObservableProperty] double _viewHeight;
    [ObservableProperty] double _viewDisplayRatio;

    static int dx = 1;
    static int dy = 1;

    static int dt = 100;

    required public View TrackerLayer { init => _tracker = new PositionTracker(value); }

    readonly MimiViewBox _viewBox;
    readonly Path _outer;
    readonly Path _inner;
    readonly Path _hole;

    readonly PositionTracker _tracker;

    MimiRegion _outerRegion;
    MimiRegion _innerRegion;
    MimiRegion _holeRegion;

    public ICommand SizeChangedCommand { get; private set; }

    internal MimikakiViewModel(MimiViewBox viewbox, Path outer, Path inner, Path hole)
    {
        _viewBox = viewbox;
        _outer = outer;
        _inner = inner;
        _hole = hole;

        SizeChangedCommand = new Command<View>(TargetSizeChanged);
    }

    async void TargetSizeChanged(View target)
    {
        await EasyTasks.WaitFor(() => !target.DesiredSize.IsZero);

        ViewWidth = target.DesiredSize.Width;
        ViewHeight = target.DesiredSize.Height;

        ViewDisplayRatio = ViewHeight / _viewBox.GetBoundsAsync().Result.Height;

        View parent = target.Parent as View;

        parent.WidthRequest = ViewWidth;
        parent.HeightRequest = ViewHeight;

        // Initialize
        InitializeModel();
    }

    void InitializeModel()
    {
        //var figs = (_outer.Data as PathGeometry).Figures;

        _outerRegion ??= new( new InternalRegion(_outer.GetPath(), dx, dy) );
        _innerRegion ??= new( new InternalRegion(_inner.GetPath(), dx, dy) );
        _holeRegion ??= new( new InternalRegion(_hole.GetPath(), dx, dy) );

        OuterRegionDrawable ??= new MimiRegionDrawable(_outerRegion);
        InnerRegionDrawable ??= new MimiRegionDrawable(_innerRegion);
        HoleRegionDrawable ??= new MimiRegionDrawable(_holeRegion);

        //RunTrackerProcess(dt);        
        //StrongReferenceMessenger.Default.Send(new MimiViewInvalidateMessage("draw"));
    }
    
    async void RunTrackerProcess(int updateInterval)
    {
        IEnumerable<ITrackerListener> listenersOfHair = 
            new[] { _outerRegion.Hairs, _innerRegion.Hairs, _holeRegion.Hairs}
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listenersOfDirt = 
            new[] { _outerRegion.Dirts, _innerRegion.Dirts, _holeRegion.Dirts }
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listeners = listenersOfHair.Concat(listenersOfDirt);

        while (true)
        {
            var current  = _tracker.CurrentState;
            var position = ScaleForDisplayRatio(current.Position);
            var velocity = ScaleForDisplayRatio(current.Velocity);
            double dt = (double)updateInterval/1000;

            StrongReferenceMessenger.Default.Send(new TrackerUpdateMessage(current));

            if (_outerRegion.Contains(position) ||
                _innerRegion.Contains(position) ||
                _holeRegion.Contains(position))
                    StrongReferenceMessenger.Default.Send(new TrackerOnMimiMessage(current));
                    

            foreach (var listener in listeners)
                listener.OnMove(position, velocity, dt);

            // generate dirt
            TryGenerateDirt();    

            StrongReferenceMessenger.Default.Send(new MimiViewInvalidateMessage("draw"));

            // dirt removed action
            CheckDirtRemoved(_outerRegion);
            CheckDirtRemoved(_innerRegion);
            CheckDirtRemoved(_holeRegion);    

            await Task.Delay(updateInterval);
        }
    }

    void TryGenerateDirt()
    {
        Random rnd = new Random();
        if (rnd.NextDouble() > 0.8)
            _outerRegion.GenerateMimiDirt();

        if (rnd.NextDouble() > 0.9)
            _innerRegion.GenerateMimiDirt();

        if (rnd.NextDouble() > 0.95)
            _holeRegion.GenerateMimiDirt();
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
            var width = dirt.Size / ViewDisplayRatio;
            var height = dirt.Size / ViewDisplayRatio;

            Rectangle rect = new Rectangle {Fill = Colors.Magenta, WidthRequest = width, HeightRequest = height};

            Point position = dirt.Position;

            var x0 = position.X;
            var y0 = position.Y;

            rect.TranslationX = x0;
            rect.TranslationY = y0;

            StrongReferenceMessenger.Default.Send(new FloatingDirtGenerateMessage(rect));
            
            region.RemoveMimiDirt(dirt);
        }
    }

    Point ScaleForDisplayRatio(Point point)
    {
        return new Point(point.X / ViewDisplayRatio, point.Y / ViewDisplayRatio);
    }


}
