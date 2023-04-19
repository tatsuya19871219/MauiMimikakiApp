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
        // Should check whether the resource of the filename exists
        FileImageSource source = ImageSource.FromFile(filename) as FileImageSource;

        TargetImage.Source = source;
    }

    public MimikakiView(MimiViewBox viewbox, Path outer, Path inner, Path hole)
    {
        InitializeComponent();

        BindingContext = new MimikakiViewModel(viewbox, outer, inner, hole) {
#if WINDOWS
            TrackerLayer = TrackableContent
#elif ANDROID
            TrackerLayer = TrackerLayer
#endif
        };

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
        var x = dirtObject.TranslationX;
        var y = dirtObject.TranslationY;

        FloatingObjectsLayer.Add(dirtObject);

        uint animationTime = 2500;

        dirtObject.RotateTo(300, animationTime);
        await dirtObject.TranslateTo(x, y+500, animationTime);

        FloatingObjectsLayer.Remove(dirtObject);
    }
 
}