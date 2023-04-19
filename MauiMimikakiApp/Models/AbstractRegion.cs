using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

abstract class AbstractRegion
{
    internal PathF OriginalPath {get; init;}
    internal Rect Bounds {get; init;}

    internal AbstractRegion(PathF pathF)
    {
        if (!pathF.Closed) throw new ArgumentException("Given path is not closed.");

        OriginalPath = pathF;

        GetMinAndMaxPoints(pathF, out var topLeft, out var bottomRight);

        var maxWidth = bottomRight.X - topLeft.X;
        var maxHeight = bottomRight.Y - topLeft.Y;

        Bounds = new Rect(topLeft, new Size(maxWidth, maxHeight));

    }

    abstract internal bool ContainsInRegion(Point point);
    abstract internal bool OnBoundary(Point point);

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

    internal bool IsOutOfBoundBox(Point point)
    {
        var (x, y) = point;

        if (x < Bounds.Left || x > Bounds.Right) return true;
        if (y < Bounds.Top || y > Bounds.Bottom) return true;

        return false;
    }

}
