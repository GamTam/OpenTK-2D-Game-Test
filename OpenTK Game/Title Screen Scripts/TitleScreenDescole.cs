using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK_Tut_1;

public class TitleScreenDescole : GameObject
{
    private float _moveSpeed = 400;

    private Vector3 realPos;
    
    public TitleScreenDescole(Game game = null, bool start = false) : base(game, start)
    {
        UpdateTexture("Descole DS");
        Alpha = 1;
    }

    public override void Start(bool overrideTransform = false)
    {
        base.Start(false);
        UpdateTexture("Descole DS");
        transform.Position = new Vector3(Game.gameCam.Position.X, _mainTex.Size.Y, transform.Position.Z);
        realPos = transform.Position;
    }

    public override void Update(FrameEventArgs args)
    {
        if (_game.KeyboardState.IsKeyDown(Keys.A))
        {
            realPos -= Vector3.UnitX * _moveSpeed * (float) args.Time;
            FlipX = false;
        }
        
        if (_game.KeyboardState.IsKeyDown(Keys.D))
        {
            realPos += Vector3.UnitX * _moveSpeed * (float) args.Time;
            FlipX = true;
        }

        if (_game.KeyboardState.IsKeyDown(Keys.W))
        {
            Alpha += (float) args.Time;
            if (Alpha > 1) Alpha = 1;
        } 
        else if (_game.KeyboardState.IsKeyDown(Keys.S))
        {
            Alpha -= (float) args.Time;
            if (Alpha < 0) Alpha = 0;
        }

        transform.Position = new Vector3(MathF.Round(realPos.X / 4f) * 4f, realPos.Y, realPos.Z);
    }
}