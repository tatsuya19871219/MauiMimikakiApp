using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiMimikakiApp.Messages;

internal class RegionDebugMessage : ValueChangedMessage<List<Point>>
{
    internal RegionDebugMessage(List<Point> value) : base(value)
    {

    }
}
