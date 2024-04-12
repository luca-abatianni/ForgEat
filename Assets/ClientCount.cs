using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using System.Threading;

public class ClientCount : NetworkBehaviour
{
    private int client_count;
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        client_count = 0;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<ClientCount>().enabled = false;
        }
        IncrementClientCount();
    }

    private void Update()
    {
        NetworkManager.Log("Player count: " + client_count);
    }

    public override void OnStopClient ()
    {
        if (base.IsOwner)
        {
            DecrementClientCount();
        }
    }

    [ServerRpc]
    private void IncrementClientCount()
    {
        client_count++;
    }

    [ServerRpc]
    private void DecrementClientCount()
    {
        client_count--;
    }

    public int getClientCount()
    {
        return client_count;
    }
}
