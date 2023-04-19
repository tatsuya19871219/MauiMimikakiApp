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

	MimikakiViewModel _vm;

	public MainPage(IAudioManager audioManager) 
	{
		InitializeComponent();

		_audioManager = audioManager;

		InitializeMimi();
	}


	async void InitializeMimi()
	{
//		await EasyTasks.WaitFor(() => MimiView.DisplayRatio.HasValue);

// 		PositionTracker tracker = null;
// #if ANDROID
// 		tracker = new PositionTracker(MimiView.MimiTrackerLayer);
// #elif WINDOWS
// 		tracker = new PositionTracker(MimiView);
// #endif

		// Set up SE player
		_kakiSEPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("kaki.mp3"));
		
        // Register messages
        StrongReferenceMessenger.Default.Register<TrackerUpdateMessage>(this, (s, e) =>
        {

			PositionTrackerState state = e.Value;

			Point position = state.Position;

			double velocity = state.Velocity.Distance(Point.Zero);

			FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

        });

		StrongReferenceMessenger.Default.Register<TrackerOnMimiMessage>(this, (s, e) =>
		{
			PositionTrackerState state = e.Value;

			Point position = state.Position;

			double velocity = state.Velocity.Distance(Point.Zero);

			// test
			_kakiSEPlayer.Speed = velocity*50;
			if(!_kakiSEPlayer.IsPlaying && velocity > 0.02) 
			{
				_kakiSEPlayer.Play();
			}
			
		});
    }

    private void StateTrigger_IsActiveChanged(object sender, EventArgs e)
    {

    }

}

