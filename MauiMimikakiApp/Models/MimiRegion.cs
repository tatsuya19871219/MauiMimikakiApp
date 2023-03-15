using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMimikakiApp.Models;

public class MimiRegion
{
    public InternalRegion Internal => _internalRegion;
    public IEnumerable<MimiHair> Hairs => _hairs;
    public IEnumerable<MimiDust> Dusts => _dusts;
    
    readonly InternalRegion _internalRegion;
    List<MimiHair> _hairs;
    List<MimiDust> _dusts;


    public MimiRegion(InternalRegion internalRegion)
    {
        _internalRegion = internalRegion;

        // make mimi hairs
    }
}
