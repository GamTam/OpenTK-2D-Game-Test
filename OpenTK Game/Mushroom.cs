using Assimp;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using System.Media;
using NAudio.Wave;


namespace Open_TK_Tut_1;

public class Mushroom : GameObject
{
    public Vector3 Dir;
    public PointLight light;

    private Game _game;
    
    Texture background = new Texture("Mushroom.png");
    
    public Mushroom(Game game, float[] vertices=null, uint[] indices=null, Shader shader=null) : base(vertices, indices, shader)
    {
        AssimpContext importer = new AssimpContext();
        PostProcessSteps postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.GenerateUVCoords;
        Scene scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "Mushroom.fbx", postProcessSteps);
        
        Vertices = scene.Meshes[0].MergeMeshData();
        Indices = scene.Meshes[0].GetUnsignedIndices();
        Shader = new Shader("shader.vert", "Lit_shader.frag");

        Game.LitObjects.Add(this);
        transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
        transform.Position = new Vector3(0, -0.5f, 0);
        transform.Scale = new Vector3(10, 10, 10);

        _game = game;

        Start();
    }

    public override void Update(FrameEventArgs args)
    {
        float distance = (float) Math.Sqrt(Math.Pow(transform.Position.X - Game.gameCam.Position.X, 2) + 
                                           Math.Pow(transform.Position.Y + 0.5f - Game.gameCam.Position.Y, 2) +
                                           Math.Pow(transform.Position.Z - Game.gameCam.Position.Z, 2));

        transform.Scale = new Vector3(MathHelper.Lerp(10, 1, distance / 3));
        
        if (distance < 0.5f)
        {
            Game.ObjectsToDestroy.Add(this);
        }
    }

    public override void Render()
    {
        background.Use(TextureUnit.Texture0);
        int id = Shader.GetUniformLocation("tex0");
        GL.ProgramUniform1(Shader.Handle, id, 0);

        base.Render();
    }

    public override void OnDestroy()
    {
        Explosion bullet = new Explosion(_game);

        Vector3 pos = bullet.transform.Position;
        
        pos = transform.Position;
        pos.Y += 0.5f;

        bullet.transform.Position = pos;
    }
}