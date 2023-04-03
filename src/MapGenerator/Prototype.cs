using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Meridian2
{
    public class Prototype
    {
        //Thist class stores information about tiles on the world map.
        //A prototype is basically a building block of a level

        //For implementation details refer to this video: https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s

        //Sockets: For every direction (up, down, left, right; in that order) the socket int
        //There are 4 types of sockets:
        //0: Nothing there, meaning no walls
        //1: Left side is wall, right side is nothing
        //2: Reverse
        //3: Whole side is wall
        public readonly int[] sockets;

        public string name;

        //The higher, the more likely this prototype is picked
        public int weight;

        //Name of prototype (i.e. "Wall1ur" for Wall1, up, right)
        public readonly Texture2D texture;


        public Prototype(Texture2D t, string n, int[] sockets, int w)
        {
            texture = t;
            name = n;
            this.sockets = sockets;
            weight = w;

        }

    }
}
