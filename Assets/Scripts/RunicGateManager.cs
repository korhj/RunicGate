using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RunicGateManager : MonoBehaviour
{
    public event EventHandler<OnTeleportEventArgs> OnTeleport;

    public class OnTeleportEventArgs : EventArgs
    {
        public Vector3Int targetTilePos;
        public Collider2D exitGateCollider;
    }

    [SerializeField]
    GameObject runicGatePrefab;

    [SerializeField]
    InterfaceDataSO interfaceDataSO;
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
        interfaceDataSO.SetRunicGateCount(0);
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
        Vector3Int targetPos = runicGates[targetIndex].TilePos;
        Collider2D exitGatecCllider2D = runicGates[
            targetIndex
        ].RunicGateObject.GetComponent<Collider2D>();

        OnTeleport?.Invoke(
            this,
            new OnTeleportEventArgs
            {
                targetTilePos = targetPos,
                exitGateCollider = exitGatecCllider2D
            }
        );
    }

    public bool TileHasGate(Vector3Int tilePos)
    {
        for (int i = 0; i < runicGates.Length; i++)
        {
            if (runicGates[i].TilePos == tilePos && runicGates[i].RunicGateObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void DeactivateRunicGate(Vector3Int tilePos)
    {
        for (int i = 0; i < runicGates.Length; i++)
        {
            if (runicGates[i].TilePos == tilePos && runicGates[i].RunicGateObject.activeSelf)
            {
                runicGates[i].RunicGateObject.SetActive(false);
                MapManager.Instance.RemoveRunicGate(tilePos);
                interfaceDataSO.SetRunicGateCount(interfaceDataSO.runicGateCount - 1);
                return;
            }
        }
    }

    public void ActivateRunicGate(Vector3Int tilePos)
    {
        for (int i = 0; i < runicGates.Length; i++)
        {
            if (!runicGates[i].RunicGateObject.activeSelf)
            {
                runicGates[i].RunicGateObject.transform.position = MapManager.Instance.TileToWorld(
                    tilePos
                );
                runicGates[i].TilePos = tilePos;
                runicGates[i].RunicGateObject.SetActive(true);
                MapManager.Instance.AddRunicGate(tilePos);
                interfaceDataSO.SetRunicGateCount(interfaceDataSO.runicGateCount + 1);
                return;
            }
        }
    }
}

struct RunicGateData
{
    public GameObject RunicGateObject;
    public Vector3Int TilePos;

    public RunicGateData(GameObject runicGateObject, Vector3Int tilePos)
    {
        RunicGateObject = runicGateObject;
        TilePos = tilePos;
    }
}
