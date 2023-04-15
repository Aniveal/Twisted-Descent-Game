using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2
{

    //This class holds settings for different kind of rooms
    public class RoomSettings
    {
        //Name of this room setting
        public string name;

        //How much should be walkable vs how mach walls, will tend more to walls though
        public float walkablePercentage = 0.5f;

        //Pre-Defined tiles that have to be in the room
        public List<Tile> tiles;

        //What prototypes can be used, eg the biome
        public List<Prototype> possiblePrototypes;

        public RoomSettings(string name, List<Prototype> protList, float walkablePercentage = 0.7f, List<Tile> tiles = null) {
            this.name = name;
            this.possiblePrototypes = protList;
            this.walkablePercentage = walkablePercentage;
            this.tiles = tiles;
        }
    }
}
