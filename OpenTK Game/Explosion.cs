using Assimp;
using NAudio.Wave;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

namespace Open_TK_Tut_1;

public class Explosion : GameObject
{
    public Game _game;

    private float _animTime = 0.1f;
    private float _animTimer;

    private int _imgCount = 16;
    private int _currentImg = 0;
    
    private WaveOutEvent _waveOut = new WaveOutEvent();
    
    Texture boom = new Texture("Explosion/explosion_0.png");
    
    public Explosion(Game game, float[] vertices=null, uint[] indices=null, Shader shader=null) : base(vertices, indices, shader)
    {
        Vertices = StaticUtilities.QuadVertices;
        Indices = StaticUtilities.QuadIndices;
        MyShader = new Shader("shader.vert", "shader.frag");

        Game.LitObjects.Add(this);
        transform.Position = new Vector3(0, 0, 0);
        transform.Scale = new Vector3(1, 1, 1);

        _game = game;
        
        string audioFilePath = StaticUtilities.SoundDirectory + "explosion.wav";
        AudioFileReader audioFile = new AudioFileReader(audioFilePath);
        _waveOut.Init(audioFile);
        _waveOut.Play();

        Start();
    }

    public override void Update(FrameEventArgs args)
    {
        _animTimer += (float) args.Time;

        if (_animTimer >= _animTime)
        {
            _currentImg += 1;

            if (_currentImg < _imgCount)
            {
                boom = new Texture($"Explosion/explosion_{_currentImg}.png");
                _animTimer = 0f;
            }
            else
            {
                Destroy(this);
            }
        }
    }
    
    public override void Render()
    {
        boom.Use(TextureUnit.Texture0);
        int id = MyShader.GetUniformLocation("tex0");
        GL.ProgramUniform1(MyShader.Handle, id, 0);

        base.Render();
    }
}