using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
