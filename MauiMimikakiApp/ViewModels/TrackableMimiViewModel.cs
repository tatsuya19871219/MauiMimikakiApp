using Microsoft.Maui.Controls.Shapes;
using MauiMimikakiApp.CustomViews;
using TakeMauiEasy;

namespace MauiMimikakiApp.ViewModels;

public class TrackableMimiViewModel
{
    PositionTracker _tracker;
    TrackableView _trackableMimi;

    Dictionary<string, Geometry> _geometryDict;

    Action OnMoveOnMimi;

    public TrackableMimiViewModel()
    {

    }

    internal void BindTrackableMimi(TrackableView trackableMimi)
    {
        _trackableMimi = trackableMimi;
    }

    async internal Task InitializeDetectableGeometries(Geometry mimiTop, Geometry mimiCenter, Geometry mimiBottom)
    {
        if (_geometryDict is not null) throw new Exception("Detectable geometries are already set.");

        _geometryDict = new();

        _geometryDict.Add("top", mimiTop);
        _geometryDict.Add("center", mimiCenter);
        _geometryDict.Add("bottom", mimiBottom);
        
        // Make DetectableRegionViews
        var topRegionView = new DetectableRegionView(mimiTop);
        var centerRegionView = new DetectableRegionView(mimiCenter);
        var bottomRegionView = new DetectableRegionView(mimiBottom);

        // Get inctances of PathInternalRegion
        var topInternalRegion = topRegionView.GenerateInternalRegion();
        var centerInternalRegion = centerRegionView.GenerateInternalRegion();
        var bottomInternalRegion = bottomRegionView.GenerateInternalRegion();

        //// Add to the trackableMimi
        await _trackableMimi.AddDetectableRegionView(topRegionView);
        await _trackableMimi.AddDetectableRegionView(centerRegionView);
        await _trackableMimi.AddDetectableRegionView(bottomRegionView);
        
        ////
        await topRegionView.VisualizeRegion("boundary", Colors.Purple);
        await topRegionView.VisualizeRegion("inner", Colors.Gray);
        await centerRegionView.VisualizeRegion("boundary", Colors.Purple);
        await centerRegionView.VisualizeRegion("inner", Colors.Gray);
        await bottomRegionView.VisualizeRegion("boundary", Colors.Purple);
        await bottomRegionView.VisualizeRegion("inner", Colors.Gray);


        //await Task.Delay(5000);

    }

    void RunTrackerProcess()
    {

    }
}

