﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Messages
{
    internal class DrawMessage : ValueChangedMessage<string>
    {
        internal DrawMessage(string value) : base(value)
        {
        }
    }
}