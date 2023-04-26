using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Meridian2; 

internal class DungeonGraph {
    private const int MinRoomSize = 15;

    private readonly MapGenerator _mg;

    private int[,] _occupationMap;

    private List<RoomSettings> _roomTypes = new();

    //This is the start room, 
    public List<Room> Rooms = new();

    public DungeonGraph(MapGenerator mg) {
        _mg = mg;
    }

    public bool isInBounds(int x, int y) {
        if (x < 0 || x > _occupationMap.GetLength(0) - 1 || y < 0 || y > _occupationMap.GetLength(1) - 1)
            return false;
        return true;
    }

    //Creates a map with nRooms rooms, on a grid with maxSize size.
    public void createDungeonMap(int maxSize, int maxRoomSize) {
        _occupationMap = new int[maxSize, maxSize];

        var allPrototypes = new List<List<Prototype>>();
        allPrototypes.Add(_mg.RockPrototypes);
        allPrototypes.Add(_mg.WallPrototypes);
        allPrototypes.Add(_mg.CliffPrototypes);

        var standardRoomSettings = new RoomSettings("RockRoom", allPrototypes);

        //Place random first room
        var startRoom = createStartRoom(MinRoomSize + 2);
        occupy(startRoom);
        Rooms.Add(startRoom);

        //Index of currently placed room
        var i = 2;
        var numFails = 0;

        while (numFails < 100) {
            //Take a random room
            var r = RnGsus.Instance.Next(Rooms.Count);
            var target = Rooms[r];

            //Take the same spot as target and random size
            var sizeX = RnGsus.Instance.Next(maxRoomSize - MinRoomSize) + MinRoomSize;
            var sizeY = RnGsus.Instance.Next(maxRoomSize - MinRoomSize) + MinRoomSize;

            var newRoom = new Room(_mg, standardRoomSettings, target.PosX, target.PosY, sizeX, sizeY, i);

            //Move the room randomly inside the other one so they still overlap
            if (RnGsus.Instance.NextDouble() < 0.5)
                newRoom.PosX += RnGsus.Instance.Next(target.SizeX - 1);
            else newRoom.PosX -= RnGsus.Instance.Next(newRoom.SizeX - 1);
            if (RnGsus.Instance.NextDouble() < 0.5)
                newRoom.PosY += RnGsus.Instance.Next(target.SizeY - 1);
            else newRoom.PosY -= RnGsus.Instance.Next(newRoom.SizeY - 1);

            //Check if the room is allready out of bounds:
            if (!isInBounds(newRoom.PosX, newRoom.PosY) ||
                !isInBounds(newRoom.PosX + newRoom.SizeX - 1, newRoom.PosY + newRoom.SizeY - 1)) {
                numFails++;
                continue;
            }

            //Move the room in a random direction, until it fits:
            var random = RnGsus.Instance.NextDouble();

            var rand = RnGsus.Instance.NextDouble();

            if (rand < 0.25)
                //Move in positive X direction
                //Move to right until a free tile is reached:
                while (isInBounds(newRoom.PosX, newRoom.PosY) && isOccupied(newRoom))
                    newRoom.PosX++;
            else if (rand < 0.5)
                //Move in negative X direction
                //Move to right until a free tile is reached:
                while (isInBounds(newRoom.PosX, newRoom.PosY) && isOccupied(newRoom))
                    newRoom.PosX--;
            else if (rand < 0.75)
                //Move in positive Y direction
                //Move to right until a free tile is reached:
                while (isInBounds(newRoom.PosX, newRoom.PosY) && isOccupied(newRoom))
                    newRoom.PosY++;
            else
                //Move in negative Y direction
                while (isInBounds(newRoom.PosX, newRoom.PosY) && isOccupied(newRoom))
                    newRoom.PosY--;

            if (!isInBounds(newRoom.PosX, newRoom.PosY)) {
                numFails++;
                continue;
            }

            //Try to add openings to other rooms:
            if (connectWith(newRoom)) {
                Rooms.Add(newRoom);
                occupy(newRoom);
                i++;
                numFails = 0;
            } else {
                numFails++;
            }
        }

        //Move all the rooms so that the starter room is in the middle:
        var displaceX = startRoom.PosX;
        var displaceY = startRoom.PosY;

        foreach (var r in Rooms) {
            r.PosX -= displaceX - 1;
            r.PosY -= displaceY + 2;
        }
    }

