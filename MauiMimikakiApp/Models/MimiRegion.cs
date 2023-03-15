using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

public class MimiRegion
{
    public PathInternalRegion Internal => _internalRegion;
    public IEnumerable<MimiHair> Hairs => _hairs;
    public IEnumerable<MimiDust> Dusts => _dusts;
    
    readonly PathInternalRegion _internalRegion;
    List<MimiHair> _hairs;
    List<MimiDust> _dusts;


    public MimiRegion(PathInternalRegion internalRegion)
    {
        _internalRegion = internalRegion;

        // make mimi hairs
    }
}
