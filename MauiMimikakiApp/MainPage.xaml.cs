using Plugin.Maui.Audio;
using MauiMimikakiApp.CustomViews;
using MauiMimikakiApp.ViewModels;
using Microsoft.Maui.Controls.Shapes;

using TakeMauiEasy;
using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;
using System.Diagnostics;

namespace MauiMimikakiApp;

public partial class MainPage : ContentPage
{
	IAudioPlayer _kakiSEPlayer;
	MimikakiViewModel _vm;

	Stopwatch _stopwatch;

	public MainPage(IAudioManager audioManager) 
	{
		InitializeComponent();

		RegisterTrackerMessages();
		PrepareSEPlayer(audioManager);

		_stopwatch = new Stopwatch();
		_stopwatch.Start();

		var config = MimikakiConfig.Load("Config.json");
	}

	void RegisterTrackerMessages()
	{
        StrongReferenceMessenger.Default.Register<TrackerUpdateMessage>(this, (s, e) =>
        {
			_stopwatch.Stop();

			int elapsedMilli = (int)_stopwatch.ElapsedMilliseconds;

			//Debug.Assert(elapsedMilli > 0);

			HeaderLabel.Text = $"Update interval: {elapsedMilli} [ms]";

			_stopwatch.Reset();
			_stopwatch.Start();

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
			try
			{
				_kakiSEPlayer.Speed = velocity * 50;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			if (!_kakiSEPlayer.IsPlaying && velocity > 0.02)
			{
				try
				{
					_kakiSEPlayer.Play();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}

		});
	}

	async void PrepareSEPlayer(IAudioManager audioManager)
	{
		_kakiSEPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("kaki.mp3"));
	}
        
}

