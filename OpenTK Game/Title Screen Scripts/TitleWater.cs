using Assimp;
using NAudio.Wave;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

namespace Open_TK_Tut_1;

public class TitleWater : GameObject
{
    public Game _game;

    private float _animTime = 0.25f;
    private float _animTimer;

    private int _imgCount = 6;
    private int _currentImg = 0;

    private float _moveSpeed = 60;
    private float _timer = 0f;
    
    private WaveOutEvent _waveOut = new WaveOutEvent();

    private static Texture[] _anim = new[]
    {
        new Texture("Title Screen Water/Water1"),
        new Texture("Title Screen Water/Water2"),
        new Texture("Title Screen Water/Water3"),
        new Texture("Title Screen Water/Water4"),
        new Texture("Title Screen Water/Water5"),
        new Texture("Title Screen Water/Water6"),
    };
    
    public TitleWater(Game game = null, bool start=false) : base(game, start)
    {
        _mainTex = _anim[0];
        
        _game = game;
    }

    public override void Start(bool overrideTransform = true)
    {
        base.Start(true);
        // _moveSpeed *= (float) new Random().NextDouble() + 0.5f;
        UpdateTexture(_anim[0]);
    }

    public override void Update(FrameEventArgs args)
    {
        _timer += (float) args.Time;
        
        if (transform.Position.X < -50f) Destroy(this);
        
        _animTimer += (float) args.Time;
        
        RealPos -= Vector3.UnitX * _moveSpeed * (float) args.Time;
 
        if (_animTimer >= _animTime)
        {
            _currentImg += 1;

            if (_currentImg >= _imgCount) _currentImg = 0;

            _mainTex = _anim[_currentImg];
            _animTimer = 0f;
        }

        transform.Position = RealPos;
    }
}