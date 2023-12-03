using Assimp;
using NAudio.Wave;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Open_TK_Tut_1;

public class Game : GameWindow
{

    private float x;

    private Shader shader;
    private Shader litShader;
    
    
    private Texture tex0;
    private Texture tex1;
    private Texture tex2;

    public static readonly List<GameObject> LitObjects = new();
    public static List<GameObject> ObjectsToDestroy = new();
    public static List<GameObject> ObjectsToAdd = new();
    public static readonly List<GameObject> UnLitObjects = new();
    public static readonly List<PointLight> Lights = new();

    public static Vector2 WindowSize;
    public static Matrix4 view;
    public static Matrix4 projection;

    public static Camera gameCam;
    private Vector2 previousMousePos;
    
    private WaveOutEvent _waveOut = new WaveOutEvent();
    private AudioFileReader audioFile;

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

        previousMousePos = new Vector2(MouseState.X, MouseState.Y);
        CursorState = CursorState.Grabbed;

        //Enable blending
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        
        shader = new Shader("shader.vert", "shader.frag");
        litShader = new Shader("shader.vert", "Lit_shader.frag");
        
        tex0 = new Texture("container.jpg");
        tex1 = new Texture("Bender.png");
        tex2 = new Texture("Descole.png");
        
        gameCam = new Camera(Vector3.UnitZ * 3, (float)Size.X / Size.Y);
        
        UnLitObjects.Add(new GameObject(StaticUtilities.QuadVertices, StaticUtilities.QuadIndices, new Shader("shader.vert", "WaveTutorial\\Waves.frag")));
        UnLitObjects[0].Start();
        UnLitObjects[0].transform.Position = Vector3.UnitX * -9;
        
        LitObjects.Add(new GameObject(StaticUtilities.BoxVertices, StaticUtilities.BoxIndices, litShader));
        LitObjects[0].Start();

