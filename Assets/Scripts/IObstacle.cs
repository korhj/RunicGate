using UnityEngine;

public interface IObstacle
{
    Vector3Int TilePosition { get; }
    public void SetParent(Transform parentTransform);
    public void MoveToTile(Vector3Int tilePos);
}
