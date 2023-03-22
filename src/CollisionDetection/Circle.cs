using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.CollisionDetection
{
    public interface Circle : Collider
    {
        Vector2 getCenter();
        Vector2 getRadius();
    }
}
