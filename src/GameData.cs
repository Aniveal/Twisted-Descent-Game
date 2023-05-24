using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using TwistedDescent.Screens;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace TwistedDescent; 

public class GameData {
    //health
    private const int StartMaxHealth = 3;

    private RopeGame _game;
    public int MaxHealth;

    public int Score = 0;
    public int Kills = 0;
    public string playerName = "PL1";

    private int previousKills = 0;
    private int KillStreak = 0;
    private double LastKillms = 0;
    private double KillStreakTimeWindow = 1000; // ms, if next kill happens within this many milliseconds, it counts towards current killstreak

    public double TimeLeft = 60f;
    public double MaxTimeLeft = 120f;

    //The current game difficulty level. Starting at 1, goes to infinity (or overflow i guess xD)>
    //The higher the more enemies spawn, they are harder, better loot, larger levels
    public int currentDifficulty = 2;
    public float levelScaling = 1.5f;

    public double levelStartTime = 0;

    //Spears data
    public int[] Spears; //basic, electric, fragile spears

    public GameData(RopeGame game) {
        resetHealth();
        GameOver = false;
        _game = game;
        Spears = new int[3] { 5, 0, 0 }; //define initial spears amount, basic/electric/fragile

        
    }

    public int Health { private set; get; }

    public bool GameOver { private set; get; }

    public void DecayTime(GameTime gameTime) {
        TimeLeft -= gameTime.ElapsedGameTime.TotalSeconds;
        if (TimeLeft < 0) {
            TimeLeft = 0;
            EndGame(true);
        }
    }

    public void AddTime(float seconds) {
        TimeLeft += seconds;
        if (TimeLeft > MaxTimeLeft) {
            TimeLeft = MaxTimeLeft;
        }
    }

    public int updateKillStreak(Double totalMilliseconds)
    {
        bool killStreakContinues = (LastKillms + KillStreakTimeWindow >= totalMilliseconds); // true if we are still within killstreak timewindow
        var killDifference = Kills - previousKills; // how many kills happend since last call

        previousKills = Kills;

        if (killDifference > 0)     // a kill happend
        {
            LastKillms = totalMilliseconds;   // update time of last kill

            if (killStreakContinues)   // kill happend within killstreak
            {
                KillStreak += killDifference;
            }
            else  // start a new killstreak
            {           
                KillStreak = killDifference;
            }
        }
        else if (!killStreakContinues)  // no new kill and time on killstreak run out
        {
            KillStreak = 0;
        }

        return KillStreak;
    }
    
    public void resetHealth() {
        MaxHealth = StartMaxHealth;
        Health = MaxHealth;
    }

    private void EndGame(bool timeOut) {
        GameOver = true;
        _game.leaderBoard.Add(new KeyValuePair<string, int>(playerName, Score));
        _game.leaderBoard = _game.leaderBoard.OrderByDescending(x => x.Value).ToList().GetRange(0, Math.Min(5, _game.leaderBoard.Count()));
        _game._finalScreen = new FinalScreen(_game, _game.Content, timeOut);
        _game._finalScreen.Initialize();
        _game.ChangeState(RopeGame.State.Final);
        _game.ResetGame();
    }
    
    public void RemoveHealth(int amount) {
        SoundEngine.Instance.SwordHit();
        Health -= 1;
        if (Health <= 0) {
            EndGame(false);
        }
    }

    public void AddHealth(int amount) {
        Health = Math.Min(MaxHealth, Health + amount);
    }

    public void IncreaseDifficulty()
    {
        currentDifficulty = (int)(currentDifficulty * levelScaling);
    }
}