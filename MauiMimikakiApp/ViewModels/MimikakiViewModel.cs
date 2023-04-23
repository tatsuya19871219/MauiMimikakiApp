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
using System.Diagnostics;

namespace MauiMimikakiApp.ViewModels;

internal partial class MimikakiViewModel : ObservableObject
{
    [ObservableProperty] double _viewWidth;
    [ObservableProperty] double _viewHeight;
    [ObservableProperty] double _viewDisplayRatio;
    [ObservableProperty] MimiRegionViewBox _outerViewBox;
    [ObservableProperty] MimiRegionViewBox _innerViewBox;
    [ObservableProperty] MimiRegionViewBox _holeViewBox;
    [ObservableProperty] MimiRegionDrawable _outerRegionDrawable;
    [ObservableProperty] MimiRegionDrawable _innerRegionDrawable;
    [ObservableProperty] MimiRegionDrawable _holeRegionDrawable;

    [ObservableProperty] bool _readyToMimikaki;


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

    //MimikakiConfig _config;
    bool _modelInitialized = false;

    internal MimikakiViewModel(MimiViewBox viewbox, Path outer, Path inner, Path hole)
    {
        _viewBox = viewbox;
        _outer = outer;
        _inner = inner;
        _hole = hole;

        ReadyToMimikaki = false;

        SizeChangedCommand = new Command<View>(TargetSizeChanged);

        //_config = MimikakiConfig.Current;

        InitializeModelAsync();
        PrepareDrawablesAsync();
    }

    async void TargetSizeChanged(View target)
    {
        await EasyTasks.WaitFor(() => !target.DesiredSize.IsZero);

        ViewWidth = target.DesiredSize.Width;
        ViewHeight = target.DesiredSize.Height;

        ViewDisplayRatio = ViewHeight / _viewBox.GetBoundsAsync().Result.Height;
    }

    async void InitializeModelAsync()
    {
        await EasyTasks.WaitFor(() => MimikakiConfig.Current is not null);
        
        var config = MimikakiConfig.Current;

        int dx = config.Params.RegionRoughness.DX;
        int dy = config.Params.RegionRoughness.DY;

        _outerRegion = new(_outer.GetPath(), dx, dy);
        _innerRegion = new(_inner.GetPath(), dx, dy);
        _holeRegion = new(_hole.GetPath(), dx, dy);

        OuterViewBox = new(_outerRegion);
        InnerViewBox = new(_innerRegion);
        HoleViewBox = new(_holeRegion);

        _modelInitialized = true;
        // //await Task.Delay(5000);

        // ReadyToMimikaki = true;

        RunGraphicsUpdateProcess(config.GraphicsUpdateInterval);
        //RunTrackerProcess(_config.TrackerUpdateInterval);
    }

    async void PrepareDrawablesAsync()
    {
        await EasyTasks.WaitFor(() => _modelInitialized);

        OuterRegionDrawable = new MimiRegionDrawable(_outerRegion, OuterViewBox);
        InnerRegionDrawable = new MimiRegionDrawable(_innerRegion, InnerViewBox);
        HoleRegionDrawable = new MimiRegionDrawable(_holeRegion, HoleViewBox);
    }

    async void RunGraphicsUpdateProcess(int updateInterval)
    {
        Stopwatch stopwatch = new Stopwatch();

        while (true)
        {
            var t = Task.Delay(updateInterval);

            StrongReferenceMessenger.Default.Send(new MimiViewInvalidateMessage("draw"));

            // wait at least updateInterval
            await t;
        }
    }
    
    async void RunTrackerProcess(int updateInterval)
    {
        Stopwatch stopwatch = new Stopwatch();


        IEnumerable<ITrackerListener> listenersOfHair = 
            new[] { _outerRegion.Hairs, _innerRegion.Hairs, _holeRegion.Hairs}
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listenersOfDirt = 
            new[] { _outerRegion.Dirts, _innerRegion.Dirts, _holeRegion.Dirts }
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listeners = listenersOfHair.Concat(listenersOfDirt);

        double dt = (double)updateInterval/1000;

        while (true)
        {

            // //try
            // {
            //     MimikakiViewUpdate(dt);
            // }
            //catch(Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);
            //}

            await Task.Delay(updateInterval);

            // stopwatch.Restart();

            //var current = _tracker.CurrentState;
            //var position = ScaleForDisplayRatio(current.Position);
            //var velocity = ScaleForDisplayRatio(current.Velocity);
            ////double dt = (double)updateInterval / 1000;

            //StrongReferenceMessenger.Default.Send(new TrackerUpdateMessage(current));

            //if (_outerRegion.Contains(position) ||
            //    _innerRegion.Contains(position) ||
            //    _holeRegion.Contains(position))
            //    StrongReferenceMessenger.Default.Send(new TrackerOnMimiMessage(current));


            //foreach (var listener in listenersOfDirt)
            //    listener.OnMove(position, velocity, dt);

            //// Debug.WriteLine($"A: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

            //TryGenerateDirt();

            //StrongReferenceMessenger.Default.Send(new MimiViewInvalidateMessage("draw"));

            //CheckDirtRemoved(_outerRegion);
            //CheckDirtRemoved(_innerRegion);
            //CheckDirtRemoved(_holeRegion);

            // Debug.WriteLine($"B: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

            // await Task.Delay(updateInterval);

            // Debug.WriteLine($"C: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

            // //stopwatch.Reset();
        }
    }

    void MimikakiViewUpdate(double dt)
    {
        IEnumerable<ITrackerListener> listenersOfDirt =
            new[] { _outerRegion.Dirts, _innerRegion.Dirts, _holeRegion.Dirts }
            .SelectMany(listener => listener);
        // stopwatch.Restart();

        var current  = _tracker.CurrentState;
            var position = ScaleForDisplayRatio(current.Position);
            var velocity = ScaleForDisplayRatio(current.Velocity);
        //double dt = (double)updateInterval/1000;

        StrongReferenceMessenger.Default.Send(new TrackerUpdateMessage(current));

        if (_outerRegion.Contains(position) ||
            _innerRegion.Contains(position) ||
            _holeRegion.Contains(position))
            StrongReferenceMessenger.Default.Send(new TrackerOnMimiMessage(current));


        foreach (var listener in listenersOfDirt)
                listener.OnMove(position, velocity, dt);

        // Debug.WriteLine($"A: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

        TryGenerateDirt();



        CheckDirtRemoved(_outerRegion);
        CheckDirtRemoved(_innerRegion);
        CheckDirtRemoved(_holeRegion);

        // Debug.WriteLine($"B: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

        // await Task.Delay(updateInterval);

        // Debug.WriteLine($"C: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

        //stopwatch.Reset();
    }

    void TryGenerateDirt()
    {
        Random rnd = new Random();
        if (rnd.NextDouble() > 0.80)
            _outerRegion.GenerateMimiDirt();

        if (rnd.NextDouble() > 0.85)
            _innerRegion.GenerateMimiDirt();

        if (rnd.NextDouble() > 0.88)
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
            var width = dirt.Size; // / ViewDisplayRatio;
            var height = dirt.Size; // / ViewDisplayRatio;

            Rectangle rect = new Rectangle {Fill = Colors.Magenta, Stroke=Colors.Magenta, WidthRequest = width, HeightRequest = height};

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
