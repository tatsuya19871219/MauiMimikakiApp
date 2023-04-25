namespace MauiMimikakiApp.Models;

abstract class AbstractRegion
{
    protected Rect _bounds {get; init;}

    internal AbstractRegion(EdgeSet edgeSet)
    {
        if (!edgeSet.Closed) throw new ArgumentException("Given path is not closed.");

        GetMinAndMaxPoints(edgeSet, out var topLeft, out var bottomRight);

        var maxWidth = bottomRight.X - topLeft.X;
        var maxHeight = bottomRight.Y - topLeft.Y;

        _bounds = new Rect(topLeft, new Size(maxWidth, maxHeight));

    }

    abstract internal bool ContainsInRegion(Point point);
    abstract internal bool OnBoundary(Point point);
    abstract internal double DistanceFromBoundary(Point point);

    internal bool ContainsInRegion(double x, double y) => ContainsInRegion( new(x,y) );
    internal bool OnBoundary(double x, double y) => OnBoundary( new(x,y) );
    internal double DistanceFromBoundary(double x, double y) => DistanceFromBoundary( new(x,y) );

    void GetMinAndMaxPoints(EdgeSet edgeSet, out Point minPoint, out Point maxPoint)
    {
        double minX = double.PositiveInfinity;
		double minY = double.PositiveInfinity;
		double maxX = double.NegativeInfinity;
		double maxY = double.NegativeInfinity;

		foreach(var point in edgeSet.GetVerticies())
		{
            //minX = (point.X > minX) ? minX : point.X;

			if (point.X < minX) minX = point.X;
			if (point.X > maxX) maxX = point.X;
			if (point.Y < minY) minY = point.Y;
			if (point.Y > maxY) maxY = point.Y;
		}

        minPoint = new Point(minX, minY);
        maxPoint = new Point(maxX, maxY);
    }

}
