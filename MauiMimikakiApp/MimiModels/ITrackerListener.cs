using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeMauiEasy;

namespace MauiMimikakiApp;

internal interface ITrackerListener
{
    void OnMove(Point position, Point velocity, double milliSecUpdateInterval);

}
