namespace MauiMimikakiApp.Models;

internal class MimiRegion
{
    public Rect Bounds => _internalRegion.Bounds;
    public IEnumerable<MimiHair> Hairs => _hairs;
    public IEnumerable<MimiDirt> Dirts => _dirts;
    
    readonly InternalRegion _internalRegion;
    readonly List<MimiHair> _hairs = new();
    readonly List<MimiDirt> _dirts = new();

    //readonly MimikakiConfig _config;
    readonly ModelParams _modelParams;
    MimiHairConfig _hairConfig => _modelParams.MimiHair;
    MimiDirtConfig _dirtConfig => _modelParams.MimiDirt;

    internal MimiRegion(PathF pathF, ModelParams modelParams)
    {
        //_config = config;
        _modelParams = modelParams;

        int dx = _modelParams.RegionRoughness.DX;
        int dy = _modelParams.RegionRoughness.DY;

        _internalRegion = new InternalRegion(new EdgeSet(pathF), dx, dy);

        // initialize mimi hairs
        //InitializeMimiHair(0.2);
        InitializeMimiHair(_modelParams.MimiHairDensity);

    }

    void InitializeMimiHair(double density)
    {
        double dx = 1/density;
        double dy = 1/density;

        double hairMargin = _hairConfig.Thinness * 1.5;

        Point center = Bounds.Center;

        var maxWidth = Bounds.Width;
        var maxHeight = Bounds.Height;

        // Discritize the region and align the center
        var x0 = Math.Round((center.X - maxWidth / 2 - dx) / dx) * dx;
        var x1 = Math.Round((center.X + maxWidth / 2 + dx) / dx) * dx;

        var y0 = Math.Round((center.Y - maxHeight / 2 - dy) / dy) * dy;
        var y1 = Math.Round((center.Y + maxHeight / 2 + dy) / dy) * dy;

        for (double x = x0; x <= x1; x+=dx)
        {
            for (double y = y0; y <= y1; y+=dy)
            {
                if (_internalRegion.ContainsInRegion(x,y)
                    && _internalRegion.DistanceFromBoundary(x,y) > hairMargin)
                            _hairs.Add(new MimiHair(new Point(x,y), _hairConfig));
            }
        }
    }

    internal void GenerateMimiDirt()
    {
        Point point = _internalRegion.GeneratePointInRegion();

        _dirts.Add(new MimiDirt(point, _dirtConfig));
    }

    internal void RemoveMimiDirt(MimiDirt dirt)
    {
        _dirts.Remove(dirt);
    }

    internal bool Contains(Point point)
    {
        return _internalRegion.ContainsInRegion(point);
    }
}
