using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.ComponentModel;

namespace Meridian2; 

public class EndRoom : Room{

    public EndRoom(MapGenerator mg, List<Prototype> protList, int index) : base(mg, 0, 0, 8, 8, index, protList, 0)
    {
        setTile(4, 3, mg.entranceExitPrototypes[1]);
        setTile(3, 3, mg.entranceExitPrototypes[1]);
        setTile(4, 4, mg.entranceExitPrototypes[0]);
        setTile(3, 4, mg.entranceExitPrototypes[1]);

        for (int x = 2; x < 6; x++)
            for (int y = 2; y < 6; y++)
                setWalkable(x, y);
        innerOpening(3, 3);

        columnDensity = 0f;
        nTreasures = 1;

    }
}