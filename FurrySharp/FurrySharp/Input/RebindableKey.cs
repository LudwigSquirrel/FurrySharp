using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace FurrySharp.Input;

public struct RebindableKey
{
    public List<Keys> Keys { get; set; }
    public List<MouseButts> MouseButts { get; set; }
    
    public RebindableKey(MouseButts butt)
    {
        Keys = new List<Keys>();
        MouseButts = new List<MouseButts> { butt };
    }
    
    public RebindableKey(List<Keys> keys)
    {
        Keys = keys;
        MouseButts = new List<MouseButts>();
    }
}