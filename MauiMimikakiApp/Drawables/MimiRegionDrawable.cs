using CommunityToolkit.Mvvm.ComponentModel;
using MauiMimikakiApp.Models;

namespace MauiMimikakiApp.Drawables;

public class MimiRegionDrawable : IDrawable
{
    readonly MimiRegion _mimiRegion;
    readonly MimiRegionViewBox _viewBox;

    readonly double _offsetX;
    readonly double _offsetY;

    internal MimiRegionDrawable(MimiRegion mimiRegion, MimiRegionViewBox viewBox) : base()
    {
        _mimiRegion = mimiRegion;

        _viewBox = viewBox;

        _offsetX = _viewBox.RegionOffsetX;
        _offsetY = _viewBox.RegionOffsetY;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // canvas.StrokeColor = Colors.Red;
        // canvas.StrokeSize = 2;

        // canvas.DrawRectangle(dirtyRect);
        //canvas.DrawRectangle(new Rect(0, 0, _viewBox.RegionWidthRequest, _viewBox.RegionHeightRequest));

        VisualizeHairs(canvas);

        VisualizeDirts(canvas);
    }

    
    void VisualizeHairs(ICanvas canvas)
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

            canvas.FillColor = hair.HairColor;
            //canvas.Alpha = 1.0f;
            canvas.FillCircle( (float)x, (float)y, (float)hair.Thinness*0.75f);
        }
    }

    void VisualizeDirts(ICanvas canvas)
    {
        foreach (var dirt in _mimiRegion.Dirts)
        {
            //if (dirt.IsRemoved) continue;

            var position = dirt.Position;

            var size = new Size(dirt.Size, dirt.Size);

            var x = position.X - _offsetX - size.Width/2;
            var y = position.Y - _offsetY - size.Height/2;

            canvas.FillColor = dirt.DirtColor;

            canvas.FillRectangle((float)x, (float)y, (float)size.Width, (float)size.Height);
        }
    }
    
}

