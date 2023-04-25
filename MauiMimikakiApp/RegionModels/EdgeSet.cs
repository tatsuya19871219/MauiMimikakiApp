using System.Collections;
using System.Diagnostics;

namespace MauiMimikakiApp.Models;

record Edge(PointF a, PointF b)
{
    public virtual bool Equals(Edge target)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (target is null) return false;

        if (this.a.Equals(target.a) && this.b.Equals(target.b)) return true;
        if (this.a.Equals(target.b) && this.b.Equals(target.a)) return true;

        return false;
    }
    
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        //throw new System.NotImplementedException();
        return base.GetHashCode();
    }
}

internal class EdgeSet : IEnumerable<Edge>
{
    readonly PathF _pathF;
    readonly List<Edge> _edges = new();
    readonly IEnumerable<PointF> _verticies;
    readonly public bool Closed;

    public EdgeSet(PathF pathF)
    {
        _pathF = pathF;
        _verticies = _pathF.Points;

        Closed = _pathF.Closed;

        PointF s = new(); // move point
        PointF a = new();
        PointF b = new();

        for (int i = 0; i < pathF.SegmentTypes.Count(); i++)
        {
            var segType = pathF.GetSegmentType(i);

            PointF[] seg = pathF.GetPointsForSegment(i);

            switch (segType)
            {
                case PathOperation.Move:
                    
                    s = a = seg[0]; 
                    break;

                case PathOperation.Line:
                    
                    b = seg[0];

                    _edges.Add(new Edge(a, b));

                    a =  b;
                    break;

                case PathOperation.Close:

                    a = _edges.Last().b;
                    b = s;

                    _edges.Add(new Edge(a, b));

                    s = new();
                    break;

                default:

                    throw new Exception("Unexpected PathOperation");

            }                        
        }
    }

    internal List<EdgeSet> Separate()
    {
        List<EdgeSet> edgeSetList = new();

        foreach (var path in _pathF.Separate())
        {
            edgeSetList.Add( new(path) );
        }

        return edgeSetList;
    }

    internal IEnumerable<PointF> GetVerticies() => _verticies;

    public IEnumerator<Edge> GetEnumerator()
    {
        foreach(var e in _edges) yield return e;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}