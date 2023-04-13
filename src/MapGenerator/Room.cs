using Microsoft.Xna.Framework;
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

        //Stores the points where there is an opening
        List<Vector2> openings;

        //All neighbouring rooms, up-down-left-right, null means none
        Room[] neighbours;

        public Tile[,] tileMap;

        private MapGenerator mg;

        private Random r;

        public Room(MapGenerator mg, int x, int y)
        {
            openings = new List<Vector2>();
            this.mg = mg;
            neighbours = new Room[4];
            sizeX = x; sizeY = y;
            tileMap = new Tile[sizeX, sizeY];
            r = new Random();
        }

        //Creates an opening to another room at x, y with length l. l goes from x,y into the positive direction. Has to be at least 3
        //Don't go too close to an edge, otherwise undefined behaviuor may happen
        public void createOpening(int x, int y, int l)
        {
            openings.Add(new Vector2(x, y));

            if (x == 0 || x == sizeX - 1)
            {
                for (int i = 1; i < l - 1; i++)
                {
                    if (i > sizeY - 1)
                        return;
                    tileMap[x, y + i].setFinalPrototype(mg.getPrototype("ground"));
                    mg.collapseTile(tileMap[x, y + i], this.tileMap);
                }
            }
            else if (y == 0 || y == sizeY - 1)
            {
                for (int i = 1; i < l - 1; i++)
                {
                    if (i > sizeX - 1)
                        return;
                    tileMap[x + i, y].setFinalPrototype(mg.getPrototype("ground"));
                    mg.collapseTile(tileMap[x + i, y], this.tileMap);
                }
            }
        }

        //Creates a walkable paths between all openings to ensure that all exits are reachable
        //Call this after all createOpening calls were made!
        //Makes a path to the midpoint of the room
        public void connectOpenings()
        {
            //Midpoint of the room
            int midX = sizeX / 2;
            int midY = sizeY / 2;

            foreach(Vector2 opening in openings)
            {
                int x, y;
                //Handle case where opening is on left or right
                if(opening.X == 0 || opening.X == sizeX - 1)
                {
                    //First, do one step towards the middle
                    x = (int)opening.X;
                    y = (int)opening.Y + 1;

                    tileMap[x, y].setFinalPrototype(mg.getPrototype("ground"));
                    mg.collapseTile(tileMap[x,y], this.tileMap);

                    while(x != midX && y != midY)
                    {
                        double rand = r.NextDouble();

                        //Corner cases
                        if (x == midX)
                        {
                            if (y < midY) y++;
                            else y--;
                            tileMap[x, y].setFinalPrototype(mg.getPrototype("ground"));
                            mg.collapseTile(tileMap[x, y], this.tileMap);
                            continue;
                        }
                        if (y == midY)
                        {
                            if (x < midX) x++;
                            else x--;
                            tileMap[x, y].setFinalPrototype(mg.getPrototype("ground"));
                            mg.collapseTile(tileMap[x, y], this.tileMap);
                            continue;
                        }

                        //Get distance from point to mid
                        int distanceX = Math.Abs(midX - x);
                        int distanceY = Math.Abs(midY - y);

                        //Farther away increases weight
                        double xWeight = distanceX / (double)(distanceX + distanceY);

                        if(rand < xWeight)
                        {
                            if (x < midX) x++;
                            else x--;
                        }
                        else
                        {
                            if(y < midY) y++;
                            else y--;
                        }
                        tileMap[x, y].setFinalPrototype(mg.getPrototype("ground"));
                        mg.collapseTile(tileMap[x, y], this.tileMap);
                    }
                }
            }
        }

        

        //Creates a border of solid walls around room everywhere where there is no opening
        //Do this after setting openings!
        public void createBorder()
        {
            for (int x = 0; x < sizeX; x++)
            {
                if(tileMap[x, 0].superpositions.Contains(mg.getPrototype("FullWall")))
                {
                    tileMap[x, 0] = new Tile(mg.getPrototype("FullWall"), x, 0);
                    mg.collapseTile(tileMap[x, 0], this.tileMap);
                }
                if (tileMap[x, sizeY - 1].superpositions.Contains(mg.getPrototype("FullWall")))
                {
                    tileMap[x, sizeY - 1] = new Tile(mg.getPrototype("FullWall"), x, sizeY - 1);
                    mg.collapseTile(tileMap[x, sizeY - 1], this.tileMap);
                }

            }
            for (int y = 0; y < sizeY; y++)
            {
                if (tileMap[0, y].superpositions.Contains(mg.getPrototype("FullWall")))
                {
                    tileMap[0, y] = new Tile(mg.getPrototype("FullWall"), 0, y);
                    mg.collapseTile(tileMap[0, y], this.tileMap);
                }

                if (tileMap[sizeX - 1, y].superpositions.Contains(mg.getPrototype("FullWall")))
                {
                    tileMap[sizeX - 1, y] = new Tile(mg.getPrototype("FullWall"), sizeX - 1, y);
                    mg.collapseTile(tileMap[sizeX - 1, y], this.tileMap);
                }
                    
            }
        }
    }
}
