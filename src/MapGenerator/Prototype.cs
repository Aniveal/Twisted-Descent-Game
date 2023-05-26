using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace TwistedDescent; 

public class Prototype {
    //Thist class stores information about tiles on the world map.
    //A prototype is basically a building block of a level

    //For implementation details refer to this video: https://www.youtube.com/watch?v=2SuvO4Gi7uY&t=658s

    //Sockets: For every direction (up, down, left, right; in that order) the socket int
    public readonly int[] Sockets;

    //Name of prototype (i.e. "Wall1ur" for Wall1, up, right)
    public Texture2D GroundTex;
    public Texture2D WallTex;

    public List<Texture2D> GroundTextures;
    public int[] TexWeights; //Weights for picking ground tiles

    public bool IsCliff;

    public string Name;

    public bool Walkable;

    //The higher, the more likely this prototype is picked
    public int Weight;


    public Prototype(Texture2D groundTex, Texture2D wallTex, string n, int[] sockets, int w, bool wal, bool isCliff = false) {
        GroundTex = groundTex;
        WallTex = wallTex;
        Name = n;
        Sockets = sockets;
        Weight = w;
        Walkable = wal;
        IsCliff = isCliff;
    }

    public Prototype(List<Texture2D> groundTextures, Texture2D wallTex, string n, int[] sockets, int w, bool wal, int[] weights, bool isCliff = false)
    {
        this.GroundTextures = groundTextures;
        WallTex = wallTex;
        Name = n;
        Sockets = sockets;
        Weight = w;
        Walkable = wal;
        IsCliff = isCliff;
        TexWeights = weights;
    }

    public Texture2D chooseTexture()
    {
        if(TexWeights == null || TexWeights.Length == 0)
            return GroundTex;
        if (TexWeights.Length != GroundTextures.Count) return GroundTex;

        List<int> weightList = new List<int>();

        int i = 0;
        foreach(int x in TexWeights)
        {
            i += x;
            weightList.Add(i);
        }

        int r = RnGsus.Instance.Next(i);

        int j = 0;
        while(weightList[j] < r)
        {
            j++;
        }

       return GroundTextures[j];
    }
}