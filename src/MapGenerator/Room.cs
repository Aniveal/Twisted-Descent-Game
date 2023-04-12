using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2
{
    public class Room
    {
        //Tile size of the room
        int sizeX, sizeY;

        //All neighbouring rooms, up-down-left-right, null means none
        Room[] neighbours;

        public Tile[,] tileMap;

        private MapGenerator mg;

        public Room(MapGenerator mg, int x, int y)
        {
            this.mg = mg;
            neighbours = new Room[4];
            sizeX = x; sizeY = y;
            tileMap = new Tile[sizeX, sizeY];
            createBorder();
        }

        //Creates an opening to another room at x, y with length l. l goes from x,y into the positive direction. Has to be at least 3
        public void createOpening(int x, int y, int l)
        {
            if (x == 0 || x == sizeX - 1)
            {
                tileMap[x, y] = null;
                for (int i = 1; i < l - 1; i++)
                {
                    if (i > sizeY - 1)
                        return;
                    tileMap[x, y + i] = new Tile(mg.getPrototype("ground"), x, y + i);
                }
                tileMap[x, y + l - 1] = null;
            }
            else if (y == 0 || y == sizeY - 1)
            {
                tileMap[x, y] = null;
                for (int i = 1; i < l - 1; i++)
                {
                    if (i > sizeX - 1)
                        return;
                    tileMap[x + i, y] = new Tile(mg.getPrototype("ground"), x + i, y);
                }
                tileMap[x + l - 1, y] = null;
            }
        }

       

        //Creates a border of solid walls around room
        private void createBorder()
        {
            for (int x = 0; x < sizeX; x++)
            {
                tileMap[x, 0] = new Tile(mg.getPrototype("FullWall"), x, 0);
                tileMap[x, sizeY - 1] = new Tile(mg.getPrototype("FullWall"), x, sizeY - 1);

            }
            for (int y = 0; y < sizeY; y++)
            {
                tileMap[0, y] = new Tile(mg.getPrototype("FullWall"), 0, y);
                tileMap[sizeX - 1, y] = new Tile(mg.getPrototype("FullWall"), sizeX - 1, y);
            }
        }
    }
}
