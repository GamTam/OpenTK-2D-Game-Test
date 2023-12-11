using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK_Tut_1;

public class Player : GameObject
{
    private float _moveSpeed = 300f;

    private float _fireRate = 0.2f;
    private float _fireTimer;
    
    private float _animTime = 0.175f;
    private float _animTimer;
    private float FlashSpeed = 60;
    private float _flashTimer;
    
    private int _currentImg = 0;
    private readonly int _imgCount = 4;

    private Texture[] _currentAnim;
    private Vector3 _prevPos;
    Random _rand = new Random();
    private float _timeSinceStart = 0;

    private float HP = 3;
    
    private static Texture[] _animUp = new[]
    {
        new Texture("Chars/Layton/u0"),
        new Texture("Chars/Layton/u1"),
        new Texture("Chars/Layton/u2"),
        new Texture("Chars/Layton/u3"),
    };
    
    private static Texture[] _animDown = new[]
    {
        new Texture("Chars/Layton/d0"),
        new Texture("Chars/Layton/d1"),
        new Texture("Chars/Layton/d2"),
        new Texture("Chars/Layton/d3"),
    };
    
    private static Texture[] _animLeft = new[]
    {
        new Texture("Chars/Layton/l0"),
        new Texture("Chars/Layton/l1"),
        new Texture("Chars/Layton/l2"),
        new Texture("Chars/Layton/l3"),
    };
    
    private static Texture[] _animRight = new[]
    {
        new Texture("Chars/Layton/r0"),
        new Texture("Chars/Layton/r1"),
        new Texture("Chars/Layton/r2"),
        new Texture("Chars/Layton/r3"),
    };

    private static Texture _debug = new Texture("Red");
    
    public override void Start(bool overrideTransform = false)
    {
        base.Start(true);
        UpdateTexture("Chars/Layton/d0");
        _currentAnim = _animDown;
        _game = StaticUtilities.CurrentGameInstance;
        Tag = "Player";
    }
    
    public override void Update(FrameEventArgs args)
    {
        #region Input

        Vector2 moveVector = new Vector2(0, 0);

        if (_game.KeyboardState.IsKeyDown(Keys.W)) moveVector.Y += 1;
        if (_game.KeyboardState.IsKeyDown(Keys.S)) moveVector.Y -= 1;
        if (_game.KeyboardState.IsKeyDown(Keys.D)) moveVector.X += 1;
        if (_game.KeyboardState.IsKeyDown(Keys.A)) moveVector.X -= 1;
        
        
        Vector2 shootVector = new Vector2(0, 0);
        
        if (_game.KeyboardState.IsKeyDown(Keys.Up)) shootVector.Y += 1;
        if (_game.KeyboardState.IsKeyDown(Keys.Down)) shootVector.Y -= 1;
        if (_game.KeyboardState.IsKeyDown(Keys.Left)) shootVector.X -= 1;
        if (_game.KeyboardState.IsKeyDown(Keys.Right)) shootVector.X += 1;

        if (_game.KeyboardState.IsKeyPressed(Keys.G)) Game.Instantiate(new Enemy(), new Vector2(512, 384));

        #endregion

        int num = _rand.Next(100000);

        _timeSinceStart += (float) (args.Time / 2f);
        Console.WriteLine(10 + _timeSinceStart / 2);
        if (num < 10 + _timeSinceStart / 2)
        {
            Vector2 enemyPos = new Vector2(0, 0);

            do
            {
                enemyPos = new Vector2(_rand.Next(-50, 1100), _rand.Next(-50, 1100));
            } while (Math.Sqrt(Math.Pow(enemyPos.X - transform.Position.X, 2) +
                               Math.Pow(enemyPos.Y - transform.Position.Y, 2)) < 150f);
                
            Game.Instantiate(new Enemy(), enemyPos);
        }

        if (shootVector != Vector2.Zero)
        {
            _fireTimer -= (float) args.Time;

            if (_fireTimer <= 0)
            {
                _fireTimer = _fireRate;
                Game.Instantiate(new Bullet() {Dir = Vector2.Normalize(shootVector)}, transform.Position);
            }
        }
        else
        {
            _fireTimer = 0;
        }
        
        _prevPos = transform.Position;
        transform.Position += new Vector3(moveVector.X, moveVector.Y, 0) * _moveSpeed * (float) args.Time;

        #region Animation

        _flashTimer -= (float) args.Time;
        if (_flashTimer > 0)
        {
            float flash = (float) (MathHelper.Sin((_flashTimer - MathHelper.PiOver2 / 2f) * FlashSpeed) / 2) + 0.5f;

            if (flash < 0.5) Alpha = 1;
            else Alpha = 0;
        } else Alpha = 1;
        
        if (moveVector != Vector2.Zero) _animTimer += (float) args.Time;
        else _animTimer = 0;
        
        if (moveVector == Vector2.Zero)
        {
            _currentImg = 0;
            _mainTex = _currentAnim[_currentImg];
        }
        else
        {
            if (moveVector == new Vector2(0, 1))
            {
                _currentAnim = _animUp;
            } 
            else if (moveVector == new Vector2(0, -1))
            {
                _currentAnim = _animDown;
            } 
            else if (moveVector == new Vector2(-1, 0))
            {
                _currentAnim = _animLeft;
            } 
            else if (moveVector == new Vector2(1, 0))
            {
                _currentAnim = _animRight;
            }
        }

        if (_animTimer >= _animTime)
        {
            _currentImg += 1;

            if (_currentImg >= _imgCount) _currentImg = 0;
            
            _animTimer = 0f;
        }
        
        _mainTex = _currentAnim[_currentImg];

        #endregion
        
        transform.Position = new Vector3(transform.Position.X, transform.Position.Y,
            250 - (transform.Position.Y - transform.Scale.Y / 2));
    }

    public void Hit()
    {
        if (_flashTimer > 0) return;

        HP -= 1;
        _flashTimer = 1.25f;
        
        if (HP < 0) _game.Close();
    }
}