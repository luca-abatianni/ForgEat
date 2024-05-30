using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (SetupMenu.serverIP != null && SetupMenu.serverIP != "")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
