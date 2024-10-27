using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIController : MonoBehaviour
{
    public LoadingScenePanel loadingScenePanel;

    public void PlayGame()
    {
        loadingScenePanel.gameObject.SetActive(true);
        loadingScenePanel.StartLoadingScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
