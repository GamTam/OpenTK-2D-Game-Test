using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class GameObject
{
    public Transform transform = new Transform(); // Every gameobject has a transform

    private int vertexBufferObject;
    private int vertexArrayObject;
    private int elementBufferObject;

    protected uint[] Indices;
    public Shader Shader;
    protected float[] Vertices;
    
    public Texture _mainTex;
    protected Game _game;

    protected bool FlipX;
    protected bool FlipY;

    public float Alpha = 1;

    private bool _started;

    public GameObject(Game game=null, bool start=false)
    {
        _game = StaticUtilities.CurrentGameInstance;
        if (start) Start();
    }

    public virtual void Start(bool overrideTransform = false)
    {
        if (_started) return;
        _started = true;

        Vertices = StaticUtilities.QuadVertices;
        Indices = StaticUtilities.QuadIndices;
        Shader = new Shader("shader.vert", "shader.frag");
        
        if (!overrideTransform) {
            UpdateTexture("Descole");

            transform.Position = Game.gameCam.Position;
            transform.Position += Vector3.UnitZ * Game.UnLitObjects.Count;
        }
        
        Game.UnLitObjects.Add(this);
        
        //VBO
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
        StaticUtilities.CheckError("2");
        //VAO
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        int id = Shader.GetAttribLocation("aPosition");
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(id);

        id = 1; // shader.GetAttribLocation("aNormals");
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(id);
        StaticUtilities.CheckError("3");

        id = Shader.GetAttribLocation("UVs");
        GL.VertexAttribPointer(id, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(id);


        //EBO
        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices,
            BufferUsageHint.StaticDraw);
    }
    
    public void UpdateTexture(Texture texture)
    {
        _mainTex = texture;
        transform.Scale = _mainTex.Size;
    }

    public void UpdateTexture(string texture)
    {
        _mainTex = new Texture(texture);
        transform.Scale = _mainTex.Size;
    }

    public virtual void Render()
    {
        _mainTex.Use(TextureUnit.Texture0);
        int id = Shader.GetUniformLocation("tex0");
        GL.ProgramUniform1(Shader.Handle, id, 0);
        
        id = Shader.GetUniformLocation("alpha");
        GL.ProgramUniform1(Shader.Handle, id, Alpha);
        
        id = Shader.GetUniformLocation("flip");
        GL.ProgramUniform2(Shader.Handle, id, new Vector2(FlipX ? -1f : 1f, FlipY ? -1f : 1f));
        
        Shader.Use();

        id = Shader.GetUniformLocation("model");
        GL.UniformMatrix4(id, true, ref transform.GetMatrix);
        id = Shader.GetUniformLocation("view");
        GL.UniformMatrix4(id, true, ref Game.view);
        id = Shader.GetUniformLocation("projection");
        GL.UniformMatrix4(id, true, ref Game.projection);
        
        id = Shader.GetUniformLocation("viewPos");
        GL.Uniform3(id, Game.gameCam.Position);
        
        StaticUtilities.CheckError("render");

        GL.BindVertexArray(vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        
        GL.BindVertexArray(0);
    }

    public void Dispose(bool gameEnd=false)
    {
        if (!gameEnd) OnDestroy();
        GL.DeleteBuffer(elementBufferObject);
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);
        Shader.Dispose();
    }
    
    public virtual void OnDestroy() {}

    public virtual void Update(FrameEventArgs args)
    {}

    public static void Destroy(GameObject obj)
    {
        Game.ObjectsToDestroy.Add(obj);
    }
}