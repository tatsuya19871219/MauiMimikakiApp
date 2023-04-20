using MauiMimikakiApp.Models;

namespace MauiMimikakiApp.Drawables;

public class MimiRegionDrawable : IDrawable
{
    public double WidthRequest {get; init;}
    public double HeightRequest {get; init;}

    public double OffsetX => _offsetX;
    public double OffsetY => _offsetY;

    readonly MimiRegion _mimiRegion;

    readonly float _padding;
    readonly double _offsetX;
    readonly double _offsetY;

    internal MimiRegionDrawable(MimiRegion mimiRegion, double padding = 0) : base()
    {
        _mimiRegion = mimiRegion;

        _padding = (float)padding;

        var bounds = _mimiRegion.Bounds;

        _offsetX = bounds.Left - _padding;
        _offsetY = bounds.Top - _padding;

        WidthRequest = bounds.Width + 2*_padding;
        HeightRequest = bounds.Height + 2*_padding;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        VisualizeHairs(canvas, Colors.Black);

        VisualizeDirts(canvas, Colors.Magenta);
    }

    
    void VisualizeHairs(ICanvas canvas, Color color)
    {   

        foreach (var hair in _mimiRegion.Hairs)
        {
            var origin = hair.Origin;
            var position = hair.Position;

            var x = position.X - _offsetX;
            var y = position.Y - _offsetY;
            var x0 = origin.X - _offsetX;
            var y0 = origin.Y - _offsetY;

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

            var x = position.X - _offsetX - size.Width/2;
            var y = position.Y - _offsetY - size.Height/2;

            //canvas.StrokeSize = 10;

            canvas.FillColor = color;

            canvas.FillRectangle((float)x, (float)y, (float)size.Width, (float)size.Height);
        }
    }
    
}

