using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiMimikakiApp.ViewModels;

internal partial class MimiDirectionSwitchViewModel : ObservableObject
{
	[ObservableProperty] bool _isRight;

    internal MimiDirectionSwitchViewModel(bool initialRight)
    {
        IsRight = initialRight;
    }
}
