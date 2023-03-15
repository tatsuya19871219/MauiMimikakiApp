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

    readonly MimiRegion _region;
    //readonly PathInternalRegion _region;

    InternalRegion _internal => _region.Internal;

    readonly float _padding;
    readonly double _offsetX;
    readonly double _offsetY;

    //public MimiRegionDrawable(PathInternalRegion region, double padding = 20) : base()
    public MimiRegionDrawable(MimiRegion region, double padding = 20) : base()
    {
        _region = region;

        _padding = (float)padding;

        _offsetX = _internal.Xs - _padding;
        _offsetY = _internal.Ys - _padding;

        WidthRequest = _internal.Xe - _internal.Xs + 2*_padding;
        HeightRequest = _internal.Ye - _internal.Ys + 2*_padding;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Purple;
        canvas.Alpha = 0.3f;
        canvas.FillRectangle(0, 0, (float)WidthRequest, (float)HeightRequest);

        // canvas.StrokeColor = Colors.Red;
        // canvas.StrokeSize = 2;
        // canvas.DrawCircle(_padding, _padding, 4);

        // canvas.FillColor = Colors.Blue;
        // VisualizeRegion(canvas, "boundary", Colors.Red);

        // canvas.FillColor = Colors.Pink;
        // VisualizeRegion(canvas, "inner", Colors.Pink);
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

