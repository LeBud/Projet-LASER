using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool IsPaused = false;

    public GameObject pausePanel;

    private void Update()
    {
        if (InputsBrain.Instance.pause.WasPressedThisFrame())
        {
            if(IsPaused)
            {
                Time.timeScale = 1.0f;
                IsPaused = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                pausePanel.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                IsPaused = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pausePanel.SetActive(true);
            }
        }
    }

    public void UnPauseBtt()
    {
        Time.timeScale = 1.0f;
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pausePanel.SetActive(false);
    }

    public void GoToMenu()
    {
        UnPauseBtt();
        SceneManager.LoadScene(0);
    }
}
