using UnityEngine;

interface BlockContainer
{
    bool IsBlock(Vector3Int position);
    void SetBlock(Vector3Int position, bool value);
}
