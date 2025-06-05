using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    StageDataSO stageDataSO;

    [SerializeField]
    InterfaceDataSO interfaceDataSO;

    void Awake()
    {
        if (stageDataSO == null)
        {
            Debug.LogError("StageDataSO is missing or has no stages");
        }
    }

    private void OnEnable()
    {
        player.OnPlayerDeath += (e, _) => GameOver();
        MapManager.Instance.OnStageClear += (e, _) => StageClear();
    }

    private void OnDisable()
    {
        player.OnPlayerDeath -= (e, _) => GameOver();
        MapManager.Instance.OnStageClear -= (e, _) => StageClear();
    }

    private void StageClear()
    {
        interfaceDataSO.SetPlayerHasObject(false);
        interfaceDataSO.SetPlayerHealthPercent(1);
        interfaceDataSO.SetRunicGateCount(0);
        interfaceDataSO.SetTargetObject(null);
        Loader.LoadNextStage();
    }

    private void GameOver()
    {
        throw new NotImplementedException();
    }
}
