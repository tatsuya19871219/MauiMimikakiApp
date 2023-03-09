using Microsoft.Maui.Controls.Shapes;

namespace MauiMimikakiApp.CustomViews;

public partial class DetectableRegionView : ContentView
{
    readonly Microsoft.Maui.Controls.Shapes.Path _regionPath;
    
    readonly bool _isVisible;

    Shape[,] _regionDots;

    PathInternalRegion _internalRegion;

    public DetectableRegionView(Geometry geometry, bool isVisible = true)
    {
        InitializeComponent();

        _regionPath = new(geometry);
        
        _regionPath.Stroke = Colors.Red;
        _regionPath.StrokeThickness = 2;
        _regionPath.Fill = Colors.White;

        _regionPath.IsVisible = _isVisible = isVisible;

        RegionPathGrid.Add(_regionPath);
        
    }

    internal PathInternalRegion GenerateInternalRegion(int dx = 5, int dy = 5)
    {
        if (_internalRegion is not null) throw new Exception("Internal region is already initialized.");

        _internalRegion = new(_regionPath.GetPath(), dx, dy);

        _regionDots = new Shape[_internalRegion.LenX, _internalRegion.LenY];

        return _internalRegion;
    }

    internal async Task VisualizeRegion(string key, Color color, int steps = 1)
    {
        if (_internalRegion is null) throw new Exception("Internal region is not generated.");
        if (!_isVisible) throw new Exception("IsVisible is set as false.");

        bool[,] flags = _internalRegion[key];

        int totalLength = flags.Length;
        int dimensions = flags.Rank;

        int lenX = flags.GetUpperBound(0) + 1;
        int lenY = flags.GetUpperBound(1) + 1;

        if (lenX != _internalRegion.LenX || lenY != _internalRegion.LenY) throw new ArgumentException("Size of flags does not match with the region instance.");

        for (int i = 0; i < _internalRegion.LenX; i+=steps)
        {
            for (int j = 0; j < _internalRegion.LenY; j+=steps)
            {
                if (!flags[i, j]) continue;

                await AddDotMark(i, j, color);
            }
        }

    }

    async Task AddDotMark(int i, int j, Color color)
    {
        int x = _internalRegion.Xs + i * _internalRegion.Dx;
        int y = _internalRegion.Ys + j * _internalRegion.Dy;

        Ellipse dotMark = new Ellipse { WidthRequest = 5, HeightRequest = 5, Fill = color };

        dotMark.HorizontalOptions = LayoutOptions.Start;
        dotMark.VerticalOptions = LayoutOptions.Start;

        dotMark.TranslationX = x - dotMark.WidthRequest / 2;
        dotMark.TranslationY = y - dotMark.HeightRequest / 2;

        RegionPathGrid.Add(dotMark);

        _regionDots[i,j] = dotMark;

        await Task.Delay(1);
    }

}