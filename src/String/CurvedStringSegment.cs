using Meridian2.CollisionDetection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.String
{
    public class CurvedStringSegment : StringSegment
    {
        Vector2 a;
        Vector2 b;
        Circle c;
        bool clockwise;
        public CurvedStringSegment(Vector2 a, Vector2 b, Circle circle, bool clockwise) 
        { 
            this.a = a; 
            this.b = b;
            this.c = circle;
            this.clockwise = clockwise;
        }
        public Vector2 getEnd()
        {
            return b;
        }

        public Vector2 getStart()
        {
            return a;
        }

        public Circle getCircle() { return c; }

        public bool isClockwise() { return clockwise; }
    }
}
