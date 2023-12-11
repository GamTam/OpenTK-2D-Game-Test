using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK_Tut_1;

public class Background : GameObject
{
    public override void Start(bool overrideTransform = false)
    {
        base.Start(false);
        transform.Position = new Vector3(transform.Position.X, transform.Position.Y, -50000);
    }
}