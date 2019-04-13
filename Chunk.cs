using UnityEngine;


public class Chunk
{
    private const int HEIGHT = 512;
    private const int WIDTH = 32;

    private const int AREA = WIDTH * WIDTH;

    private readonly bool[] data = new bool[HEIGHT * WIDTH * WIDTH];

    private int VectorToIndex(Vector3Int vector)
    {
        return vector.y * AREA + vector.z * WIDTH + vector.x;
    }

    public bool IsBlock(Vector3Int vector)
    {
        return data[VectorToIndex(vector)];
    }

    public void SetBlock(Vector3Int vector, bool value)
    {
        data[VectorToIndex(vector)] = value;
    }
}
