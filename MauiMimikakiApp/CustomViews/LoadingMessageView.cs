namespace MauiMimikakiApp.CustomViews;

public class LoadingMessageView : ContentView
{
	Label _loadingLabel;
	public LoadingMessageView()
	{
		_loadingLabel = new Label {Text = "Hey", IsVisible = false};

		Content = new VerticalStackLayout
		{
			Children = {_loadingLabel}
		};

		RunLoadingAnimation();
	}

	async void RunLoadingAnimation()
	{
		string loadingMessage = "耳を構築しています";
		_loadingLabel.Text = loadingMessage;
		_loadingLabel.IsVisible = true;

		int k = 0;
		while (true) 
		{
			// if (token.IsCancellationRequested) 
			// {
			// 	LoadingLabel.IsVisible = false;
			// 	token.ThrowIfCancellationRequested();
			// }
			_loadingLabel.Text = loadingMessage + new string('.', k);
			await Task.Delay(250);
			k++;
			if(k > 3) k = 0;
		}
	}

	internal void Show()
	{
		_loadingLabel.IsVisible = true;
	}

	internal void Hide()
	{
		_loadingLabel.IsVisible = false;
	}
}