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

        _tracker = new PositionTracker(_trackableMimi);
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
        _trackableMimi.AddDetectableRegionView(topRegionView);
        _trackableMimi.AddDetectableRegionView(centerRegionView);
        _trackableMimi.AddDetectableRegionView(bottomRegionView);
        
        //List<Task> tasks = new();

        //int stepsVisualization = 1;

        //tasks.Add( topRegionView.VisualizeRegion("boundary", Colors.Purple) );
        //tasks.Add( topRegionView.VisualizeRegion("inner", Colors.Gray, stepsVisualization) );
        //tasks.Add( centerRegionView.VisualizeRegion("boundary", Colors.Purple) );
        //tasks.Add( centerRegionView.VisualizeRegion("inner", Colors.Gray, stepsVisualization) );
        //tasks.Add( bottomRegionView.VisualizeRegion("boundary", Colors.Purple) );
        //tasks.Add( bottomRegionView.VisualizeRegion("inner", Colors.Gray, stepsVisualization) );

        //await Task.WhenAll(tasks);
    }

    internal void InvokeTrackerProcess()
    {

    }

    void RunTrackerProcess()
    {

    }
}

