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
	IAudioPlayer _kakiSEPlayer;

	MimikakiViewModel _vm;

	public MainPage(IAudioManager audioManager) 
	{
		InitializeComponent();

		RegisterTrackerMessages();
		PrepareSEPlayer(audioManager);
	}

	void RegisterTrackerMessages()
	{
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

	async void PrepareSEPlayer(IAudioManager audioManager)
	{
		_kakiSEPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("kaki.mp3"));
	}
        
}

