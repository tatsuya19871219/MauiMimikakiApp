using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

public class MimiRegion
{
    //public InternalRegion Internal => _internalRegion;
    public Rect Bounds => _internalRegion.Bounds;
    public IEnumerable<MimiHair> Hairs => _hairs;
    public IEnumerable<MimiDust> Dusts => _dusts;
    
    readonly InternalRegion _internalRegion;
    readonly List<MimiHair> _hairs = new();
    readonly List<MimiDust> _dusts = new();


    public MimiRegion(InternalRegion internalRegion)
    {
        _internalRegion = internalRegion;

        // var topleft = _internalRegion.TopLeft;
        // var size = new Size(_internalRegion.MaxWidth, _internalRegion.MaxHeight);

        // Bounds = new Rect(topleft, size);

        // initialize mimi hairs
        InitializeMimiHair(0.2);

    }

    void InitializeMimiHair(double density)
    {
        double dx = 1/density;
        double dy = 1/density;

        double hairMargin = 0;

        // var topleft = _internalRegion.TopLeft;
        // var bottomright = _internalRegion.BottomRight;
        // var center = _internalRegion.Center;

        // var maxWidth = _internalRegion.MaxWidth;
        // var maxHeight = _internalRegion.MaxHeight;

        Point center = Bounds.Center;

        var maxWidth = Bounds.Width;
        var maxHeight = Bounds.Height;

        // for (double x = topleft.X + hairMargin; x <= bottomright.X - hairMargin; x+=dx)
        // {
        //     for (double y = topleft.Y + hairMargin; y <= bottomright.Y - hairMargin; y+=dy)
        //     {
        //         if (_internalRegion.ContainsInRegion(x,y) 
        //             && _internalRegion.DistanceFromBoundary(x,y) > hairMargin)
        //                     _hairs.Add(new MimiHair(new Point(x,y)));
        //     }
        // }

        for (double x = center.X - maxWidth/2; x <= center.X + maxWidth/2; x+=dx)
        {
            for (double y = center.Y - maxHeight/2; y <= center.Y + maxHeight/2; y+=dy)
            {
                if (_internalRegion.ContainsInRegion(x,y)
                    && _internalRegion.DistanceFromBoundary(x,y) > hairMargin)
                            _hairs.Add(new MimiHair(new Point(x,y)));
            }
        }
    }
}
