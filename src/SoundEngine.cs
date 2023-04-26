using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace Meridian2; 

public class SoundEngine {
    private readonly RopeGame _game;

    //A list of all the possible gravel Footstep sounds to chose from
    private readonly List<SoundEffect> _gravelFootsteps;
    private readonly int _nGravelFootsteps = 20;

    public SoundEngine(RopeGame game) {
        _game = game;
        _gravelFootsteps = new List<SoundEffect>();
        LoadContent();
    }

    // this is NOT an override from game object
    private void LoadContent() {
        Debug.WriteLine("Loading Content from SoundEngine...");


        for (var i = 0; i < _nGravelFootsteps; i++)
            _gravelFootsteps.Add(_game.Content.Load<SoundEffect>("Sound/GravelFootsteps/gravel" + i));

        Debug.WriteLine("Done!");
    }

    public void playGravelFootstep() {
        var r = new Random();
        _gravelFootsteps[r.Next(20)].Play();
    }
}