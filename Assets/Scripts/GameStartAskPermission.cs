using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using FishNet.Example;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartAskPermission : MonoBehaviour
{
    private UdpClient Server;
    private const int port = 8899;

    // Start is called before the first frame update
    void Awake()
    {
        DestroyUnusefulObjects();
        Server = new UdpClient(port);
        Server.BeginReceive(OnReceive, null);
    }

    void OnReceive(IAsyncResult res) 
    {
        // Mock function to ask network permissions on windows
    }

    void Update()
    {
        StartCoroutine(WaitABitThenCloseFakeServer(20));
    }

    IEnumerator WaitABitThenCloseFakeServer(float duration)
    {
        //Debug.Log("Waiting " + duration);
        yield return new WaitForSeconds(duration);
        //Debug.Log("Closing server after " + duration);
        Server.Close();
    }

    void DestroyUnusefulObjects()
    {
        Debug.Log("We're destroying unuseful objects");
        
        GameObject player;
        GameObject networkManager;

        player = GameObject.Find("Player(Clone)");
        if(player != null)
            Destroy(player);
        player = GameObject.Find("Player 1(Clone)");
        if(player != null)
            Destroy(player);
        player = GameObject.Find("Player 2(Clone)");
        if(player != null)
            Destroy(player);
        player = GameObject.Find("Player 3(Clone)");
        if(player != null)
            Destroy(player);

        networkManager = GameObject.Find("NetworkManager");
        if(networkManager != null)
            Destroy(networkManager);
        networkManager = GameObject.Find("NetworkManager Variant");
        if(networkManager != null)
            Destroy(networkManager);
    }
}
