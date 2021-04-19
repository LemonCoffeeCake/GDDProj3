using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        GameManager.instance.Unpause();
        GameManager.instance.ResetLevelCount();
        SceneManager.LoadScene("MainMenu");
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
        Time.timeScale = 1f;
        Destroy(gameObject);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
        GameManager.instance.Unpause();
    }
}
