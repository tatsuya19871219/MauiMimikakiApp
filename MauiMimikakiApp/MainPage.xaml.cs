using MauiMimikakiApp.CustomViews;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;

using TakeMauiEasy;

namespace MauiMimikakiApp;

public partial class MainPage : ContentPage
{
	readonly TrackableMimiViewModel _vm;
	PositionTracker _tracker;

	//double _alignmentLeft = 0;

	public MainPage(TrackableMimiViewModel vm)
	{
		InitializeComponent();

		_vm = vm;

		InitializeTrackableMimi();
		
	}


	async void InitializeTrackableMimi()
	{
		// Start loading indicator
		//LoadingLabel.IsVisible = true;
		// var tokenSource = new CancellationTokenSource();
		// var token = tokenSource.Token;

		// StartLoadingAnimation(token);
		LoadingMessage.Show();

		Image mimi = new Image {Source = "mimi.png"}; // without HeightRequest/WidthRequest

		//await MimiTrackableView.SetTargetView(mimi, 600);
		await MimiTrackableView.SetTargetView(mimi, (double)PathDictionary["MimiExpectedHeight"]);

		while (true)
		{
			if (MimiTrackableView.Width > 0) break;
			await Task.Delay(100);
		}

		_vm.BindTrackableMimi(MimiTrackableView);

        var mimiTopGeometry = GetGeometryFromString(PathDictionary["MimiTopPathGeometryString"] as string);
        var mimiCenterGeometry = GetGeometryFromString(PathDictionary["MimiCenterPathGeometryString"] as string);
        var mimiBottomGeometry = GetGeometryFromString(PathDictionary["MimiBottomPathGeometryString"] as string);

		await _vm.InitializeDetectableGeometries(mimiTopGeometry, mimiCenterGeometry, mimiBottomGeometry);	

		//_vm.InitializeDetectableGeometries(mimiTopGeometry, mimiCenterGeometry, mimiBottomGeometry);

		//await Task.Delay(10000);

        // These should be awaitable
		// MimiTrackableView.RegistDetectableRegion(mimiTopGeometry);
        // MimiTrackableView.RegistDetectableRegion(mimiCenterGeometry);
        // MimiTrackableView.RegistDetectableRegion(mimiBottomGeometry);


        // _tracker = MimiTrackableView.GetPositionTracker();

		// RunTrackerProcess();

		// End loading indicator
		//tokenSource.Cancel();
		//LoadingLabel.IsVisible = false;
		LoadingMessage.Hide();
	}

	// async Task StartLoadingAnimation(CancellationToken token)
	// {
	// 	string loadingMessage = "耳を構築しています";
	// 	LoadingLabel.Text = loadingMessage;
	// 	LoadingLabel.IsVisible = true;

	// 	int k = 0;
	// 	while (true) 
	// 	{
	// 		if (token.IsCancellationRequested) 
	// 		{
	// 			LoadingLabel.IsVisible = false;
	// 			token.ThrowIfCancellationRequested();
	// 		}
	// 		LoadingLabel.Text = loadingMessage + new string('.', k);
	// 		await Task.Delay(250);
	// 		k++;
	// 		if(k > 3) k = 0;
	// 	}

	// 	//LoadingLabel.IsVisible = false;
	// }


	Geometry GetGeometryFromString(string pathString)
	{
		return (Geometry)new PathGeometryConverter().ConvertFromInvariantString(pathString);
    }

	async void RunTrackerProcess()
	{
		while (true)
		{
			PositionTrackerState current = _tracker.CurrentState;

			Point position = current.Position;

			double velocity = current.Velocity.Distance(Point.Zero);


			FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

			await Task.Delay(100);
		}
	}

}

