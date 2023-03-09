using Microsoft.Maui.Controls.Shapes;

namespace MauiMimikakiApp.CustomViews;

public partial class DetectableRegionView : ContentView
{

    internal readonly PathInternalRegion InternalRegion;
    readonly Shape[,] _regionDots;

    public DetectableRegionView(Geometry geometry)
    {
        InitializeComponent();

        var regionPath = new Microsoft.Maui.Controls.Shapes.Path(geometry);
        
        regionPath.Stroke = Colors.Red;
        regionPath.StrokeThickness = 2;
        regionPath.Fill = Colors.White;

        RegionPathGrid.Add(regionPath);

        PathInternalRegion pathRegion = new PathInternalRegion(regionPath.GetPath());

        InternalRegion = pathRegion;

        _regionDots = new Shape[pathRegion.LenX, pathRegion.LenY];


        ShowFlaggedRegion(pathRegion, pathRegion.IsBoundary, Colors.Purple);

        ShowFlaggedRegion(pathRegion, pathRegion.IsInner, Colors.LightGray);


    }


    void ShowFlaggedRegion(PathInternalRegion region, bool[,] flags, Color color)
	{
        int totalLength = flags.Length;
        int dimensions = flags.Rank;

        int lenX = flags.GetUpperBound(0) + 1;
        int lenY = flags.GetUpperBound(1) + 1;

        if (lenX != region.LenX || lenY != region.LenY) throw new ArgumentException("Size of flags does not match with the region instance.");

        for (int i = 0; i < region.LenX; i++)
        {
            for (int j = 0; j < region.LenY; j++)
            {
                if (!flags[i, j]) continue;

                int x = region.Xs + i * region.Dx;
                int y = region.Ys + j * region.Dy;

                Ellipse dotMark = new Ellipse { WidthRequest = 5, HeightRequest = 5, Fill = color };

                dotMark.HorizontalOptions = LayoutOptions.Start;
                dotMark.VerticalOptions = LayoutOptions.Start;

                dotMark.TranslationX = x - dotMark.WidthRequest / 2;
                dotMark.TranslationY = y - dotMark.HeightRequest / 2;

                RegionPathGrid.Add(dotMark);

                _regionDots[i,j] = dotMark;
            }
        }
    }

}