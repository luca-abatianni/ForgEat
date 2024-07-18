using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    PrimaryPower primaryPower;

    public static bool isPaused = false;
    public GameObject pauseMenuCanvas;

    void Update()
    {
        //Debug.Log("Phase: " + game_state);
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            ShowPauseMenu();
        }

    }
    public void ShowPauseMenu()
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenuCanvas.SetActive(true);
            PlayerCanMove(false);
        }
        else if (!isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenuCanvas.SetActive(false);
            PlayerCanMove(true);
        }
    }

    public void ResumeFromPauseMenu()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void PlayerCanMove(bool val)
    {
        Debug.Log("PlayerCanMove called");

        // player = MenuChoices.playerSkin switch
        // {
        //     0 => GameObject.Find("Player(Clone)"),
        //     1 => GameObject.Find("Player 1(Clone)"),
        //     2 => GameObject.Find("Player 2(Clone)"),
        //     3 => GameObject.Find("Player 3(Clone)"),
        //     _ => null,
        // };

        GameObject[] allPlayerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in allPlayerObjects)
        {
            if (p.GetComponent<PlayerController>().enabled)
            {
                player = p;
                break;
            }
        }

        Debug.Log("We found player " + player.ToSafeString());

        if (player!=null)
        {
            playerController = player.GetComponent<PlayerController>();
            primaryPower = player.GetComponent<PrimaryPower>();
            //NetworkConnection net_connection = player.GetComponent<NetworkObject>().Owner;
            //Debug.Log("-----Network connection " + net_connection.ToSafeString());

            playerController.SetCanMoveByClient(InstanceFinder.ClientManager.Connection, val);
            primaryPower.canShoot = val;
        }
    }
}

