using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Meridian2
{
    internal class DungeonGraph
    {
        //This is the start room, 
        public List<Room> rooms = new List<Room>(); 

        private int[,] occupationMap;

        private MapGenerator mg;

        const int minRoomSize = 5;

        public DungeonGraph(MapGenerator mg)
        {
            this.mg = mg;
        }

        public bool isInBounds(int x, int y)
        {
            if (x < 0 || x > occupationMap.GetLength(0) - 1 || y < 0 || y > occupationMap.GetLength(1) - 1)
                return false;
            return true;
        }

        //Creates a map with nRooms rooms, on a grid with maxSize size.
        public void createDungeonMap(int maxSize, int maxRoomSize)
        {
            occupationMap = new int[maxSize, maxSize];

            RoomSettings standardRoomSettings = new RoomSettings("Standard Room", mg.rockPrototypes);

            //Place random first room
            Room startRoom = createStartRoom(maxSize, maxRoomSize);
            occupy(startRoom);
            rooms.Add(startRoom);

            //Index of currently placed room
            int i = 2;
            int numFails = 0;

            while(numFails < 100)
            {
                //Take a random room
                int r = RNGsus.Instance.Next(rooms.Count);
                Room target = rooms[r];

                //Take the same spot as target and random size
                int sizeX = RNGsus.Instance.Next(maxRoomSize - minRoomSize) + minRoomSize;
                int sizeY = RNGsus.Instance.Next(maxRoomSize - minRoomSize) + minRoomSize;

                Room newRoom = new Room(mg, standardRoomSettings, target.posX, target.posY, sizeX, sizeY, i);

                //Move the room randomly inside the other one so they still overlap
                if (RNGsus.Instance.NextDouble() < 0.5)
                    newRoom.posX += RNGsus.Instance.Next(target.sizeX - 4);
                else newRoom.posX -= RNGsus.Instance.Next(newRoom.sizeX - 4);
                if (RNGsus.Instance.NextDouble() < 0.5)
                    newRoom.posY += RNGsus.Instance.Next(target.sizeY - 4);
                else newRoom.posY -= RNGsus.Instance.Next(newRoom.sizeY - 4);


                

                //Check if the room is allready out of bounds:
                if(!isInBounds(newRoom.posX, newRoom.posY) || !isInBounds(newRoom.posX + newRoom.sizeX - 1, newRoom.posY + newRoom.sizeY - 1))
                {
                    numFails++;
                    continue;
                }

                //Move the room in a random direction, until it fits:
                Double random = RNGsus.Instance.NextDouble();
                int x = newRoom.posX;
                int y = newRoom.posY;

                double rand = RNGsus.Instance.NextDouble();

                if (rand < 0.25)
                {
                    //Move in positive X direction

                    //Move to right until a free tile is reached:
                    while (isInBounds(x, y) && occupationMap[x, y] != 0)
                    {
                        x++;
                    }

                    newRoom.posX = x;
                    newRoom.posY = y;
                    if (isOccupied(newRoom))
                    {
                        numFails++;
                        continue;
                    }
                    else
                    {
                        numFails = 0;
                        rooms.Add(newRoom);
                        occupy(newRoom);
                        i++;
                    }
                }
                else if (rand < 0.5)
                {
                    //Move in negative X direction

                    //Move to right until a free tile is reached:
                    while (isInBounds(x, y) && occupationMap[x, y] != 0)
                    {
                        x--;
                    }

                    newRoom.posX = x - newRoom.sizeX - 1;
                    newRoom.posY = y;

                    if (isOccupied(newRoom))
                    {
                        numFails++;
                        continue;
                    }
                    else
                    {
                        numFails = 0;
                        rooms.Add(newRoom);
                        occupy(newRoom);
                        i++;
                    }
                }
                else if (rand < 0.75)
                {
                    //Move in positive Y direction

                    //Move to right until a free tile is reached:
                    while (isInBounds(x, y) && occupationMap[x, y] != 0)
                    {
                        y++;
                    }

                    newRoom.posX = x;
                    newRoom.posY = y;

                    if (isOccupied(newRoom))
                    {
                        numFails++;
                        continue;
                    }
                    else
                    {
                        numFails = 0;
                        rooms.Add(newRoom);
                        occupy(newRoom);
                        i++;
                    }
                }
                else
                {
                    //Move in negative Y direction
                    while (isInBounds(x, y) && occupationMap[x, y] != 0)
                    {
                        y--;
                    }

                    newRoom.posX = x;
                    newRoom.posY = y - newRoom.sizeY - 1;

                    if (isOccupied(newRoom))
                    {
                        numFails++;
                        continue;
                    }
                    else
                    {
                        numFails = 0;
                        rooms.Add(newRoom);
                        occupy(newRoom);
                        i++;
                    }
                }


            }

            //Move all the rooms so that the starter room is in the middle:
            int displaceX = startRoom.posX;
            int displaceY = startRoom.posY;

            foreach(Room r in rooms)
            {
                r.posX -= displaceX;
                r.posY -= displaceY;
                r.initializeTileMap();
            }

        }

        //Checks if a room collides with one allready on the grid
        public bool isOccupied(Room room)
        {
            for(int x = room.posX; x < room.posX + room.sizeX ; x++)
            {
                for(int y = room.posY; y < room.posY + room.sizeY; y++)
                {
                    if (!isInBounds(x, y))
                        return true;
                    if (occupationMap[x, y] != 0)
                        return true;
                }
            }
            return false;
        }
        //Occupies grid space for a room
        public void occupy(Room room)
        {
            Debug.WriteLine("Adding Room nr " + room.index);
            for (int x = room.posX; x < room.posX + room.sizeX; x++)
            {
                for (int y = room.posY; y < room.posY + room.sizeY; y++)
                {
                    occupationMap[x, y] = room.index;
                }
            }
        }

        public Room createStartRoom(int maxSize, int maxRoomSize)
        {
            //Settings of the start room
            RoomSettings startRoomSettings = new RoomSettings("StartRoom", mg.rockPrototypes);

            int x = RNGsus.Instance.Next(maxSize - maxRoomSize);
            int y = RNGsus.Instance.Next(maxSize - maxRoomSize);
            int sizeX = RNGsus.Instance.Next(maxRoomSize - minRoomSize) + minRoomSize;
            int sizeY = RNGsus.Instance.Next(maxRoomSize - minRoomSize) + minRoomSize;

            Room room = new Room(mg, startRoomSettings,x, y, sizeX, sizeY, 1);
            return room;
        }

    }
}
