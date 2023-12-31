﻿using Assimp;
using NAudio.Wave;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

namespace Open_TK_Tut_1;

public class Explosion : GameObject
{
    public Game _game;

    private float _animTime = 0.1f;
    private float _animTimer;

    private int _imgCount = 16;
    private int _currentImg = 0;
    
    private WaveOutEvent _waveOut = new WaveOutEvent();
    
    Texture boom = new Texture("Explosion/explosion_0");
    
    public Explosion(Game game = null, bool start=false) : base(game, start)
    {
        _mainTex = boom;
        
        _game = game;
    }

    public override void Start(bool overrideTransform = false)
    {
        base.Start();
        
        transform.Scale = new Vector3(_mainTex.Size.X, _mainTex.Size.Y, transform.Position.Z);

        Game.SoundManager.Play("explosion");
    }

    public override void Update(FrameEventArgs args)
    {
        _animTimer += (float) args.Time;

        if (_animTimer >= _animTime)
        {
            _currentImg += 1;

            if (_currentImg < _imgCount)
            {
                _mainTex = new Texture($"Explosion/explosion_{_currentImg}");
                _animTimer = 0f;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}