using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace TwistedDescent; 

public class StartRoom : Room{



    public StartRoom(MapGenerator mg, List<Prototype> protList, int pX, int pY) : base(mg, 0, 0, 8, 8, 1, protList, 0, false)
    {

        for (int x = 2; x < 6; x++)
            for (int y = 2; y < 6; y++)
                setWalkable(x, y);
        innerOpening(3, 3);

        this.setTile(3, 1, mg.entranceExitPrototypes[0]);
        this.setTile(4, 1, mg.entranceExitPrototypes[1]);

        this.PosX = pX;
        this.PosY = pY;

        nTreasures = 1;
        columnDensity = 0f;
    }
}