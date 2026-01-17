using UnityEngine;

public class Corridor
{
    public Vector2Int start;
    public Vector2Int end;
    public int width;

    public Corridor(Vector2Int start, Vector2Int end, int width = 2)
    {
        this.start = start;
        this.end = end;
        this.width = width;
    }
}
