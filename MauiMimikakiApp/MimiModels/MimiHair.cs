using Microsoft.Maui.Graphics.Converters;

namespace MauiMimikakiApp.Models;

internal class MimiHair : ITrackerListener
{
    internal Point Origin => _origin;
    internal Point Position => _position;
    internal double Thinness => _thinness;
    readonly internal Color HairColor;

    Point _origin;
    Point _position;
    
    //Point _force;

    double _thinness;
    readonly double _originalThinness;

    readonly double _springConst;

    readonly MimiHairConfig _config;

    internal MimiHair(Point origin, MimiHairConfig config)
    {
        _origin = origin;
        _position = origin;

        _config = config;

        _springConst = _config.springConst;
        //_force = Point.Zero;

        _thinness = _originalThinness = _config.thinness;

        var colorTypeConverter = new ColorTypeConverter();

        HairColor = (Color) colorTypeConverter.ConvertFromString(_config.colorName);
    }

    
    
    public void OnMove(Point position, Point velocity, double dt)
    {
        var distance = position.Distance(_origin);
        
        if (distance < _config.thinness*3)
        {
            var diff = new Point(position.X-_origin.X, position.Y-_origin.Y);
            
            double amplitude = 5;

            double w = amplitude/diff.Distance(Point.Zero);

            var displacement = new Point(-w*diff.X, -w*diff.Y);

            Displace(displacement);
        }
    }

    void Displace(Point displacement)
    {
        _position = _position.Offset(displacement.X, displacement.Y);

        UpdatePositionAsync();
    }

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
}
