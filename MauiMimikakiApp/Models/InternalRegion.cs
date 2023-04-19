using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

internal class InternalRegion : AbstractRegion
{
 
    // internal List<Point> GetBoundaryPointList()
    // {
    //     List<Point> boundary = new();

    //     for (int j = 0; j < _lenY; j++)
    //     {
    //         for (int i = 0; i <_lenX; i++)
    //         {
    //             if (_isBoundary[i,j])
    //                 boundary.Add( GetPositionOfIndex(i, j) );
    //         }
    //     }

    //     return boundary;
    // }

    readonly List<SubRegion> subRegions = new();
    readonly Random _rnd = new();

    public InternalRegion(PathF pathF, int dx = 5, int dy = 5) : base(pathF)
    {
        List<PathF> paths = pathF.Separate();

        foreach(var subPath in paths)
        {
            List<PointF> sharedVertecies;

            sharedVertecies = paths.Where(path => !path.Equals(subPath))
                                    .SelectMany(path => path.Points.Intersect(subPath.Points), (_, sharedPoints) => sharedPoints)
                                    .ToList();

            subRegions.Add( new(subPath, sharedVertecies, dx, dy) );
        }
    }
    

    override internal bool ContainsInRegion(Point point)
    {
        return subRegions.Any( region => region.ContainsInRegion(point) );
                            
    }

    override internal bool OnBoundary(Point point)
    {
        return subRegions.Any( region => region.OnBoundary(point) );
    }

    
    // public double? DistanceFromBoundary(double x, double y)
    // {

    //     //if (IsOutOfBoundBox(x, y)) return null;
    //     if (!ContainsInRegion(x, y)) return null;
    //     if (IsBoundary(x, y)) return 0;

    //     var (idx_x, idx_y) = ConvertToRegionIndex(x, y);

    //     int idxFromBoundX = Math.Min(idx_x, _lenX-idx_x-1);
    //     int idxFromBoundY = Math.Min(idx_y, _lenY-idx_y-1);    

    //     double minDistance = double.PositiveInfinity;

    //     for (int i = -idxFromBoundX; i < idxFromBoundX; i++)
    //     {
    //         for (int j = -idxFromBoundY; j < idxFromBoundY; j++)
    //         {
    //             if (_isBoundary[idx_x+i, idx_y+j])
    //             {
    //                 Point point = new Point(_dx*i, _dy*j);
    //                 double distance = point.Distance(Point.Zero);

    //                 if (minDistance > distance) minDistance = distance;
    //             }
    //         }
    //     }

    //     return minDistance;
    // }

    public Point GeneratePointInRegion()
    {
        while (true)
        {
            var tryX = GenerateRandomUniform(Bounds.Left, Bounds.Right);
            var tryY = GenerateRandomUniform(Bounds.Top, Bounds.Bottom);

            Point point = new(tryX, tryY);

            if (ContainsInRegion(point)) return point;
        }
    }

    double GenerateRandomUniform(double minValue, double maxValue)
    {
        return _rnd.NextDouble() * (maxValue - minValue) + minValue;
    }

}
