namespace MauiMimikakiApp.Models;

public class MimiHair : ITrackerListener
{
    public Point Origin => _origin;
    public Point Position => _position;

    public double Thinness => _thinness;

    Point _origin;
    Point _position;
    
    //Point _force;

    double _thinness;
    readonly double _originalThinness;

    readonly double _springConst;

    public MimiHair(Point origin, double springConst = 0.5)
    {
        _origin = origin;
        _position = origin;

        _springConst = springConst;
        //_force = Point.Zero;

        _thinness = _originalThinness = 2.0;
    }

    // public void Displace(Point displacement)
    // {
    //     _position = _position.Offset(displacement.X, displacement.Y);

    //     UpdatePosition();
    // }

    async void UpdatePositionAsync(int milliSec = 100, double cutoffDisplacement = 0.2, double cutoffVelocity = 0.2)
    {
        double dt = (double)milliSec/1000;

        while(true)
        {
            var dx = _position.X - _origin.X;
            var dy = _position.Y - _origin.Y;

            var vx = - _springConst * dx;
            var vy = - _springConst * dy;

            var velocity = new Point(vx, vy);

            _position = _position.Offset(vx*dt, vy*dt);

            await Task.Delay(milliSec);

            if ( Math.Abs(velocity.Distance(Point.Zero)) < cutoffVelocity 
                && Math.Abs(_position.Distance(_origin)) < cutoffDisplacement) break;
        }

        _position = _origin;
    }

    
    public void OnMove(Point position, Point velocity, double milliSecUpdateInterval)
    {
        double dt = milliSecUpdateInterval;

        if (position.Distance(_origin) < 5)
        {
            _thinness = 1.2 * _originalThinness;
        }
        else 
        {
            _thinness = _originalThinness;
        }
    }
}
