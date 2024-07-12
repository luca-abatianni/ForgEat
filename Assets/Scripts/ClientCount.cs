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
    private Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private static ClientCount instance;
    public static void InitializeNewPlayer(int clientID)
    {
        instance._players.Add(clientID, new Player());
    }
    public static void UpdateScore(int player, int amount)
    {
        if (instance._players.TryGetValue(player, out Player thisPlayer))
        {
            thisPlayer.Score += amount;
        }

    }
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        client_count = 0;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _players.Add(OwnerId, this.GetComponent<Player>());
        InitializeNewPlayer(OwnerId);
        GameUIManager.PlayerJoined(OwnerId);
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

    public override void OnStopClient()
    {
        if (base.IsOwner)
        {
            DecrementClientCount();
        }
        GameUIManager.PlayerLeft(OwnerId);
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
    class Player
    {
        public int Score = 0;
    }
}
