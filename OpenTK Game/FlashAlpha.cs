using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class FlashAlpha : GameObject
{
    public float FadeInSpeed = 1f;

    public float _fadeInTimer;
    public bool _square;
    
    public FlashAlpha(Game game = null, bool start = false) : base(game, start)
    {
        Alpha = 0f;
    }

    public override void Update(FrameEventArgs args)
    {
        _fadeInTimer += (float) args.Time;
        
        if (!_square) Alpha = (float) (MathHelper.Sin((_fadeInTimer - MathHelper.PiOver2 / 2f) * FadeInSpeed) / 2) + 0.5f;
        else Alpha = (float) (MathHelper.Sin((_fadeInTimer - MathHelper.PiOver2 / 2f) * FadeInSpeed) / 2) >= 0 ? 1 : 0;
    }
}