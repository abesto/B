using UnityEngine;
using System.Collections.Generic;


public class World : BlockContainer
{
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    public Dictionary<Vector2Int, Chunk> Chunks { get => chunks; set => chunks = value; }

    public Chunk GetChunk(Vector2Int chunkIndex)
    {
        if (!Chunks.ContainsKey(chunkIndex))
        {
            Chunks.Add(chunkIndex, new Chunk());
        }
        return Chunks[chunkIndex];
    }

    public Chunk GetChunk(Vector3Int position)
    {
        return GetChunk(new Vector2Int(position.x, position.z));
    }

    public static Vector2Int WorldPositionToChunkIndex(Vector2Int position)
    {
        return new Vector2Int(
            position.x >= 0 ? position.x / Chunk.WIDTH : -Mathf.CeilToInt(((float)-position.x) / Chunk.WIDTH),
            position.y >= 0 ? position.y / Chunk.WIDTH : -Mathf.CeilToInt(((float)-position.y) / Chunk.WIDTH)
        );
    }

    public static Vector2Int WorldPositionToChunkIndex(Vector3Int position)
    {
        return WorldPositionToChunkIndex(new Vector2Int(position.x, position.z));
    }

    public static Vector3Int WorldPositionToChunkPosition(Vector3Int position)
    {
        return new Vector3Int(
            ((position.x % Chunk.WIDTH) + Chunk.WIDTH) % Chunk.WIDTH,
            position.y,
            ((position.z % Chunk.WIDTH) + Chunk.WIDTH) % Chunk.WIDTH
        );
    }

    public bool IsBlock(Vector3Int position)
    {
        return GetChunk(WorldPositionToChunkIndex(position)).IsBlock(WorldPositionToChunkPosition(position));
    }

    public void SetBlock(Vector3Int position, bool value)
    {
        GetChunk(WorldPositionToChunkIndex(position)).SetBlock(WorldPositionToChunkPosition(position), value);
    }
}
