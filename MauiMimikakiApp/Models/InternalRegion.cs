using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp;

public class InternalRegion
{
    //public bool[,] IsInner => _isInner;
    //public bool[,] IsBoundary => _isBoundary;
    readonly public Rect Bounds;
    // public double MaxWidth => _bottomRight.X - _topLeft.X;
    // public double MaxHeight => _bottomRight.Y - _topLeft.Y;
    // public Point TopLeft => _topLeft;
    // public Point BottomRight => _bottomRight;
    // public Point Center => TopLeft.Offset(MaxWidth/2, MaxHeight/2);

    readonly PathF _originalPath;
    internal PathF GetOriginalPath() => _originalPath;

    readonly Point _topLeft;
    readonly Point _bottomRight;

    // public int Xs => _xs;
    // public int Ys => _ys;
    // public int Xe => _xe;
    // public int Ye => _ye;

    // public int Dx => _dx;
    // public int Dy => _dy;

    // public int LenX => _lenX;
    // public int LenY => _lenY;

    // readonly Dictionary<string, bool[,]> _dict = new();
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

    readonly Random _rnd = new();

    // Indexer
    // public bool[,] this[string key]
    // {
    //     get
    //     {
    //         if (!_dict.ContainsKey(key)) throw new ArgumentException("Key should be 'inner' or 'boundary'.");
    //         return _dict[key];
    //     }
    // }

    public InternalRegion(PathF pathF, int dx = 5, int dy = 5)
    {
        if (!pathF.Closed) throw new ArgumentException("Given path is not closed.");

        _originalPath = pathF;

        _dx = dx;
        _dy = dy;

        GetMinAndMaxPoints(pathF, out _topLeft, out _bottomRight);

        var maxWidth = _bottomRight.X - _topLeft.X;
        var maxHeight = _bottomRight.Y - _topLeft.Y;

        Bounds = new Rect(_topLeft, new Size(maxWidth, maxHeight));

        _xs = (int)Math.Floor((double)_topLeft.X / dx) * dx;
		_ys = (int)Math.Floor((double)_topLeft.Y / dy) * dy;

		_xe = (int)Math.Ceiling((double)_bottomRight.X / dx) * dx;
		_ye = (int)Math.Ceiling((double)_bottomRight.Y / dy) * dy;


        _lenX = (_xe - _xs) / dx + 1;
		_lenY = (_ye - _ys) / dy + 1;

        _isInner = new bool[_lenX, _lenY];
        _isBoundary = new bool[_lenX, _lenY];

        // _dict.Add("inner", _isInner);
        // _dict.Add("boundary", _isBoundary);

        FillBoundaryPoints(pathF);

        //FillInternalRegion();
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

    bool IsOutOfBoundBox(double x, double y)
    {
        if (x < _topLeft.X || x > _bottomRight.X) return true;
        if (y < _topLeft.Y || y > _bottomRight.Y) return true;

        return false;
    }


    //public bool ContainsInRegion(Point point) => ContainsInRegion(point.X, point.Y);
    public bool ContainsInRegion(double x, double y)
    {
        // if (x < _xs || x > _xe) return false;
        // if (y < _ys || y > _ye) return false;

        if (IsOutOfBoundBox(x, y)) return false;

        var (idx_x, idx_y) = ConvertToRegionIndex(x, y);

        return _isInner[idx_x, idx_y];
    }

    //public bool IsBoundary(Point point) => IsBoundary(point.X, point.Y);
    public bool IsBoundary(double x, double y)
    {
        // if (x < _xs || x > _xe) return false;
        // if (y < _ys || y > _ye) return false;

        if (IsOutOfBoundBox(x, y)) return false;

        var (idx_x, idx_y) = ConvertToRegionIndex(x, y);

        return _isBoundary[idx_x, idx_y];
    }

    public double? DistanceFromBoundary(double x, double y)
    {

        //if (IsOutOfBoundBox(x, y)) return null;
        if (!ContainsInRegion(x, y)) return null;
        if (IsBoundary(x, y)) return 0;

        var (idx_x, idx_y) = ConvertToRegionIndex(x, y);

        int idxFromBoundX = Math.Min(idx_x, _lenX-idx_x-1);
        int idxFromBoundY = Math.Min(idx_y, _lenY-idx_y-1);    

        double minDistance = double.PositiveInfinity;

        for (int i = -idxFromBoundX; i < idxFromBoundX; i++)
        {
            for (int j = -idxFromBoundY; j < idxFromBoundY; j++)
            {
                if (_isBoundary[idx_x+i, idx_y+j])
                {
                    Point point = new Point(_dx*i, _dy*j);
                    double distance = point.Distance(Point.Zero);

                    if (minDistance > distance) minDistance = distance;
                }
            }
        }

        return minDistance;
    }

    public Point GeneratePointInRegion()
    {
        double tryX; // = GenerateRandomUniform(Bounds.Left, Bounds.Right);
        double tryY; // = GenerateRandomUniform(Bounds.Top, Bounds.Bottom);

        while (true)
        {
            tryX = GenerateRandomUniform(Bounds.Left, Bounds.Right);
            tryY = GenerateRandomUniform(Bounds.Top, Bounds.Bottom);

            if (ContainsInRegion(tryX, tryY)) break;
        }

        return new Point(tryX, tryY);
    }

    double GenerateRandomUniform(double minValue, double maxValue)
    {
        return _rnd.NextDouble() * (maxValue - minValue) + minValue;
    }
}
