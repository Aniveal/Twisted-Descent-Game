using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using tainicom.Aether.Physics2D.Content;

namespace Meridian2
{
    internal class DungeonGraph
    {
        //This is the start room, 
        public List<Room> rooms = new List<Room>(); 

        private int[,] occupationMap;

        private MapGenerator mg;

        const int minRoomSize = 15;

        List<RoomSettings> roomTypes = new List<RoomSettings>();

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

            List<List<Prototype>> allPrototypes = new List<List<Prototype>>();
            allPrototypes.Add(mg.rockPrototypes);
            allPrototypes.Add(mg.wallPrototypes);

            RoomSettings standardRoomSettings = new RoomSettings("RockRoom", allPrototypes);

            //Place random first room
            Room startRoom = createStartRoom(minRoomSize + 2);
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
                    newRoom.posX += RNGsus.Instance.Next(target.sizeX - 1);
                else newRoom.posX -= RNGsus.Instance.Next(newRoom.sizeX - 1);
                if (RNGsus.Instance.NextDouble() < 0.5)
                    newRoom.posY += RNGsus.Instance.Next(target.sizeY - 1);
                else newRoom.posY -= RNGsus.Instance.Next(newRoom.sizeY - 1);

                //Check if the room is allready out of bounds:
                if(!isInBounds(newRoom.posX, newRoom.posY) || !isInBounds(newRoom.posX + newRoom.sizeX - 1, newRoom.posY + newRoom.sizeY - 1))
                {
                    numFails++;
                    continue;
                }

                //Move the room in a random direction, until it fits:
                Double random = RNGsus.Instance.NextDouble();

                double rand = RNGsus.Instance.NextDouble();

                if (rand < 0.25)
                {
                    //Move in positive X direction

                    //Move to right until a free tile is reached:
                    while (isInBounds(newRoom.posX, newRoom.posY) && isOccupied(newRoom))
                    {
                        newRoom.posX++;
                    }
                }
                else if (rand < 0.5)
                {
                    //Move in negative X direction

                    //Move to right until a free tile is reached:
                    while (isInBounds(newRoom.posX, newRoom.posY) && isOccupied(newRoom))
                    {
                        newRoom.posX--;
                    }
                }
                else if (rand < 0.75)
                {
                    //Move in positive Y direction

                    //Move to right until a free tile is reached:
                    while (isInBounds(newRoom.posX, newRoom.posY) && isOccupied(newRoom))
                    {
                        newRoom.posY++;
                    }
                }
                else
                {
                    //Move in negative Y direction
                    while (isInBounds(newRoom.posX, newRoom.posY) && isOccupied(newRoom))
                    {
                        newRoom.posY--;
                    }
                }

                if (!isInBounds(newRoom.posX, newRoom.posY))
                {
                    numFails++;
                    continue;
                }
                
                //Try to add openings to other rooms:
                if(connectWith(newRoom))
                {
                    rooms.Add(newRoom);
                    occupy(newRoom);
                    i++;
                    numFails = 0;
                }
                else numFails++;

            }

            //Move all the rooms so that the starter room is in the middle:
            int displaceX = startRoom.posX;
            int displaceY = startRoom.posY;

