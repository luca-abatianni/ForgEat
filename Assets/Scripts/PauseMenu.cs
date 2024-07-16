using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;

    void Start()
    {
        PlayerCanMove(false);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void PlayerCanMove(bool val)
    {
        switch (MenuChoices.playerSkin) 
        {
            case 0:
                player = GameObject.Find("Player(Clone)");
                break;
            case 1:
                player = GameObject.Find("Player 1(Clone)");
                break;
            case 2:
                player = GameObject.Find("Player 2(Clone)");  
                break;
            case 3:
                player = GameObject.Find("Player 3(Clone)");
                break;
            default:
                player = null;
                break;
        }

        Debug.Log("Pause Menu found object player: " + player.ToSafeString());

        if (player!=null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerController.SetCanMove(null, val);
        }
    }
}

