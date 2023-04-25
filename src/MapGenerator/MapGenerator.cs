﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 
//This is the map generator creating rooms using wave function collapse. 
//Put new Prototypes into the createPrototypeList
//Might want to add later to use .json files
//Based on https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s

public class MapGenerator {
    private List<Texture2D> _cliffTextures;

    //Graph of the rooms of this map, a graph consisting of rooms
    private DungeonGraph _graph;

    //Parent game
    private readonly RopeGame _rg;

    //All rock textures
    private List<Texture2D> _rockTextures;

    private List<Texture2D> _wallTextures;
    public List<Prototype> CliffPrototypes = new();

    //The list of all possible prototypes we want to use in the map generation
    public List<Prototype> RockPrototypes = new();

    //Room List
    public List<Room> RoomList;

    public List<Prototype> WallPrototypes = new();

    public MapGenerator(RopeGame ropeGame) {
        _rg = ropeGame;
        createPrototypeLists();
    }

    public void createProceduralMap(int size, int maxRoomSize) {
        var graph = new DungeonGraph(this);

        graph.createDungeonMap(size, maxRoomSize);
        RoomList = graph.Rooms;

        foreach (var room in RoomList) {
            room.generateRoom();

            var nColumns = room.SizeX * room.SizeY / 30;

            room.placeColumns(nColumns);
            room.placeEnemies(nColumns / 2);
        }
    }

    //Creates a map with 3 rooms
    public bool hardcodedMap() {
        var rs = new RoomSettings("StarterRoom", new List<List<Prototype>> { RockPrototypes });
        var r1 = new Room(this, rs, 0, -3, 10, 10, 0); //(-5, -5), (5, 5)
        var r2 = new Room(this, rs, 10, -3, 20, 15, 1); //(5, -5), (15, 10)
        var r3 = new Room(this, rs, -30, 7, 40, 50, 2); //(-5, 5), (2, 55)
        var r4 = new Room(this, rs, 20, 12, 10, 45, 3); //(15, 10), (25, 55)

        r1.createOpening(0, 1, 3);
        r1.createOpening(1, 0, 3);

        r1.createOpening(2, r1.SizeY - 1, 1);
        r3.createOpening(r3.SizeX - 8, 0, 1);

        r1.createOpening(r1.SizeX - 1, 2, 1);
        r2.createOpening(0, 2, 1);

        r2.createOpening(15, r2.SizeY - 1, 1);
        r4.createOpening(5, 0, 1);

        r4.createOpening(2, r4.SizeY - 1, 1);

        RoomList = new List<Room> { r1, r2, r3, r4 };

        foreach (var room in RoomList) {
            room.generateRoom();

            var nColumns = room.SizeX * room.SizeY / 30;

            room.placeColumns(nColumns);
            room.placeEnemies(nColumns / 4);
        }


        return true;
    }


