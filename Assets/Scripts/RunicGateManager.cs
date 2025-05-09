using System;
using System.Linq;
using UnityEngine;

public class RunicGateManager : MonoBehaviour
{
    public event EventHandler<OnTeleportEventArgs> OnTeleport;

    public class OnTeleportEventArgs : EventArgs
    {
        public Vector3Int targetTileCoordinates;
        public Collider2D exitGateCollider;
    }

    [SerializeField]
    GameObject runicGatePrefab;
    private RunicGateData[] runicGates;

    private void Start()
    {
        runicGates = new RunicGateData[2];
        for (int i = 0; i < runicGates.Length; i++)
        {
            GameObject runicGate = Instantiate(runicGatePrefab);
            runicGate.SetActive(false);
            RunicGate runicGateComponent = runicGate.GetComponent<RunicGate>();
            runicGateComponent.OnRunicGateEntered += RunicGate_OnRunicGateEntered;

            runicGates[i] = new RunicGateData(runicGate, Vector3Int.zero);
        }
    }

    private void RunicGate_OnRunicGateEntered(
        object sender,
        RunicGate.OnRunicGateEnteredEventArgs e
    )
    {
        if (!runicGates.All(g => g.RunicGateObject.activeSelf))
            return;

        int triggeredIndex = Array.FindIndex(
            runicGates,
            g => g.RunicGateObject == e.runicGateGameObject
        );

        if (triggeredIndex < 0)
            return;

        int targetIndex = (triggeredIndex + 1) % runicGates.Length;
        Vector3Int targetCoordinates = runicGates[targetIndex].TileCoordinates;
        Collider2D exitGatecCllider2D = runicGates[
            targetIndex
        ].RunicGateObject.GetComponent<Collider2D>();

        OnTeleport?.Invoke(
            this,
            new OnTeleportEventArgs
            {
                targetTileCoordinates = targetCoordinates,
                exitGateCollider = exitGatecCllider2D
            }
        );
    }

    public void ToggleRunicGate(Vector3Int tileCoordinates)
    {
        for (int i = 0; i < runicGates.Length; i++)
        {
            if (
                runicGates[i].TileCoordinates == tileCoordinates
                && runicGates[i].RunicGateObject.activeSelf
            )
            {
                runicGates[i].RunicGateObject.SetActive(false);
                return;
            }
        }
        for (int i = 0; i < runicGates.Length; i++)
        {
            if (!runicGates[i].RunicGateObject.activeSelf)
            {
                runicGates[i].RunicGateObject.transform.position = MapManager.Instance.TileToWorld(
                    tileCoordinates
                );
                runicGates[i].TileCoordinates = tileCoordinates;

                runicGates[i].RunicGateObject.SetActive(true);
                return;
            }
        }
    }
}

struct RunicGateData
{
    public GameObject RunicGateObject;
    public Vector3Int TileCoordinates;

    public RunicGateData(GameObject runicGateObject, Vector3Int tileCoordinates)
    {
        RunicGateObject = runicGateObject;
        TileCoordinates = tileCoordinates;
    }
}
