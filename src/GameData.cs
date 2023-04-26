using System;

namespace Meridian2; 

public class GameData {
    //health
    private const int StartMaxHealth = 3;

    private RopeGame _game;
    public int MaxHealth;

    public int Score = 0;

    //Spears data
    public int[] Spears; //basic, electric, fragile spears

    public GameData(RopeGame game) {
        resetHealth();
        GameOver = false;
        _game = game;
        Spears = new int[3] { 10, 5, 5 }; //define initial spears amount, basic/electric/fragile
    }

    public int Health { private set; get; }

    public bool GameOver { private set; get; }

    public void resetHealth() {
        MaxHealth = StartMaxHealth;
        Health = MaxHealth;
    }

    public void RemoveHealth(int amount) {
        Health -= 1;
        if (Health <= 0) GameOver = true;
    }

    public void AddHealth(int amount) {
        Health = Math.Max(MaxHealth, Health + amount);
    }
}