            foreach(Room r in rooms)
            {
                r.posX -= displaceX - 1;
                r.posY -= displaceY + 2;
            }

        }

        //tries to connect a room to all others and returns true if succeeded
        public bool connectWith(Room r1)
        {
            bool isConnected = false;

            //position on the rooms for the opening, relative to room position!!!
            int x1, y1;
            int x2, y2;


            //Try connecting every room
            foreach(Room r2 in rooms)
            {
                if (r2 == r1)
                    continue;

                //if x direction touches:
                if (r1.posX + r1.sizeX == r2.posX || r2.posX + r2.sizeX == r1.posX)
                {
                    //find maching x coordinate
                    if (r1.posX + r1.sizeX == r2.posX)
                    {
                        x1 = r1.sizeX - 1;
                        x2 = 0;
                    }
                    else
                    {
                        x1 = 0;
                        x2 = r2.sizeX - 1;
                    }

                    int yMin, yMax;

                    yMin = Math.Max(r1.posY, r2.posY) + 1;
                    yMax = Math.Min(r1.posY + r1.sizeY, r2.posY + r2.sizeY) - 1;

                    if (yMax < yMin)
                        continue;

                    y1 = y2 = RNGsus.Instance.Next(yMax - yMin) + yMin;

                    y1 = y1 - r1.posY;
                    y2 = y2 - r2.posY;

                    if (!(1 < y1 && y1 < r1.sizeY - 2))
                        continue;
                    if (!(1 < y2 && y2 < r2.sizeY - 2))
                        continue;

                }

                //if y direction touches
                else if(r1.posY + r1.sizeY == r2.posY || r2.posY + r2.sizeY == r1.posY)
                {
                    //find maching x coordinate
                    if (r1.posY + r1.sizeY == r2.posY)
                    {
                        y1 = r1.sizeY - 1;
                        y2 = 0;
                    }
                    else
                    {
                        y1 = 0;
                        y2 = r2.sizeY - 1;
                    }

                    int xMin, xMax;

                    xMin = Math.Max(r1.posX, r2.posX) + 1;
                    xMax = Math.Min(r1.posX + r1.sizeX, r2.posX + r2.sizeX) - 1;

                    if (xMax < xMin)
                        continue;

                    x1 = x2 = RNGsus.Instance.Next(xMax - xMin) + xMin;

                    x1 = x1 - r1.posX;
                    x2 = x2 - r2.posX;

                    if (!(1 < x1 && x1 < r1.sizeX - 2))
                        continue;
                    if (!(1 < x2 && x2 < r2.sizeX - 2))
                        continue;

                }
                else
                {
                    //Room not touching
                    continue;
                }

                
                r1.createOpening(x1, y1, 1);
                r2.createOpening(x2, y2, 1);
                isConnected = true;
                            
            }
            return isConnected;
        }

        public Room getRoomByIndex(int i)
        {
            foreach(Room room in rooms)
            {
                if (room.index == i)
                    return room;
            }
            return null;
        }

        public void addOpening(Room r1, Room r2)
        {
            int x, y; //the position of the opening of room 1
            //Check which of the four cases it is:
            if (r1.posX + r1.sizeX == r2.posX || r1.posX - r2.sizeX == r2.posX)
            {
                if (r1.posX - r2.sizeX == r2.posX)
                {
                    //r2 is on the left, swap
                    Room temp = r1;
                    r1 = r2;
                    r2 = temp;
                }

                //r2 is on the right
                int startY, endY;
                if (r2.posY > r1.posY)
                    startY = r2.posY;
                else startY = r1.posY;
                if (r2.posY + r2.sizeY < r1.posY + r1.sizeY)
                    endY = r2.posY + r2.sizeY;
                else endY = r1.posY + r1.sizeY;

                //Cant have doors at corner
                endY -= 2;
                startY += 2;
                int l = endY - startY;

                if (l < 1)
                {
                    Debug.WriteLine("Error, l was < 1");
                    return;
                }

                //Create openings
                y = RNGsus.Instance.Next(l) + startY;
                x = r2.posX - 1;
                r1.createOpening(x - r1.posX, y - r1.posY, 1);
                r2.createOpening(x + 1 - r2.posX, y - r2.posY, 1);
            }
            else return; 
            if (r1.posY + r1.sizeY == r2.posY || r1.posY - r2.sizeY == r2.posY)
            {
                //r2 on top, swap
                if(r1.posY - r2.sizeY == r2.posY)
                {
                    Room temp = r1;
                    r1 = r2;
                    r2 = temp;
                }
                //r2 is on the bottom
                int startX, endX;
                if (r2.posX > r1.posX)
                    startX = r2.posX;
                else startX = r1.posX;
                if (r2.posX + r2.sizeX < r1.posX + r1.sizeX)
                    endX = r2.posX;
                else endX = r1.posX;

                //Cant have doors at corner
                endX -= 2;
                startX +=2;
                int l = endX - startX;

                if (l < 1)
                {
                    Debug.WriteLine("Error, l was < 1");
                    return;
                }

                //Create openings
                y = r2.posY - 1;
                x = RNGsus.Instance.Next(l) + startX;
                r1.createOpening(x - r1.posX, y - r1.posY, 1);
                r2.createOpening(x - r2.posX, y - r2.posY + 1, 1);

            }
            
            else { 
                Debug.WriteLine("Could not add Opening between rooms " + r1 + "," + r2); 
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

        public Room createStartRoom(int maxRoomSize)
        {
            //Settings of the start room
            RoomSettings startRoomSettings = new RoomSettings("StartRoom", new List<List<Prototype>> () { mg.rockPrototypes });

            int x = 0;
            int y = 0;
            int sizeX = RNGsus.Instance.Next(maxRoomSize - minRoomSize) + minRoomSize;
            int sizeY = RNGsus.Instance.Next(maxRoomSize - minRoomSize) + minRoomSize;

            Room room = new Room(mg, startRoomSettings,x, y, sizeX, sizeY, 1);

            for (int i = 2; i < 5; i++)
                for (int j = 2; j < 5; j++)
                    room.setWalkable(i, j);

            room.innerOpening(2, 2);

            return room;
        }

    }
}
