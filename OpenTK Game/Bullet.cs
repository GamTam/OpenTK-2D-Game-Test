using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class Bullet : GameObject
{
    public Vector2 Dir = Vector2.Zero;
    private float _moveSpeed = 700f;

    public override void Start(bool overrideTransform = false)
    {
        base.Start();
        UpdateTexture("Bullet");
    }

    public override void Update(FrameEventArgs args)
    {
        transform.Position += new Vector3(Dir.X, Dir.Y, 0) * _moveSpeed * (float) args.Time;
        
        
        transform.Position = new Vector3(transform.Position.X, transform.Position.Y, transform.Position.Y + 10);
    }
}