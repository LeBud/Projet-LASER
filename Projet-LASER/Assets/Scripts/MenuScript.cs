using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame(int value)
    {
        SceneManager.LoadScene(value);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
