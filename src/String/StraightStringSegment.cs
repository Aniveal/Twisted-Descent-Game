using Meridian2.CollisionDetection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.String
{
    public class StraightStringSegment : StringSegment, Line
    {
        Vector2 a;
        Vector2 b;

        public StraightStringSegment(Vector2 a, Vector2 b) 
        {
            this.a = a; 
            this.b = b;
        }
        public Vector2 getEnd()
        {
            return b;
        }

        public Vector2 getStart()
        {
            return a;
        }
    }
}
