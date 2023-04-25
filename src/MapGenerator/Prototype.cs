using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 

public class Prototype {
    //Thist class stores information about tiles on the world map.
    //A prototype is basically a building block of a level

    //For implementation details refer to this video: https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s

    //Sockets: For every direction (up, down, left, right; in that order) the socket int
    //There are 4 types of sockets:
    //0: Nothing there, meaning no walls
    //1: Left side is wall, right side is nothing
    //2: Reverse
    //3: Whole side is wall
    public readonly int[] Sockets;

    //Name of prototype (i.e. "Wall1ur" for Wall1, up, right)
    public readonly Texture2D Texture;
    public bool IsCliff;

    public string Name;

    public bool Walkable;

    //The higher, the more likely this prototype is picked
    public int Weight;


    public Prototype(Texture2D t, string n, int[] sockets, int w, bool wal, bool isCliff = false) {
        Texture = t;
        Name = n;
        Sockets = sockets;
        Weight = w;
        Walkable = wal;
        IsCliff = isCliff;
    }
}