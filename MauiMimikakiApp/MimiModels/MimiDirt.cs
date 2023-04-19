using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

public class MimiDirt : ITrackerListener
{
    public bool IsRemoved { get; private set; } = false;
    readonly public Point Position;
    //readonly double hardness;

    readonly public double Size;

    public MimiDirt(Point position)
    {
        this.Position = position;

        this.Size = 5;
        //
        //DoTestWork();
    }

    // //
    // async void DoTestWork()
    // {
    //     await Task.Delay(2000);
    //     IsRemoved = true;
    // }

    public void OnMove(Point position, Point velocity, double milliSecUpdateInterval)
    {
        double dt = milliSecUpdateInterval;

        if (position.Distance(Position) < Size)
        {
            IsRemoved = true;
        }
    }
}
