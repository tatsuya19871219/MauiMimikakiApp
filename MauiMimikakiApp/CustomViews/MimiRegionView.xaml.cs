using Microsoft.Maui.Controls.Shapes;

namespace MauiMimikakiApp.CustomViews;

public partial class MimiRegionView : ContentView
{
    readonly Microsoft.Maui.Controls.Shapes.Path _regionPath;
    
    readonly bool _isVisible;

    Shape[,] _regionDots;

    DetectableRegionDrawable _drawable;

    PathInternalRegion _internalRegion;

    public MimiRegionView(Geometry geometry, bool isVisible = true)
    {
        InitializeComponent();

        _regionPath = new(geometry);
        
        // _regionPath.Stroke = Colors.Red;
        // _regionPath.StrokeThickness = 2;
        // _regionPath.Fill = Colors.White;

        // _regionPath.IsVisible =
        _isVisible = isVisible;

        //egionPathGrid.Add(_regionPath);
        
    }

    internal PathInternalRegion GenerateInternalRegion(int dx = 5, int dy = 5)
    {
        if (_internalRegion is not null) throw new Exception("Internal region is already initialized.");

        _internalRegion = new(_regionPath.GetPath(), dx, dy);

        _regionDots = new Shape[_internalRegion.LenX, _internalRegion.LenY];

        //
        _drawable = new(_internalRegion);
        //RegionPathGrid.Add(new GraphicsView {Drawable=drawable, HeightRequest=200, WidthRequest=200});
        // RegionPathGrid.Add(
        //     new GraphicsView {Drawable=_drawable, 
        //                         HeightRequest=_drawable.HeightRequest, WidthRequest=_drawable.WidthRequest,
        //                         TranslationX=_drawable.OffsetX, TranslationY=_drawable.OffsetY,
        //                         HorizontalOptions=LayoutOptions.Start, VerticalOptions=LayoutOptions.Start}
        //     );

        RegionDrawable.HeightRequest = _drawable.HeightRequest;
        RegionDrawable.WidthRequest = _drawable.WidthRequest;
        RegionDrawable.TranslationX = _drawable.OffsetX;
        RegionDrawable.TranslationY = _drawable.OffsetY;

        RegionDrawable.Drawable = _drawable;

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

        //await Task.Delay(10);

        if (lenX != _internalRegion.LenX || lenY != _internalRegion.LenY) throw new ArgumentException("Size of flags does not match with the region instance.");

        for (int i = 0; i < _internalRegion.LenX; i+=steps)
        {
            for (int j = 0; j < _internalRegion.LenY; j+=steps)
            {
                if (!flags[i, j]) continue;

                await AddDotMark(i, j, color);

                await Task.Delay(1);
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

        //await Task.Delay(1);
        await Task.Delay(0);
    }

    // Drawable
    class DetectableRegionDrawable : IDrawable
    {

        public double WidthRequest {get; init;}
        public double HeightRequest {get; init;}

        public double OffsetX => _offsetX;
        public double OffsetY => _offsetY;

        readonly PathInternalRegion _region;

        readonly float _padding;
        readonly double _offsetX;
        readonly double _offsetY;

        public DetectableRegionDrawable(PathInternalRegion region, double padding = 20) : base()
        {
            _region = region;

            _padding = (float)padding;

            _offsetX = _region.Xs - _padding;
            _offsetY = _region.Ys - _padding;

            WidthRequest = _region.Xe - _region.Xs + 2*_padding;
            HeightRequest = _region.Ye - _region.Ys + 2*_padding;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.Purple;
            canvas.Alpha = 0.3f;
            canvas.FillRectangle(0, 0, (float)WidthRequest, (float)HeightRequest);

            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 2;
            canvas.DrawCircle(_padding, _padding, 4);

            canvas.FillColor = Colors.Blue;
            VisualizeRegion(canvas, "boundary", Colors.Red);

            canvas.FillColor = Colors.Pink;
            VisualizeRegion(canvas, "inner", Colors.Pink);
        }

        void VisualizeRegion(ICanvas canvas, string key, Color color, int steps = 1)
        {
            if (_region is null) throw new Exception("Internal region is not generated.");
            //if (!_isVisible) throw new Exception("IsVisible is set as false.");

            bool[,] flags = _region[key];

            int totalLength = flags.Length;
            int dimensions = flags.Rank;

            int lenX = flags.GetUpperBound(0) + 1;
            int lenY = flags.GetUpperBound(1) + 1;

            
            if (lenX != _region.LenX || lenY != _region.LenY) throw new ArgumentException("Size of flags does not match with the region instance.");

            for (int i = 0; i < _region.LenX; i+=steps)
            {
                for (int j = 0; j < _region.LenY; j+=steps)
                {
                    if (!flags[i, j]) continue;

                    //DrawDotMark(i, j, color);
                    double x = _region.Xs + i * _region.Dx - _offsetX;
                    double y = _region.Ys + j * _region.Dy - _offsetY;

                    canvas.FillCircle( (float)x, (float)y, 3);

                    //await Task.Delay(1);
                }
            }

        }

        // void DrawDotMark(int i, int j, Color color)
        // {
        //     int x = _region.Xs + i * _region.Dx;
        //     int y = _region.Ys + j * _region.Dy;



        //     // Ellipse dotMark = new Ellipse { WidthRequest = 5, HeightRequest = 5, Fill = color };

        //     // dotMark.HorizontalOptions = LayoutOptions.Start;
        //     // dotMark.VerticalOptions = LayoutOptions.Start;

        //     // dotMark.TranslationX = x - dotMark.WidthRequest / 2;
        //     // dotMark.TranslationY = y - dotMark.HeightRequest / 2;

        //     // RegionPathGrid.Add(dotMark);

        //     // _regionDots[i,j] = dotMark;

        //     // //await Task.Delay(1);
        //     // await Task.Delay(0);
        // }

    }
}