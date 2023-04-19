using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;
using MauiMimikakiApp.Models;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using System.Reflection;
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
        //var assembly = GetType().GetTypeInfo().Assembly;
        //var files = assembly.GetManifestResourceNames().ToList();
        //var res = Application.Current.Resources.ToList();

        // Should check whether the resource of the filename exists
        FileImageSource source = ImageSource.FromFile(filename) as FileImageSource;

        TargetImage.Source = source;
    }


    MimikakiViewModel _vm;

    MimiViewBox _viewbox;

    // public double? DisplayRatio => _displayRatio;
    // double? _displayRatio = null;

    public MimikakiView(MimiViewBox viewbox, Path outer, Path inner, Path hole)
    {
        InitializeComponent();

        //TargetImage.BindingContext = this;

        _viewbox = viewbox;

        _vm = new MimikakiViewModel(viewbox, outer, inner, hole) {
#if WINDOWS
            TrackerLayer = TrackableContent
#elif ANDROID
            TrackerLayer = TrackerLayer
#endif
        };

        this.BindingContext = _vm;

        // Resister messages
        StrongReferenceMessenger.Default.Register<MimiViewInvalidateMessage>(this, (s, e) =>
        {
            switch (e.Value)
            {
                case "draw" :
                    MainThread.InvokeOnMainThreadAsync(InvalidateRegions);                    
                    break;

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
        
    }

    void InvalidateRegions()
    {
        OuterRegion.Invalidate();
        InnerRegion.Invalidate();
        HoleRegion.Invalidate();
    }
    
    
    // for test
    async void AddFloatingDirt(Shape dirtObject)
    {
        // Rectangle rect = new Rectangle {Fill = Colors.Magenta, WidthRequest = 8, HeightRequest = 8};

        // Point position = dirt.Position;

        //dirtObject.Stroke = Colors.Green;

        // var width0 = dirtObject.WidthRequest;
        // var height0 = dirtObject.HeightRequest;

        // var x0 = dirtObject.TranslationX;
        // var y0 = dirtObject.TranslationY;

        // dirtObject.WidthRequest /= _displayRatio.Value;
        // dirtObject.HeightRequest /= _displayRatio.Value;

        // // dirtObject.WidthRequest *= 1.2;
        // // dirtObject.HeightRequest *= 1.2;

        // var x = dirtObject.TranslationX = x0 + width0/2 - dirtObject.WidthRequest/2;
        // var y = dirtObject.TranslationY = y0 + height0/2 - dirtObject.HeightRequest/2;

        var x = dirtObject.TranslationX;
        var y = dirtObject.TranslationY;

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

 
}