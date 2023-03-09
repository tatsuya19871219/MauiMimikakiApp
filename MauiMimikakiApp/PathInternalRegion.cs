using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp;

internal class PathInternalRegion
{
    public bool[,] IsInner => _isInner;
    public bool[,] IsBoundary => _isBoundary;

    public int Xs => _xs;
    public int Ys => _ys;
    public int Xe => _xe;
    public int Ye => _ye;

    public int Dx => _dx;
    public int Dy => _dy;

    public int LenX => _lenX;
    public int LenY => _lenY;


    readonly bool[,] _isInner;
    readonly bool[,] _isBoundary;

    readonly int _xs;
    readonly int _ys;
    readonly int _xe;
    readonly int _ye;

    readonly int _dx;
    readonly int _dy;

    readonly int _lenX;
    readonly int _lenY;

    readonly Point _topLeft;
    readonly Point _bottomRight;


    public PathInternalRegion(PathF pathF, int dx = 5, int dy = 5)
    {
        if (!pathF.Closed) throw new ArgumentException("Given path is not closed.");

        _dx = dx;
        _dy = dy;

        GetMinAndMaxPoints(pathF, out _topLeft, out _bottomRight);

        _xs = (int)Math.Floor((double)_topLeft.X / dx) * dx;
		_ys = (int)Math.Floor((double)_topLeft.Y / dy) * dy;

		_xe = (int)Math.Ceiling((double)_bottomRight.X / dx) * dx;
		_ye = (int)Math.Ceiling((double)_bottomRight.Y / dy) * dy;


        _lenX = (_xe - _xs) / dx + 1;
		_lenY = (_ye - _ys) / dy + 1;

        _isInner = new bool[_lenX, _lenY];
        _isBoundary = new bool[_lenX, _lenY];

        FillBoundaryPoints(pathF);

        FillInternalRegion();
    }

    void GetMinAndMaxPoints(PathF pathF, out Point minPoint, out Point maxPoint)
    {
        double minX = double.PositiveInfinity;
		double minY = double.PositiveInfinity;
		double maxX = double.NegativeInfinity;
		double maxY = double.NegativeInfinity;

		for (int i = 0; i < pathF.Count; i++)
		{
			var path = pathF[i];

			if (path.X < minX) minX = path.X;
			if (path.X > maxX) maxX = path.X;
			if (path.Y < minY) minY = path.Y;
			if (path.Y > maxY) maxY = path.Y;
		}

        minPoint = new Point(minX, minY);
        maxPoint = new Point(maxX, maxY);
    }

    void FillBoundaryPoints(PathF pathF)
    {
        for (int i = 0; i < pathF.Count; i++)
        {
            Point pA = pathF[i];
            Point pB = (i < pathF.Count - 1) ? pathF[i+1] : pathF[0];

            if (pA.Equals(pB)) continue;

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

                    var (idx_x, idx_y) = ConvertToRegionIndex(x,y);
                    _isBoundary[idx_x, idx_y] = true;
                    
                }
            }
            else
            {
                for (int k = 0; k < Math.Abs(diffY) / _dy; k++)
                {
                    double y = pA.Y + Math.Sign(diffY) * k * _dy;
                    double x = eqXLine(y);

                    var (idx_x, idx_y) = ConvertToRegionIndex(x,y);
                    _isBoundary[idx_x, idx_y] = true;

                }
            }
        }
    }


    (int idx_x, int idx_y) ConvertToRegionIndex(double x, double y)
    {
        int idx_x = (int) Math.Round( (x-_xs) / _dx );
        int idx_y = (int) Math.Round( (y-_ys) / _dy );

        return (idx_x, idx_y);
    }


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


    public bool ContainsInRegion(Point point) => ContainsInRegion(point.X, point.Y);
    public bool ContainsInRegion(double x, double y)
    {
        if (x < _xs || x > _xe) return false;
        if (y < _ys || y > _ye) return false;

        var (idx_x, idx_y) = ConvertToRegionIndex(x, y);

        return _isInner[idx_x, idx_y] || _isBoundary[idx_x, idx_y];
    }
}
