using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using TakeMauiEasy;

namespace MauiMimikakiApp.CustomViews;

public partial class TrackableMimiView : ContentView
{
    // Image Source
    public static BindableProperty TargetImageSourceProperty = BindableProperty.Create("TargetImageSource", typeof(ImageSource), typeof(TrackableMimiView), null);

    public ImageSource TargetImageSource
    {
        get => (ImageSource)GetValue(TargetImageSourceProperty);
        set => SetValue(TargetImageSourceProperty, value);
    }

    public double TargetImageOriginalHeight { get; init; }

    //PositionTracker _tracker = null;
    //View _targetView = null;

    //double _targetWidthRequest;
    //double _targetHeightRequest;

    //double _displayRatio;

    public TrackableMimiView() //(TrackableMimiViewModel vm)
    {
        InitializeComponent();

        TargetImage.BindingContext = this;

        // Register DrawMessages
        StrongReferenceMessenger.Default.Register<DrawMessage>(this, (s, e) =>
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                TopRegion.Invalidate();
                CenterRegion.Invalidate();
                BottomRegion.Invalidate();
            });
        });
        
        //RunInvalidateProcess();
    }

    private async void TargetImage_SizeChanged(object sender, EventArgs e)
    {
        if (TargetImageSource is null) throw new ArgumentNullException("Image source is null.");
        if (TargetImageOriginalHeight < 0) throw new Exception("Invalid TargetImageOriginalHeight is set.");

        //while (true)
        //{
        //    if (TargetImage.DesiredSize.Width > 0) break;
        //    await Task.Delay(100);
        //}

        await EasyTasks.WaitFor(() => TargetImage.DesiredSize.Width > 0);

        this.WidthRequest = TargetImage.DesiredSize.Width;
        this.HeightRequest = TargetImage.DesiredSize.Height;
        
        var displayRatio = this.DesiredSize.Height / TargetImageOriginalHeight;

        FrontLayer.AnchorX = 0;
        FrontLayer.AnchorY = 0;
        FrontLayer.Scale = displayRatio;
    }

    // async void RunInvalidateProcess()
    // {
    //     while(true)
    //     {
    //         TopRegion.Invalidate();
    //         await Task.Delay(100);
    //     }
    // }

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

    //protected override void OnBindingContextChanged()
    //{
    //    base.OnBindingContextChanged();
    //}

    //protected override void OnSizeAllocated(double width, double height)
    //{
    //    base.OnSizeAllocated(width, height);
    //}

}