using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

internal class SubRegion : AbstractRegion
{
    readonly int _dx;
    readonly int _dy;

    readonly List<int> _xGrid;
    readonly List<int> _yGrid;
    int _lenX => _xGrid.Count;
    int _lenY => _yGrid.Count;

    readonly bool[,] _isInner;
    readonly bool[,] _isBoundary;

    public SubRegion(PathF pathF, List<PointF> sharedVertecies, int dx = 5, int dy = 5) : base(pathF)
    {
        _dx = dx;
        _dy = dy;

        var xs = (int)Math.Floor(Bounds.Left / dx) * dx;
		var ys = (int)Math.Floor(Bounds.Top / dy) * dy;

		var xe = (int)Math.Ceiling(Bounds.Right / dx) * dx;
		var ye = (int)Math.Ceiling(Bounds.Bottom / dy) * dy;

        var lenX = (xe - xs) / dx + 1;
		var lenY = (ye - ys) / dy + 1;

        _xGrid = Enumerable.Range(0, lenX).Select(i => xs + i*dx).ToList();
        _yGrid = Enumerable.Range(0, lenY).Select(i => ys + i*dy).ToList();

        _isInner = new bool[lenX, lenY];
        _isBoundary = new bool[lenX, lenY];

        FillBoundaryPoints(pathF, sharedVertecies);

        FillInternalRegion();
    }

    // Fill boundary by linear interpolation
    void FillBoundaryPoints(PathF pathF, List<PointF> sharedVertexies)
    {
        for (int i = 0; i < pathF.Count; i++)
        {
            Point pA = pathF[i];
            Point pB = (i < pathF.Count - 1) ? pathF[i+1] : pathF[0];

            if (pA.Equals(pB)) continue;

            bool checkAsRegion = sharedVertexies.Contains(pA) && sharedVertexies.Contains(pB);
            var markAsBroundary = (Point point) => 
            {
                var (idx_x, idx_y) = ConvertToRegionIndex(point);
                _isBoundary[idx_x, idx_y] = true;

                if (checkAsRegion) _isInner[idx_x, idx_y] = true;
            };

            double slope = (pB.Y - pA.Y) / (pB.X - pA.X);

            Func<double, double> eqYLine = (x) => pB.Y + slope * (x - pB.X);
            Func<double, double> eqXLine = (y) => 1/slope * (y - pB.Y) + pB.X;

            double diffX = pB.X - pA.X;
            double diffY = pB.Y - pA.Y;
            
            if (Math.Abs(diffX) > _dx)
            {
                for (int k = 0; k < Math.Abs(diffX) / _dx; k++)
                {
                    double x = pA.X + Math.Sign(diffX) * k * _dx;
                    double y = eqYLine(x);

                    markAsBroundary( new(x,y) );
                    // var (idx_x, idx_y) = ConvertToRegionIndex( new(x,y) );
                    // _isBoundary[idx_x, idx_y] = true;
                    
                }
            }
            else
            {
                for (int k = 0; k < Math.Abs(diffY) / _dy; k++)
                {
                    double y = pA.Y + Math.Sign(diffY) * k * _dy;
                    double x = eqXLine(y);

                    markAsBroundary( new(x,y) );
                    // var (idx_x, idx_y) = ConvertToRegionIndex( new(x,y) );
                    // _isBoundary[idx_x, idx_y] = true;

                }
            }

        }
    }


    // Fill internal region by scanning boundary (heuristic)
    // Note: This method assumes the boundary represents approximately-convex hull
    void FillInternalRegion()
    {
        for (int i = 0; i < _lenX; i++)
        {

            int boundaryCountSum = Enumerable.Range(0, _lenY).Select(idx => _isBoundary[i,idx] ? 1 : 0).Sum();

            if (boundaryCountSum <= 1) continue;

            int boundaryCount = 0;

            for (int j = 0; j < _lenY; j++)
            {

                if (_isBoundary[i,j])
                {
                    if ( j + 1 < _lenY && _isBoundary[i, j + 1] ) continue;

                    if ( j + 1 < _lenY )
                    {
                        int restCountSum = Enumerable.Range(j + 1, _lenY - j - 1).Select(idx => _isBoundary[i, idx] ? 1 : 0).Sum();
                        if (restCountSum < 1) break;
                    }

                    boundaryCount++;
                }
                else
                {
                    if (boundaryCount%2 == 1) _isInner[i,j] = true;
                }

            }
        }
    }

    override internal bool ContainsInRegion(Point point)
    {
        if (IsOutOfBoundBox(point)) return false;

        var (idx_x, idx_y) = ConvertToRegionIndex(point);

        return _isInner[idx_x, idx_y];
    }

    override internal bool OnBoundary(Point point)
    {
        if (IsOutOfBoundBox(point)) return false;

        var (idx_x, idx_y) = ConvertToRegionIndex(point);

        return _isBoundary[idx_x, idx_y] && !_isInner[idx_x, idx_y];    
    }

    (int idx_x, int idx_y) ConvertToRegionIndex(Point point)
    {
        var (x, y) = point;

        int idx_x = (int) Math.Round( (x-_xGrid[0]) / _dx );
        int idx_y = (int) Math.Round( (y-_yGrid[0]) / _dy );

        return (idx_x, idx_y);
    }

    // Point GetPositionOfIndex(int idx_x, int idx_y)
    // {
    //     double x = _xs + idx_x * _dx + _dx/2;
    //     double y = _ys + idx_y * _dy + _dy/2;

    //     return new Point(x,y);
    // }

}

