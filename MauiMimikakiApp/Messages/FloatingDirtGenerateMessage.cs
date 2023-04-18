using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MauiMimikakiApp.Models;
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
