using Plugin.Maui.Audio;

using TakeMauiEasy;
using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;
using System.Diagnostics;

namespace MauiMimikakiApp;

public partial class MainPage : ContentPage
{
	IAudioPlayer _kakiSEPlayer;

	Stopwatch _stopwatch;

	public MainPage(IAudioManager audioManager) 
	{
		InitializeComponent();

		var config = MimikakiConfig.Load("Config.json");

		RegisterTrackerMessages();
		PrepareSEPlayer(audioManager);

		_stopwatch = new Stopwatch();
		_stopwatch.Start();

	}

	void RegisterTrackerMessages()
	{
		StrongReferenceMessenger.Default.Register<MimiViewInvalidateMessage>(this, (s, e) =>
		{
			UpdateHeaderText();
		});

        StrongReferenceMessenger.Default.Register<TrackerUpdateMessage>(this, (s, e) =>
        {
			PositionTrackerState state = e.Value;

			UpdateFooterText(state);

            double velocity = state.Velocity.Distance(Point.Zero);

			//Debug.WriteLine($"velocity : {velocity}");

			if (velocity < 0.02)
			{
				if (_kakiSEPlayer.IsPlaying && _kakiSEPlayer.CurrentPosition > _kakiSEPlayer.Duration*0.2)
				{
					_kakiSEPlayer.Stop();
					Debug.WriteLine("Stopped.");
				}
				return;
			}
		});

		StrongReferenceMessenger.Default.Register<TrackerOnMimiMessage>(this, (s, e) =>
		{
			PositionTrackerState state = e.Value;

			//Point position = state.Position;

			double velocity = state.Velocity.Distance(Point.Zero);

			if (velocity < 0.02) return;


			if (!_kakiSEPlayer.IsPlaying)
			{
                //var currentSESpeed = _kakiSEPlayer.Speed;

                const double highCutoffVelocity = 1.0;
                const double lowCutoffVelocity = 0.2;

                var newSESpeed = velocity switch
                {
                    (> highCutoffVelocity) => 1.1,
                    (< lowCutoffVelocity) => 1.0,
                    _ => 1.05
                };

                //Debug.WriteLine($"CanSetSpeed {_kakiSEPlayer.CanSetSpeed}");
                _kakiSEPlayer.Speed = newSESpeed;


                _kakiSEPlayer.Play();
			}

			//Debug.WriteLine($"Current SE Speed: {_kakiSEPlayer.Speed}");
			//Debug.WriteLine($"IsPlaying {_kakiSEPlayer.IsPlaying}");

		});
	}

	void UpdateHeaderText()
	{
		_stopwatch.Stop();

		int elapsedMilli = (int)_stopwatch.ElapsedMilliseconds;

		//Debug.Assert(elapsedMilli > 0);

		HeaderLabel.Text = $"Update interval: {elapsedMilli} [ms]";

		_stopwatch.Reset();
		_stopwatch.Start();
	}

	void UpdateFooterText(PositionTrackerState state)
	{
		Point position = state.Position;

		double velocity = state.Velocity.Distance(Point.Zero);

		FooterLabel.Text = $"(x,y) = ({position.X:F1}, {position.Y:F1}), |v| = {velocity:F3} [px/ms]";

	}

	async void PrepareSEPlayer(IAudioManager audioManager)
	{
		var filename = MimikakiConfig.Current.KakiSoundFilename;
		
		using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
		
		_kakiSEPlayer = audioManager.CreatePlayer(stream);
	}
        
}

