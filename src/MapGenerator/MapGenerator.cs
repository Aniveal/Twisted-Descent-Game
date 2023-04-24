﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

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

        //Room List
        public List<Room> roomList;

        //All rock textures
        private List<Texture2D> rockTextures;

        private List<Texture2D> wallTextures;

        private List<Texture2D> cliffTextures;

        //The list of all possible prototypes we want to use in the map generation
        public List<Prototype> rockPrototypes = new List<Prototype>();

        public List<Prototype> wallPrototypes = new List<Prototype>();
        public List<Prototype> cliffPrototypes = new List<Prototype>();

        public MapGenerator(RopeGame ropeGame)
        {
            rg = ropeGame;
            createPrototypeLists();
        }

        public void createProceduralMap(int size, int maxRoomSize)
        {
            DungeonGraph graph = new DungeonGraph(this);

            graph.createDungeonMap(size, maxRoomSize);
            roomList = graph.rooms;

            foreach (Room room in roomList)
            {
                room.generateRoom();

                int nColumns = room.sizeX * room.sizeY / 30;

                room.placeColumns(nColumns);
                room.placeEnemies(nColumns / 2);
            }
        }

        //Creates a map with 3 rooms
        public bool hardcodedMap()
        {
            RoomSettings rs = new RoomSettings("StarterRoom", new List<List<Prototype>>() { rockPrototypes });
            Room r1 = new Room(this, rs, 0, -3, 10, 10, 0); //(-5, -5), (5, 5)
            Room r2 = new Room(this, rs, 10, -3, 20, 15, 1);  //(5, -5), (15, 10)
            Room r3 = new Room(this, rs, -30, 7, 40, 50, 2);   //(-5, 5), (2, 55)
            Room r4 = new Room(this, rs, 20, 12, 10, 45, 3); //(15, 10), (25, 55)

            r1.createOpening(0, 1, 3);
            r1.createOpening(1, 0, 3);

            r1.createOpening(2, r1.sizeY - 1, 1);
            r3.createOpening(r3.sizeX - 8, 0, 1);

            r1.createOpening(r1.sizeX - 1, 2, 1);
            r2.createOpening(0, 2, 1);

            r2.createOpening(15, r2.sizeY - 1, 1);
            r4.createOpening(5, 0, 1);

            r4.createOpening(2, r4.sizeY - 1, 1);

            roomList = new List<Room> { r1, r2, r3, r4 };

            foreach(Room room in roomList)
            {
                room.generateRoom();

                int nColumns = room.sizeX * room.sizeY / 30;

                room.placeColumns(nColumns);
                room.placeEnemies(nColumns / 4);
            }


            return true;
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
            Texture2D ground = rg.Content.Load<Texture2D>("Sprites/ground");

            //Create prototypes for each texture; Look from bottom or from right side!!!
            //0: nothing; 1: right wall; 2: left wall; 3: all wall
            rockPrototypes.Add(new Prototype(rockTextures[0], "Wall1rd", new int[] { 0, 1, 0, 2 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[1], "Wall1ru", new int[] { 1, 0, 0, 1 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[2], "Wall1lu", new int[] { 2, 0, 1, 0 }, 1,false));
            rockPrototypes.Add(new Prototype(rockTextures[3], "Wall1ld", new int[] { 0, 2, 2, 0 }, 1, false));

            rockPrototypes.Add(new Prototype(rockTextures[4], "Wall2l", new int[] { 2, 2, 3, 0 }, 5, false));
            rockPrototypes.Add(new Prototype(rockTextures[5], "Wall2u", new int[] { 3, 0, 1, 1 }, 5, false));
            rockPrototypes.Add(new Prototype(rockTextures[6], "Wall2r", new int[] { 1, 1, 0, 3 }, 5, false));
            rockPrototypes.Add(new Prototype(rockTextures[7], "Wall2d", new int[] { 0, 3, 2, 2 }, 5, false));
            rockPrototypes.Add(new Prototype(rockTextures[8], "Wall3ul", new int[] { 1, 3, 2, 3 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[11], "Wall3ur", new int[] { 2, 3, 3, 2 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[9], "Wall3dl", new int[] { 3, 1, 1, 3 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[10], "Wall3dr", new int[] { 3, 2, 3, 1 }, 1, false));
            rockPrototypes.Add(new Prototype(rockTextures[12], "FullWall", new int[] { 3, 3, 3, 3 }, 100, false));

            rockPrototypes.Add(new Prototype(ground, "ground", new int[] { 0, 0, 0, 0 }, 600, true));

            wallTextures = new List<Texture2D>
            {
                rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_01"), //0
                rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_02"), //1
                rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_03"), //2
                rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_04"), //3
                rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_05"), //4
                rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_06") //5
            };

            //New slots: 5: Rock Wall
            wallPrototypes.Add(new Prototype(wallTextures[0], "RockWall_ud", new int[] { 5, 5, 0, 0 }, 30, false)); //base
            wallPrototypes.Add(new Prototype(wallTextures[0], "RockWall_ud", new int[] { 5, 0, 0, 0 }, 5, false)); //end piece
            wallPrototypes.Add(new Prototype(wallTextures[0], "RockWall_ud", new int[] { 0, 5, 0, 0 }, 5, false)); //end piece
            wallPrototypes.Add(new Prototype(wallTextures[1], "RockWall_lr", new int[] { 0, 0, 5, 5 }, 30, false)); //base
            wallPrototypes.Add(new Prototype(wallTextures[1], "RockWall_lr", new int[] { 0, 0, 0, 5 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[1], "RockWall_lr", new int[] { 0, 0, 5, 0 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[2], "RockWall_ur", new int[] { 5, 0, 0, 5 }, 30, false)); //base
            wallPrototypes.Add(new Prototype(wallTextures[2], "RockWall_ur", new int[] { 5, 0, 0, 0 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[2], "RockWall_ur", new int[] { 0, 0, 0, 5 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[3], "RockWall_bl", new int[] { 0, 5, 5, 0 }, 30, false)); //base
            wallPrototypes.Add(new Prototype(wallTextures[3], "RockWall_bl", new int[] { 0, 0, 5, 0 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[3], "RockWall_bl", new int[] { 0, 5, 0, 0 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[4], "RockWall_ul", new int[] { 5, 0, 5, 0 }, 30, false)); //base
            wallPrototypes.Add(new Prototype(wallTextures[4], "RockWall_ul", new int[] { 0, 0, 5, 0 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[4], "RockWall_ul", new int[] { 5, 0, 0, 0 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[5], "RockWall_dr", new int[] { 0, 5, 0, 5 }, 30, false));
            wallPrototypes.Add(new Prototype(wallTextures[5], "RockWall_dr", new int[] { 0, 0, 0, 5 }, 5, false));
            wallPrototypes.Add(new Prototype(wallTextures[5], "RockWall_dr", new int[] { 0, 5, 0, 0 }, 5, false));

            
            cliffTextures = new List<Texture2D>
            {
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1b"), //0
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1f"), //1
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1l"), //2
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1r"), //3
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2lb"), //4
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2lf"), //5
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2rb"), //6
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2rf"), //7
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3l"), //8
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3f"), //9
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3r"), //10
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3b"), //11
                rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_4") //12
            };

            //Create prototypes for each texture; Look from bottom or from right side!!!
            //New slots: 6: cliff left, ground right    7: cliff right, ground left        8: full Cliff
            cliffPrototypes.Add(new Prototype(cliffTextures[0], "Cliff_1rd", new int[] { 8, 6, 8, 7 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[1], "Cliff_1ul", new int[] { 7, 8, 6, 8 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[2], "Cliff_1dl", new int[] { 8, 7, 7, 8 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[3], "Cliff_1dr", new int[] { 6, 8, 8, 6 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[4], "Cliff_2u", new int[] { 8, 0, 7, 7 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[5], "Cliff_2r", new int[] { 7, 7, 0, 8 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[6], "Cliff_2l", new int[] { 6, 6, 8, 0 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[7], "Cliff_2d", new int[] { 0, 8, 7, 7 }, 2000, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[8], "Cliff_3ur", new int[] { 7, 0, 0, 7 }, 500, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[9], "Cliff_3dr", new int[] { 0, 7, 0, 6 }, 500, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[10], "Cliff_3dl", new int[] { 0, 6, 6, 0 }, 500, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[11], "Cliff_3ul", new int[] { 6, 0, 7, 0 }, 500, false));
            cliffPrototypes.Add(new Prototype(cliffTextures[12], "FullCliff", new int[] { 8, 8, 8, 8 }, 40000, false));




        }

    }
}