    //Initializes the prototypes
    private void createPrototypeLists() {
        //Load the textures we want for the Prototypes
        _rockTextures = new List<Texture2D> {
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1b"), // 0
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1r"), // 1
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1f"), // 2 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1l"), // 3
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2lf"), // 4
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2rf"), // 5 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2rb"), // 6
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2lb"), // 7
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3b"), // 8
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3r"), // 9
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3f"), // 10
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3l"), // 11
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_4") // 12
        };
        var ground = _rg.Content.Load<Texture2D>("Sprites/ground");

        //Create prototypes for each texture; Look from bottom or from right side!!!
        //0: nothing; 1: right wall; 2: left wall; 3: all wall
        RockPrototypes.Add(new Prototype(_rockTextures[0], "Wall1rd", new[] { 0, 1, 0, 2 }, 1, false));
        RockPrototypes.Add(new Prototype(_rockTextures[1], "Wall1ru", new[] { 1, 0, 0, 1 }, 1, false));
        RockPrototypes.Add(new Prototype(_rockTextures[2], "Wall1lu", new[] { 2, 0, 1, 0 }, 1, false));
        RockPrototypes.Add(new Prototype(_rockTextures[3], "Wall1ld", new[] { 0, 2, 2, 0 }, 1, false));

        RockPrototypes.Add(new Prototype(_rockTextures[4], "Wall2l", new[] { 2, 2, 3, 0 }, 5, false));
        RockPrototypes.Add(new Prototype(_rockTextures[5], "Wall2u", new[] { 3, 0, 1, 1 }, 5, false));
        RockPrototypes.Add(new Prototype(_rockTextures[6], "Wall2r", new[] { 1, 1, 0, 3 }, 5, false));
        RockPrototypes.Add(new Prototype(_rockTextures[7], "Wall2d", new[] { 0, 3, 2, 2 }, 5, false));
        RockPrototypes.Add(new Prototype(_rockTextures[8], "Wall3ul", new[] { 1, 3, 2, 3 }, 1, false));
        RockPrototypes.Add(new Prototype(_rockTextures[11], "Wall3ur", new[] { 2, 3, 3, 2 }, 1, false));
        RockPrototypes.Add(new Prototype(_rockTextures[9], "Wall3dl", new[] { 3, 1, 1, 3 }, 1, false));
        RockPrototypes.Add(new Prototype(_rockTextures[10], "Wall3dr", new[] { 3, 2, 3, 1 }, 1, false));
        RockPrototypes.Add(new Prototype(_rockTextures[12], "FullWall", new[] { 3, 3, 3, 3 }, 100, false));

        RockPrototypes.Add(new Prototype(ground, "ground", new[] { 0, 0, 0, 0 }, 600, true));

        _wallTextures = new List<Texture2D> {
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_01"), //0
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_02"), //1
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_03"), //2
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_04"), //3
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_05"), //4
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_06") //5
        };

        //New slots: 5: Rock Wall
        WallPrototypes.Add(new Prototype(_wallTextures[0], "RockWall_ud", new[] { 5, 5, 0, 0 }, 30, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[0], "RockWall_ud", new[] { 5, 0, 0, 0 }, 5, false)); //end piece
        WallPrototypes.Add(new Prototype(_wallTextures[0], "RockWall_ud", new[] { 0, 5, 0, 0 }, 5, false)); //end piece
        WallPrototypes.Add(new Prototype(_wallTextures[1], "RockWall_lr", new[] { 0, 0, 5, 5 }, 30, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[1], "RockWall_lr", new[] { 0, 0, 0, 5 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[1], "RockWall_lr", new[] { 0, 0, 5, 0 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[2], "RockWall_ur", new[] { 5, 0, 0, 5 }, 30, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[2], "RockWall_ur", new[] { 5, 0, 0, 0 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[2], "RockWall_ur", new[] { 0, 0, 0, 5 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[3], "RockWall_bl", new[] { 0, 5, 5, 0 }, 30, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[3], "RockWall_bl", new[] { 0, 0, 5, 0 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[3], "RockWall_bl", new[] { 0, 5, 0, 0 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[4], "RockWall_ul", new[] { 5, 0, 5, 0 }, 30, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[4], "RockWall_ul", new[] { 0, 0, 5, 0 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[4], "RockWall_ul", new[] { 5, 0, 0, 0 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[5], "RockWall_dr", new[] { 0, 5, 0, 5 }, 30, false));
        WallPrototypes.Add(new Prototype(_wallTextures[5], "RockWall_dr", new[] { 0, 0, 0, 5 }, 5, false));
        WallPrototypes.Add(new Prototype(_wallTextures[5], "RockWall_dr", new[] { 0, 5, 0, 0 }, 5, false));


        _cliffTextures = new List<Texture2D> {
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1b"), //0
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1f"), //1
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1l"), //2
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_1r"), //3
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2lb"), //4
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2lf"), //5
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2rb"), //6
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_2rf"), //7
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3l"), //8
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3f"), //9
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3r"), //10
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_3b"), //11
            _rg.Content.Load<Texture2D>("Sprites/Cliff/cliff_4") //12
        };

        //Create prototypes for each texture; Look from bottom or from right side!!!
        //New slots: 6: cliff left, ground right    7: cliff right, ground left        8: full Cliff
        CliffPrototypes.Add(new Prototype(_cliffTextures[0], "Cliff_1rd", new[] { 8, 6, 8, 7 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[1], "Cliff_1ul", new[] { 7, 8, 6, 8 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[2], "Cliff_1dl", new[] { 8, 7, 7, 8 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[3], "Cliff_1dr", new[] { 6, 8, 8, 6 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[4], "Cliff_2u", new[] { 8, 0, 7, 7 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[5], "Cliff_2r", new[] { 7, 7, 0, 8 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[6], "Cliff_2l", new[] { 6, 6, 8, 0 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[7], "Cliff_2d", new[] { 0, 8, 7, 7 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[8], "Cliff_3ur", new[] { 7, 0, 0, 7 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[9], "Cliff_3dr", new[] { 0, 7, 0, 6 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[10], "Cliff_3dl", new[] { 0, 6, 6, 0 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[11], "Cliff_3ul", new[] { 6, 0, 7, 0 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[12], "FullCliff", new[] { 8, 8, 8, 8 }, 40000, false, true));
    }
}