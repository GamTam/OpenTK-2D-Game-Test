using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class GameObject
{
    public Transform transform = new Transform(); // Every gameobject has a transform

    private int vertexBufferObject;
    private int vertexArrayObject;
    private int elementBufferObject;

    protected uint[] Indices;
    public Shader MyShader;
    protected float[] Vertices;

    public GameObject(float [] vertices, uint[] indices, Shader shader)
    {
        Indices = indices; 
        MyShader = shader;
        Vertices = vertices;
        StaticUtilities.CheckError("1");
    }

    public void Start()
    {
        //VBO
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
        StaticUtilities.CheckError("2");
        //VAO
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        int id = MyShader.GetAttribLocation("aPosition");
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(id);

        id = 1; // shader.GetAttribLocation("aNormals");
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(id);
        StaticUtilities.CheckError("3");

        id = MyShader.GetAttribLocation("UVs");
        GL.VertexAttribPointer(id, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(id);


        //EBO
        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices,
            BufferUsageHint.StaticDraw);
    }

    public virtual void Render()
    {
        MyShader.Use();
        
        int id = MyShader.GetUniformLocation("model");
        GL.UniformMatrix4(id, true, ref transform.GetMatrix);
        id = MyShader.GetUniformLocation("view");
        GL.UniformMatrix4(id, true, ref Game.view);
        id = MyShader.GetUniformLocation("projection");
        GL.UniformMatrix4(id, true, ref Game.projection);
        
        id = MyShader.GetUniformLocation("viewPos");
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
    }
    
    public virtual void OnDestroy() {}

    public virtual void Update(FrameEventArgs args)
    {}

    public static void Destroy(GameObject obj)
    {
        Game.ObjectsToDestroy.Add(obj);
    }
}