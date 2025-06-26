using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public event EventHandler TogglePause;

    [SerializeField]
    private GameObject pausedScreen;

    [SerializeField]
    private Button pauseButton;

    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button mainMenuButton;

    [SerializeField]
    private Button quitButton;

    private void Awake()
    {
        Hide();
        pauseButton.onClick.AddListener(() =>
        {
            Show();
        });
        resumeButton.onClick.AddListener(() =>
        {
            Hide();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.LoadMainMenu();
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

    private void Show()
    {
        pauseButton.gameObject.SetActive(false);
        pausedScreen.SetActive(true);
        TogglePause?.Invoke(this, EventArgs.Empty);
    }

    private void Hide()
    {
        pauseButton.gameObject.SetActive(true);
        pausedScreen.SetActive(false);
        TogglePause?.Invoke(this, EventArgs.Empty);
    }
}
