using Plugin.Maui.Audio;
using MauiMimikakiApp.CustomViews;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;

using TakeMauiEasy;
using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;

namespace MauiMimikakiApp;

public partial class MainPage : ContentPage
{
	readonly IAudioManager _audioManager;

	IAudioPlayer _kakiSEPlayer;

	TrackableMimiViewModel _vm;

	public MainPage(IAudioManager audioManager) 
	{
		InitializeComponent();

		_audioManager = audioManager;

		InitializeMimi();
	}


	async void InitializeMimi()
	{
		//MimiGrid.IsVisible = false;

		await EasyTasks.WaitFor(() => MimiView.DisplayRatio.HasValue);

		PositionTracker tracker = null;
#if ANDROID
		tracker = new PositionTracker(MimiView.MimiTrackerLayer);
#elif WINDOWS
		tracker = new PositionTracker(MimiView);
#endif

		double displayRatio = MimiView.DisplayRatio.Value;

		//MimiView.BindingContext = _vm = InstantiateMimiViewModel(tracker, displayRatio);

		// Set up SEs
		_kakiSEPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("kaki.mp3"));
		
		// Set Sample loop logic
		// _vm.OnMoveOnMimi += (state) =>
		// {
		// 	// Point position = state.Position;

		// 	// double velocity = state.Velocity.Distance(Point.Zero);

		// 	// FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

		// 	// test
		// 	//_kakiSEPlayer.Speed = velocity*50;
		// 	//if(!_kakiSEPlayer.IsPlaying && velocity > 0.02) 
		// 	//{
		// 	//	_kakiSEPlayer.Play();
		// 	//}

		// };

		//_vm.InvokeTrackerProcess(100);

        //MimiGrid.IsVisible = true;

        // Register DrawMessages
        StrongReferenceMessenger.Default.Register<TrackerUpdateMessage>(this, (s, e) =>
        {

			PositionTrackerState state = e.Value;

			Point position = state.Position;

			double velocity = state.Velocity.Distance(Point.Zero);

			FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

        });
    }

	//TrackableMimiViewModel InstantiateMimiViewModel(PositionTracker tracker, double displayRatio)	
	//{

	//	var mimiTopGeometry = GetGeometryFromString(PathDictionary["MimiTopPathGeometryString"] as string);
	//	var mimiCenterGeometry = GetGeometryFromString(PathDictionary["MimiCenterPathGeometryString"] as string);
	//	var mimiBottomGeometry = GetGeometryFromString(PathDictionary["MimiBottomPathGeometryString"] as string);

	//	return new(tracker, mimiTopGeometry, mimiCenterGeometry, mimiBottomGeometry, displayRatio);

	//	//return new(tracker, null, null, null, displayRatio);
	//}

	
	Geometry GetGeometryFromString(string pathString)
	{
		return (Geometry)new PathGeometryConverter().ConvertFromInvariantString(pathString);
    }

    private void StateTrigger_IsActiveChanged(object sender, EventArgs e)
    {

    }

}

