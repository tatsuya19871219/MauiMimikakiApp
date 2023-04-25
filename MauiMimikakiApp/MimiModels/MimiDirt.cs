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

    double _amountOfStimulus = 0;

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
        var speed = velocity.Distance(Point.Zero);

        // set response radius regarding the speed
        var responseSize = (2 + Math.Min(speed, 1)) * this.Size;

        if (position.Distance(this.Position) < responseSize)
        {
            _amountOfStimulus += 100*speed*dt;

            if (_amountOfStimulus > _hardness) IsRemoved = true;
        }
    }
}
