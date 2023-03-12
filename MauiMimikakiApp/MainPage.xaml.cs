using MauiMimikakiApp.CustomViews;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;

using TakeMauiEasy;

namespace MauiMimikakiApp;

public partial class MainPage : ContentPage
{
	readonly TrackableMimiViewModel _vm;
	//PositionTracker _tracker;


	public MainPage(TrackableMimiViewModel vm)
	{
		InitializeComponent();

		_vm = vm;

		InitializeTrackableMimi();
	}


	async void InitializeTrackableMimi()
	{

		await MimiTrackableView.RequestTargetHeight((double)PathDictionary["MimiExpectedHeight"]);

		// // not needed
		// while (true)
		// {
		// 	if (MimiTrackableView.Width > 0) break;
		// 	await Task.Delay(100);
		// }

		_vm.BindTrackableMimi(MimiTrackableView);

        var mimiTopGeometry = GetGeometryFromString(PathDictionary["MimiTopPathGeometryString"] as string);
        var mimiCenterGeometry = GetGeometryFromString(PathDictionary["MimiCenterPathGeometryString"] as string);
        var mimiBottomGeometry = GetGeometryFromString(PathDictionary["MimiBottomPathGeometryString"] as string);

		await _vm.InitializeDetectableGeometries(mimiTopGeometry, mimiCenterGeometry, mimiBottomGeometry);	

		//_vm.InitializeDetectableGeometries(mimiTopGeometry, mimiCenterGeometry, mimiBottomGeometry);

        // _tracker = MimiTrackableView.GetPositionTracker();

		// RunTrackerProcess();

		_vm.OnMoveOnMimi += (state) =>
		{
			Point position = state.Position;

			double velocity = state.Velocity.Distance(Point.Zero);

			FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

		};

		_vm.InvokeTrackerProcess(100);

	}

	
	Geometry GetGeometryFromString(string pathString)
	{
		return (Geometry)new PathGeometryConverter().ConvertFromInvariantString(pathString);
    }

	// async void RunTrackerProcess()
	// {
	// 	while (true)
	// 	{
	// 		PositionTrackerState current = _tracker.CurrentState;

	// 		Point position = current.Position;

	// 		double velocity = current.Velocity.Distance(Point.Zero);


	// 		FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

	// 		await Task.Delay(100);
	// 	}
	// }

}

