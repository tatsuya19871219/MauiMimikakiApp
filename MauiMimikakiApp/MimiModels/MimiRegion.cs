using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

internal class MimiRegion
{
    public Rect Bounds => _internalRegion.Bounds;
    public IEnumerable<MimiHair> Hairs => _hairs;
    public IEnumerable<MimiDirt> Dirts => _dirts;
    
    readonly InternalRegion _internalRegion;
    readonly List<MimiHair> _hairs = new();
    readonly List<MimiDirt> _dirts = new();

    // internal PathF OriginalPath => _internalRegion.GetOriginalPath();

    // internal List<Point> Boundary => _internalRegion.GetBoundaryPointList();


    internal MimiRegion(PathF pathF, int dx, int dy)
    {
        _internalRegion = new(pathF, dx, dy);

        // initialize mimi hairs
        InitializeMimiHair(0.2);

    }

    void InitializeMimiHair(double density)
    {
        double dx = 1/density;
        double dy = 1/density;

        double hairMargin = 0;

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
                if (_internalRegion.ContainsInRegion(new(x,y))
                    && _internalRegion.DistanceFromBoundary(new(x,y)) > hairMargin)
                            _hairs.Add(new MimiHair(new Point(x,y)));
            }
        }
    }

    internal void GenerateMimiDirt()
    {
        Point point = _internalRegion.GeneratePointInRegion();

        _dirts.Add(new MimiDirt(point));
    }

    internal void RemoveMimiDirt(MimiDirt dirt)
    {
        _dirts.Remove(dirt);
    }

    internal bool Contains(Point point)
    {
        return _internalRegion.ContainsInRegion(point);
    }
}
