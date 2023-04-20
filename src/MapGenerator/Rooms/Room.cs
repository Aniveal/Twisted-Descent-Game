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
        //The settings for this room
        protected RoomSettings rs;

        public int index;

        //Tile size of the room
        public int sizeX, sizeY;

        //Top left edge of the room,
        public int posX, posY;

        //Stores the points where there is an opening
        protected List<Vector2> openings;

        //The map of this room
        public Tile[,] tileMap;

        //Column Positions
        public List<Vector2> columns = new List<Vector2>();

        //Enemies
        public List<Vector2> enemyPositions = new List<Vector2>();

        //Pointer to the mapGenerator this room is part of
        protected MapGenerator mg;

        //If all tiles have been initialized
        protected bool initialized = false;

        public Room(MapGenerator mg, RoomSettings rs, int x, int y, int sizeX, int sizeY, int index)
        {
            this.index = index;
            openings = new List<Vector2>();
            this.mg = mg;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.posX = x;
            this.posY = y;
            tileMap = new Tile[sizeX, sizeY];
            this.rs = rs;
            initializeTileMap();
        }

        //Finishes the room generation, results in a workable tileMap
        public void generateRoom()
        {
            createBorder();
            connectOpenings();
            runWaveFunctionCollapse();
            
        }

        //Adds an inner opening, i.e. a guaranteed connected tile
        public void innerOpening(int x, int y)
        {
            openings.Add(new Vector2(x, y));

        }

        public void placeColumns(int n)
        {
            columns.Clear();

            int i = 0;
            int j = 0;

            while(i < n)
            {
                float x = (float)RNGsus.Instance.NextDouble() * sizeX;
                float y = (float)RNGsus.Instance.NextDouble() * sizeY;

                if (tileMap[(int)Math.Floor(x), (int)Math.Floor(y)].finalPrototype.walkable)
                {
                    if (noColumnsNear(x, y))
                    {
                        columns.Add(new Vector2(x, y));
                        i++;
                    }
                    else j++;
                }
                else j++;

                if(j > 1000)
                {
                    Debug.WriteLine("Didnt find walkable space!!!");
                    return;
                }
            }
        }

        bool noColumnsNear(float x, float y)
        {
            foreach (Vector2 col in columns)
            {
                if (Math.Abs(col.X - x) < 1 && Math.Abs(col.Y - y) < 1)
                    return false;
            }

                return true;
        }

        public void placeEnemies(int n)
        {
            enemyPositions.Clear();

            int i = 0;
            int j = 0;

            while (i < n)
            {
                float x = (float)RNGsus.Instance.NextDouble() * sizeX;
                float y = (float)RNGsus.Instance.NextDouble() * sizeY;

                if (tileMap[(int)x, (int)y].finalPrototype.walkable)
                {
                    enemyPositions.Add(new Vector2((int)x, (int)y));
                    i++;
                }
                else j++;

                if (j > 1000)
                {
                    Debug.WriteLine("Didnt find walkable space!!!");
                    return;
                }
            }
        }

        //Fill out the whole tilemap with new Tiles
        public void initializeTileMap()
        {
            //Create all tiles, initialize with full set of prototypes
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    tileMap[x, y] = new Tile(rs.possiblePrototypes, this);
                    tileMap[x, y].x = x;
                    tileMap[x, y].y = y;
                }
            }
            initialized = true;
        }

        //Creates an opening to another room at x, y with length l. l goes from x,y into the positive direction. Has to be at least 3
        //Don't go too close to an edge, otherwise undefined behaviuor may happen
        public void createOpening(int x, int y, int l)
        {
            Debug.WriteLine("Create Opening");

            if(!(x == 0 || x == sizeX - 1 || y == 0 || y == sizeY - 1))
            {
                Debug.WriteLine("Error, opening cant be created, wrong values");
            }
            

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
                    tileMap[x, y + i].setFinalPrototype(getPrototype("ground"));
                    collapseTile(tileMap[x, y + i]);
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
                    tileMap[x + i, y].setFinalPrototype(getPrototype("ground"));
                    collapseTile(tileMap[x + i, y]);
                }
            }
            else Debug.WriteLine("Error while creating opening: Point " + x + "," + y + " is not on the border!!! SizeX = " + sizeX + ", SizeY = "+sizeY);
        }

        public void setTile(int x, int y, Prototype p)
        {
            tileMap[x, y].setFinalPrototype(p);
        }

        public void setWalkable(int x, int y)
        {
            tileMap[x, y].makeWalkable();
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

                Debug.Write("Opening.... " + x + ", " + y);

                //Handle case where opening is on left or right
                if (x <= 1 || x >= (sizeX - 2))
                {
                    //First, do one step towards the middle
                    while(x <= 1)
                    {
                        tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                        collapseTile(tileMap[x, y]);
                        x = x + 1;
                    }
                        
                    while (x >= (sizeX - 2))
                    {
                        tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                        collapseTile(tileMap[x, y]);
                        x = x - 1;
                    } 

                    tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                    collapseTile(tileMap[x, y]);
                }
                
                //opening on top or bottom
                if (y <= 1 || y >= (sizeX - 2))
                {
                    //First, do one step towards the middle
                    while (y <= 1)
                    {
                        tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                        collapseTile(tileMap[x, y]);
                        y = y + 1;
                    }

                    while (y >= (sizeY - 2))
                    {
                        tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                        collapseTile(tileMap[x, y]);
                        y = y - 1;
                    }

                    tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                    collapseTile(tileMap[x, y]);
                }

                while (!(x == midX && y == midY))
                {
                    Debug.WriteLine("Setting tile to walkable!!!  x = " + x + ", y = " + y);
                    double rand = RNGsus.Instance.NextDouble();

                    //Corner cases
                    if (x == midX)
                    {
                        if (y < midY) y++;
                        else y--;
                        tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                        collapseTile(tileMap[x, y]);
                        continue;
                    }
                    if (y == midY)
                    {
                        if (x < midX) x++;
                        else x--;
                        tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                        collapseTile(tileMap[x, y]);
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
                    tileMap[x, y].setFinalPrototype(getPrototype("ground"));
                    collapseTile(tileMap[x, y]);
                }
                Debug.Write("Complete!\n");
                
            }
        }


        //Gets a prototype reference by name
        public Prototype getPrototype(string s)
        {
            foreach (Prototype prototype in rs.possiblePrototypes)
                if (prototype.name == s)
                    return prototype;
            return null;
        }


        //Creates a border of solid walls around room everywhere where there is no opening
        //Do this after setting openings!
        public void createBorder()
        {
            Debug.WriteLine("Creating Borders");

            for (int x = 0; x < sizeX; x++)
            {
                if(tileMap[x, 0].superpositions.Contains(getPrototype("FullWall")))
                {
                    tileMap[x, 0].setFinalPrototype(getPrototype("FullWall"));
                    collapseTile(tileMap[x, 0]);
                }
                if (tileMap[x, sizeY - 1].superpositions.Contains(getPrototype("FullWall")))
                {
                    tileMap[x, sizeY - 1].setFinalPrototype(getPrototype("FullWall")); ;
                    collapseTile(tileMap[x, sizeY - 1]);
                }

            }
            for (int y = 0; y < sizeY; y++)
            {
                if (tileMap[0, y].superpositions.Contains(getPrototype("FullWall")))
                {
                    tileMap[0, y].setFinalPrototype(getPrototype("FullWall")); ;
                    collapseTile(tileMap[0, y]);
                }

                if (tileMap[sizeX - 1, y].superpositions.Contains(getPrototype("FullWall")))
                {
                    tileMap[sizeX - 1, y].setFinalPrototype(getPrototype("FullWall")); ;
                    collapseTile(tileMap[sizeX - 1, y]);
                }
                    
            }
        }

        //Runs wave function collapse to generate this room, tiles must be initialized before!
        private void runWaveFunctionCollapse()
        {
            int mapX = tileMap.GetLength(0);
            int mapY = tileMap.GetLength(1);

            Debug.WriteLine("Started Wave function collapse! Map x = " + mapX + ", mapY = " + mapY);

            int counter = 0;
            while (true)
            {
                //Find lowest amount of superpositions
                Tile t;
                int n = 100000;

                foreach (Tile tile in tileMap)
                {
                    if (tile.superpositions.Count > 1 && tile.superpositions.Count < n)
                        n = tile.superpositions.Count;
                }

                //Get list of all possible tiles
                List<Tile> list = new List<Tile>();
                //weights all summed up
                int totalWeights = 0;
                foreach (Tile tile in tileMap)
                {
                    if (tile.superpositions.Count == n)
                    {
                        list.Add(tile);
                    }
                }

                if (list.Count <= 0)
                    break;

                Tile chosen = list[RNGsus.Instance.Next(list.Count)];

                chosen.chooseRandomPrototype();

                //Collapse the wave function:
                collapseTile(chosen);


                counter++;
                if (counter > mapX * mapY)
                {
                    Debug.WriteLine("Error, wave function could not terminate");
                    break;
                }


            }

        }

        //Collapse a single tile in a tilemap and recursively collapse all neighbours.
        //Call this whenever you change a tile manually in a tilemap
        public void collapseTile(Tile tile)
        {
            int mapX = tileMap.GetLength(0);
            int mapY = tileMap.GetLength(1);

            if ((tile.x - 1) >= 0)
                if (tileMap[tile.x - 1, tile.y].collapseFunction(tile, "right"))
                    collapseTile(tileMap[tile.x - 1, tile.y]); //if there was a change in this tile, recurse


            if ((tile.x + 1) < mapX)
                if (tileMap[tile.x + 1, tile.y].collapseFunction(tile, "left"))
                    collapseTile(tileMap[tile.x + 1, tile.y]);

            if ((tile.y - 1) >= 0)
                if (tileMap[tile.x, tile.y - 1].collapseFunction(tile, "down"))
                    collapseTile(tileMap[tile.x, tile.y - 1]);

            if ((tile.y + 1) < mapY)
                if (tileMap[tile.x, tile.y + 1].collapseFunction(tile, "up"))
                    collapseTile(tileMap[tile.x, tile.y + 1]);
        }

    }
}
