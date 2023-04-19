using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiMimikakiApp.Models;

namespace MauiMimikakiApp.Drawables;

public class MimiRegionDrawable : IDrawable
{
    public double WidthRequest {get; init;}
    public double HeightRequest {get; init;}

    public double OffsetX => _offsetX;
    public double OffsetY => _offsetY;

    readonly MimiRegion _mimiRegion;
    //InternalRegion _internal => _mimiRegion.Internal;

    readonly float _padding;
    readonly double _offsetX;
    readonly double _offsetY;

    public MimiRegionDrawable(MimiRegion mimiRegion, double padding = 0) : base()
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
        // canvas.FillColor = Colors.Purple;
        // canvas.Alpha = 0.3f;
        // canvas.FillRectangle(0, 0, (float)WidthRequest, (float)HeightRequest);

        VisualizeOriginalPath(canvas);

        VisualizeHairs(canvas, Colors.Black);

        VisualizeDirts(canvas, Colors.Magenta);

        // canvas.FillColor = Colors.Blue;
        // VisualizeRegion(canvas, "boundary", Colors.Red);

        // canvas.FillColor = Colors.Pink;
        // VisualizeRegion(canvas, "inner", Colors.Pink);
    }

    void VisualizeOriginalPath(ICanvas canvas)
    {
        var pathF = _mimiRegion.OriginalPath;

        canvas.FillColor = Colors.Red;

        for (int i = 0; i < pathF.Count; i++)
        {
            Point p = pathF[i];

            var x = p.X - _offsetX;
            var y = p.Y - _offsetY;

            canvas.FillCircle((float)x, (float)y, 3);
        }

        var boundary = _mimiRegion.Boundary;

        
        canvas.FillColor = Colors.Green;

        for (int i = 0; i < boundary.Count; i++)
        {
            Point p = boundary[i];

            var x = p.X - _offsetX;
            var y = p.Y - _offsetY;

            canvas.FillCircle((float)x, (float)y, 2);
        }

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

    // void VisualizeRegion(ICanvas canvas, string key, Color color, int steps = 1)
    // {
    //     if (_region is null) throw new Exception("Internal region is not generated.");

    //     bool[,] flags = _internal[key];

    //     int totalLength = flags.Length;
    //     int dimensions = flags.Rank;

    //     int lenX = flags.GetUpperBound(0) + 1;
    //     int lenY = flags.GetUpperBound(1) + 1;

        
    //     if (lenX != _internal.LenX || lenY != _internal.LenY) throw new ArgumentException("Size of flags does not match with the region instance.");

    //     for (int i = 0; i < _internal.LenX; i+=steps)
    //     {
    //         for (int j = 0; j < _internal.LenY; j+=steps)
    //         {
    //             if (!flags[i, j]) continue;

    //             double x = _internal.Xs + i * _internal.Dx - _offsetX;
    //             double y = _internal.Ys + j * _internal.Dy - _offsetY;

    //             canvas.FillCircle( (float)x, (float)y, 3);

    //         }
    //     }

    // }

}

