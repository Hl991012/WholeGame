


using UnityEngine;

public struct MapData
{
    public int curLevel;
    public int w;
    public int h;
    public MapPosData[] L;
}
public struct MapPosData
{
    public int x;
    public int y;


    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}
