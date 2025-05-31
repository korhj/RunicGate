using UnityEngine;

public interface IObstacle
{
    Vector3Int TilePos { get; }
    public void SetParent(Transform parentTransform);
    public void MoveToTile(Vector3Int tilePos);
}
