using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class Enemy : GameObject
{
    private bool _isHit;
    private float _speed = 200;
    
    private float _animTime = 0.05f;
    private float _animTimer;

    private int _imgCount = 2;
    private int _currentImg = 0;

    private float FlashSpeed = 75;
    private float _flashTimer;

    private int HP = 3;
    private Player _target;

    private float _scaleTime = 0.1f;
    private float _scaleTimer = 0f;
    
    private static Texture[] _anim = new[]
    {
        new Texture("Chars/Flying/0"),
        new Texture("Chars/Flying/1")
    };
    
    private static Texture[] _hitAnim = new[]
    {
        new Texture("Chars/Flying/0_red"),
        new Texture("Chars/Flying/1_red")
    };
    
    public override void Start(bool overrideTransform = true)
    {
        base.Start();
        UpdateTexture(_anim[0]);
        Game.Enemies.Add(this);
        _target = null;
        transform.Scale = new Vector3(0, 0, 1);

        foreach (GameObject obj in Game.UnLitObjects)
        {
            if (obj.Tag.Equals("Player"))
            {
                _target = (Player) obj;
                break;
            }
        }
    }

    public override void Update(FrameEventArgs args)
    {
        _animTimer += (float) args.Time;
        _flashTimer -= (float) args.Time;

        if (_scaleTimer < _scaleTime)
        {
            _scaleTimer += (float) args.Time;
            transform.Scale = new Vector3(MathHelper.Lerp(0, _mainTex.Size.X, _scaleTimer / _scaleTime),
                MathHelper.Lerp(0, _mainTex.Size.Y, _scaleTimer / _scaleTime), 1);
        }
        else transform.Scale = _mainTex.Size;

        Vector2 moveVector = Vector2.Normalize(new Vector2(_target.transform.Position.X, _target.transform.Position.Y) - new Vector2(transform.Position.X, transform.Position.Y));

        if (_animTimer >= _animTime)
        {
            _currentImg += 1;

            if (_currentImg >= _imgCount) _currentImg = 0;
            
            _animTimer = 0f;
        }

        if (_flashTimer > 0)
        {
            float flash = (float) (MathHelper.Sin((_flashTimer - MathHelper.PiOver2 / 2f) * FlashSpeed) / 2) + 0.5f;

            if (flash < 0.5) _mainTex = _anim[_currentImg];
            else _mainTex = _hitAnim[_currentImg];
        } else _mainTex = _anim[_currentImg];
        
        if (IsColliding(_target))
        {
            _target.Hit();
            Destroy(this);
        }

        transform.Position += new Vector3(moveVector.X, moveVector.Y, 0) * _speed * (float) args.Time;
        transform.Position = new Vector3(transform.Position.X, transform.Position.Y, 250 - transform.Position.Y);
    }

    public void Hit()
    {
        HP -= 1;
        _flashTimer = 0.5f;

        if (HP <= 0)
        {
            Destroy(this);
            Game.Instantiate(new Explosion(), transform.Position, Vector3.One * 100);
        }
    }

    public override void OnDestroy()
    {
        Game.Enemies.Remove(this);
    }
}