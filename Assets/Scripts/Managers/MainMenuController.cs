using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] Canvas howToPlay;

    private void Start()
    {
        SwitchToMainMenu();
    }

    public void SwitchToHowToPlay()
    {
        mainMenuCanvas.enabled = false;
        howToPlay.enabled = true;
    }

    public void SwitchToMainMenu()
    {
        mainMenuCanvas.enabled = true;
        howToPlay.enabled = false;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
}
