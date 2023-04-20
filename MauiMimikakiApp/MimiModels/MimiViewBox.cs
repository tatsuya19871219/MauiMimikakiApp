using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

public class MimiViewBox
{
    required public Rect Bounds { private get; init; }
    public async Task<Rect> GetBoundsAsync()
    {
        while(true)
        {
            if (!Bounds.Size.IsZero) break;
            await Task.Delay(100);
        }

        return Bounds;
    }
}
