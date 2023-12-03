using Assimp;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

namespace Open_TK_Tut_1;

public class Bullet : GameObject
{
    public Vector3 Dir;
    public PointLight light;
    
    Texture tex = new Texture("Descole.png");
    
    public Bullet(Game game, float[] vertices=null, uint[] indices=null, Shader shader=null) : base(vertices, indices, shader)
    {
        AssimpContext importer = new AssimpContext();
        PostProcessSteps postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs;
        Scene scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "Bullet.fbx", postProcessSteps);
        
        Vertices = scene.Meshes[0].MergeMeshData();
        Indices = scene.Meshes[0].GetUnsignedIndices();
        Shader = new Shader("shader.vert", "Bullet_shader.frag");
        
        tex.Use(TextureUnit.Texture0);
        int id = Shader.GetUniformLocation("tex");
        GL.ProgramUniform1(Shader.Handle, id, 2);

        Game.LitObjects.Add(this);
        transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
        transform.Position = new Vector3(0, 0, 0);
        transform.Scale = new Vector3(0.25f, 0.25f, 0.25f);

        light = new PointLight(new Vector3(0, 1, 1), 2f);
        Game.Lights.Add(light);
        
        Start();
    }

    private float _moveSpeed = 10f;
    private double _deathTime = 2;
    private double _deathTimer = 0;
    public override void Update(FrameEventArgs args)
    {
        // transform.Rotation += new Vector3(0, (float) args.Time * 3, 0);
        // transform.Rotation = new Vector3(transform.Rotation.X, transform.Rotation.Y, 0);
        //
        // Console.WriteLine(transform.Rotation);
        Vector3 newDir = new Vector3(Dir.X * (float) args.Time * _moveSpeed, Dir.Y * (float) args.Time * _moveSpeed, Dir.Z * (float) args.Time * _moveSpeed);
        transform.Position += newDir;
        light.Transform.Position = transform.Position;
        _deathTimer += args.Time;

        if (_deathTimer >= _deathTime)
        {
            Game.Lights.Remove(light);
            Game.ObjectsToDestroy.Add(this);
        }
        
        Console.WriteLine(transform.Position);
    }
}