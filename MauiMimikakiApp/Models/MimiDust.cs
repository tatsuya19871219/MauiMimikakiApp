using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

internal class MimiDust
{
    readonly public Point Position;
    //readonly double hardness;

    public MimiDust(Point position)
    {
        this.Position = position;
    }
}
