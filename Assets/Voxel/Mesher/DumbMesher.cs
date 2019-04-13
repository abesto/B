using UnityEngine;
using System.Collections.Generic;

class DumbMesher : Mesher
{
    static Vector3Int up = Vector3Int.up;
    static Vector3Int down = Vector3Int.down;
    static Vector3Int left = Vector3Int.left;
    static Vector3Int right = Vector3Int.right;
    static Vector3Int forward = new Vector3Int(0, 0, 1);

    public Mesh GenerateMesh(Chunk chunk)
    {
        Mesh mesh = new Mesh();
        mesh.name = "dumb";
        mesh.vertices = GenerateVertices(chunk);
        mesh.triangles = GenerateTriangles(mesh.vertices);
        mesh.uv = GenerateUV(mesh.vertices);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private Vector3[] GenerateVertices(Chunk chunk)
    {
        Vector3Int[] blocks = chunk.Blocks;
        Vector3[] vertices = new Vector3[blocks.Length * 24];
        for (int i = 0; i < blocks.Length; i += 1)
        {
            int offset = i * 24;
            // Front
            vertices[offset] = blocks[i];
            vertices[offset + 1] = blocks[i] + right;
            vertices[offset + 2] = blocks[i] + up;
            vertices[offset + 3] = blocks[i] + up + right;

            // Top
            vertices[offset + 4] = blocks[i] + up;
            vertices[offset + 5] = blocks[i] + up + right;
            vertices[offset + 6] = blocks[i] + up + forward;
            vertices[offset + 7] = blocks[i] + up + forward + right;

            // Back
            vertices[offset + 8] = blocks[i] + forward + right;
            vertices[offset + 9] = blocks[i] + forward;
            vertices[offset + 10] = blocks[i] + up + forward + right;
            vertices[offset + 11] = blocks[i] + up + forward;

            // Left
            vertices[offset + 12] = blocks[i] + forward;
            vertices[offset + 13] = blocks[i];
            vertices[offset + 14] = blocks[i] + up + forward;
            vertices[offset + 15] = blocks[i] + up;

            // Right
            vertices[offset + 16] = blocks[i] + right;
            vertices[offset + 17] = blocks[i] + right + forward;
            vertices[offset + 18] = blocks[i] + up + right;
            vertices[offset + 19] = blocks[i] + up + right + forward;

            // Bottom
            vertices[offset + 20] = blocks[i];
            vertices[offset + 21] = blocks[i] + right;
            vertices[offset + 22] = blocks[i] + forward;
            vertices[offset + 23] = blocks[i] + forward + right;
        }
        return vertices;
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
