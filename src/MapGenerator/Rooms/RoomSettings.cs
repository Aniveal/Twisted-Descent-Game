using System.Collections.Generic;

namespace Meridian2; 

//This class holds settings for different kind of rooms
public class RoomSettings {
    //Name of this room setting
    public string Name;

    //What prototypes can be used, eg the biome
    public List<Prototype> PossiblePrototypes = new();

    //Pre-Defined tiles that have to be in the room
    public List<Tile> Tiles;

    //How much should be walkable vs how mach walls, will tend more to walls though
    public float WalkablePercentage = 0.5f;

    public RoomSettings(string name, List<List<Prototype>> protLists, float walkablePercentage = 0.7f,
        List<Tile> tiles = null) {
        Name = name;
        foreach (var prot in protLists)
        foreach (var p in prot)
            PossiblePrototypes.Add(p);
        WalkablePercentage = walkablePercentage;
        Tiles = tiles;
    }
}