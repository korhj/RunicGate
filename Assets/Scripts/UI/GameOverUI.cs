using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button mainMenuButton;

    [SerializeField]
    private Button quitButton;

    private void Awake()
    {
        restartButton.onClick.AddListener(() =>
        {
            Loader.ReLoad();
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
}
