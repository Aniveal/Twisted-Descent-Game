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
        public Vector2 position;

        //The superposition list of this tile
        public List<Prototype> superpositions = new List<Prototype>();

        public Prototype finalPrototype;

        public int x;
        public int y;

        public int numSuperpositions { get; set; }

        public Tile(List<Prototype> protList)
        {
            superpositions.AddRange(protList);
            numSuperpositions = protList.Count();
            position = new Vector2 (0, 0);
        }

        //Collapse the wave function where the socket on the direction dir is incompatible (dir is the direction from here to where to fit)
        //Return value is if the superposition has changed or not, i.e. if we have to propagate
        public bool collapseFunction(Tile other, string dir)
        {
            Debug.WriteLine("Start Collapse between cells: ");

            //Allready finished!
            if (numSuperpositions <= 1)
            {
                Debug.WriteLine("Allready Collapsed");
                return false;
            }
                

            bool changed = true;

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
                        case "up": if(thisPrototype.sockets[0] == otherPrototype.sockets[1]) protList.Add(otherPrototype); break;
                        case "down": if (thisPrototype.sockets[1] == otherPrototype.sockets[0]) protList.Add(otherPrototype); break;
                        case "left": if (thisPrototype.sockets[2] == otherPrototype.sockets[3]) protList.Add(otherPrototype); break;
                        case "right": if (thisPrototype.sockets[3] == otherPrototype.sockets[2]) protList.Add(otherPrototype); break;
                        default: break;
                    }
                }
            }

            Debug.WriteLine("Result: ");
            foreach(Prototype p in protList)
            {
                Debug.WriteLine(p.sockets[0] + " " + p.sockets[1] + " " + p.sockets[2] + " " + p.sockets[3] + " ");
            }

            if (superpositions.Count == protList.Count)
            {
                changed = false;
            }


            superpositions = protList;

            numSuperpositions = superpositions.Count();

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
            numSuperpositions = 1;
        }

    }
}