        AssimpContext importer = new AssimpContext();
        PostProcessSteps postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs;
        Scene scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "Bomb.fbx", postProcessSteps);
        LitObjects.Add(new GameObject(scene.Meshes[0].MergeMeshData(), scene.Meshes[0].GetUnsignedIndices(), litShader));
        LitObjects[0].transform.Position = new Vector3(-2, 0, 0);
        LitObjects[1].Start();
        LitObjects[1].transform.Position = new Vector3(2, 0, 0);
        LitObjects[1].transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
        
        scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "plane.fbx", postProcessSteps);
        LitObjects.Add(new GameObject(scene.Meshes[0].MergeMeshData(), scene.Meshes[0].GetUnsignedIndices(), 
            new Shader("WaveTutorial\\Waves.vert", "Lit_shader.frag")));
        LitObjects[2].Start();
        LitObjects[2].transform.Position = Vector3.UnitX * -6;

        new Mushroom(this);
        
        StaticUtilities.CheckError("B");


        Lights.Add(new PointLight(new Vector3(1,0,0.5f), 1));
        Lights[0].Transform.Position = Vector3.UnitX + Vector3.UnitY * 6 + Vector3.UnitZ * 3;
        Lights.Add(new PointLight(new Vector3(0.25f,0,1), 2));
        Lights[1].Transform.Position = Vector3.UnitX + Vector3.UnitY * -6 + Vector3.UnitZ * 3;
        //light.Transform.Position
        StaticUtilities.CheckError("C");

        shader.Use();

        tex0.Use(TextureUnit.Texture0);
        int id = shader.GetUniformLocation("tex0");
        GL.ProgramUniform1(shader.Handle, id, 0);
        
        tex1.Use(TextureUnit.Texture1);
        id = shader.GetUniformLocation("tex1");
        GL.ProgramUniform1(shader.Handle, id, 1);

        litShader.Use(); 
        tex0.Use(TextureUnit.Texture0);
        id = litShader.GetUniformLocation("tex0");
        GL.ProgramUniform1(litShader.Handle, id, 0);
        
        tex1.Use(TextureUnit.Texture1);
        id = litShader.GetUniformLocation("tex1");
        GL.ProgramUniform1(litShader.Handle, id, 1);
        
        string audioFilePath = StaticUtilities.MusicDirectory + "Descole.wav";
        audioFile = new AudioFileReader(audioFilePath);
        _waveOut.Init(audioFile);
        _waveOut.Volume = 0.5f;
        _waveOut.Play();
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

        shader.Dispose();

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
        
        tex0.Use(TextureUnit.Texture0);
        tex1.Use(TextureUnit.Texture1);
        tex2.Use(TextureUnit.Texture2);
        
        shader.Use();
        int getCpuColor = shader.GetUniformLocation("uniformColor");
        x += (float) args.Time;
        
        GL.Uniform4(getCpuColor, new Color4((float) x, 0f, 0f, 1.0f));
        
        UnLitObjects[0].MyShader.Use();
        int idx = UnLitObjects[0].MyShader.GetUniformLocation("time");
        GL.Uniform1(idx, x);
        
        LitObjects[2].MyShader.Use();
        idx = LitObjects[2].MyShader.GetUniformLocation("time");
        GL.Uniform1(idx, x);

        StaticUtilities.CheckError("B");
        
        foreach (GameObject unlit in UnLitObjects)
        {
            unlit.Render();
        }
        foreach (GameObject lit in LitObjects)
        {
            int id;
            lit.MyShader.Use();
            
            for (int i = 0; i < Lights.Count; i++)
            {
                PointLight currentLight = Lights[i];
                PointLightDefinition[1] = i.ToString();
                string merged = string.Concat(PointLightDefinition);
                
                id = lit.MyShader.GetUniformLocation(merged + "lightColor");
                GL.Uniform3(id, currentLight.Color);
                
                StaticUtilities.CheckError("A");
                
                id = lit.MyShader.GetUniformLocation(merged + "lightPosition");
                GL.Uniform3(id, currentLight.Transform.Position);
                
                StaticUtilities.CheckError("B");
                
                id = lit.MyShader.GetUniformLocation(merged + "lightIntensity");
                GL.Uniform1(id, currentLight.Intensity);
                
                StaticUtilities.CheckError("C");
            }
            
            id = lit.MyShader.GetUniformLocation("pointLightCount");
            GL.Uniform1(id, Lights.Count);
            
            lit.Render();
        }
        
        //MUST BE LAST
        SwapBuffers();
    }

    private float n;
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        // Console.WriteLine($"{audioFile.CurrentTime.TotalSeconds}, {80.590078}");
        if (audioFile.Position >= audioFile.WaveFormat.AverageBytesPerSecond * 155.504833)
        {
            audioFile.Position -= (long) ((155.504833 - 1.547445) * audioFile.WaveFormat.AverageBytesPerSecond);
            Console.WriteLine($"Looped {audioFile.FileName}");
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        n += (float)args.Time;
        // UnLitObjects[0].transform.Rotation = new Vector3(0,0,UnLitObjects[0].transform.Rotation.Z + (float)args.Time);
        // UnLitObjects[0].transform.Scale = new Vector3(1, (float)Math.Sin(n) * 5, 1);

        Lights[0].Transform.Position = Vector3.UnitY * MathF.Sin(n) * 5 + Vector3.UnitZ * 5;
        Lights[0].Transform.Position = Quaternion.FromAxisAngle(Vector3.UnitY, n) * Lights[0].Transform.Position;
        
        
        const float cameraSpeed = 1.5f;
        const float sensitivity = 0.2f;

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            gameCam.Position += gameCam.Forward * cameraSpeed * (float)args.Time; // Forward
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            gameCam.Position -= gameCam.Forward * cameraSpeed * (float)args.Time; // Backwards
        }

        if (KeyboardState.IsKeyDown(Keys.A))
        {
            gameCam.Position -= gameCam.Right * cameraSpeed * (float)args.Time; // Left
        }

        if (KeyboardState.IsKeyDown(Keys.D))
        {
            gameCam.Position += gameCam.Right * cameraSpeed * (float)args.Time; // Right
        }

        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            gameCam.Position += gameCam.Up * cameraSpeed * (float)args.Time; // Up
        }

        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            gameCam.Position -= gameCam.Up * cameraSpeed * (float)args.Time; // Down
        }

        if (MouseState.IsButtonPressed(MouseButton.Button1))
        {
            Console.WriteLine("Pew!");
            Bullet bullet = new Bullet(this);
            bullet.transform.Position = gameCam.Position + (gameCam.Forward * 0.25f);
            bullet.Dir = gameCam.Forward;

            float pitch = MathHelper.DegreesToRadians(gameCam.Pitch);
            float yaw = MathHelper.DegreesToRadians(gameCam.Yaw);
            
            bullet.transform.Rotation = new Vector3(yaw, pitch, 0);
            Console.WriteLine(gameCam.Forward);
        }

        // Get the mouse state

        // Calculate the offset of the mouse position
        var deltaX = MouseState.X - previousMousePos.X;
        var deltaY = MouseState.Y - previousMousePos.Y;
        previousMousePos = new Vector2(MouseState.X, MouseState.Y);

        // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
        gameCam.Yaw += deltaX * sensitivity;
        gameCam.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top

        foreach (GameObject obj in LitObjects)
        {
            obj.Update(args);
        }

        foreach (GameObject obj in ObjectsToDestroy)
        {
            LitObjects.Remove(obj);
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