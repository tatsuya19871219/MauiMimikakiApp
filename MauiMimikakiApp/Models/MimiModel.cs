namespace MauiMimikakiApp.Models;

internal class MimiModel
{
    //readonly int _height;
    //readonly int _width;

    public MimiRegion Top => _topRegion;
    public MimiRegion Center => _centerRegion;
    public MimiRegion Bottom => _bottomRegion;

    readonly MimiRegion _topRegion;
    readonly MimiRegion _centerRegion;
    readonly MimiRegion _bottomRegion;


    // public MimiModel(int height, MimiRegion top, MimiRegion center, MimiRegion bottom)
    // {
    //     //_height = height;
    //     _topRegion = top;
    //     _centerRegion = center;
    //     _bottomRegion = bottom;
    // }

    public MimiModel(PathInternalRegion top, PathInternalRegion center, PathInternalRegion bottom)
    {
        _topRegion = new(top);
        _centerRegion = new(center);
        _bottomRegion = new(bottom);
    }
}

