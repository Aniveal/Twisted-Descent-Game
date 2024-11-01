﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;


namespace TwistedDescent; 
//This is the map generator creating rooms using wave function collapse. 
//Put new Prototypes into the createPrototypeList
//Might want to add later to use .json files
//Based on https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s

public class MapGenerator {
    private List<Texture2D> _cliffTextures;

    public Prototype FinishPrototype;
    public Prototype FinishSmallPrototype;
    public Prototype FinishPrototypeEmpty;

    //Graph of the rooms of this map, a graph consisting of rooms
    private DungeonGraph _graph;

    //Parent game
    public readonly RopeGame _rg;

    //All rock textures
    private List<Texture2D> _rockTextures;
    private List<Texture2D> _groundTextures;

    private Texture2D _exitTexture;
    

    public List<Prototype> entranceExitPrototypes = new();

    private List<Texture2D> _wallTextures;
    public List<Prototype> CliffPrototypes = new();

    //The list of all possible prototypes we want to use in the map generation
    public List<Prototype> RockPrototypes = new();

    //Room List
    public List<Room> RoomList;

    public List<Prototype> WallPrototypes = new();

    public Prototype finishSmallPrototype;

    public MapGenerator(RopeGame ropeGame) {
        _rg = ropeGame;
        createPrototypeLists();
    }

    //Creates a new map with difficulty level "difficulty". difficulty increases linearly.
    //difficulty 10 has twice as many enemies and rooms as difficulty 5
    public void createProceduralMap(int difficulty) {
        var graph = new DungeonGraph(this);

        int size = 10000;
        int roomSize = 30;

        //Number of rooms: difficulty
        int nRooms = difficulty - 1;

        bool generationFail = true;
        while(generationFail)
        {
            generationFail = false;
            RnGsus.Instance.NewSeed();
            graph = new DungeonGraph(this);
            size = (nRooms + 1) * (roomSize / 2);
            graph.createDungeonMap(size, roomSize, nRooms, _rg.GameData.currentDifficulty, 0);
            RoomList = graph.Rooms;

            foreach (var room in RoomList)
            {
                int tries = 0;
                while (tries < 10)
                {
                    bool success = room.generateRoom();
                    if (!success)
                        tries++;
                    else break;
                }

                if (tries > 99)
                {
                    generationFail = true;
                    break;
                }
            }
        }
        Debug.WriteLine("Created Map with " + RoomList.Count + " rooms");
    }

    public void CreateTutorialMap() {

        RoomList = new List<Room>();
        Room tRoom = new TutorialRoom(this, RockPrototypes);
        RoomList.Add(tRoom);

        //Set a random texture for each tile
        foreach (Tile t in tRoom.TileMap)
        {
            t.SetTexture();
        }
    }


