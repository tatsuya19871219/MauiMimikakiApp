using CommunityToolkit.Mvvm.Messaging;
using MauiMimikakiApp.Messages;
using Plugin.Maui.Audio;
using System.Diagnostics;
using TakeMauiEasy;

namespace MauiMimikakiApp;

public partial class MainPage : ContentPage
{
	IAudioPlayer _kakiSEPlayer;
	MimikakiConfig _config;
	Stopwatch _stopwatch;

	public MainPage(IAudioManager audioManager) 
	{
		InitializeComponent();

		_config = MimikakiConfig.Load("Config.json");

		RegisterTrackerMessages();
		PrepareSEPlayer(audioManager, (bool)MimiResourceDict["IsRight"]);

		_stopwatch = new Stopwatch();
		_stopwatch.Start();

		DirectionSwitch.Tapped += DirectionSwitch_Tapped;
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

			if (velocity < _config.SEcutoffVelocity)
			{
				if (_kakiSEPlayer.IsPlaying && _kakiSEPlayer.CurrentPosition > _kakiSEPlayer.Duration*0.2)
				{
					_kakiSEPlayer.Stop();
					//Debug.WriteLine("Stopped.");
				}
				return;
			}
		});

		StrongReferenceMessenger.Default.Register<TrackerOnMimiMessage>(this, (s, e) =>
		{
			PositionTrackerState state = e.Value;

			//Point position = state.Position;

			double velocity = state.Velocity.Distance(Point.Zero);

			if (velocity < _config.SEcutoffVelocity) return;


			if (!_kakiSEPlayer.IsPlaying)
			{
                //var currentSESpeed = _kakiSEPlayer.Speed;

                const double highHingeVelocity = 1.0;
                const double lowHingeVelocity = 0.2;

				double highSpeed = 1.1;
				double lowSpeed = 1.0;

                var newSESpeed = velocity switch
                {
                    (> highHingeVelocity) => highSpeed,
                    (< lowHingeVelocity) => lowSpeed,
                    _ => lowSpeed + (highSpeed-lowSpeed) * (velocity - lowHingeVelocity) / (highHingeVelocity - lowHingeVelocity)
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

	void DirectionSwitch_Tapped(bool isRight)
	{
		SetPlayerBalance(isRight);
	}

	void SetPlayerBalance(bool isRight)
	{
		_kakiSEPlayer.Balance = isRight ? 1 : -1;
	}

	async void PrepareSEPlayer(IAudioManager audioManager, bool rightInInit)
	{
		var filename = MimikakiConfig.Current.KakiSoundFilename;
		
		using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
		
		_kakiSEPlayer = audioManager.CreatePlayer(stream);

		SetPlayerBalance(rightInInit);
	}
        
}

