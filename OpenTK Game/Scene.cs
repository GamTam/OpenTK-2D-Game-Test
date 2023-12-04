using OpenTK.Mathematics;

namespace Open_TK_Tut_1;

public class Scene
{
    public SceneObj[] ObjectsToSpawn;
    public string SongToPlay;
}

public struct SceneObj
{
    public GameObject Object;
    public Vector2 ObjPos;
    public Vector2 ObjRot;
    public Vector2 ObjScale;
    public string ObjTexture;
}