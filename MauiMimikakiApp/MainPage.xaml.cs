using MauiMimikakiApp.CustomViews;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;

using TakeMauiEasy;

namespace MauiMimikakiApp;

public partial class MainPage : ContentPage
{
	TrackableMimiViewModel _vm;

	public MainPage() 
	{
		InitializeComponent();

		InitializeMimiViewModel();
	}


	async void InitializeMimiViewModel()
	{
		//MimiGrid.IsVisible = false;

		await EasyTasks.WaitFor(() => MimiView.DisplayRatio.HasValue);

		var tracker = new PositionTracker(MimiView.MimiTrackerLayer);

		double displayRatio = MimiView.DisplayRatio.Value;

		MimiView.BindingContext = _vm = InstantiateMimiViewModel(tracker, displayRatio);

		// Set Sample loop logic
		_vm.OnMoveOnMimi += (state) =>
		{
			Point position = state.Position;

			double velocity = state.Velocity.Distance(Point.Zero);

			FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

		};

		_vm.InvokeTrackerProcess(100);

		//MimiGrid.IsVisible = true;
	}

	TrackableMimiViewModel InstantiateMimiViewModel(PositionTracker tracker, double displayRatio)	
	{
		var mimiTopGeometry = GetGeometryFromString(PathDictionary["MimiTopPathGeometryString"] as string);
        var mimiCenterGeometry = GetGeometryFromString(PathDictionary["MimiCenterPathGeometryString"] as string);
        var mimiBottomGeometry = GetGeometryFromString(PathDictionary["MimiBottomPathGeometryString"] as string);

		return new(tracker, mimiTopGeometry, mimiCenterGeometry, mimiBottomGeometry, displayRatio);
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

