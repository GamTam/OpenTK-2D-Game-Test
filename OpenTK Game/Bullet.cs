using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Open_TK_Tut_1;

public class Bullet : GameObject
{
    public Vector2 Dir = Vector2.Zero;
    private float _moveSpeed = 700f;
    
    private float _timer;

    public override void Start(bool overrideTransform = false)
    {
        base.Start();
        UpdateTexture("Bullet");
    }

    public override void Update(FrameEventArgs args)
    {
        transform.Position += new Vector3(Dir.X, Dir.Y, 0) * _moveSpeed * (float) args.Time;

        _timer += (float) args.Time;
        if (_timer > 10f)
        {
            Destroy(this);
        }

        foreach (Enemy enemy in Game.Enemies)
        {
            if (IsColliding(enemy))
            {
                enemy.Hit();
                Destroy(this);
            }
        }
        
        transform.Position = new Vector3(transform.Position.X, transform.Position.Y, 250 - (transform.Position.Y + 10));
    }
}