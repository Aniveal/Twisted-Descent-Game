using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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
    private SoundEffect _buttonHit;
    private SoundEffect _amphora;
    private SoundEffect _electricityImpact;
    private SoundEffect _electricityColumn;
    private SoundEffect _explosion;
    private SoundEffect _wilhelmScream;
    private Song _song;
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
        _explosion = _game.Content.Load<SoundEffect>("Sound/Hits/Explosion");
        _electricityImpact = _game.Content.Load<SoundEffect>("Sound/Hits/ElectricityImpact");
        _electricityColumn = _game.Content.Load<SoundEffect>("Sound/ElectricityArc");
        _buttonHit = _game.Content.Load<SoundEffect>("Sound/Interactions/ButtonPress");
        _amphora = _game.Content.Load<SoundEffect>("Sound/Hits/AmphoraSmash1");
        _wilhelmScream = _game.Content.Load<SoundEffect>("Sound/WilhelmScream");
        _song = _game.Content.Load<Song>("Sound/Theseus");

        MediaPlayer.Volume = 1f;
        SoundEffect.MasterVolume = 1f;

        MediaPlayer.IsRepeating = true;

        Debug.WriteLine("Done!");
    }

    public void playGravelFootstep() {
        var pitch = (RnGsus.Instance.Next(3) - 1) / 8f;
        _gravelFootsteps[RnGsus.Instance.Next(20)].Play(0.6f, pitch, 0f);
    }

    public void playTheme()
    {
        if(MediaPlayer.State == MediaState.Stopped)
            MediaPlayer.Play(_song);
        MediaPlayer.Resume();
    }
    public void pauseTheme()
    {
        MediaPlayer.Pause();
    }

    public void SetEffectVolume(float volume)
    {
        SoundEffect.MasterVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        MediaPlayer.Volume = volume;
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
    public void Amphora()
    {
        _amphora.Play();
        _explosion.Play();
    }
    
    public void ButtonClick()
    {
        _buttonHit.Play();
    }

    public void ElectroColumn()
    {
        _electricityColumn.Play();
    }

    public void ElectroShock()
    {
        _electricityImpact.Play();
    }

    public void WilhelmScream() {
        _wilhelmScream.Play();
    }
}