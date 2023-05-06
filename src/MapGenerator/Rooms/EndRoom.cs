using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.ComponentModel;

namespace Meridian2; 

public class EndRoom : Room{

    public EndRoom(MapGenerator mg, List<Prototype> protList, int index) : base(mg, 0, 0, 7, 7, index, protList, 0)
    {
        setTile(3, 3, mg.FinishPrototype);

        for (int x = 2; x < 5; x++)
            for (int y = 2; y < 5; y++)
                setWalkable(x, y);
        innerOpening(3, 3);

        columnDensity = 0f;
        nTreasures = 1;

    }
}