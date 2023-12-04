using Assimp;
using NAudio.Wave;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Open_TK_Tut_1;

public class Game : GameWindow
{

    public static readonly List<GameObject> LitObjects = new();
    public static List<GameObject> ObjectsToDestroy = new();
    public static List<GameObject> ObjectsToAdd = new();
    public static List<GameObject> UnLitObjects = new();
    public static List<PointLight> Lights = new();

    public static Vector2 WindowSize;
    public static Matrix4 view;
    public static Matrix4 projection;

    public static Camera gameCam;
    private Vector2 previousMousePos;

    private MusicManager _musicManager = new MusicManager();

    private readonly string[] PointLightDefinition = new[]
    {
        "pointLights[",
        "INDEX",
        "]."
    };

    public Game(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings() { Title = title, Size = (width, height) })
    {
        WindowSize = new Vector2(width, height);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0, 0, 0, 0); 

        //Enable blending
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        
        gameCam = new Camera(new Vector3(), (float)WindowSize.X / WindowSize.Y);
        gameCam.Position = new Vector3(WindowSize.X / 2f, WindowSize.Y / 2f, 0);
        gameCam.Yaw = -90;
        gameCam.Pitch = 0;
        
        GameObject bg = new GameObject(this);
        bg.UpdateTexture("Layton's Office");

        Player player = new Player(this);
        player.transform.Position = gameCam.Position;
        player.transform.Position += Vector3.UnitZ * 3;
        player.transform.Position =
            new Vector3(player.transform.Position.X, player._mainTex.Size.Y, player.transform.Position.Z);

        StaticUtilities.CheckError("B");
        
        _musicManager.Play("Keera");

        foreach (GameObject tempObj in LitObjects)
        {
            tempObj.Start();
        }
    }

    protected override void OnUnload()
    {
        //Free GPU RAM
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        
        GL.UseProgram(0);
        
        for (int i = UnLitObjects.Count-1; i >= 0; --i)
        {
            UnLitObjects[i].Dispose(true);
        }
        for (int i = LitObjects.Count-1; i >= 0; --i)
        {
            LitObjects[i].Dispose(true);    
        }

        base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        //MUST BE FIRST
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        view = gameCam.GetViewMatrix();
        projection = gameCam.GetProjectionMatrix();

        StaticUtilities.CheckError("B");
        
        UnLitObjects = UnLitObjects.OrderBy(o=>o.transform.Position.Z).ToList();
        
        foreach (GameObject unlit in UnLitObjects)
        {
            unlit.Render();
        }
        
        //MUST BE LAST
        SwapBuffers();
    }

    private float n;
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _musicManager.Update();

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        const float cameraSpeed = 1.5f;

        if (KeyboardState.IsKeyDown(Keys.F))
        {
            Title = "Descole: The Video Game";
            
            GameObject descole = new GameObject(this);
            descole.UpdateTexture("Descole DS");
            descole.transform.Position = new Vector3(descole.transform.Position.X, descole._mainTex.Size.Y, 1);

            Explosion explosion = new Explosion(this);
            explosion.transform.Position = new Vector3(descole.transform.Position.X, descole.transform.Position.Y, 2);
            explosion.transform.Scale = Vector3.One * 500f;
            
            _musicManager.Play("Descole");
        }

        foreach (GameObject obj in UnLitObjects)
        {
            obj.Update(args);
        }

        foreach (GameObject obj in ObjectsToDestroy)
        {
            UnLitObjects.Remove(obj);
            obj.Dispose();
        }

        ObjectsToDestroy = new();
    }
    

protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        gameCam.Fov -= e.OffsetY;
    }
    
    
   
}