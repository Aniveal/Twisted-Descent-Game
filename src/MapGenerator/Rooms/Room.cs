using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Meridian2.Columns;
using Microsoft.Xna.Framework;

namespace Meridian2; 

public class Room {
    //Column Positions
    public List<Vector2> Columns = new();

    //0,1,2 basic, fragile, electric. Only used for TutorialRoom
    public List<int> columnTypes = new();

    //Enemies
    public List<Vector2> EnemyPositions = new();
    public List<int> EnemyTypes = new();

    public List<Vector2> AmphoraPositions = new();

    public List<Vector2> TreasurePositions = new();

    public int roomDifficulty;

    //How many columns there should be, as a percentage of walkable tiles
    public float columnDensity = 0.15f;
    public float amphoraDensity = 0.05f;

    public float[] columnWeight;

    public int Index;

    //If all tiles have been initialized
    protected bool Initialized;

    //Pointer to the mapGenerator this room is part of
    protected MapGenerator Mg;

    public int NInnerOpenings;

    //Stores the points where there is an opening
    protected List<Vector2> Openings;

    //Top left edge of the room,
    public int PosX, PosY;

    //Tile size of the room
    public int SizeX, SizeY;

    //The map of this room
    public Tile[,] TileMap;

    protected int nColumns;
    protected int nTreasures = 0;


    //
    public List<Prototype> protList = new();

    public Room(MapGenerator mg, int x, int y, int sizeX, int sizeY, int index, List<Prototype> protList, int diff, bool treasure = false, float[] columnWeight = null)
    {
        Index = index;
        Openings = new List<Vector2>();
        Mg = mg;
        SizeX = sizeX;
        SizeY = sizeY;
        PosX = x;
        PosY = y;
        TileMap = new Tile[sizeX, sizeY];
        NInnerOpenings = (int)Math.Sqrt(sizeX * sizeY);
        this.protList = protList;
        initializeTileMap();
        if (columnWeight == null)
            this.columnWeight = new float[] { 0.1f, 0.9f, 1f };
        
        else this.columnWeight = columnWeight;
        roomDifficulty = diff;
    }

    //Finishes the room generation, results in a workable tileMap
    public bool generateRoom() {
        createBorder();
        addMoreFloor();
        connectOpenings();
        runWaveFunctionCollapse();

        int walkable = 0;


        //check if generation succeeded
        foreach(Tile t in TileMap)
        {
            if (t == null || t.FinalPrototype == null) return false;
            if (t.FinalPrototype.Walkable)
                walkable++;
        }

        nColumns = (int)(walkable * columnDensity);

        //Add columns
        placeColumns(nColumns);

        addTreasures();
        addAmphoras();

        if (roomDifficulty > 0)
        {
            int enemyBudget = Math.Max(1, (int)(roomDifficulty * (SizeX * SizeY) * 0.005f));
            placeEnemies(Math.Max(1, RnGsus.Instance.Next(enemyBudget)));
        }

        return true;
    }

    //Adds an inner opening, i.e. a guaranteed connected tile
    public void innerOpening(int x, int y) {
        Openings.Add(new Vector2(x, y));
    }

    public void placeColumns(int n) {
        Columns.Clear();

        var i = 0;
        var j = 0;

        while (i < n) {
            var x = (float)RnGsus.Instance.NextDouble() * SizeX;
            var y = (float)RnGsus.Instance.NextDouble() * SizeY;

            if (TileMap[(int)Math.Floor(x), (int)Math.Floor(y)].FinalPrototype.Walkable) {
                y = y - 1f;
                if (noColumnsNear(x, y, 1)) {
                    Columns.Add(new Vector2(x, y));
                    i++;
                } else {
                    j++;
                }
            } else {
                j++;
            }

            if (j > 1000) {
                Debug.WriteLine("Didnt find walkable space!!!");
                return;
            }
        }
    }

    private bool noColumnsNear(float x, float y, float r) {
        foreach (var col in Columns)
            if (Math.Abs(col.X - x) < r && Math.Abs(col.Y - y) < r)
                return false;

        return true;
    }

    private bool noTreasuresNear(float x, float y, float r)
    {
        foreach (var C in TreasurePositions)
            if (Math.Abs(C.X - x) < 1 && Math.Abs(C.Y - y) < 1)
                return false;

        return true;
    }

    private bool noAmphoraNear(float x, float y, float r)
    {
        foreach (var C in AmphoraPositions)
            if (Math.Abs(C.X - x) < r && Math.Abs(C.Y - y) < r)
                return false;

        return true;
    }

    private void addAmphoras()
    {
        int nAmphoras = (int)((SizeX * SizeY) * amphoraDensity * RnGsus.Instance.NextDouble());
        int i = 0;
        int j = 0;
        while (i < nAmphoras)
        {
            if (j > 1000)
                break;
            var x = (float)RnGsus.Instance.NextDouble() * SizeX;
            var y = (float)RnGsus.Instance.NextDouble() * SizeY;

            if (TileMap[(int)x, (int)y].FinalPrototype.Walkable)
            {
                y = y - 1f;
                if (noColumnsNear(x, y, 0.5f) && noTreasuresNear(x, y, 0.5f) && noAmphoraNear(x,y, 0.2f))
                {
                    AmphoraPositions.Add(new Vector2(x, y));
                    i++;
                }
            }
            j++;
        }
    }

