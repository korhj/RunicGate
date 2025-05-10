using System;
using UnityEngine;

public class CursedMimic : MonoBehaviour, IPlayerInteractable
{
    public event EventHandler<OnTouchedMimicEventArgs> OnTouchedMimic;

    public class OnTouchedMimicEventArgs : EventArgs
    {
        public GameObject mimicGameObject;
    }

    private Vector3Int tileCoordinates;

    void Start()
    {
        tileCoordinates = new(1, 4, 2);
        MoveMimicToTile(tileCoordinates);
    }

    void Update() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTouchedMimic?.Invoke(
            this,
            new OnTouchedMimicEventArgs { mimicGameObject = this.gameObject }
        );
    }

    private void MoveMimicToTile(Vector3Int newTileCoordinates)
    {
        Vector3 worldPos = MapManager.Instance.TileToWorld(newTileCoordinates);
        transform.position = worldPos;
        tileCoordinates = newTileCoordinates;
    }

    public void DropMimic(Vector3Int coordinates)
    {
        gameObject.SetActive(true);
        MoveMimicToTile(coordinates);
    }

    public GameObject Interact()
    {
        gameObject.SetActive(false);
        return gameObject;
    }
}
