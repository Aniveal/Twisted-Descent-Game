using Meridian2.CollisionDetection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.String
{
    public class SimpleString
    {
        public Vector2 anchor; //last fixed position
        public Vector2 dynamicEnd; //should be player pos
        CollisionDetector collisionDetector;
        public List<StringSegment> segments; //Last segment in list should always be straight TODO: enforce

        public SimpleString(CollisionDetector detector, Vector2 startPos)
        {
            collisionDetector = detector;
            anchor = startPos;
            dynamicEnd = startPos;
            segments = new List<StringSegment>
            {
                new StraightStringSegment(startPos, startPos)
            };
        }

        public void Update(Vector2 newpos)
        {
            //determine new string position in vacuum
            int curIndex = segments.Count - 1;
            StraightStringSegment curSeg = (StraightStringSegment)segments[curIndex];
            Vector2 newStartPos = new Vector2();
            if (curIndex > 0 && segments[curIndex-1] is CurvedStringSegment) //Previous segment is curved
            {
                CurvedStringSegment prevSeg = (CurvedStringSegment)segments[curIndex-1];
                //Compute new point where string leaves circle
                Vector2 vToCir = Vector2.Subtract(newpos, prevSeg.getCircle().getCenter());
                vToCir.Normalize();
                vToCir = Vector2.Multiply(vToCir, prevSeg.getCircle().getRadius());
                if (prevSeg.isClockwise()) //rotate by 90 counterclockwise to obtain point if string goes clockwise around pillar
                {
                    newStartPos.X = -vToCir.Y;
                    newStartPos.Y = vToCir.X;
                }
                else
                {
                    newStartPos.X = vToCir.Y;
                    newStartPos.Y = -vToCir.X;
                }
            } else
            {
                newStartPos = anchor;
            }
            
            //collision detection
            List<object> colliders = collisionDetector.getObjectsInTriangle(anchor, dynamicEnd, newpos);
            if (colliders.Count == 0)
            {
                dynamicEnd = newpos;
                return;
            }
        }

        public void Draw()
        {

        }
    }
}
