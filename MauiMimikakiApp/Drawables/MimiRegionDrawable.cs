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
    InternalRegion _internal => _mimiRegion.Internal;

    readonly float _padding;
    readonly double _offsetX;
    readonly double _offsetY;

    public MimiRegionDrawable(MimiRegion mimiRegion, double padding = 0) : base()
    {
        _mimiRegion = mimiRegion;

        _padding = (float)padding;

        var topleft = _internal.TopLeft;

        _offsetX = topleft.X - _padding;
        _offsetY = topleft.Y - _padding;

        WidthRequest = _internal.MaxWidth + 2*_padding;
        HeightRequest = _internal.MaxHeight + 2*_padding;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Purple;
        canvas.Alpha = 0.3f;
        canvas.FillRectangle(0, 0, (float)WidthRequest, (float)HeightRequest);

        // canvas.StrokeColor = Colors.Red;
        // canvas.StrokeSize = 2;
        // canvas.DrawCircle(_padding, _padding, 4);

        VisualizeHair(canvas, Colors.LightGray, 2.0f);

        // canvas.FillColor = Colors.Blue;
        // VisualizeRegion(canvas, "boundary", Colors.Red);

        // canvas.FillColor = Colors.Pink;
        // VisualizeRegion(canvas, "inner", Colors.Pink);
    }

    void VisualizeHair(ICanvas canvas, Color color, float thinness)
    {   
        foreach (var hair in _mimiRegion.Hairs)
        {
            var origin = hair.Origin;
            var position = hair.Position;

            var x = position.X - _offsetX;
            var y = position.Y - _offsetY;
            var x0 = origin.X - _offsetX;
            var y0 = origin.Y - _offsetY;

            canvas.FillColor = color;
            canvas.FillCircle( (float)x0, (float)y0, thinness);

            canvas.FillColor = Colors.Black;
            canvas.FillCircle( (float)x, (float)y, thinness*0.5f);
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

