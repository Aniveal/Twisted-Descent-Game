using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Meridian2; 

public class Prototype {
    //Thist class stores information about tiles on the world map.
    //A prototype is basically a building block of a level

    //For implementation details refer to this video: https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s

    //Sockets: For every direction (up, down, left, right; in that order) the socket int
    public readonly int[] Sockets;

    //Name of prototype (i.e. "Wall1ur" for Wall1, up, right)
    public readonly Texture2D Texture;
    public List<Texture2D> additionalTextures;

    public bool IsCliff;

    public string Name;

    public bool Walkable;

    //The higher, the more likely this prototype is picked
    public int Weight;


    public Prototype(Texture2D t, string n, int[] sockets, int w, bool wal, bool isCliff = false, List<Texture2D> additionalTextures = null) {
        Texture = t;
        Name = n;
        Sockets = sockets;
        Weight = w;
        Walkable = wal;
        IsCliff = isCliff;
        this.additionalTextures = additionalTextures;
    }
}