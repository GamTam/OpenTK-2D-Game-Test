using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK_Tut_1;

public class Player : GameObject
{
    private float _moveSpeed = 400f;
    
    public Player(Game game = null, bool start = false) : base(game, start)
    {
        UpdateTexture("Descole DS");
        Alpha = 1;
    }

    public override void Start(bool overrideTransform = false)
    {
        base.Start(true);
        // UpdateTexture("Descole DS");
    }

    public override void Update(FrameEventArgs args)
    {
        if (_game.KeyboardState.IsKeyDown(Keys.A))
        {
            transform.Position -= Vector3.UnitX * _moveSpeed * (float) args.Time;
            FlipX = false;
        }
        
        if (_game.KeyboardState.IsKeyDown(Keys.D))
        {
            transform.Position += Vector3.UnitX * _moveSpeed * (float) args.Time;
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
    }
}