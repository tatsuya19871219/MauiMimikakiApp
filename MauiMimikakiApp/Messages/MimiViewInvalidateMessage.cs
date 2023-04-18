using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Messages
{
    internal class MimiViewInvalidateMessage : ValueChangedMessage<string>
    {
        internal MimiViewInvalidateMessage(string value) : base(value)
        {
        }
    }
}
