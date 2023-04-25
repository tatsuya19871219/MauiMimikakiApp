using CommunityToolkit.Mvvm.Messaging.Messages;
using TakeMauiEasy;

namespace MauiMimikakiApp.Messages
{
    internal class TrackerOnMimiMessage : ValueChangedMessage<PositionTrackerState>
    {
        public TrackerOnMimiMessage(PositionTrackerState value) : base(value)
        {
        }
    }
}
