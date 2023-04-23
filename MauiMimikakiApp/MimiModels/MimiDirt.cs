namespace MauiMimikakiApp.Models;

internal class MimiDirt : ITrackerListener
{
    internal bool IsRemoved { get; private set; } = false;
    readonly internal Point Position;
    //readonly double hardness;

    readonly MimiDirtConfig _config;

    readonly internal double Size;

    internal MimiDirt(Point position, MimiDirtConfig config)
    {
        Position = position;

        _config = config;

        Size = _config.Size;
    }

    public void OnMove(Point position, Point velocity, double dt)
    {
        if (position.Distance(Position) < Size)
        {
            IsRemoved = true;
        }
    }
}
