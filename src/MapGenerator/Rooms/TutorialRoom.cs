
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Meridian2;

public class TutorialRoom : Room
{
    public TutorialRoom(MapGenerator mg, List<Prototype> protList) : base(mg, -9, -16, 80, 32, 1, protList, 0) {
    
        PlaceTiles();
        PlaceElements();
    }

    private void Horizontal(int y, int x1, int x2, Prototype p) {
        for (int i = x1; i <= x2; i++) {
            TileMap[i, y].FinalPrototype = p;
        }
    }

    private void Vertical(int x, int y1, int y2, Prototype p) {
        for (int i = y1; i <= y2; i++) {
            TileMap[x, i].FinalPrototype = p;
        }
    }

    private void Square(int x1, int x2, int y1, int y2, Prototype g) {
        for (int i = x1; i <= x2; i++) {
            for (int j = y1; j <= y2; j++) {
                TileMap[i, j].FinalPrototype = g;
            }
        }
    }

    private void SquareRoom(int x1, int x2, int y1, int y2) {
        //top left corner
        TileMap[x1, y1].FinalPrototype = protList[10];
        //top right corner
        TileMap[x2, y1].FinalPrototype = protList[9];
        //bottom right corner
        TileMap[x2, y2].FinalPrototype = protList[8];
        //bottom left corner
        TileMap[x1, y2].FinalPrototype = protList[11];
        //top wall
        Horizontal(y1, x1+1, x2-1, protList[5]);
        //right wall
        Vertical(x2, y1+1, y2-1, protList[6]);
        //bottom wall
        Horizontal(y2, x1+1, x2-1, protList[7]);
        //left wall
        Vertical(x1, y1+1, y2-1, protList[4]);
        //ground
        Square(x1+1, x2-1, y1+1, y2-1, protList[13]);
    }

    private void CliffSquare(int x1, int x2, int y1, int y2) {
        //top left corner
        TileMap[x1, y1].FinalPrototype = Mg.CliffPrototypes[9];
        //top right corner
        TileMap[x2, y1].FinalPrototype = Mg.CliffPrototypes[10];
        //bot right corner
        TileMap[x2, y2].FinalPrototype = Mg.CliffPrototypes[11];
        //bot left corner
        TileMap[x1, y2].FinalPrototype = Mg.CliffPrototypes[8];
        //top wall
        Horizontal(y1, x1+1, x2-1, Mg.CliffPrototypes[7]);
        //right wall
        Vertical(x2, y1+1, y2-1, Mg.CliffPrototypes[6]);
        //bottom wall
        Horizontal(y2, x1+1, x2-1, Mg.CliffPrototypes[4]);
        //left wall
        Vertical(x1, y1+1, y2-1, Mg.CliffPrototypes[5]);
        //ground
        Square(x1+1, x2-1, y1+1, y2-1, Mg.CliffPrototypes[12]);
    }

    public void PlaceTiles() {
        //get the prototypes
        Prototype full = protList[12];
        Prototype ground = protList[13];
        Prototype bl1 = protList[3];
        Prototype br1 = protList[0];
        Prototype tr1 = protList[1];
        Prototype tl1 = protList[2];
        Prototype t2 = protList[5];
        Prototype r2 = protList[6];
        Prototype b2 = protList[7];
        Prototype l2 = protList[4];
        //Set all to full wall
        foreach(Tile  t in TileMap) {
            t.FinalPrototype = full;
        }
        //corridor walls
        Horizontal(15, 11, 68, t2);
        Horizontal(17, 11, 68, b2);
        //Generate Rooms
        //Entry room
        SquareRoom(10, 14, 14, 18);
        TileMap[14,17].FinalPrototype = br1;
        TileMap[14,15].FinalPrototype = tr1;
        //Room 1
        SquareRoom(18, 22, 14, 18);
        TileMap[18, 15].FinalPrototype = tl1;
        TileMap[18,17].FinalPrototype = bl1;
        TileMap[22,15].FinalPrototype = tr1;
        TileMap[22,17].FinalPrototype = br1;
        //room 2
        SquareRoom(26, 31, 13, 19);
        TileMap[26, 15].FinalPrototype = tl1;
        TileMap[26,17].FinalPrototype = bl1;
        TileMap[31,15].FinalPrototype = tr1;
        TileMap[31,17].FinalPrototype = br1;
        //room 3
        SquareRoom(35,44, 11, 20);
        TileMap[35, 15].FinalPrototype = tl1;
        TileMap[35,17].FinalPrototype = bl1;
        TileMap[44,15].FinalPrototype = tr1;
        TileMap[44,17].FinalPrototype = br1;
        //room 4
        SquareRoom(48, 62, 11, 22);
        TileMap[48, 15].FinalPrototype = tl1;
        TileMap[48,17].FinalPrototype = bl1;
        TileMap[62,15].FinalPrototype = tr1;
        TileMap[62,17].FinalPrototype = br1;
        //room 5
        SquareRoom(65, 69, 13, 19);
        TileMap[65, 15].FinalPrototype = tl1;
        TileMap[65,17].FinalPrototype = bl1;
        //Corridor ground
        Horizontal(16, 11, 68, ground);
        //end tile
        TileMap[67, 16].FinalPrototype = Mg.FinishPrototype;
        CliffSquare(55, 59, 14, 18);
    }

    private void PlaceElements() {
        //room 1
        EnemyPositions.Add(new Vector2 (20, 16.5f));
        //Room 2
        EnemyPositions.Add(new Vector2(29.5f, 15.5f));
        Columns.Add(new Vector2(28, 15.5f));
        columnTypes.Add(2);
        //Room 3
        EnemyPositions.Add(new Vector2(42, 16));
        Columns.Add(new Vector2(36, 16));
        columnTypes.Add(1);
        Columns.Add(new Vector2(38, 14));
        columnTypes.Add(0);
        Columns.Add(new Vector2(38, 18));
        columnTypes.Add(0);
        AmphoraPositions.Add(new Vector2(38, 16));
        //room 4
        TreasurePositions.Add(new Vector2(49, 16));
        Columns.Add(new Vector2(51.5f, 16));
        columnTypes.Add(1);
        EnemyPositions.Add(new Vector2(53, 16));
    }
}