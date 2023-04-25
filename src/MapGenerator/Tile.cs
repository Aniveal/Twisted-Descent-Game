using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2; 

public class Tile {
    public Body Body;

    public Prototype FinalPrototype;

    public Room ParentRoom;

    //The superposition list of this tile
    public List<Prototype> Superpositions;

    //Index in parents tileMap
    public int X, Y;

    //Instantiate a Tile with multiple possible prototypes
    public Tile(List<Prototype> protList, Room parentRoom) {
        Superpositions = new List<Prototype>();
        Superpositions.AddRange(protList);
        X = Y = 0;
        ParentRoom = parentRoom;
    }

    //Create a specific tile
    public Tile(Prototype prot, int x, int y, Room parentRoom) {
        X = x;
        Y = y;
        FinalPrototype = prot;
        Superpositions = new List<Prototype> { prot };
        ParentRoom = parentRoom;
    }

    public bool makeWalkable() {
        var protList = new List<Prototype>();

        foreach (var p in Superpositions)
            if (p.Walkable)
                protList.Add(p);

        if (protList.Count() == 0)
            return false;

        setFinalPrototype(protList[RnGsus.Instance.Next(protList.Count)]);
        ParentRoom.collapseTile(this);
        return true;
    }

    public void removePrototype(string name) {
        foreach (var proto in Superpositions)
            if (proto.Name == name)
                Superpositions.Remove(proto);
    }

    public int getX() {
        return X + ParentRoom.PosX;
    }

    public int getY() {
        return Y + ParentRoom.PosY;
    }

    //Collapse the wave function where the socket on the direction dir is incompatible (dir is the direction from here to where to fit)
    //Return value is if the superposition has changed or not, i.e. if we have to propagate
    public bool collapseFunction(Tile other, string dir) {
        //Allready finished!
        if (Superpositions.Count <= 1) return false;


        var changed = true;
        var n = Superpositions.Count;

        //new list of superpositions
        var protList = new List<Prototype>();

        //Iterate over all prototypes in sup and only keep the ones which are compatible
        foreach (var otherPrototype in other.Superpositions)
        foreach (var thisPrototype in Superpositions)
            //Add all possible neighbour prototypes
            switch (dir) {
                case "up":
                    if (thisPrototype.Sockets[0] == -1 ||
                        otherPrototype.Sockets[1] == -1 ||
                        thisPrototype.Sockets[0] == otherPrototype.Sockets[1]) protList.Add(thisPrototype);
                    break;
                case "down":
                    if (thisPrototype.Sockets[1] == -1 ||
                        otherPrototype.Sockets[0] == -1 ||
                        thisPrototype.Sockets[1] == otherPrototype.Sockets[0]) protList.Add(thisPrototype);
                    break;
                case "left":
                    if (thisPrototype.Sockets[2] == -1 ||
                        otherPrototype.Sockets[3] == -1 ||
                        thisPrototype.Sockets[2] == otherPrototype.Sockets[3]) protList.Add(thisPrototype);
                    break;
                case "right":
                    if (thisPrototype.Sockets[2] == -1 ||
                        otherPrototype.Sockets[3] == -1 ||
                        thisPrototype.Sockets[3] == otherPrototype.Sockets[2]) protList.Add(thisPrototype);
                    break;
            }

        protList = protList.Distinct().ToList();
        Superpositions.Clear();
        Superpositions.AddRange(protList);

        if (Superpositions.Count == n) changed = false;


        //Finished!!
        if (Superpositions.Count == 1) FinalPrototype = Superpositions[0];
        if (Superpositions.Count == 0) Debug.WriteLine("There are no possible blueprints for this cell!!!");

        return changed;
    }

    public void setFinalPrototype(Prototype p, bool force = false) {
        if (force || Superpositions.Contains(p)) {
            FinalPrototype = p;
            Superpositions.Clear();
            Superpositions = new List<Prototype> { p };
        } else {
            Debug.WriteLine("Tried setting prototype not in superposition list!!!");
        }
    }


    public void chooseRandomPrototype() {
        //Step 1: get sum of all weights
        var totalWeight = 0;
        foreach (var p in Superpositions) totalWeight += p.Weight;

        //Step 2: find element
        var weightSum = 0;
        var randomNumber = RnGsus.Instance.Next(totalWeight);
        for (var i = 0; i < Superpositions.Count; i++) {
            weightSum += Superpositions[i].Weight;
            if (weightSum > randomNumber) {
                FinalPrototype = Superpositions[i];
                break;
            }
        }

        Superpositions = new List<Prototype> { FinalPrototype };


        //Debug.WriteLine("Chose prot: " + finalPrototype.name + "    WeightSum = " + weightSum + "   randomNumber == " + randomNumber + "       Total Weight: " + totalWeight);
    }
}