using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace Meridian2; 

public sealed class SoundEngine {

    static SoundEngine()
    {

    }
    
    RopeGame _game;

    //A list of all the possible gravel Footstep sounds to chose from
    private readonly List<SoundEffect> _gravelFootsteps;
    private readonly int _nGravelFootsteps = 20;
    private SoundEffect _ropeFling;
    private SoundEffect _chest;
    private SoundEffect _squish;
    private SoundEffect _columnCollapse;
    private readonly List<SoundEffect> _swordHits = new();

    private SoundEngine() {
        
        _gravelFootsteps = new List<SoundEffect>();
        
    }

    public static SoundEngine Instance { get; } = new();

    public void SetRopeGame(RopeGame game)
    {
        _game = game;
        LoadContent();
    }

    // this is NOT an override from game object
    private void LoadContent() {
        Debug.WriteLine("Loading Content from SoundEngine...");


        for (var i = 0; i < _nGravelFootsteps; i++)
            _gravelFootsteps.Add(_game.Content.Load<SoundEffect>("Sound/GravelFootsteps/gravel" + i));
        for (var i = 1; i < 5; i++)
            _swordHits.Add(_game.Content.Load<SoundEffect>("Sound/Hits/SwordHit" + i));
        _squish = _game.Content.Load<SoundEffect>("Sound/Hits/Squish");
        _chest = _game.Content.Load<SoundEffect>("Sound/Interactions/Chest5");
        _ropeFling = _game.Content.Load<SoundEffect>("Sound/Rope/RopeFling1");
        _columnCollapse = _game.Content.Load<SoundEffect>("Sound/ColumnCollapse");

        Debug.WriteLine("Done!");
    }

    public void playGravelFootstep() {
        _gravelFootsteps[RnGsus.Instance.Next(20)].Play();
    }

    public void ChestSound()
    {
        _chest.Play();
    }
    public void FlingSound()
    {
        _ropeFling.Play();
    }
    public void CollapseColumn()
    {
        _columnCollapse.Play();
    }
    public void Squish()
    {
        _squish.Play();
    }
    public void SwordHit()
    {
        _swordHits[RnGsus.Instance.Next(4)].Play();
    }
}