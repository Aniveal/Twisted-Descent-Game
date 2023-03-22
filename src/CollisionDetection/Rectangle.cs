using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.CollisionDetection
{
    internal interface Rectangle : Collider
    {
        Vector2 getA();
        Vector2 getB();
    }
}
