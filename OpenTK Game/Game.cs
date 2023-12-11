using System.Collections;
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

    public static MusicManager MusicManager = new MusicManager();
    public static SoundManager SoundManager = new SoundManager();

    public static Texture DefaultTex;
    
    public static readonly Scene TitleScreen = new Scene()
    {
        ObjectsToSpawn = new[]
        {
            new SceneObj()
            {
                Object = new GameObject(),
                ObjPos = new Vector2(512, 384),
                ObjTexture = "Title Screen"
            },
            
            new SceneObj()
            {
                Object = new GameObject(),
                ObjPos = new Vector2(512, 384),
                ObjTexture = "Title Screen_fg"
            },
            
            new SceneObj()
            {
                Object = new FadeIn()
                {
                    FadeInSpeed = 3f
                },
                ObjPos = new Vector2(512, 384),
                ObjTexture = "Title Gradient"
            },
            
            new SceneObj()
            {
                Object = new TitleScreenFadeTimer()
            }
        },
        
        SongToPlay = "Layton"
    };

    public static readonly Scene GameScene = new Scene()
    {
        ObjectsToSpawn = new []
        {
            new SceneObj()
            {
                Object = new Background(),
                ObjPos = new Vector2(512, 384),
                ObjTexture = "StartingRoom"
            },
            
            new SceneObj()
            {
                Object = new Player(),
                ObjPos = new Vector2(512, 384)
            },
            
            new SceneObj()
            {
                Object = new FadeOut() {FadeInSpeed = 2.5f},
                ObjPos = new Vector2(512, 384),
                ObjTexture = "Black Screen",
                Depth = 5000f
            }
        },
        
        SongToPlay = "Lobby_JFA"
    };
    
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

        if (StaticUtilities.CurrentGameInstance == null) StaticUtilities.CurrentGameInstance = this;
        else return;

        GL.ClearColor(0, 0, 0, 0); 

        //Enable blending
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        
        gameCam = new Camera(new Vector3(), (float) WindowSize.X / WindowSize.Y);
        gameCam.Position = new Vector3(WindowSize.X / 2f, WindowSize.Y / 2f, 0);
        gameCam.Yaw = -90;
        gameCam.Pitch = 0;

        DefaultTex = new Texture("Descole");
        
        //LoadScene(TitleScreen);
        LoadScene(GameScene);

        StaticUtilities.CheckError("B");

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
        
        UnLitObjects = UnLitObjects.OrderBy(o=>o.transform.Position.X).ToList();
        UnLitObjects = UnLitObjects.OrderBy(o=>-o.transform.Position.Y).ToList();
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

        MusicManager.Update();

        foreach (GameObject obj in ObjectsToAdd)
        {
            UnLitObjects.Add(obj);
        }

        ObjectsToAdd = new List<GameObject>();

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        const float cameraSpeed = 1.5f;

        foreach (GameObject obj in UnLitObjects)
        {
            obj.Update(args);
            obj.UpdateHitBox();
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

    public static void Unload()
    {
        foreach (GameObject obj in UnLitObjects)
        {
            obj.Dispose();
        }

        UnLitObjects = new List<GameObject>();
        
        gameCam.Position = new Vector3(WindowSize.X / 2f, WindowSize.Y / 2f, 0);
    }

    public static void LoadScene(Scene scene)
    {
        Unload();
        
        int i = 0;
        foreach (SceneObj obj in scene.ObjectsToSpawn)
        {
            GameObject tempObject = Instantiate(obj.Object);
            if (!string.IsNullOrEmpty(obj.ObjTexture)) tempObject.UpdateTexture(obj.ObjTexture);
            if (obj.ObjScale != Vector2.Zero) tempObject.transform.Scale = new Vector3(obj.ObjScale.X, obj.ObjScale.Y, 1);
            if (obj.ObjPos != Vector2.Zero) tempObject.transform.Position = new Vector3(obj.ObjPos.X, obj.ObjPos.Y, 0);
            if (obj.ObjRot != Vector2.Zero) tempObject.transform.Rotation = new Vector3(obj.ObjRot.X, obj.ObjRot.Y, 0);
            tempObject.transform.Position += obj.Depth == 0 ? Vector3.UnitZ * i : Vector3.UnitZ * obj.Depth;
            i++;
        }
        
        if (!string.IsNullOrEmpty(scene.SongToPlay)) MusicManager.Play(scene.SongToPlay);
    }

    public static GameObject Instantiate(GameObject obj)
    {
        obj.Start();

        return obj;
    }

    public static GameObject Instantiate(GameObject obj, Vector2 pos, Vector2? scale = null,
        Vector2? rot = null)
    {
        obj.Start();

        Vector2 realPos;
        Vector2 realRot;
        Vector2 realScale;

        realPos = pos;
        
        if (rot == null) realRot = Vector2.Zero;
        else realRot = (Vector2) rot;
        
        if (scale == null) realScale = new Vector2(obj._mainTex.Size.X, obj._mainTex.Size.Y);
        else realScale = (Vector2) scale;
        
        obj.transform.Position = new Vector3(realPos.X, realPos.Y, UnLitObjects.Count);
        obj.transform.Rotation = new Vector3(realRot.X, realRot.Y, 0);
        obj.transform.Scale = new Vector3(realScale.X, realScale.Y, 1);

        return obj;
    }

    public static GameObject Instantiate(GameObject obj, Vector3 pos, Vector3? scale = null, Vector3? rot = null)
    {
        obj.Start();
        
        if (rot == null) rot = Vector3.Zero;
        if (scale == null) scale = obj._mainTex.Size;
        
        obj.RealPos = (Vector3) pos;
        obj.transform.Position = (Vector3) pos;
        obj.transform.Rotation = (Vector3) rot;
        obj.transform.Scale = (Vector3) scale;

        return obj;
    }
   
}