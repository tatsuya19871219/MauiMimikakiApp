using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Maui.Controls.Shapes;

namespace MauiMimikakiApp.Messages
{
    internal class FloatingDirtGenerateMessage : ValueChangedMessage<Shape>
    {
        internal FloatingDirtGenerateMessage(Shape dirt) : base(dirt)
        {
        }
    }
}
