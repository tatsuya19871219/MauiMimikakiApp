using Microsoft.Maui.Graphics.Converters;

namespace MauiMimikakiApp.Models;

internal class MimiDirt : ITrackerListener
{
    internal bool IsRemoved { get; private set; } = false;
    readonly internal Point Position;

    readonly MimiDirtConfig _config;

    readonly internal double Size;
    readonly double _hardness;
    readonly internal Color DirtColor;

    internal MimiDirt(Point position, MimiDirtConfig config)
    {
        Position = position;

        _config = config;

        Size = _config.size;

        _hardness = _config.hardness;

        var colorTypeConverter = new ColorTypeConverter();

        DirtColor = (Color)colorTypeConverter.ConvertFromString(_config.colorName);
    }

    public void OnMove(Point position, Point velocity, double dt)
    {
        if (position.Distance(Position) < Size)
        {
            IsRemoved = true;
        }
    }
}
