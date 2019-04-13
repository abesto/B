using UnityEngine;
using System.Collections.Generic;

class CullingMesher : Mesher
{
    static Vector3Int up = Vector3Int.up;
    static Vector3Int down = Vector3Int.down;
    static Vector3Int left = Vector3Int.left;
    static Vector3Int right = Vector3Int.right;
    static Vector3Int forward = new Vector3Int(0, 0, 1);
    static Vector3Int back = new Vector3Int(0, 0, -1);

    public Mesh GenerateMesh(Chunk chunk)
    {
        Mesh mesh = new Mesh();
        mesh.name = "culling";
        mesh.vertices = GenerateVertices(chunk);
        mesh.triangles = GenerateTriangles(mesh.vertices);
        mesh.uv = GenerateUV(mesh.vertices);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private Vector3[] GenerateVertices(Chunk chunk)
    {
        List<Vector3> vertices = new List<Vector3>();
        Vector3Int[] blocks = chunk.Blocks;
        foreach (Vector3Int block in blocks)
        {
            // Front
            if (!chunk.IsBlock(block + back))
            {
                vertices.AddRange(new Vector3[]
                {
                    block,
                    block + right,
                    block + up,
                    block + up + right
                });
            }

            // Top
            if (!chunk.IsBlock(block + up))
            {
                vertices.AddRange(new Vector3[]
                {
                    block + up,
                    block + up + right,
                    block + up + forward,
                    block + up + forward + right
                });
            }

            // Back
            if (!chunk.IsBlock(block + forward))
            {
                vertices.AddRange(new Vector3[]
                {
                    block + forward + right,
                    block + forward,
                    block + up + forward + right,
                    block + up + forward
                });
            }

            // Left
            if (!chunk.IsBlock(block + left))
            {
                vertices.AddRange(new Vector3[]
                {
                    block + forward,
                    block,
                    block + up + forward,
                    block + up
                });
            }

            // Right
            if (!chunk.IsBlock(block + right))
            {
                vertices.AddRange(new Vector3[]
                {
                    block + right,
                    block + right + forward,
                    block + up + right,
                    block + up + right + forward
                });
            }

            // Bottom
            if (!chunk.IsBlock(block + down))
            {
                vertices.AddRange(new Vector3[]
                {
                    block + right,
                    block,
                    block + forward + right,
                    block + forward
                });
            }
        }

        return vertices.ToArray();
    }

    private Vector2[] GenerateUV(Vector3[] vertices)
    {
        Vector2[] uv = new Vector2[vertices.Length];
        int quadCount = vertices.Length / 4;
        for (int quad = 0; quad < quadCount; quad += 1)
        {
            uv[4 * quad] = new Vector2(0, 0);
            uv[4 * quad + 1] = new Vector2(1, 0);
            uv[4 * quad + 2] = new Vector2(0, 1);
            uv[4 * quad + 3] = new Vector2(1, 1);
        }
        return uv;
    }

    private int[] GenerateTriangles(Vector3[] vertices)
    {
        int quadCount = vertices.Length / 4;
        int[] triangles = new int[quadCount * 2 * 3];

        for (int quad = 0; quad < quadCount; quad += 1)
        {
            triangles[quad * 6] = quad * 4;
            triangles[quad * 6 + 1] = quad * 4 + 2;
            triangles[quad * 6 + 2] = quad * 4 + 1;

            triangles[quad * 6 + 3] = quad * 4 + 2;
            triangles[quad * 6 + 4] = quad * 4 + 3;
            triangles[quad * 6 + 5] = quad * 4 + 1;
        }

        return triangles;
    }
}