    //tries to connect a room to all others and returns true if succeeded
    public bool connectWith(Room r1) {
        var isConnected = false;

        //position on the rooms for the opening, relative to room position!!!
        int x1, y1;
        int x2, y2;


        //Try connecting every room
        foreach (var r2 in Rooms) {
            if (r2 == r1)
                continue;

            //if x direction touches:
            if (r1.PosX + r1.SizeX == r2.PosX || r2.PosX + r2.SizeX == r1.PosX) {
                //find maching x coordinate
                if (r1.PosX + r1.SizeX == r2.PosX) {
                    x1 = r1.SizeX - 1;
                    x2 = 0;
                } else {
                    x1 = 0;
                    x2 = r2.SizeX - 1;
                }

                int yMin, yMax;

                yMin = Math.Max(r1.PosY, r2.PosY) + 1;
                yMax = Math.Min(r1.PosY + r1.SizeY, r2.PosY + r2.SizeY) - 1;

                if (yMax < yMin)
                    continue;

                y1 = y2 = RnGsus.Instance.Next(yMax - yMin) + yMin;

                y1 = y1 - r1.PosY;
                y2 = y2 - r2.PosY;

                if (!(1 < y1 && y1 < r1.SizeY - 2))
                    continue;
                if (!(1 < y2 && y2 < r2.SizeY - 2))
                    continue;
            }

            //if y direction touches
            else if (r1.PosY + r1.SizeY == r2.PosY || r2.PosY + r2.SizeY == r1.PosY) {
                //find maching x coordinate
                if (r1.PosY + r1.SizeY == r2.PosY) {
                    y1 = r1.SizeY - 1;
                    y2 = 0;
                } else {
                    y1 = 0;
                    y2 = r2.SizeY - 1;
                }

                int xMin, xMax;

                xMin = Math.Max(r1.PosX, r2.PosX) + 1;
                xMax = Math.Min(r1.PosX + r1.SizeX, r2.PosX + r2.SizeX) - 1;

                if (xMax < xMin)
                    continue;

                x1 = x2 = RnGsus.Instance.Next(xMax - xMin) + xMin;

                x1 = x1 - r1.PosX;
                x2 = x2 - r2.PosX;

                if (!(1 < x1 && x1 < r1.SizeX - 2))
                    continue;
                if (!(1 < x2 && x2 < r2.SizeX - 2))
                    continue;
            } else {
                //Room not touching
                continue;
            }


            r1.createOpening(x1, y1, 1);
            r2.createOpening(x2, y2, 1);
            isConnected = true;
        }

        return isConnected;
    }

    public Room getRoomByIndex(int i) {
        foreach (var room in Rooms)
            if (room.Index == i)
                return room;
        return null;
    }

    public void addOpening(Room r1, Room r2) {
        int x, y; //the position of the opening of room 1
        //Check which of the four cases it is:
        if (r1.PosX + r1.SizeX == r2.PosX || r1.PosX - r2.SizeX == r2.PosX) {
            if (r1.PosX - r2.SizeX == r2.PosX) {
                //r2 is on the left, swap
                var temp = r1;
                r1 = r2;
                r2 = temp;
            }

            //r2 is on the right
            int startY, endY;
            if (r2.PosY > r1.PosY)
                startY = r2.PosY;
            else startY = r1.PosY;
            if (r2.PosY + r2.SizeY < r1.PosY + r1.SizeY)
                endY = r2.PosY + r2.SizeY;
            else endY = r1.PosY + r1.SizeY;

            //Cant have doors at corner
            endY -= 2;
            startY += 2;
            var l = endY - startY;

            if (l < 1) {
                Debug.WriteLine("Error, l was < 1");
                return;
            }

            //Create openings
            y = RnGsus.Instance.Next(l) + startY;
            x = r2.PosX - 1;
            r1.createOpening(x - r1.PosX, y - r1.PosY, 1);
            r2.createOpening(x + 1 - r2.PosX, y - r2.PosY, 1);
        } else {
            return;
        }

        if (r1.PosY + r1.SizeY == r2.PosY || r1.PosY - r2.SizeY == r2.PosY) {
            //r2 on top, swap
            if (r1.PosY - r2.SizeY == r2.PosY) {
                var temp = r1;
                r1 = r2;
                r2 = temp;
            }

            //r2 is on the bottom
            int startX, endX;
            if (r2.PosX > r1.PosX)
                startX = r2.PosX;
            else startX = r1.PosX;
            if (r2.PosX + r2.SizeX < r1.PosX + r1.SizeX)
                endX = r2.PosX;
            else endX = r1.PosX;

            //Cant have doors at corner
            endX -= 2;
            startX += 2;
            var l = endX - startX;

            if (l < 1) {
                Debug.WriteLine("Error, l was < 1");
                return;
            }

            //Create openings
            y = r2.PosY - 1;
            x = RnGsus.Instance.Next(l) + startX;
            r1.createOpening(x - r1.PosX, y - r1.PosY, 1);
            r2.createOpening(x - r2.PosX, y - r2.PosY + 1, 1);
        } else {
            Debug.WriteLine("Could not add Opening between rooms " + r1 + "," + r2);
        }
    }

    //Checks if a room collides with one allready on the grid
    public bool isOccupied(Room room) {
        for (var x = room.PosX; x < room.PosX + room.SizeX; x++)
        for (var y = room.PosY; y < room.PosY + room.SizeY; y++) {
            if (!isInBounds(x, y))
                return true;
            if (_occupationMap[x, y] != 0)
                return true;
        }

        return false;
    }

    //Occupies grid space for a room
    public void occupy(Room room) {
        Debug.WriteLine("Adding Room nr " + room.Index);
        for (var x = room.PosX; x < room.PosX + room.SizeX; x++)
        for (var y = room.PosY; y < room.PosY + room.SizeY; y++)
            _occupationMap[x, y] = room.Index;
    }

    public Room createStartRoom(int maxRoomSize) {
        //Settings of the start room
        var startRoomSettings = new RoomSettings("StartRoom", new List<List<Prototype>> { _mg.RockPrototypes });

        var x = 0;
        var y = 0;
        var sizeX = RnGsus.Instance.Next(maxRoomSize - MinRoomSize) + MinRoomSize;
        var sizeY = RnGsus.Instance.Next(maxRoomSize - MinRoomSize) + MinRoomSize;

        var room = new Room(_mg, startRoomSettings, x, y, sizeX, sizeY, 1);

        for (var i = 2; i < 5; i++)
        for (var j = 2; j < 5; j++)
            room.setWalkable(i, j);

        room.innerOpening(2, 2);

        return room;
    }
}