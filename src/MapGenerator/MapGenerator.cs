using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Meridian2
{
    //This is the map generator creating rooms using wave function collapse. 
    //Put new Prototypes into the createPrototypeList
    //Might want to add later to use .json files
    //Based on https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s


    public class MapGenerator
    {
        //Parent game
        private RopeGame rg;

        //Graph of the rooms of this map, a graph consisting of rooms
        private DungeonGraph graph;

        //All rock textures
        private List<Texture2D> rockTextures;

        //The list of all possible prototypes we want to use in the map generation
        public List<Prototype> rockPrototypes = new List<Prototype>();

        public MapGenerator(RopeGame ropeGame)
        {
            rg = ropeGame;
            createPrototypeLists();
        }

        public Room createSingleRoom()
        {
            RoomSettings rs = new RoomSettings("StarterRoom", rockPrototypes);
            Room room = new Room(this, rs, 30, 30);

            room.createOpening(29, 15, 5);
            room.createOpening(2, 0, 5);
            room.connectOpenings();
            room.generateRoom();
            return room;
        }
        
        //Initializes the prototypes
        private void createPrototypeLists()
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
            rockPrototypes.Add(new Prototype(rockTextures[0], "Wall1rd", new int[] { 0, 1, 0, 2 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[1], "Wall1ru", new int[] { 1, 0, 0, 1 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[2], "Wall1lu", new int[] { 2, 0, 1, 0 }, 1,false));
            rockPrototypes.Add(new Prototype(rockTextures[3], "Wall1ld", new int[] { 0, 2, 2, 0 }, 1, false));

            rockPrototypes.Add(new Prototype(rockTextures[4], "Wall2l", new int[] { 2, 2, 3, 0 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[5], "Wall2u", new int[] { 3, 0, 1, 1 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[6], "Wall2r", new int[] { 1, 1, 0, 3 },1, false));
            rockPrototypes.Add(new Prototype(rockTextures[7], "Wall2d", new int[] { 0, 3, 2, 2 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[8], "Wall3ul", new int[] { 1, 3, 2, 3 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[11], "Wall3ur", new int[] { 2, 3, 3, 2 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[9], "Wall3dl", new int[] { 3, 1, 1, 3 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[10], "Wall3dr", new int[] { 3, 2, 3, 1 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[12], "FullWall", new int[] { 3, 3, 3, 3 }, 100, false));

            rockPrototypes.Add(new Prototype(ground, "ground", new int[] { 0, 0, 0, 0 }, 200, true));
        }

        

        



    }
}
