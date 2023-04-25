namespace MauiMimikakiApp.Models;

internal class InternalRegion : AbstractRegion
{
    public Rect Bounds => _bounds;
    readonly List<SubRegion> subRegions = new();
    readonly Random _rnd = new();

    public InternalRegion(EdgeSet edgeSet, int dx = 5, int dy = 5) : base(edgeSet)
    {
        List<EdgeSet> edgeSetList = edgeSet.Separate();

        foreach(var subEdgeSet in edgeSetList)
        {                         
            var countDuplicates = subEdgeSet.Select(edge => edgeSet.Where(e => e.Equals(edge)).Count()-1).ToList();

            List<Edge> sharedEdges = subEdgeSet.Where((value, idx) => countDuplicates[idx] > 0).ToList();

            subRegions.Add( new(subEdgeSet, sharedEdges, dx, dy) );
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
            var tryX = GenerateRandomUniform(_bounds.Left, _bounds.Right);
            var tryY = GenerateRandomUniform(_bounds.Top, _bounds.Bottom);

            Point point = new(tryX, tryY);

            if (ContainsInRegion(point)) return point;
        }
    }

    double GenerateRandomUniform(double minValue, double maxValue)
    {
        return _rnd.NextDouble() * (maxValue - minValue) + minValue;
    }

}
