using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameAnnouncement : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI announcement_ui;
    public void ORPC_Winner(NetworkConnection winner)
    {
        if (winner.ClientId == InstanceFinder.ClientManager.Connection.ClientId)
        {
            announcement_ui.text = "You won!";
        }
        else
        {
            announcement_ui.text = "You lost!";
        }
    }

    public void ORPC_Clear()
    {
        announcement_ui.text = "";
    }
}
