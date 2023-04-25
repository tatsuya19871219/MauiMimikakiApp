namespace MauiMimikakiApp;

internal interface ITrackerListener
{
    void OnMove(Point position, Point velocity, double milliSecUpdateInterval);
}
