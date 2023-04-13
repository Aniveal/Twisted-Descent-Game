using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2
{
    //This is the map generator creating rooms using wave function collapse. 
    //Put new Prototypes into the createPrototypeList
    //Might want to add later to use .json files
    //Based on https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s


    public class MapGenerator
    {
        private RopeGame rg;

        private List<Texture2D> rockTextures;

        //The list of all possible prototypes we want to use in the map generation
        private List<Prototype> prototypes = new List<Prototype>();

        public MapGenerator(RopeGame ropeGame)
        {
            rg = ropeGame;
            createPrototypeList();
        }

        public Tile[,] createMap(int mapX, int mapY)
        {
            Tile[,] tilemap = new Tile[mapX, mapY];
            

            runWaveFunctionCollapse(tilemap);

            return tilemap;

        }

        //Creates the tilemap for a fully enclosed room 
        public Tile[,] createRoom(int mapX, int mapY)
        {
            Room room = new Room(this, mapX, mapY);

            initializeWaveFunction(room.tileMap);

            
            
            room.createOpening(0, 4, 3);
            room.createOpening(mapX - 1, mapY - 5, 4);
            room.createOpening(6, 0, 6);

            room.createBorder();

            room.connectOpenings();

            runWaveFunctionCollapse(room.tileMap);

            return room.tileMap;
        }

        //Fills out entire grid with all prototypes
        public void initializeWaveFunction(Tile[,] tileMap)
        {
            int mapX = tileMap.GetLength(0);
            int mapY = tileMap.GetLength(1);

            //Create all tiles, initialize with full set of prototypes
            for (int x = 0; x < mapX; x++)
            {
                for (int y = 0; y < mapY; y++)
                {
                    tileMap[x, y] = new Tile(prototypes);
                    tileMap[x, y].x = x;
                    tileMap[x, y].y = y;
                }
            }
        }

        //Gets the prototype by name
        public Prototype getPrototype(string s)
        {
            foreach (Prototype prototype in prototypes)
                if (prototype.name == s)
                    return prototype;
            return null;
        }

        private void createPrototypeList()
        {
            //Load the textures we want for the Prototypes
            rockTextures = new List<Texture2D> {
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_1b"), // 0
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_1r"), // 1
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_1f"), // 2 
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_1l"), // 3
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_2lf"), // 4
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_2rf"), // 5 
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_2rb"), // 6
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_2lb"), // 7
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_3b"), // 8
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_3r"), // 9
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_3f"), // 10
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_3l"), // 11
                rg.Content.Load<Texture2D>("Sprites/Rock/wall_4") // 12
            };
            Texture2D ground = rg.Content.Load<Texture2D>("ground");

            //Create prototypes for each texture; Look from bottom or from right side!!!
            //0: nothing; 1: right wall; 2: left wall; 3: all wall
            prototypes.Add(new Prototype(rockTextures[0], "Wall1rd", new int[] { 0, 1, 0, 2 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[1], "Wall1ru", new int[] { 1, 0, 0, 1 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[2], "Wall1lu", new int[] { 2, 0, 1, 0 }, 5,false));
            prototypes.Add(new Prototype(rockTextures[3], "Wall1ld", new int[] { 0, 2, 2, 0 }, 5, false));
            
            prototypes.Add(new Prototype(rockTextures[4], "Wall2l", new int[] { 2, 2, 3, 0 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[5], "Wall2u", new int[] { 3, 0, 1, 1 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[6], "Wall2r", new int[] { 1, 1, 0, 3 },5, false));
            prototypes.Add(new Prototype(rockTextures[7], "Wall2d", new int[] { 0, 3, 2, 2 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[8], "Wall3ul", new int[] { 1, 3, 2, 3 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[11], "Wall3ur", new int[] { 2, 3, 3, 2 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[9], "Wall3dl", new int[] { 3, 1, 1, 3 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[10], "Wall3dr", new int[] { 3, 2, 3, 1 }, 5, false));
            prototypes.Add(new Prototype(rockTextures[12], "FullWall", new int[] { 3, 3, 3, 3 }, 5, false));

            prototypes.Add(new Prototype(ground, "ground", new int[] { 0, 0, 0, 0 }, 100, true));



        }

        private void runWaveFunctionCollapse(Tile[,] tileMap)
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

                //Choose one tile at random:
                Random random = new Random();
                Tile chosen = list[random.Next(list.Count)];

                chosen.chooseRandomPrototype();

                Debug.WriteLine("Starting collapsing round " + counter);
                //Collapse the wave function:
                collapseTile(chosen, tileMap);
                

                counter++;
                if (counter > mapX * mapY)
                {
                    Debug.WriteLine("Error, wave function could not terminate");
                    break;
                }


            }

        }

        //Collapse a tile and recursively collapse all neighbours
        public void collapseTile(Tile tile, Tile[,] tilemap)
        {
            int mapX = tilemap.GetLength(0);
            int mapY = tilemap.GetLength(1);

            if ((tile.x - 1) >= 0)
                if (tilemap[tile.x - 1, tile.y].collapseFunction(tile, "right"))
                    collapseTile(tilemap[tile.x - 1, tile.y], tilemap); //if there was a change in this tile, recurse
                    

            if ((tile.x + 1) < mapX)
                if (tilemap[tile.x + 1, tile.y].collapseFunction(tile, "left"))
                    collapseTile(tilemap[tile.x + 1, tile.y], tilemap);

            if ((tile.y - 1) >= 0)
                if(tilemap[tile.x, tile.y - 1].collapseFunction(tile, "down"))
                    collapseTile(tilemap[tile.x, tile.y - 1], tilemap);

            if ((tile.y + 1) < mapY)
                if(tilemap[tile.x, tile.y + 1].collapseFunction(tile, "up"))
                    collapseTile(tilemap[tile.x, tile.y + 1], tilemap);
        }




    }
}
