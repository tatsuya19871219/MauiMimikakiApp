using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;
using MauiMimikakiApp.Models;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using TakeMauiEasy;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace MauiMimikakiApp.CustomViews;

public partial class MimikakiView : ContentView
{
    string _filename;
    required public string Filename 
    {
        get  => _filename;
        init => InitTargetImage(_filename = value);
    }

    void InitTargetImage(string filename)
    {
        // Resource check
        FileImageSource source = ImageSource.FromFile(filename) as FileImageSource;
        
        TargetImage.Source = source;
    }


    MimikakiViewModel _vm;

    MimiViewBox _viewbox;

    public double? DisplayRatio => _displayRatio;
    double? _displayRatio = null;

    public MimikakiView(MimiViewBox viewbox, Path outer, Path inner, Path hole)
    {
        InitializeComponent();

        //TargetImage.BindingContext = this;

        _viewbox = viewbox;

        _vm = new MimikakiViewModel(viewbox, outer, inner, hole) {TrackerLayer = TrackerLayer};

        this.BindingContext = _vm;   // This will overwrite the BindingContext of its children
                                        // (e.g., TargetImage.BindingContext = this; will be no effect)

        //TargetImage.BindingContext = this; // overwrite

        //TargetImage.SetBinding(Image.SourceProperty, new Binding(nameof(TargetImageSource), source: this));
        //TargetImage.BindingContext = _vm;

        //FrontLayer.BindingContext = _vm;
        //TargetImage.BindingContext = this;

        //GetBoundsAsync();

        //_viewbox = viewbox.Bounds;
        //_viewbox = viewbox.GetBoundsAsync().Result;

        //var bounds = _viewbox.GetBoundsAsync().Result;

        // Pass arguments to the ViewModel
    }

    // async void GetBoundsAsync()
    // {
    //     var bounds = await _viewbox.GetBoundsAsync();
    // }

    public MimikakiView() //(TrackableMimiViewModel vm)
    {
        InitializeComponent();

        TargetImage.BindingContext = this;

        // Register DrawMessages
        StrongReferenceMessenger.Default.Register<MimiViewInvalidateMessage>(this, (s, e) =>
        {
            switch (e.Value)
            {
                case "draw" :
                    
                    //MainThread.InvokeOnMainThreadAsync(() =>
                    //{
                    //    TopRegion.Invalidate();
                    //    CenterRegion.Invalidate();
                    //    BottomRegion.Invalidate();
                    //});
                    
                    break;

                // case "float" :

                //     MainThread.InvokeOnMainThreadAsync(() =>
                //     {
                //         AddFloatingDirt();
                //     });

                //     break;

                default:
                    break;    
            }
        });

        StrongReferenceMessenger.Default.Register<FloatingDirtGenerateMessage>(this, (s, e) =>
        {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    AddFloatingDirt(e.Value);
                });
        });
        
        //RunInvalidateProcess();
    }

    //private async void TargetImage_SizeChanged(object sender, EventArgs e)
    //{
    //    if (TargetImageSource is null) throw new ArgumentNullException("Image source is null.");
    //    if (TargetImageOriginalHeight < 0) throw new Exception("Invalid TargetImageOriginalHeight is set.");

    //    //while (true)
    //    //{
    //    //    if (TargetImage.DesiredSize.Width > 0) break;
    //    //    await Task.Delay(100);
    //    //}

    //    await EasyTasks.WaitFor(() => TargetImage.DesiredSize.Width > 0);

    //    this.WidthRequest = TargetImage.DesiredSize.Width;
    //    this.HeightRequest = TargetImage.DesiredSize.Height;
        
    //    _displayRatio = this.DesiredSize.Height / TargetImageOriginalHeight;

    //    FrontLayer.AnchorX = 0;
    //    FrontLayer.AnchorY = 0;
    //    FrontLayer.Scale = _displayRatio.Value;

    //    // Initialize view model here
    //}

    // for test
    async void AddFloatingDirt(Shape dirtObject)
    {
        // Rectangle rect = new Rectangle {Fill = Colors.Magenta, WidthRequest = 8, HeightRequest = 8};

        // Point position = dirt.Position;

        //dirtObject.Stroke = Colors.Green;

        var width0 = dirtObject.WidthRequest;
        var height0 = dirtObject.HeightRequest;

        var x0 = dirtObject.TranslationX;
        var y0 = dirtObject.TranslationY;

        dirtObject.WidthRequest /= _displayRatio.Value;
        dirtObject.HeightRequest /= _displayRatio.Value;

        // dirtObject.WidthRequest *= 1.2;
        // dirtObject.HeightRequest *= 1.2;

        var x = dirtObject.TranslationX = x0 + width0/2 - dirtObject.WidthRequest/2;
        var y = dirtObject.TranslationY = y0 + height0/2 - dirtObject.HeightRequest/2;

        // rect.TranslationX = x0 - rect.Width/2;
        // rect.TranslationY = y0 - rect.Height/2;

        FloatingObjectsLayer.Add(dirtObject);

        uint animationTime = 2500;

        dirtObject.RotateTo(300, animationTime);
        await dirtObject.TranslateTo(x, y+500, animationTime);

        //await Task.Delay((int)animationTime);

        FloatingObjectsLayer.Remove(dirtObject);
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