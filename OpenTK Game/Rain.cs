using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class RainSpawner : GameObject
{
    private Random rand = new Random();

    public override void Start(bool overrideTransform = false)
    {
        base.Start(false);
        Alpha = 0;
    }

    public override void Update(FrameEventArgs args)
    {
        int num = rand.Next(1000);
        if (num < 75)
        {
            Game.Instantiate(new Rain(), new Vector2(rand.Next(1024), 1000));
        }
    }
}

public class Rain : GameObject
{
    private float moveSpeed = 600f;
    private Random rand = new Random();
    private float _deathTimer;

    private static Texture RainTex = new Texture("Rain");
    public override void Start(bool overrideTransform = false)
    {
        base.Start();
        UpdateTexture(RainTex);
    }
    
    public override void Update(FrameEventArgs args)
    {
        _deathTimer += (float) args.Time;
        
        transform.Position += -Vector3.UnitY * moveSpeed * (float) args.Time;
        transform.Position = new Vector3(transform.Position.X, transform.Position.Y, 250 - transform.Position.Y);
        
        int num = rand.Next(1000);
        if (num == 1 || _deathTimer > 5f)
        {
            Game.Instantiate(new RainSplash(), transform.Position);
            Destroy(this);
        }
    }
}

public class RainSplash : GameObject
{
    public Game _game;

    private float _animTime = 0.1f;
    private float _animTimer;

    private int _imgCount = 6;
    private int _currentImg = 0;
    
    Texture boom = new Texture("Water Splash/splash_0");
    
    public RainSplash(Game game = null, bool start=false) : base(game, start)
    {
        _mainTex = boom;
        
        _game = game;
    }

    public override void Start(bool overrideTransform = false)
    {
        base.Start();
        
        transform.Scale = new Vector3(_mainTex.Size.X, _mainTex.Size.Y, transform.Position.Z);
    }

    public override void Update(FrameEventArgs args)
    {
        _animTimer += (float) args.Time;

        if (_animTimer >= _animTime)
        {
            _currentImg += 1;

            if (_currentImg < _imgCount)
            {
                _mainTex = new Texture($"Water Splash/splash_{_currentImg}");
                _animTimer = 0f;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}