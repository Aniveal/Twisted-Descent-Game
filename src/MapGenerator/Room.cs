using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            sizeX = x; 
            sizeY = y;
            tileMap = new Tile[sizeX, sizeY];
            r = new Random();
            initializeTileMap();
        }

        public void initializeTileMap()
        {

            //Create all tiles, initialize with full set of prototypes
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    tileMap[x, y] = new Tile(mg.prototypes);
                    tileMap[x, y].x = x;
                    tileMap[x, y].y = y;
                }
            }
        }

        //Creates an opening to another room at x, y with length l. l goes from x,y into the positive direction. Has to be at least 3
        //Don't go too close to an edge, otherwise undefined behaviuor may happen
        public void createOpening(int x, int y, int l)
        {
            
            openings.Add(new Vector2(x, y));

            if (x == 0 || x == sizeX - 1)
            {
                for (int i = 0; i < l; i++)
                {
                    if (i > sizeY - 1)
                    {
                        Debug.WriteLine("Error, l was too large in createOpening()");
                        return;
                    }
                    tileMap[x, y + i].setFinalPrototype(mg.getPrototype("ground"));
                    mg.collapseTile(tileMap[x, y + i], this.tileMap);
                }
            }
            else if (y == 0 || y == sizeY - 1)
            {
                for (int i = 0; i < l; i++)
                {
                    if (i > sizeX - 1)
                    {
                        Debug.WriteLine("Error, l was too large in createOpening()");
                        return;
                    }
                    tileMap[x + i, y].setFinalPrototype(mg.getPrototype("ground"));
                    mg.collapseTile(tileMap[x + i, y], this.tileMap);
                }
            }
            else Debug.WriteLine("Error while creating opening: Point " + x + "," + y + " is not on the border!!! SizeX = " + sizeX + ", SizeY = "+sizeY);
        }

        //Creates a walkable paths between all openings to ensure that all exits are reachable
        //Call this after all createOpening calls were made!
        //Makes a path to the midpoint of the room
        public void connectOpenings()
        {
            //Midpoint of the room
            int midX = sizeX / 2;
            int midY = sizeY / 2;

            Debug.WriteLine("Connecting the openings: midX = " + midX + "  And midY = " + midY);

            foreach(Vector2 opening in openings)
            {
                
                int x = (int)opening.X; int y = (int)opening.Y;

                Debug.Write("Opening.... " + x + ", " + y +  " midX = " + midX + " , midY = " + midY);

                //Handle case where opening is on left or right
                while (x <= 1 || x >= (sizeX - 2))
                {
                    //First, do one step towards the middle
                    if(opening.X <= 1)
                        x = x + 1;
                    else x = x - 1;

                    tileMap[x, y].setFinalPrototype(mg.getPrototype("ground"));
                    mg.collapseTile(tileMap[x, y], this.tileMap);
                }
                
                //opening on top or bottom
                while (y <= 1 || y >= (sizeX - 2))
                {
                    //First, do one step towards the middle
                    if(opening.Y <= 1)
                        y = y + 1;
                    else y = y - 1;

                    tileMap[x, y].setFinalPrototype(mg.getPrototype("ground"));
                    mg.collapseTile(tileMap[x, y], this.tileMap);
                }

                while (!(x == midX && y == midY))
                {
                    Debug.WriteLine("In while loop!!!  x = " + x + ", y = " + y);
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
                Debug.Write("Complete!\n");
                
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