    private void addTreasures()
    {
        int i = 0;
        int j = 0;
        while (i < nTreasures)
        {
            if (j > 1000)
                break;
            var x = (float)(RnGsus.Instance.NextDouble() * SizeX);
            var y = (float)(RnGsus.Instance.NextDouble() * SizeY);

            if (TileMap[(int)x, (int)y].FinalPrototype.Walkable)
            {
                y -= 1f;
                if (noColumnsNear(x, y, 1))
                {
                    TreasurePositions.Add(new Vector2(x, y));
                    i++;
                }
            }
            j++;
        }
    }

    public void placeEnemies(int n) {
        EnemyPositions.Clear();

        var i = 0;
        var j = 0;

        while (i < n) {
            var x = (float)RnGsus.Instance.NextDouble() * SizeX;
            var y = (float)RnGsus.Instance.NextDouble() * SizeY;

            if (TileMap[(int)x, (int)y].FinalPrototype.Walkable) {
                
                EnemyPositions.Add(new Vector2((int)x, (int)y));
                i += 1;
            } else {
                j++;
            }

            if (j > 1000) {
                Debug.WriteLine("Didnt find walkable space!!!");
                return;
            }
        }
    }

    //Fill out the whole tilemap with new Tiles
    public void initializeTileMap() {
        //Create all tiles, initialize with full set of prototypes
        for (var x = 0; x < SizeX; x++)
        for (var y = 0; y < SizeY; y++) {
            TileMap[x, y] = new Tile(protList, this);
            TileMap[x, y].X = x;
            TileMap[x, y].Y = y;
        }

        Initialized = true;
    }

    private void addMoreFloor() {
        for (var i = 0; i < NInnerOpenings; i++)
            innerOpening(RnGsus.Instance.Next(SizeX - 4) + 2, RnGsus.Instance.Next(SizeY - 4) + 2);
    }

    //Creates an opening to another room at x, y with length l. l goes from x,y into the positive direction. Has to be at least 3
    //Don't go too close to an edge, otherwise undefined behaviuor may happen
    public void createOpening(int x, int y, int l) {
        Debug.WriteLine("Create Opening");

        if (!(x == 0 || x == SizeX - 1 || y == 0 || y == SizeY - 1))
            Debug.WriteLine("Error, opening cant be created, wrong values");


        Openings.Add(new Vector2(x, y));

        if (x == 0 || x == SizeX - 1)
            for (var i = 0; i < l; i++) {
                if (i > SizeY - 1) {
                    Debug.WriteLine("Error, l was too large in createOpening()");
                    return;
                }

                TileMap[x, y + i].setFinalPrototype(getPrototype("ground"));
                collapseTile(TileMap[x, y + i]);
            }
        else if (y == 0 || y == SizeY - 1)
            for (var i = 0; i < l; i++) {
                if (i > SizeX - 1) {
                    Debug.WriteLine("Error, l was too large in createOpening()");
                    return;
                }

                TileMap[x + i, y].setFinalPrototype(getPrototype("ground"));
                collapseTile(TileMap[x + i, y]);
            }
        else
            Debug.WriteLine("Error while creating opening: Point " + x + "," + y + " is not on the border!!! SizeX = " +
                            SizeX + ", SizeY = " + SizeY);
    }

    public void setTile(int x, int y, Prototype p) {
        TileMap[x, y].setFinalPrototype(p, true);
    }

    public void setWalkable(int x, int y) {
        TileMap[x, y].makeWalkable();
    }

    //Creates a walkable paths between all openings to ensure that all exits are reachable
    //Call this after all createOpening calls were made!
    //Makes a path to the midpoint of the room
    public void connectOpenings() {
        //Midpoint of the room
        var midX = SizeX / 2;
        var midY = SizeY / 2;

        Debug.WriteLine("Connecting the openings: midX = " + midX + "  And midY = " + midY);

        foreach (var opening in Openings) {
            var x = (int)opening.X;
            var y = (int)opening.Y;

            Debug.Write("Opening.... " + x + ", " + y);

            //Handle case where opening is on left or right
            if (x <= 1 || x >= SizeX - 2) {
                //First, do one step towards the middle
                while (x <= 1) {
                    TileMap[x, y].makeWalkable();
                    x = x + 1;
                }

                while (x >= SizeX - 2) {
                    TileMap[x, y].makeWalkable();
                    x = x - 1;
                }

                TileMap[x, y].makeWalkable();
            }

            //opening on top or bottom
            if (y <= 1 || y >= SizeY - 2) {
                //First, do one step towards the middle
                while (y <= 1) {
                    TileMap[x, y].makeWalkable();
                    y = y + 1;
                }

                while (y >= SizeY - 2) {
                    TileMap[x, y].makeWalkable();
                    y = y - 1;
                }

                TileMap[x, y].makeWalkable();
            }

            while (!(x == midX && y == midY)) {
                Debug.WriteLine("Setting tile to walkable!!!  x = " + x + ", y = " + y);
                var rand = RnGsus.Instance.NextDouble();

                //Corner cases
                if (x == midX) {
                    if (y < midY) y++;
                    else y--;
                    TileMap[x, y].makeWalkable();
                    continue;
                }

                if (y == midY) {
                    if (x < midX) x++;
                    else x--;
                    TileMap[x, y].makeWalkable();
                    continue;
                }

                //Get distance from point to mid
                var distanceX = Math.Abs(midX - x);
                var distanceY = Math.Abs(midY - y);

                //Farther away increases weight
                var xWeight = distanceX / (double)(distanceX + distanceY);

                if (rand < xWeight) {
                    if (x < midX) x++;
                    else x--;
                } else {
                    if (y < midY) y++;
                    else y--;
                }

                TileMap[x, y].makeWalkable();
            }

            Debug.Write("Complete!\n");
        }
    }


