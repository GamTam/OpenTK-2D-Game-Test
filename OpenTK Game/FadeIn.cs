﻿using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class FadeIn : GameObject
{
    public float FadeInSpeed = 5f;
    public bool FadedIn = false;

    public float _fadeInTimer;
    
    public FadeIn(Game game = null, bool start = false) : base(game, start)
    {
        Alpha = 0f;
    }

    public override void Update(FrameEventArgs args)
    {
        if (_fadeInTimer > FadeInSpeed && !FadedIn)
        {
            _fadeInTimer = FadeInSpeed;
            FadedIn = true;
        }
        else if (!FadedIn)
        {
            _fadeInTimer += (float) args.Time;
        }
        
        Alpha = _fadeInTimer / FadeInSpeed;
    }
}