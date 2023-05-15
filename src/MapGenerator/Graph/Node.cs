using System.Collections.Generic;

namespace TwistedDescent; 

internal class Node {
    //A list of all neighbours (adjacency list)
    public List<Node> Neighbours;

    //The room that is associated with this node: 
    public Room Room;

    //Coordinates
    public int X, Y;

    public Node(int x, int y) {
        X = x;
        Y = y;
    }
}