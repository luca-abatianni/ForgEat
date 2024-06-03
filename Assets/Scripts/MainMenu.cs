using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (MenuChoices.serverIP != null && MenuChoices.serverIP != "")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
