using Meridian2.Screens;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Meridian2; 

public class GameData {
    //health
    private const int StartMaxHealth = 3;

    private RopeGame _game;
    public int MaxHealth;

    public int Score = 0;
    public int Kills = 0;

    //The current game difficulty level. Starting at 1, goes to infinity (or overflow i guess xD)>
    //The higher the more enemies spawn, they are harder, better loot, larger levels
    public int currentDifficulty = 2;
    public float levelScaling = 1.5f;

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
        SoundEngine.Instance.SwordHit();
        Health -= 1;
        if (Health <= 0)
        {
            GameOver = true;
            _game._finalScreen = new FinalScreen(_game, _game.Content);
            _game._finalScreen.Initialize();
            _game.ChangeState(RopeGame.State.Final);
            _game.ResetGame();
        }
    }

    public void AddHealth(int amount) {
        Health = Math.Max(MaxHealth, Health + amount);
    }

    public void IncreaseDifficulty()
    {
        currentDifficulty = (int)(currentDifficulty * levelScaling);
    }
}