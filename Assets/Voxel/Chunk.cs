using UnityEngine;
using System.Collections.Generic;


public class Chunk : BlockContainer
{
    public const int HEIGHT = 512;
    public const int WIDTH = 32;

    private const int AREA = WIDTH * WIDTH;

    private readonly bool[] data = new bool[HEIGHT * WIDTH * WIDTH];

    private int VectorToIndex(Vector3Int vector)
    {
        return vector.y * AREA + vector.z * WIDTH + vector.x;
    }

    public bool IsBlock(Vector3Int vector)
    {
        int index = VectorToIndex(vector);
        if (index < 0 || index >= data.Length)
        {
            return false;
        }
        return data[VectorToIndex(vector)];
    }

    public void SetBlock(Vector3Int vector, bool value)
    {
        int index = VectorToIndex(vector);
        if (index >= 0 && index < data.Length)
        {
            data[VectorToIndex(vector)] = value;
        }
    }

    public Vector3Int[] Blocks
    {
        get
        {
            List<Vector3Int> blocks = new List<Vector3Int>();
            Vector3Int pos = new Vector3Int();
            for (pos.x = 0; pos.x < WIDTH; pos.x += 1)
            {
                for (pos.z = 0; pos.z < WIDTH; pos.z += 1)
                {
                    for (pos.y = 0; pos.y < HEIGHT; pos.y += 1)
                    {
                        if (IsBlock(pos))
                        {
                            blocks.Add(new Vector3Int(pos.x, pos.y, pos.z));
                        }
                    }
                }
            }
            return blocks.ToArray();
        }
    }
}
