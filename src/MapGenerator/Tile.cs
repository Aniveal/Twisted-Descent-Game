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

namespace Meridian2
{
    public class Tile
    {
        public int x, y;

        //The superposition list of this tile
        public List<Prototype> superpositions;

        public Prototype finalPrototype;

        public Tile(List<Prototype> protList)
        {
            superpositions = new List<Prototype>();
            superpositions.AddRange(protList);
            x = y = 0;
        }

        //Collapse the wave function where the socket on the direction dir is incompatible (dir is the direction from here to where to fit)
        //Return value is if the superposition has changed or not, i.e. if we have to propagate
        public bool collapseFunction(Tile other, string dir)
        {
            Debug.WriteLine("Start Collapsing cell " + this.x + " " + this.y + " to " + dir);

            //Allready finished!
            if (superpositions.Count <= 1)
            {
                Debug.WriteLine("Allready Collapsed");
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
                    Debug.WriteLine("This Prototype: " + thisPrototype.sockets[0] + " " + thisPrototype.sockets[1] + " " + thisPrototype.sockets[2] + " " + thisPrototype.sockets[3] + " ");
                    Debug.WriteLine("Other Prototype: " + otherPrototype.sockets[0] + " " + otherPrototype.sockets[1] + " " + otherPrototype.sockets[2] + " " + otherPrototype.sockets[3] + " ");
                    //Add all possible neighbour prototypes
                    switch (dir)
                    {
                        case "up": if(thisPrototype.sockets[0] == otherPrototype.sockets[1]) protList.Add(thisPrototype); break;
                        case "down": if (thisPrototype.sockets[1] == otherPrototype.sockets[0]) protList.Add(thisPrototype); break;
                        case "left": if (thisPrototype.sockets[2] == otherPrototype.sockets[3]) protList.Add(thisPrototype); break;
                        case "right": if (thisPrototype.sockets[3] == otherPrototype.sockets[2]) protList.Add(thisPrototype); break;
                        default: break;
                    }
                }
            }
            protList = protList.Distinct().ToList();
            superpositions.Clear();
            superpositions.AddRange(protList);

            Debug.WriteLine("Result: ");
            foreach(Prototype p in protList)
            {
                Debug.WriteLine(p.sockets[0] + " " + p.sockets[1] + " " + p.sockets[2] + " " + p.sockets[3] + " ");
            }

            if (superpositions.Count == n)
            {
                changed = false;
            }

            

            //Finished!!
            if (superpositions.Count == 1)
            {
                Debug.WriteLine("Finished!");
                finalPrototype = superpositions[0];
            }
            if(superpositions.Count == 0) {
                Debug.WriteLine("There are no possible blueprints for this cell!!!");
            }

            return changed;

        }

        public void chooseRandomPrototype()
        {
            finalPrototype = superpositions[new Random().Next(superpositions.Count)];
            superpositions = new List<Prototype> { finalPrototype };
        }

    }
}
