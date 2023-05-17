using System;
using System.Collections.Generic;
using System.Diagnostics;
using TwistedDescent.Columns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using tainicom.Aether.Physics2D.Collision;
using TwistedDescent.Theseus;

namespace TwistedDescent; 

public sealed class SoundEngine {

    static SoundEngine()
    {

    }

    public float musicVolume = 1f;
    public float effectVolume = 1f;
    
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

    private Player player;

    private SoundEngine() {
        
        _gravelFootsteps = new List<SoundEffect>();
        
    }

    public static SoundEngine Instance { get; } = new();

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

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

        MediaPlayer.Volume = musicVolume;
        SoundEffect.MasterVolume = effectVolume;

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
        this.effectVolume = volume;
        SoundEffect.MasterVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        this.musicVolume = volume;
        MediaPlayer.Volume = volume;
    }




    //Calculates the volume based on distance
    public float CalculateIntensity(Vector2 position)
    {
        double distance = Vector2.Distance(position, this.player.Body.Position);

        if(distance <= 1) return 1;

        //Assume base sound is 100db loud
        double reduction = Math.Abs(20f * Math.Log(distance * 3));

        float intensity = Math.Min(1.0f, Math.Max(1f - (float)(reduction * 0.01), 0.1f));

        return intensity;
    }

    public float CalculatePan(Vector2 position)
    {
        //PAN DOES NOT WORK DUE TO MONOGAME BUG
        return 1f;
        float difference = position.X - this.player.Body.Position.X;

        if (difference < -10)
            return -1f;
        else if (difference > 10) return 1f;
        else return difference / 5f;

        
    }

    public void PlayEffect(SoundEffect effect, Vector2 position)
    {
        SoundEffectInstance i = effect.CreateInstance();
        i.Volume = CalculateIntensity(position);
        //i.Pan = CalculatePan(position);
        i.Play();
    }

    public void ChestSound()
    {
        _chest.Play();
    }
    public void FlingSound(Vector2 position)
    {
        PlayEffect(_ropeFling, position);
    }
    public void CollapseColumn(Vector2 position)
    {
        PlayEffect(_columnCollapse, position);
    }
    public void Squish(Vector2 position)
    {
        PlayEffect(_squish, position);
    }
    public void SwordHit()
    {
        _swordHits[RnGsus.Instance.Next(4)].Play();
    }
    public void Amphora(Vector2 position)
    {
        PlayEffect(_amphora, position);
        PlayEffect(_explosion, position);
    }
    
    public void ButtonClick()
    {
        _buttonHit.Play();
    }

    public SoundEffectInstance GetElectroColumnInstance(Vector2 position)
    {
        SoundEffectInstance i = _electricityColumn.CreateInstance();
        i.Volume = CalculateIntensity(position);
        return i;
    }

    public void ElectroShock(Vector2 position)
    {
        PlayEffect(_electricityImpact, position);
    }

    public void WilhelmScream(Vector2 position)
    {
        PlayEffect(_wilhelmScream, position);
    }
}