using UnityEngine;

public class CursedMimic : MonoBehaviour, IPlayerInteractable
{
    private Vector3Int tileCoordinates;

    void Start()
    {
        tileCoordinates = new(1, 4, 2);
        MoveMimicToTile(tileCoordinates);
    }

    void Update() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collision this: {transform.position} other: {other.transform.position}");
    }

    private void MoveMimicToTile(Vector3Int newTileCoordinates)
    {
        Vector3 worldPos = MapManager.Instance.TileToWorld(newTileCoordinates);
        transform.position = worldPos;
        tileCoordinates = newTileCoordinates;
    }

    public GameObject Interact(Vector3Int coordinates)
    {
        throw new System.NotImplementedException();
    }
}
