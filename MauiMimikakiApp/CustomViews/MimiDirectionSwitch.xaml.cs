using System.Globalization;
using MauiMimikakiApp.ViewModels;

namespace MauiMimikakiApp.CustomViews;

public partial class MimiDirectionSwitch : ContentView
{
	public static BindableProperty IsRightProperty = BindableProperty.Create(nameof(IsRight), typeof(bool), typeof(MimiDirectionSwitch), null);
	public bool IsRight
	{
		get => (bool)GetValue(IsRightProperty); 
		set => SetValue(IsRightProperty, value);
	}

	required public bool RightInInit
	{
		init => GoTo(value);
	}

	readonly Point _rightPosition;
	readonly Point _leftPosition;

	public MimiDirectionSwitch()
	{
		InitializeComponent();
		BindingContext = this;

		var res = this.Resources;

		double width = (double)res["SwitchBodyWidth"];

		_rightPosition = _leftPosition = new();
		_rightPosition.X += width/4;
		_leftPosition.X -= width/4;
	}

	private void SwitchTipTapped(object sender, TappedEventArgs e)
	{
		GoTo(!IsRight);
	}

	void GoTo(bool isRight)
	{
		IsRight = isRight;
		var dest = isRight ? _rightPosition : _leftPosition;
		
		SwitchTip.TranslateTo(dest.X, dest.Y, 100);
	}

}

internal class BoolToDirectionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isRight = (bool)value;

		return isRight ? "Right" : "Left";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
