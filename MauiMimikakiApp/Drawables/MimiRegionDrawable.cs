using CommunityToolkit.Mvvm.ComponentModel;
using MauiMimikakiApp.Models;

namespace MauiMimikakiApp.Drawables;

public partial class MimiRegionDrawable : ObservableObject, IDrawable
{
    [ObservableProperty] double _regionWidthRequest;
    [ObservableProperty] double _regionHeightRequest;
    [ObservableProperty] double _regionOffsetX;
    [ObservableProperty] double _regionOffsetY;

    readonly MimiRegion _mimiRegion;


    internal MimiRegionDrawable(MimiRegion mimiRegion, double padding = 0) : base()
    {
        _mimiRegion = mimiRegion;

        var bounds = _mimiRegion.Bounds;

        RegionOffsetX = bounds.Left - padding;
        RegionOffsetY = bounds.Top - padding;

        RegionWidthRequest = bounds.Width + 2*padding;
        RegionHeightRequest = bounds.Height + 2*padding;

        //double density = DeviceDisplay.Current.MainDisplayInfo.Density;

    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // canvas.StrokeColor = Colors.Red;
        // canvas.StrokeSize = 2;
        // canvas.DrawRectangle(dirtyRect);

        VisualizeHairs(canvas, Colors.Black);

        VisualizeDirts(canvas, Colors.Magenta);
    }

    
    void VisualizeHairs(ICanvas canvas, Color color)
    {   

        foreach (var hair in _mimiRegion.Hairs)
        {
            var origin = hair.Origin;
            var position = hair.Position;

            var x = position.X - RegionOffsetX;
            var y = position.Y - RegionOffsetY;
            var x0 = origin.X - RegionOffsetX;
            var y0 = origin.Y - RegionOffsetY;

            canvas.FillColor = Colors.LightGray;
            //canvas.Alpha = (float)(hair.Thinness - 2) * 3;
            canvas.FillCircle( (float)x0, (float)y0, (float)hair.Thinness);

            canvas.FillColor = color;
            //canvas.Alpha = 1.0f;
            canvas.FillCircle( (float)x, (float)y, (float)hair.Thinness*0.25f);
        }
    }

    void VisualizeDirts(ICanvas canvas, Color color)
    {
        foreach (var dirt in _mimiRegion.Dirts)
        {
            if (dirt.IsRemoved) continue;

            var position = dirt.Position;

            var size = new Size(dirt.Size, dirt.Size);

            var x = position.X - RegionOffsetX - size.Width/2;
            var y = position.Y - RegionOffsetY - size.Height/2;

            canvas.FillColor = color;

            canvas.FillRectangle((float)x, (float)y, (float)size.Width, (float)size.Height);
        }
    }
    
}

