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
    private StageDataSO.GameStage currentStage;

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
        Loader.LoadNextStage();
    }

    private void GameOver()
    {
        throw new NotImplementedException();
    }
}