    //Initializes the prototypes
    private void createPrototypeLists() 
    {

        Texture2D _entranceTexture = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/level_entrance_l");
        Texture2D _entranceTexture2 = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/level_entrance_r");
        Texture2D _entranceTexture_ground = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/ground_level_entrance_l");
        Texture2D _entranceTexture2_ground = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/ground_level_entrance_r");

        Texture2D FinishTextureSmall = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/level_exit_1_1");
        Texture2D FinishTexture1 = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/exit_lowerleft");
        Texture2D FinishTexture2 = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/exit_lowerright");
        Texture2D FinishTexture3 = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/exit_upperleft");
        Texture2D FinishTexture4 = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/exit_upperright");

        FinishPrototype = new Prototype(FinishTexture1, null, "finish", new int[] { 0, 0, 0, 0 }, 0, false);

        //Texture2D _exitTexture = _rg.Content.Load<Texture2D>("Sprites/EntranceExit/Level_entrance.png");
        //Texture2D _entranceTexture = _rg.Content.Load<Texture2D>("Sprites/GroundTiles/RockTile_01");

        FinishSmallPrototype = new Prototype(FinishTextureSmall, null, "finish", new[] { 0, 0, 0, 0 }, 1, false);

        entranceExitPrototypes.Add(new Prototype(_entranceTexture_ground, _entranceTexture, "StartL", new[] { 3, 0, 1, 1 }, 1, false));
        entranceExitPrototypes.Add(new Prototype(_entranceTexture2_ground, _entranceTexture2, "StartR", new[] { 3, 0, 1, 1 }, 1, false));

        entranceExitPrototypes.Add(new Prototype(FinishTexture1, null, "finish", new int[] { 0, 0, 0, 0 }, 0, false));
        entranceExitPrototypes.Add(new Prototype(FinishTexture2, null, "finish", new int[] { 0, 0, 0, 0 }, 0, false));
        entranceExitPrototypes.Add(new Prototype(FinishTexture3, null, "finish", new int[] { 0, 0, 0, 0 }, 0, false));
        entranceExitPrototypes.Add(new Prototype(FinishTexture4, null, "finish", new int[] { 0, 0, 0, 0 }, 0, false));


        //Load the textures we want for the Prototypes
        _rockTextures = new List<Texture2D> {
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_1b"), //0
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_1r"), //1
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_1f"), //2
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_1l"), //3
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_2lf"), //4
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_2rf"),
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_2rb"),
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_2lb"),
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_3b"),
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_3r"),
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_3f"),
            _rg.Content.Load<Texture2D>("Sprites/Rock/ground_wall_3l"),
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1b"), // 12
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1r"), // 13
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1f"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_1l"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2lf"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2rf"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2rb"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_2lb"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3b"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3r"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3f"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_3l"), // 
            _rg.Content.Load<Texture2D>("Sprites/Rock/wall_4") //
   
        };
        _groundTextures = new List<Texture2D>();

        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/ground"));

        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_00"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_01"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_02"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_03"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_04"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_05"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_06"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_07"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_08"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_09"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_11"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_rocks_11"));

        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_bones_00"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_bones_01"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_bones_02"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_bones_03"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_bones_04"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/ground_bones_05"));

        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/plant_ground_00"));
        _groundTextures.Add(_rg.Content.Load<Texture2D>("Sprites/GroundTiles/plant_ground_01"));

        int[] GroundTextureWeights = { 60, 2, 2, 2, 6, 6, 8, 8, 4, 4, 6, 8, 8, 1, 1, 1, 1, 1, 1, 2, 2};

        //Create prototypes for each texture; Look from bottom or from right side!!!
        //0: nothing; 1: right wall; 2: left wall; 3: all wall
        RockPrototypes.Add(new Prototype(_rockTextures[0], _rockTextures[12], "Wall1rd", new[] { 0, 1, 0, 2 }, 1, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[1], _rockTextures[13], "Wall1ru", new[] { 1, 0, 0, 1 }, 1, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[2], _rockTextures[14],"Wall1lu", new[] { 2, 0, 1, 0 }, 1, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[3], _rockTextures[15],"Wall1ld", new[] { 0, 2, 2, 0 }, 1, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[4], _rockTextures[16],"Wall2l", new[] { 2, 2, 3, 0 }, 2, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[5], _rockTextures[17],"Wall2u", new[] { 3, 0, 1, 1 }, 2, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[6], _rockTextures[18],"Wall2r", new[] { 1, 1, 0, 3 }, 2, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[7], _rockTextures[19],"Wall2d", new[] { 0, 3, 2, 2 }, 2, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[8], _rockTextures[20],"Wall3ul", new[] { 1, 3, 2, 3 }, 1, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[9], _rockTextures[21],"Wall3dl", new[] { 3, 1, 1, 3 }, 1, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[10], _rockTextures[22],"Wall3dr", new[] { 3, 2, 3, 1 }, 1, false, false));
        RockPrototypes.Add(new Prototype(_rockTextures[11], _rockTextures[23], "Wall3ur", new[] { 2, 3, 3, 2 }, 1, false, false));
        RockPrototypes.Add(new Prototype(null, _rockTextures[24], "FullWall", new[] { 3, 3, 3, 3 }, 16000, false));

        RockPrototypes.Add(new Prototype(_groundTextures, null, "ground", new[] { 0, 0, 0, 0 }, 10000, true, GroundTextureWeights));

        _wallTextures = new List<Texture2D> {
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_01"), //0
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_02"), //1
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_03"), //2
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_04"), //3
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_05"), //4
            _rg.Content.Load<Texture2D>("Sprites/Wall/rockwall_06"), //5
            _rg.Content.Load<Texture2D>("Sprites/Wall/ground_rockwall_01"), //6
            _rg.Content.Load<Texture2D>("Sprites/Wall/ground_rockwall_02"), 
            _rg.Content.Load<Texture2D>("Sprites/Wall/ground_rockwall_03"), 
            _rg.Content.Load<Texture2D>("Sprites/Wall/ground_rockwall_04"), 
            _rg.Content.Load<Texture2D>("Sprites/Wall/ground_rockwall_05"), 
            _rg.Content.Load<Texture2D>("Sprites/Wall/ground_rockwall_06"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/WallToRock_01"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/WallToRock_02"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/WallToRock_03"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/WallToRock_04"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/Ground_WallToRock_01"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/Ground_WallToRock_02"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/Ground_WallToRock_03"),
            _rg.Content.Load<Texture2D>("Sprites/WalltoRock/Ground_WallToRock_04")

        };

        //New slots: 5: Rock Wall
        WallPrototypes.Add(new Prototype(_wallTextures[6], _wallTextures[0], "RockWall_ud", new[] { 5, 5, 0, 0 }, 3, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[6], _wallTextures[0], "RockWall_ud", new[] { 5, 0, 0, 0 }, 1, false)); //end piece
        WallPrototypes.Add(new Prototype(_wallTextures[6], _wallTextures[0], "RockWall_ud", new[] { 0, 5, 0, 0 }, 1, false)); //end piece
        WallPrototypes.Add(new Prototype(_wallTextures[7], _wallTextures[1], "RockWall_lr", new[] { 0, 0, 5, 5 }, 3, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[7], _wallTextures[1], "RockWall_lr", new[] { 0, 0, 0, 5 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[7], _wallTextures[1], "RockWall_lr", new[] { 0, 0, 5, 0 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[8], _wallTextures[2],  "RockWall_ur", new[] { 5, 0, 0, 5 }, 3, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[8], _wallTextures[2], "RockWall_ur", new[] { 5, 0, 0, 0 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[8], _wallTextures[2], "RockWall_ur", new[] { 0, 0, 0, 5 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[9], _wallTextures[3], "RockWall_bl", new[] { 0, 5, 5, 0 }, 3, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[9], _wallTextures[3],"RockWall_bl", new[] { 0, 0, 5, 0 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[9], _wallTextures[3], "RockWall_bl", new[] { 0, 5, 0, 0 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[10], _wallTextures[4], "RockWall_ul", new[] { 5, 0, 5, 0 }, 3, false)); //base
        WallPrototypes.Add(new Prototype(_wallTextures[10], _wallTextures[4], "RockWall_ul", new[] { 0, 0, 5, 0 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[10], _wallTextures[4], "RockWall_ul", new[] { 5, 0, 0, 0 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[11], _wallTextures[5], "RockWall_dr", new[] { 0, 5, 0, 5 }, 3, false));
        WallPrototypes.Add(new Prototype(_wallTextures[11], _wallTextures[5], "RockWall_dr", new[] { 0, 0, 0, 5 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[11], _wallTextures[5], "RockWall_dr", new[] { 0, 5, 0, 0 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[16], _wallTextures[12], "WallToRock_01", new[] { 2, 2, 3, 5 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[17], _wallTextures[13], "WallToRock_02", new[] { 3, 5, 1, 1 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[18], _wallTextures[14], "WallToRock_03", new[] { 1, 1, 5, 3 }, 1, false));
        WallPrototypes.Add(new Prototype(_wallTextures[19], _wallTextures[15], "WallToRock_04", new[] { 5, 3, 2, 2 }, 1, false));


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

        /*Create prototypes for each texture; Look from bottom or from right side!!!
        //New slots: 6: cliff left, ground right    7: cliff right, ground left        8: full Cliff
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[0], "Cliff_1rd", new[] { 8, 6, 8, 7 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[1], "Cliff_1ul", new[] { 7, 8, 6, 8 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[2], "Cliff_1dl", new[] { 8, 7, 7, 8 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[3],  "Cliff_1dr", new[] { 6, 8, 8, 6 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[4],"Cliff_2u", new[] { 8, 0, 7, 7 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[5], "Cliff_2r", new[] { 7, 7, 0, 8 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[6],  "Cliff_2l", new[] { 6, 6, 8, 0 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[7],  "Cliff_2d", new[] { 0, 8, 7, 7 }, 2000, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[8],  "Cliff_3ur", new[] { 7, 0, 0, 7 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[9], "Cliff_3dr", new[] { 0, 7, 0, 6 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[10], "Cliff_3dl", new[] { 0, 6, 6, 0 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(null, _cliffTextures[11], "Cliff_3ul", new[] { 6, 0, 7, 0 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(null, null , "FullCliff", new[] { 8, 8, 8, 8 }, 40000, false, true));*/
        
        //Create prototypes for each texture; Look from bottom or from right side!!!
        //New slots: 6: cliff left, ground right    7: cliff right, ground left        8: full Cliff
        CliffPrototypes.Add(new Prototype(_cliffTextures[0], null, "Cliff_1rd", new[] { 8, 6, 8, 7 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[1], null, "Cliff_1ul", new[] { 7, 8, 6, 8 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[2], null, "Cliff_1dl", new[] { 8, 7, 7, 8 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[3], null, "Cliff_1dr", new[] { 6, 8, 8, 6 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[4], null, "Cliff_2u", new[] { 8, 0, 7, 7 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[5], null, "Cliff_2r", new[] { 7, 7, 0, 8 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype( _cliffTextures[6], null, "Cliff_2l", new[] { 6, 6, 8, 0 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[7], null, "Cliff_2d", new[] { 0, 8, 7, 7 }, 1000, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[8], null, "Cliff_3ur", new[] { 7, 0, 0, 7 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[9], null, "Cliff_3dr", new[] { 0, 7, 0, 6 }, 500, false, true));
        CliffPrototypes.Add(new Prototype( _cliffTextures[10], null, "Cliff_3dl", new[] { 0, 6, 6, 0 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[11], null, "Cliff_3ul", new[] { 6, 0, 7, 0 }, 500, false, true));
        CliffPrototypes.Add(new Prototype(_cliffTextures[12], null , "FullCliff", new[] { 8, 8, 8, 8 }, 3000, false, true));
    }
}