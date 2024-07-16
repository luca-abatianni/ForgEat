using System.Collections;
using System.Collections.Generic;
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

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void PlayerCanMove(bool val)
    {
        player = MenuChoices.playerSkin switch
        {
            0 => GameObject.Find("Player(Clone)"),
            1 => GameObject.Find("Player 1(Clone)"),
            2 => GameObject.Find("Player 2(Clone)"),
            3 => GameObject.Find("Player 3(Clone)"),
            _ => null,
        };

        if (player!=null)
        {
            playerController = player.GetComponent<PlayerController>();
            primaryPower = player.GetComponent<PrimaryPower>();
            NetworkConnection net_connection = playerController.GetComponent<NetworkObject>().Owner;
            playerController.SetCanMove(net_connection, val);
            primaryPower.canShoot = val;
        }
    }
}

