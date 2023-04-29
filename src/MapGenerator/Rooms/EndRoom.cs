using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.ComponentModel;

namespace Meridian2; 

public class EndRoom : Room{

    public EndRoom(MapGenerator mg, List<Prototype> protList) : base(mg, 0, 0, 7, 7, 1, protList, 0)
    {
        for (int x = 2; x < 5; x++)
            for (int y = 2; y < 5; y++)
                setWalkable(x, y);
        innerOpening(3, 3);
    }
}