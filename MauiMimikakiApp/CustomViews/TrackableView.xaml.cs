using Microsoft.Maui.Controls.Shapes;
using TakeMauiEasy;

namespace MauiMimikakiApp.CustomViews;

public partial class TrackableView : ContentView
{
    //PositionTracker _tracker = null;
    View _targetView = null;

    //double _targetWidthRequest;
    double _targetHeightRequest;

    double _displayRatio;

    public TrackableView()
    {
        InitializeComponent();
    }

    public async Task SetTargetView(View targetView, double targetHeightRequest)
    {
        if (_targetView is not null) throw new ArgumentException("Target view is already set.");
        if (targetView is null) throw new ArgumentNullException("Given target view is null.");
        
        if (targetHeightRequest < 0) throw new ArgumentException("HeightRequest of target view should be a positive value");

        _targetView = targetView;
        //_targetWidthRequest = targetView.WidthRequest;
        _targetHeightRequest = targetHeightRequest;

        await InitializeTrackableView();

        //_tracker = new PositionTracker(this);

        //RunSampleTracker();
    }

    async Task InitializeTrackableView()
    {
        _targetView.ZIndex = -1;

        TrackableContent.Add(_targetView);

        while (true)
        {
            if (_targetView.DesiredSize.Width > 0) break;
            await Task.Delay(100);
        }

        this.WidthRequest = _targetView.DesiredSize.Width;
        this.HeightRequest = _targetView.DesiredSize.Height;

        // 
        _displayRatio = _targetView.DesiredSize.Height / _targetHeightRequest;

    }

    
    // public void RegistDetectableRegion(Geometry geometry)
    // {
    //     if (_displayRatio < 0) throw new Exception("Initialization is not finished or failed.");

    //     var detectableRegion = new DetectableRegionView(geometry);

    //     detectableRegion.AnchorX = 0;
    //     detectableRegion.AnchorY = 0;
    //     detectableRegion.Scale = _displayRatio;

    //     DetectableRegions.Add(detectableRegion);
    // }

    public async Task AddDetectableRegionView(DetectableRegionView detectableRegionView)
    {
        if (_displayRatio < 0) throw new Exception("Initialization is not finished or failed.");

        detectableRegionView.AnchorX = 0;
        detectableRegionView.AnchorY = 0;
        detectableRegionView.Scale = _displayRatio;

        DetectableRegions.Add(detectableRegionView);

        await Task.Yield();
    }

    // async void RunSampleTracker()
    // {
    //     while(true)
    //     {
    //         var current = _tracker.CurrentState;
    //         var pointInDisplay = current.Position;
    //         var point = new Point(pointInDisplay.X/_displayRatio, pointInDisplay.Y/_displayRatio);

    //         foreach (DetectableRegionView detectableRegion in DetectableRegions)
    //         {
    //             bool isContains = detectableRegion.InternalRegion.ContainsInRegion(point);

    //             if (isContains) detectableRegion.BackgroundColor = Colors.Red;
    //             else detectableRegion.BackgroundColor = Colors.Transparent;
    //         }            

    //         await Task.Delay(100);
    //     }
    // }

    // public PositionTracker GetPositionTracker() => _tracker;

}