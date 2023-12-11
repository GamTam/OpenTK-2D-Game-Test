using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK_Tut_1;

public class TitleScreenFadeTimer : GameObject
{
    private float _titleTime = 4.438f;
    private float _subTime = 13.337f;
    private float _enterTime = 22.236f;
    private float _fadeOutTime = 5002f;
    private float _loadSceneTime = 5005f;
    
    private float _timer = 0f;

    private bool _spawnedLogo;
    private bool _spawnedSubLogo;
    private bool _spawnedEnter;
    private bool _spawnedFadeOut;

    private FadeIn _title;
    private FadeIn _subTitle;
    private FlashAlpha _flash;

    private FadeOut _fadeOut;

    private bool _fadingOut = false;
    private bool _pressedEnter = false;

    private int _descoleCount = 0;
    
    public TitleScreenFadeTimer()
    {
        Alpha = 0f;
    }

    public override void Start(bool overrideTransform = false)
    {
        base.Start(false);
        _fadeOut = new FadeOut() {FadeInSpeed = 2.5f};
        _fadeOut.UpdateTexture("Black Screen");
        GameObject obj = Game.Instantiate(_fadeOut, new Vector2(512, 384));
        obj.transform.Position += Vector3.UnitZ * 4;
    }

    public override void Update(FrameEventArgs args)
    {
        _game = StaticUtilities.CurrentGameInstance;
        _timer += (float) args.Time;

        int rand = new Random().Next(5000);
        
        if (rand == 24) Game.Instantiate(new TitleWater(), new Vector3(1100f, (new Random().Next(53) * 4) - 2, 0.5f));
        
        if (_game.KeyboardState.IsKeyPressed(Keys.Enter) && !_fadingOut)
        {
            _pressedEnter = true;
            
            if (_timer > _enterTime)
            {
                _fadingOut = true;
                
                Game.SoundManager.Play("startGame");
                Game.MusicManager.Stop();
            }
            
            _timer = 5000f;

            if (!_fadeOut.FadedIn)
            {
                _fadeOut._fadeInTimer = 100000f;
            }

            if (_title != null) _title._fadeInTimer = 100000f;
            if (_subTitle != null) _subTitle._fadeInTimer = 100000f;
            if (_flash != null)
            {
                _flash.FadeInSpeed = 20;
                _flash._square = true;
            }
        }

        if (_timer > _titleTime && !_spawnedLogo)
        {
            _spawnedLogo = true;
            _title = new FadeIn(_game);
            GameObject obj = Game.Instantiate(_title, new Vector3(512, 692, 20), new Vector3(297, 46, 1));
            obj._mainTex = new Texture("Logo");
            
            if (_pressedEnter) _title._fadeInTimer = 100000f;
        }
        
        if (_timer > _subTime && !_spawnedSubLogo)
        {
            _spawnedSubLogo = true;
            _subTitle = new FadeIn(_game);
            GameObject obj = Game.Instantiate(_subTitle, new Vector3(512, 580, 20), new Vector3(247, 47, 20));
            obj._mainTex = new Texture("SubLogo");
            
            if (_pressedEnter) _subTitle._fadeInTimer = 100000f;
        }
        
        if (_timer > _enterTime && !_spawnedEnter)
        {
            _spawnedEnter = true;
            _flash = new FlashAlpha(_game)
            {
                FadeInSpeed = 2f
            };
            GameObject obj = Game.Instantiate(_flash, new Vector3(512, 300, 20), new Vector3(212, 23, 20));
            obj._mainTex = new Texture("Press Enter");
            
            if (_fadingOut)
            {
                _flash.FadeInSpeed = 20;
                _flash._square = true;
            } 
            else if (_pressedEnter)
            {
                _flash._fadeInTimer = MathHelper.PiOver2;
            }
        }

        if (_timer > _fadeOutTime && _fadingOut && !_spawnedFadeOut)
        {
            _spawnedFadeOut = true;
            FadeIn fadeIn = new FadeIn() { FadeInSpeed = 1f };
            fadeIn.UpdateTexture("Black Screen");
            GameObject obj = Game.Instantiate(fadeIn, new Vector3(512, 384, 500f));
            obj.transform.Position += Vector3.UnitZ * 4;
        }

        if (_timer > _loadSceneTime && _fadingOut)
        {
            _game.Close();
        }
        
        if (_game.KeyboardState.IsKeyPressed(Keys.F) && !_fadingOut)
        {
            _game.Title = "The Descole Game";
            
            TitleScreenDescole descole = new TitleScreenDescole(_game);
            _descoleCount += 1;
            GameObject obj = Game.Instantiate(descole, new Vector3(Game.gameCam.Position.X, descole._mainTex.Size.Y, 20 + _descoleCount));

            Explosion explosion = new Explosion(_game);
            Game.Instantiate(explosion, new Vector3(obj.transform.Position.X, obj.transform.Position.Y, 21 + _descoleCount), Vector3.One * 500f);
            
            Game.MusicManager.Play("Descole");
        }
    }
}