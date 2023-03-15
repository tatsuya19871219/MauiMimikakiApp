namespace MauiMimikakiApp.Models;

internal class MimiModel
{
    
    public MimiRegion Top => _topRegion;
    public MimiRegion Center => _centerRegion;
    public MimiRegion Bottom => _bottomRegion;

    readonly MimiRegion _topRegion;
    readonly MimiRegion _centerRegion;
    readonly MimiRegion _bottomRegion;

    public MimiModel(InternalRegion top, InternalRegion center, InternalRegion bottom)
    {
        _topRegion = new(top);
        _centerRegion = new(center);
        _bottomRegion = new(bottom);
    }
}

