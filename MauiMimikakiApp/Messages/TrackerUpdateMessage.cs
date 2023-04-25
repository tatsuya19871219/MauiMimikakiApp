using CommunityToolkit.Mvvm.Messaging.Messages;
using TakeMauiEasy;

namespace MauiMimikakiApp.Messages
{
    internal class TrackerUpdateMessage : ValueChangedMessage<PositionTrackerState>
    {
        internal TrackerUpdateMessage(PositionTrackerState state) : base(state)
        {
        }
    }
}

