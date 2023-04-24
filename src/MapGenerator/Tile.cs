using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2
{
    public class Tile
    {
        //Index in parents tileMap
        public int x, y;

        public Body body;

        public Room parentRoom;

        //The superposition list of this tile
        public List<Prototype> superpositions;

        public Prototype finalPrototype;

        //Instantiate a Tile with multiple possible prototypes
        public Tile(List<Prototype> protList, Room parentRoom)
        {
            superpositions = new List<Prototype>();
            superpositions.AddRange(protList);
            x = y = 0;
            this.parentRoom = parentRoom;
        }

        //Create a specific tile
        public Tile(Prototype prot, int x, int y, Room parentRoom)
        {
            this.x = x;
            this.y = y;
            finalPrototype = prot;
            superpositions = new List<Prototype> { prot };
            this.parentRoom = parentRoom;
        }

        public bool makeWalkable()
        {
            List<Prototype> protList = new List<Prototype>();

            foreach(Prototype p in superpositions)
            {
                if(p.walkable)
                    protList.Add(p);
            }

            if (protList.Count() == 0)
                return false;

            setFinalPrototype(protList[RNGsus.Instance.Next(protList.Count)]);
            parentRoom.collapseTile(this);
            return true;
        }

        public void removePrototype(string name)
        {
            foreach(Prototype proto in superpositions)
            {
                if (proto.name == name)
                {
                    superpositions.Remove(proto);
                }
            }
        }

        public int getX() { return x + parentRoom.posX; }
        public int getY() { return y + parentRoom.posY; }

        //Collapse the wave function where the socket on the direction dir is incompatible (dir is the direction from here to where to fit)
        //Return value is if the superposition has changed or not, i.e. if we have to propagate
        public bool collapseFunction(Tile other, string dir)
        {

            //Allready finished!
            if (superpositions.Count <= 1)
            {
                return false;
            }
                

            bool changed = true;
            int n = superpositions.Count;

            //new list of superpositions
            List<Prototype> protList = new List<Prototype>();

            //Iterate over all prototypes in sup and only keep the ones which are compatible
            foreach (Prototype otherPrototype in other.superpositions)
            {
                foreach (Prototype thisPrototype in this.superpositions)
                {
                    //Add all possible neighbour prototypes
                    switch (dir)
                    {
                        case "up": if(thisPrototype.sockets[0] == -1 || 
                                    otherPrototype.sockets[1] == -1 || 
                                    thisPrototype.sockets[0] == otherPrototype.sockets[1]) protList.Add(thisPrototype); break;
                        case "down": if (thisPrototype.sockets[1] == -1 ||
                                    otherPrototype.sockets[0] == -1 || 
                                    thisPrototype.sockets[1] == otherPrototype.sockets[0]) protList.Add(thisPrototype); break;
                        case "left": if (thisPrototype.sockets[2] == -1 ||
                                    otherPrototype.sockets[3] == -1 || 
                                    thisPrototype.sockets[2] == otherPrototype.sockets[3]) protList.Add(thisPrototype); break;
                        case "right": if (thisPrototype.sockets[2] == -1 ||
                                    otherPrototype.sockets[3] == -1 || 
                                    thisPrototype.sockets[3] == otherPrototype.sockets[2]) protList.Add(thisPrototype); break;
                        default: break;
                    }
                }
            }
            protList = protList.Distinct().ToList();
            superpositions.Clear();
            superpositions.AddRange(protList);

            if (superpositions.Count == n)
            {
                changed = false;
            }

            

            //Finished!!
            if (superpositions.Count == 1)
            {
                finalPrototype = superpositions[0];
            }
            if(superpositions.Count == 0) {
                Debug.WriteLine("There are no possible blueprints for this cell!!!");
            }

            return changed;

        }

        public void setFinalPrototype(Prototype p, bool force = false)
        {
            if (force || superpositions.Contains(p))
            {
                finalPrototype = p;
                superpositions.Clear();
                superpositions = new List<Prototype> { p };
            }

            else Debug.WriteLine("Tried setting prototype not in superposition list!!!");
        }
            

        public void chooseRandomPrototype()
        {
            //Step 1: get sum of all weights
            int totalWeight = 0;
            foreach (Prototype p in superpositions)
            {
                totalWeight += p.weight;
            }

            //Step 2: find element
            int weightSum = 0;
            int randomNumber = RNGsus.Instance.Next(totalWeight);
            for(int i = 0; i < superpositions.Count; i++)
            {
                weightSum += superpositions[i].weight;
                if (weightSum > randomNumber)
                {
                    finalPrototype = superpositions[i];
                    break;
                }

            }

            superpositions = new List<Prototype> { finalPrototype };

            
            //Debug.WriteLine("Chose prot: " + finalPrototype.name + "    WeightSum = " + weightSum + "   randomNumber == " + randomNumber + "       Total Weight: " + totalWeight);
        }

    }
}
