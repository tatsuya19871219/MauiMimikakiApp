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

    MimikakiConfig _config;

    bool _targetImageInitialized = false;
    bool _modelInitialized = false;
    bool _drawableInitialized = false;

    Random _rnd = new();

    internal MimikakiViewModel(MimiViewBox viewbox, Path outer, Path inner, Path hole)
    {
        _viewBox = viewbox;
        _outer = outer;
        _inner = inner;
        _hole = hole;

        ReadyToMimikaki = false;

        SizeChangedCommand = new Command<View>(TargetSizeChanged);

        InitializeModelAsync(() => MimikakiConfig.Current is not null);
        PrepareDrawablesAsync(() => _modelInitialized);

        InvokeMimikakiAsync(() => _targetImageInitialized && _modelInitialized && _drawableInitialized);
    }

    async void TargetSizeChanged(View target)
    {
        await EasyTasks.WaitFor(() => !target.DesiredSize.IsZero);

        ViewWidth = target.DesiredSize.Width;
        ViewHeight = target.DesiredSize.Height;

        ViewDisplayRatio = ViewHeight / _viewBox.GetBoundsAsync().Result.Height;

        _targetImageInitialized = true;
    }

    async void InitializeModelAsync(Func<bool> condition)
    {
        Stopwatch sw = Stopwatch.StartNew();

        //await EasyTasks.WaitFor(() => MimikakiConfig.Current is not null);
        await EasyTasks.WaitFor(condition);
        
        _config = MimikakiConfig.Current;

        var modelParams = _config.Params;

        // Debug.WriteLine($"A : Elapsed {sw.ElapsedMilliseconds} [ms]");

        _outerRegion = new(_outer.GetPath(), modelParams);
        _innerRegion = new(_inner.GetPath(), modelParams);
        _holeRegion = new(_hole.GetPath(), modelParams);

        // Debug.WriteLine($"B : Elapsed {sw.ElapsedMilliseconds} [ms]");

        OuterViewBox = new(_outerRegion);
        InnerViewBox = new(_innerRegion);
        HoleViewBox = new(_holeRegion);

        _modelInitialized = true;
    }

    async void PrepareDrawablesAsync(Func<bool> condition)
    {
        //await EasyTasks.WaitFor(() => _modelInitialized);
        await EasyTasks.WaitFor(condition);

        OuterRegionDrawable = new MimiRegionDrawable(_outerRegion, OuterViewBox);
        InnerRegionDrawable = new MimiRegionDrawable(_innerRegion, InnerViewBox);
        HoleRegionDrawable = new MimiRegionDrawable(_holeRegion, HoleViewBox);

        _drawableInitialized = true;
    }

    async void InvokeMimikakiAsync(Func<bool> condition)
    {
        //await EasyTasks.WaitFor(() => _targetImageInitialized && _modelInitialized && _drawableInitialized);
        await EasyTasks.WaitFor(condition);

        // var config = MimikakiConfig.Current;

        RunGraphicsUpdateProcess(_config.GraphicsUpdateInterval);
        RunTrackerProcess(_config.TrackerUpdateInterval);

        ReadyToMimikaki = true;
    } 

    async void RunGraphicsUpdateProcess(int graphicsUpdateInterval)
    {        
        Stopwatch stopwatch = new Stopwatch();

        while (true)
        {
            var t = Task.Delay(graphicsUpdateInterval);

            StrongReferenceMessenger.Default.Send(new MimiViewInvalidateMessage("draw"));

            // Wait at least the updateInterval [ms]
            await t;
        }
    }
    
    async void RunTrackerProcess(int trackerUpdateInterval)
    {
        Stopwatch stopwatch = new Stopwatch();

        IEnumerable<ITrackerListener> listenersOfHair = 
            new[] { _outerRegion.Hairs, _innerRegion.Hairs, _holeRegion.Hairs}
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listenersOfDirt = 
            new[] { _outerRegion.Dirts, _innerRegion.Dirts, _holeRegion.Dirts }
            .SelectMany(listener => listener);

        IEnumerable<ITrackerListener> listeners = listenersOfHair.Concat(listenersOfDirt);

        double dt = (double)trackerUpdateInterval/1000; //[sec]

        var probs = _config.Params.MimiDirtGenerationProbs;

        // Dirt generation probability in dt
        double probOuter = probs.outer * dt;
        double probInner = probs.inner * dt;
        double probHole = probs.hole * dt;

        while (true)
        {
            var t = Task.Delay(trackerUpdateInterval);

            stopwatch.Restart();

            var current = _tracker.CurrentState;
            var position = ScaleForDisplayRatio(current.Position);
            var velocity = ScaleForDisplayRatio(current.Velocity);

            StrongReferenceMessenger.Default.Send(new TrackerUpdateMessage(current));

            if (_outerRegion.Contains(position) ||
                _innerRegion.Contains(position) ||
                _holeRegion.Contains(position))
               StrongReferenceMessenger.Default.Send(new TrackerOnMimiMessage(current));


            foreach (var listener in listeners)
               listener.OnMove(position, velocity, dt);

            //Debug.WriteLine($"A: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

            TryGenerateDirt(_outerRegion, probOuter);
            TryGenerateDirt(_innerRegion, probInner);
            TryGenerateDirt(_holeRegion, probHole);

            CheckDirtRemoved(_outerRegion);
            CheckDirtRemoved(_innerRegion);
            CheckDirtRemoved(_holeRegion);

            //Debug.WriteLine($"B: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");

            await t;

            stopwatch.Reset();

            //Debug.WriteLine($"C: Elapsed {stopwatch.ElapsedMilliseconds} [ms]");
        }
    }

    void TryGenerateDirt(MimiRegion region, double prob)
    {
        if (_rnd.NextDouble() < prob) region.GenerateMimiDirt();
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
            var width = dirt.Size;
            var height = dirt.Size;

            Rectangle rect = new Rectangle {Fill = dirt.DirtColor, Stroke=dirt.DirtColor, WidthRequest = width, HeightRequest = height};

            Point position = dirt.Position;

            var x0 = position.X - width/2;
            var y0 = position.Y - height/2;

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
