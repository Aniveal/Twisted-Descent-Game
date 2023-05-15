using System;

namespace TwistedDescent; 

//Use this class only for random numbers!!!
public sealed class RnGsus {
    //Set seed to 0 to get a random one
    public int Seed = 0;
    private Random _random;

    static RnGsus() { }

    private RnGsus() {
        if (Seed == 0)
        {
            _random = new Random();
            Seed = _random.Next();
            _random = new Random(Seed);

        }
        else _random = new Random(Seed);
    }

    public static RnGsus Instance { get; } = new();

    //Returns a random number between 0 and 1 (Same as Random.NextDouble)
    public double NextDouble() {
        return _random.NextDouble();
    }

    //Same as Random.Next()
    public int Next() {
        return _random.Next();
    }

    public void NewSeed()
    {
        Seed = _random.Next();
        _random = new Random(Seed);
    }

    public int Next(int i) {
        return _random.Next(i);
    }
}