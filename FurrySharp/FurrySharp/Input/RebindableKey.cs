using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace FurrySharp.Input;

public struct RebindableKey
{
    public List<Keys> Keys { get; set; }
    
    public RebindableKey(Keys key)
    {
        Keys = new List<Keys> { key };
    }
    
    public RebindableKey(List<Keys> keys)
    {
        Keys = keys;
    }
}