    //Gets a prototype reference by name
    public Prototype getPrototype(string s) {
        foreach (var prototype in protList)
            if (prototype.Name == s)
                return prototype;
        return null;
    }


    //Creates a border of solid walls around room everywhere where there is no opening
    //Do this after setting openings!
    public void createBorder() {
        Debug.WriteLine("Creating Borders");

        for (var x = 0; x < SizeX; x++) {
            if (TileMap[x, 0].Superpositions.Contains(getPrototype("FullWall"))) {
                TileMap[x, 0].setFinalPrototype(getPrototype("FullWall"));
                collapseTile(TileMap[x, 0]);
            }

            if (TileMap[x, SizeY - 1].Superpositions.Contains(getPrototype("FullWall"))) {
                TileMap[x, SizeY - 1].setFinalPrototype(getPrototype("FullWall"));
                
                collapseTile(TileMap[x, SizeY - 1]);
            }
        }

        for (var y = 0; y < SizeY; y++) {
            if (TileMap[0, y].Superpositions.Contains(getPrototype("FullWall"))) {
                TileMap[0, y].setFinalPrototype(getPrototype("FullWall"));
                
                collapseTile(TileMap[0, y]);
            }

            if (TileMap[SizeX - 1, y].Superpositions.Contains(getPrototype("FullWall"))) {
                TileMap[SizeX - 1, y].setFinalPrototype(getPrototype("FullWall"));
                
                collapseTile(TileMap[SizeX - 1, y]);
            }
        }
    }

    //Runs wave function collapse to generate this room, tiles must be initialized before!
    private void runWaveFunctionCollapse() {
        var mapX = TileMap.GetLength(0);
        var mapY = TileMap.GetLength(1);

        Debug.WriteLine("Started Wave function collapse! Map x = " + mapX + ", mapY = " + mapY);

        var counter = 0;
        while (true) {
            //Find lowest amount of superpositions
            Tile t;
            var n = 100000;

            foreach (var tile in TileMap)
                if (tile.Superpositions.Count > 1 && tile.Superpositions.Count < n)
                    n = tile.Superpositions.Count;

            //Get list of all possible tiles
            var list = new List<Tile>();
            //weights all summed up
            var totalWeights = 0;
            foreach (var tile in TileMap)
                if (tile.Superpositions.Count == n)
                    list.Add(tile);

            if (list.Count <= 0)
                break;

            var chosen = list[RnGsus.Instance.Next(list.Count)];

            chosen.chooseRandomPrototype();

            //Collapse the wave function:
            collapseTile(chosen);


            counter++;
            if (counter > mapX * mapY) {
                Debug.WriteLine("Error, wave function could not terminate");
                break;
            }
            
        }
    }

    //Collapse a single tile in a tilemap and recursively collapse all neighbours.
    //Call this whenever you change a tile manually in a tilemap
    public void collapseTile(Tile tile) {
        var mapX = TileMap.GetLength(0);
        var mapY = TileMap.GetLength(1);

        if (tile.X - 1 >= 0)
            if (TileMap[tile.X - 1, tile.Y].collapseFunction(tile, "right"))
                collapseTile(TileMap[tile.X - 1, tile.Y]); //if there was a change in this tile, recurse


        if (tile.X + 1 < mapX)
            if (TileMap[tile.X + 1, tile.Y].collapseFunction(tile, "left"))
                collapseTile(TileMap[tile.X + 1, tile.Y]);

        if (tile.Y - 1 >= 0)
            if (TileMap[tile.X, tile.Y - 1].collapseFunction(tile, "down"))
                collapseTile(TileMap[tile.X, tile.Y - 1]);

        if (tile.Y + 1 < mapY)
            if (TileMap[tile.X, tile.Y + 1].collapseFunction(tile, "up"))
                collapseTile(TileMap[tile.X, tile.Y + 1]);
    }
}