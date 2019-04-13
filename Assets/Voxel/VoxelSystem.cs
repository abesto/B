using UnityEngine;
using System.Collections.Generic;
using System;


[ExecuteAlways]
public class VoxelSystem : MonoBehaviour
{
    public Material material;
    public Camera camera;
    public World world;

    public GameObject highlighter;
    public GameObject neighborHighlighter;

    private Mesher mesher = new CullingMesher();
    private Vector3Int cursor = new Vector3Int();
    private Vector3Int cursorNeighbor = new Vector3Int();

    [ContextMenu("Update mesh")]
    void Construct()
    {
        world = new World();
        GeneratePyramids(world);

        foreach (Vector2Int chunkIndex in world.Chunks.Keys)
        {
            UpdateMesh(chunkIndex);
        }
    }

    void UpdateMesh(Vector2Int chunkIndex)
    {
        Mesh mesh = mesher.GenerateMesh(world.GetChunk(chunkIndex));
        GameObject obj = GetChunkGameObject(chunkIndex);
        obj.GetComponent<MeshFilter>().mesh = mesh;
        obj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void GeneratePyramids(World world)
    {
        for (int offset = 0; offset < 4 * 32; offset += 32)
        {
            for (int y = 0; y < 16; y += 1)
            {
                for (int x = y; x < 32 - y; x += 1)
                {
                    for (int z = y; z < 32 - y; z += 1)
                    {
                        world.SetBlock(new Vector3Int(offset + x, y + 128, z), true);
                    }
                }
            }
        }
    }

    private GameObject GetChunkGameObject(Vector2Int chunkIndex)
    {
        string name = "Chunk " + chunkIndex.ToString();
        Transform childTransform = transform.Find(name);
        if (childTransform == null)
        {
            GameObject obj = new GameObject(name);
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>().sharedMaterial = material;
            obj.AddComponent<MeshCollider>();
            obj.transform.parent = transform;
            obj.transform.Translate(new Vector3(Chunk.WIDTH * chunkIndex.x, 0, Chunk.WIDTH * chunkIndex.y));
            AddChunkBorderLines(chunkIndex, obj);
            return obj;
        } else {
            return childTransform.gameObject; 
        }
    }

    private void AddChunkBorderLines(Vector2Int chunkIndex, GameObject chunkObj)
    {
        AddLine(chunkIndex, new Vector2Int(0, 0), chunkObj);
        AddLine(chunkIndex, new Vector2Int(0, 1), chunkObj);
        AddLine(chunkIndex, new Vector2Int(1, 0), chunkObj);
        AddLine(chunkIndex, new Vector2Int(1, 1), chunkObj);
    }

    private void AddLine(Vector2Int chunkIndex, Vector2Int offset, GameObject chunkObj)
    {
        Vector3Int start = (
            Vector3Int.right * chunkIndex.x + 
            new Vector3Int(0, 0, 1) * chunkIndex.y 
            + new Vector3Int(offset.x, 0, offset.y)
        ) * Chunk.WIDTH;
        Vector3Int end = start + Vector3Int.up * Chunk.HEIGHT;

        GameObject border = new GameObject("Border " + offset);
        LineRenderer lr = border.AddComponent<LineRenderer>();
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.transform.parent = chunkObj.transform;

    }

    void Start()
    {
        Construct();
    }

    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            return;
        }

        Highlight();

        if (Input.GetMouseButtonDown(0))
        {
            world.SetBlock(cursor, false);
            UpdateMesh(World.WorldPositionToChunkIndex(cursor));
            Highlight();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            world.SetBlock(cursorNeighbor, true);
            UpdateMesh(World.WorldPositionToChunkIndex(cursorNeighbor));
            Highlight();
        }

    }

    float FractionalPart(float f)
    {
        float diff = Mathf.Abs(f - Mathf.Floor(f));
        return Mathf.Min(diff, 1 - diff);
    }

    void Highlight()
    {
        RaycastHit hit;
        if (!Physics.Raycast(camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)), out hit))
        {
            return;
        }

        Vector3 hp = hit.point;
        Vector3Int guess = new Vector3Int(
            Mathf.FloorToInt(hp.x),
            Mathf.FloorToInt(hp.y),
            Mathf.FloorToInt(hp.z)
        );

        List<Tuple<Vector3Int, Vector3Int>> candidates = new List<Tuple<Vector3Int, Vector3Int>>();

        float fracX = FractionalPart(hp.x);
        float fracY = FractionalPart(hp.y);
        float fracZ = FractionalPart(hp.z);

        if (fracX <= fracY && fracX <= fracZ) {
            candidates.Add(new Tuple<Vector3Int, Vector3Int>(
                guess, guess + Vector3Int.left
            ));
        }

        if (fracY <= fracX && fracY <= fracZ)
        {
            candidates.Add(new Tuple<Vector3Int, Vector3Int>(
                guess, guess + Vector3Int.down
            ));
        }

        if (fracZ <= fracX && fracZ <= fracY)
        {
            candidates.Add(new Tuple<Vector3Int, Vector3Int>(
                guess, guess + new Vector3Int(0, 0, -1)
            ));
        }

        foreach (Tuple<Vector3Int, Vector3Int> candidate in candidates)
        {
            if (world.IsBlock(candidate.Item1))
            {
                cursor = candidate.Item1;
                cursorNeighbor = candidate.Item2;
                break;
            }
            if (world.IsBlock(candidate.Item2))
            {
                cursor = candidate.Item2;
                cursorNeighbor = candidate.Item1;
                break;
            }
        }

        highlighter.transform.localPosition = new Vector3(
            cursor.x + 0.5f,
            cursor.y + 0.5f,
            cursor.z + 0.5f
        );

        // Debug where new blocks are placed
        if (neighborHighlighter != null)
        {
            neighborHighlighter.transform.localPosition = new Vector3(
                cursorNeighbor.x + 0.5f,
                cursorNeighbor.y + 0.5f,
                cursorNeighbor.z + 0.5f
            );
        }
    }

}
