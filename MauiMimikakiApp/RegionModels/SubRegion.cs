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

    readonly List<Point> _boundaryList;

    public SubRegion(PathF pathF, List<PointF> sharedVertices, int dx = 5, int dy = 5) : base(pathF)
    {
        _dx = dx;
        _dy = dy;

        var margin = 2*Math.Max(dx, dy);

        var xs = (int)Math.Floor(Bounds.Left / dx) * dx - margin;
		var ys = (int)Math.Floor(Bounds.Top / dy) * dy - margin;

		var xe = (int)Math.Ceiling(Bounds.Right / dx) * dx + margin;
		var ye = (int)Math.Ceiling(Bounds.Bottom / dy) * dy + margin;

        var lenX = (xe - xs) / dx + 1;
		var lenY = (ye - ys) / dy + 1;

        _xGrid = Enumerable.Range(0, lenX).Select(i => xs + i*dx).ToList();
        _yGrid = Enumerable.Range(0, lenY).Select(i => ys + i*dy).ToList();

        _isInner = new bool[lenX, lenY];
        _isBoundary = new bool[lenX, lenY];

        FillBoundaryPoints(pathF, sharedVertices);

        FillInternalRegion();

        _boundaryList = GetBoundaryPointList();
    }

    // Fill boundary by linear interpolation
    void FillBoundaryPoints(PathF pathF, List<PointF> sharedVertices)
    {
        for (int i = 0; i < pathF.Count; i++)
        {
            Point pA = pathF[i];
            Point pB = (i < pathF.Count - 1) ? pathF[i+1] : pathF[0];

            if (pA.Equals(pB)) continue;

            bool checkAsRegion = sharedVertices.Contains(pA) && sharedVertices.Contains(pB);

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
                }
            }
            else
            {
                for (int k = 0; k < Math.Abs(diffY) / _dy; k++)
                {
                    double y = pA.Y + Math.Sign(diffY) * k * _dy;
                    double x = eqXLine(y);

                    markAsBroundary( new(x,y) );                    
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

    internal override double DistanceFromBoundary(Point point)
    {
        if (OnBoundary(point)) return 0;

        var distanceList = _boundaryList.Select( p => p.Distance(point) ).ToList();

        return distanceList.Min();
    }

    // internal override double DistanceFromBoundary(Point point)
    // {

    //     if (!ContainsInRegion(point)) return double.NaN;
    //     if (OnBoundary(point)) return 0;

    //     var (idx_x, idx_y) = ConvertToRegionIndex(point);

    //     int idxFromBoundX = Math.Min(idx_x, _lenX-idx_x-1);
    //     int idxFromBoundY = Math.Min(idx_y, _lenY-idx_y-1);    

    //     double minDistance = double.PositiveInfinity;

    //     for (int i = -idxFromBoundX; i < idxFromBoundX; i++)
    //     {
    //         for (int j = -idxFromBoundY; j < idxFromBoundY; j++)
    //         {
    //             if (_isBoundary[idx_x+i, idx_y+j])
    //             {
    //                 var p = new Point(_dx*i, _dy*j);
    //                 double distance = p.Distance(Point.Zero);

    //                 if (minDistance > distance) minDistance = distance;
    //             }
    //         }
    //     }

    //     return minDistance;
    // }

    (int idx_x, int idx_y) ConvertToRegionIndex(Point point)
    {
        var (x, y) = point;

        int idx_x = (int) Math.Round( (x-_xGrid[0]) / _dx );
        int idx_y = (int) Math.Round( (y-_yGrid[0]) / _dy );

        return (idx_x, idx_y);
    }

    Point GetPositionOfIndex(int idx_x, int idx_y)
    {
        double x = _xGrid[0] + idx_x * _dx + _dx/2;
        double y = _yGrid[0] + idx_y * _dy + _dy/2;

        return new Point(x,y);
    }

    List<Point> GetBoundaryPointList()
    {
        List<Point> boundary = new();

        for (int j = 0; j < _lenY; j++)
        {
            for (int i = 0; i <_lenX; i++)
            {
                if (_isBoundary[i,j])
                    boundary.Add( GetPositionOfIndex(i, j) );
            }
        }

        return boundary;
    }

}

