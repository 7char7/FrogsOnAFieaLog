using UnityEngine;

public class Room
{
    public Vector2Int position;
    public Vector2Int size;
    public int depth;

    public int Left => position.x;
    public int Right => position.x + size.x;
    public int Bottom => position.y;
    public int Top => position.y + size.y;
    public Vector2Int Center => new Vector2Int(position.x + size.x / 2, position.y + size.y / 2);

    public Room(Vector2Int position, Vector2Int size)
    {
        this.position = position;
        this.size = size;
        this.depth = 0;
    }

    public bool Overlaps(Room other, int padding = 0)
    {
        return Left < other.Right + padding &&
               Right > other.Left - padding &&
               Bottom < other.Top + padding &&
               Top > other.Bottom - padding;
    }

    public bool Contains(Vector2Int point)
    {
        return point.x >= Left && point.x < Right &&
               point.y >= Bottom && point.y < Top;
    }
}
