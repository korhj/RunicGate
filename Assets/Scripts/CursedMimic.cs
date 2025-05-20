using System;
using UnityEngine;

public class CursedMimic : MonoBehaviour, IPlayerInteractable
{
    public event EventHandler<OnTouchedMimicEventArgs> OnTouchedMimic;

    public class OnTouchedMimicEventArgs : EventArgs
    {
        public GameObject mimicGameObject;
    }

    private Vector3Int tilePos;

    void Start()
    {
        tilePos = new(1, 4, 2);
        MoveMimicToTile(tilePos);
    }

    void Update() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
        {
            return;
        }
        OnTouchedMimic?.Invoke(
            this,
            new OnTouchedMimicEventArgs { mimicGameObject = this.gameObject }
        );
    }

    private void MoveMimicToTile(Vector3Int newTilePos)
    {
        Vector3 worldPos = MapManager.Instance.TileToWorld(newTilePos);
        transform.position = worldPos;
        tilePos = newTilePos;
    }

    public void DropMimic(Vector3Int Pos)
    {
        gameObject.SetActive(true);
        MoveMimicToTile(Pos);
    }

    public GameObject Interact()
    {
        gameObject.SetActive(false);
        return gameObject;
    }
}
