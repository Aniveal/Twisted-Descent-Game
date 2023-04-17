using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2
{
    internal class Node
    {
        //The room that is associated with this node: 
        public Room room;

        //Coordinates
        public int x, y;

        //A list of all neighbours (adjacency list)
        public List<Node> neighbours;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

    }
}
