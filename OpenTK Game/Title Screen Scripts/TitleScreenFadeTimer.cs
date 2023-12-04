using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK_Tut_1;

public class TitleScreenFadeTimer : GameObject
{
    private float _titleTime = 4.438f;
    private float _subTime = 13.337f;
    private float _enterTime = 22.236f;
    
    private float _timer = 0f;

    private bool _spawnedLogo;
    private bool _spawnedSubLogo;
    private bool _spawnedEnter;

    private FadeIn _title;
    private FadeIn _subTitle;
    private FlashAlpha _flash;

    private FadeOut _fadeOut;

    private bool _fadingOut = false;
    
    public TitleScreenFadeTimer()
    {
        Alpha = 0f;
    }

    public override void Start(bool overrideTransform = false)
    {
        base.Start(false);
        _fadeOut = new FadeOut() {FadeInSpeed = 2.5f};
        GameObject obj = Game.Instantiate(_fadeOut, new Vector2(512, 384));
        obj.UpdateTexture("Black Screen");
        obj.transform.Position += Vector3.UnitZ * 4;
    }

    public override void Update(FrameEventArgs args)
    {
        _game = StaticUtilities.CurrentGameInstance;
        _timer += (float) args.Time;
        
        if (_game.KeyboardState.IsKeyPressed(Keys.Enter) && !_fadingOut)
        {
            _fadingOut = true;

            _timer = 500000f;
            Game.SoundManager.Play("startGame");
            Game.MusicManager.Stop();

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
            
            if (_fadingOut) _title._fadeInTimer = 100000f;
        }
        
        if (_timer > _subTime && !_spawnedSubLogo)
        {
            _spawnedSubLogo = true;
            _subTitle = new FadeIn(_game);
            GameObject obj = Game.Instantiate(_subTitle, new Vector3(512, 580, 20), new Vector3(247, 47, 20));
            obj._mainTex = new Texture("SubLogo");
            
            if (_fadingOut) _subTitle._fadeInTimer = 100000f;
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
        }
        
        if (_game.KeyboardState.IsKeyPressed(Keys.F) && !_fadingOut)
        {
            _game.Title = "Descole: The Video Game";
            
            TitleScreenDescole descole = new TitleScreenDescole(_game);
            GameObject obj = Game.Instantiate(descole, new Vector2(Game.gameCam.Position.X, descole._mainTex.Size.Y));

            Explosion explosion = new Explosion(_game);
            Game.Instantiate(explosion, new Vector3(obj.transform.Position.X, obj.transform.Position.Y, obj.transform.Position.Z + 1), Vector3.One * 500f);
            
            Game.MusicManager.Play("Descole");
        }
    }
}