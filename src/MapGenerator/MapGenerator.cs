﻿using Microsoft.Xna.Framework.Graphics;
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

        private int mapX;
        private int mapY;

        //The map, consisting of a 2d array of Tiles
        private Tile[,] tilemap;

        //The list of all possible prototypes we want to use in the map generation
        private List<Prototype> prototypes = new List<Prototype>();

        public MapGenerator(RopeGame ropeGame)
        {
            rg = ropeGame;
        }

        public Tile[,] createMap(int mapX, int mapY)
        {
            this.mapX = mapX;
            this.mapY = mapY;
            tilemap = new Tile[mapX, mapY];
            

            createPrototypeList();

            runWaveFunctionCollapse();

            return tilemap;

        }

        private void createPrototypeList()
        {
            //Load the textures we want for the Prototypes
            rockTextures = new List<Texture2D> {
                rg.Content.Load<Texture2D>("wall_1b"), // 1
                rg.Content.Load<Texture2D>("wall_1r"), // 2
                rg.Content.Load<Texture2D>("wall_1f"), // 3 
                rg.Content.Load<Texture2D>("wall_1l"), // 4
                rg.Content.Load<Texture2D>("wall_2lf"), // 5
                rg.Content.Load<Texture2D>("wall_2rf"), // 6 
                rg.Content.Load<Texture2D>("wall_2rb"), // 7
                rg.Content.Load<Texture2D>("wall_2lb"), // 8
                rg.Content.Load<Texture2D>("wall_3b"), // 9
                rg.Content.Load<Texture2D>("wall_3r"), // 10
                rg.Content.Load<Texture2D>("wall_3f"), // 11
                rg.Content.Load<Texture2D>("wall_3l"), // 12
                rg.Content.Load<Texture2D>("wall_4") // 13
            };

            //Create prototypes for each texture; Look from bottom or from right side!!!
            //0: nothing; 1: right wall; 2: left wall; 3: all wall
            prototypes.Add(new Prototype(rockTextures[0], "Wall1rd", new int[] { 0, 1, 0, 2 }));
            prototypes.Add(new Prototype(rockTextures[1], "Wall1ru", new int[] { 1, 0, 1, 0 }));
            prototypes.Add(new Prototype(rockTextures[2], "Wall1lu", new int[] { 2, 0, 0, 1 }));
            prototypes.Add(new Prototype(rockTextures[3], "Wall1ld", new int[] { 0, 2, 2, 0 }));
            //prototypes.Add(new Prototype(rockTextures[4], "Wall2l", new int[] { 2, 1, 3, 0 }));
            //prototypes.Add(new Prototype(rockTextures[5], "Wall2u", new int[] { 3, 0, 1, 2 }));
            //prototypes.Add(new Prototype(rockTextures[6], "Wall2r", new int[] { 1, 2, 0, 3 }));
            //prototypes.Add(new Prototype(rockTextures[7], "Wall2d", new int[] { 0, 3, 2, 1 }));
            //prototypes.Add(new Prototype(rockTextures[8], "Wall3ul", new int[] { 1, 3, 2, 3 }));
            //prototypes.Add(new Prototype(rockTextures[11], "Wall3ur", new int[] { 2, 3, 3, 2 }));
            //prototypes.Add(new Prototype(rockTextures[9], "Wall3dl", new int[] { 3, 1, 1, 3 }));
            //prototypes.Add(new Prototype(rockTextures[10], "Wall3dr", new int[] { 3, 2, 3, 1 }));


        }

        private void runWaveFunctionCollapse()
        {
            Debug.WriteLine("Started Wave function collapse!");

            //Create all tiles
            for(int x = 0; x < mapX; x++)
            {
                for(int y = 0; y < mapY; y++)
                {
                    tilemap[x, y] = new Tile(prototypes);
                    tilemap[x, y].position = new Microsoft.Xna.Framework.Vector2(x, y);
                }
            }

            int counter = 0;
            while (true)
            {
                //Find lowest amount of superpositions
                Tile t;
                int n = 100000;
                foreach (Tile tile in tilemap)
                {
                    if (tile.numSuperpositions > 1 && tile.numSuperpositions < n)
                        n = tile.numSuperpositions;
                }

                Debug.WriteLine("N is: " + n);

                //Get list of all possible tiles
                List<Tile> list = new List<Tile>();
                foreach (Tile tile in tilemap)
                {
                    if (tile.numSuperpositions == n)
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
                collapseTile(chosen);
                

                counter++;
                if (counter > 2000000)
                {
                    Debug.WriteLine("Error, wave function could not terminate");
                    break;
                }


            }


        }

        //Collapse a tile and recursively collapse all neighbours
        public void collapseTile(Tile tile)
        {
            if ((tile.x - 1) > 0)
                if (tilemap[tile.x - 1, tile.y].collapseFunction(tile, "right"))
                    collapseTile(tilemap[tile.x - 1, tile.y]); //if there was a change in this tile, recurse

            if ((tile.x + 1) < mapX)
                if (tilemap[tile.x + 1, tile.y].collapseFunction(tile, "left"))
                    collapseTile(tilemap[tile.x + 1, tile.y]);

            if ((tile.y - 1) > 0)
                if(tilemap[tile.x, tile.y - 1].collapseFunction(tile, "up"))
                    collapseTile(tilemap[tile.x, tile.y - 1]);

            if ((tile.y + 1) < mapY)
                if(tilemap[tile.x, tile.y + 1].collapseFunction(tile, "down"))
                    collapseTile(tilemap[tile.x, tile.y + 1]);
        }




    }
}
