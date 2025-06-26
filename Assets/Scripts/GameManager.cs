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

    [SerializeField]
    GameOverUI gameOverUI;

    [SerializeField]
    PauseUI pauseUI;

    void Awake()
    {
        if (stageDataSO == null)
        {
            Debug.LogError("StageDataSO is missing or has no stages");
        }
        gameOverUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        player.OnPlayerDeath += (e, _) => GameOver();
        MapManager.Instance.OnStageClear += (e, _) => StageClear();
        pauseUI.TogglePause += (e, _) => TogglePause();
    }

    private void OnDisable()
    {
        player.OnPlayerDeath -= (e, _) => GameOver();
        MapManager.Instance.OnStageClear -= (e, _) => StageClear();
        pauseUI.TogglePause -= (e, _) => TogglePause();
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
        gameOverUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            return;
        }
        Time.timeScale = 0f;
    }
}
