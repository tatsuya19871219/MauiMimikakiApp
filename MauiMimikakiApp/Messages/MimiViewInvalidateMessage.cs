using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MauiMimikakiApp.Messages
{
    internal class MimiViewInvalidateMessage : ValueChangedMessage<string>
    {
        internal MimiViewInvalidateMessage(string value) : base(value)
        {
        }
    }
}
