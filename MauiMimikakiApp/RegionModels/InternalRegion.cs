namespace MauiMimikakiApp.Models;

internal class InternalRegion : AbstractRegion
{
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

    override internal double DistanceFromBoundary(Point point)
    {
        return subRegions.Min( region => region.DistanceFromBoundary(point) );
    }
    
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
