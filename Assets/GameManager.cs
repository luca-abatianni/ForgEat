using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;

public class GameManager : NetworkBehaviour
{
    enum GameState
    {
        WaitingOnClients,
        FirstPhase,
        SecondPhase,
        End
    };

    [SerializeField]
    private int num_clients;
    [SerializeField]
    private int max_clients;

    private GameState game_state;

    [SerializeField] private SpawnBarriers spawn_barriers; 


    public override void OnStartServer()
    {
        base.OnStartServer();
        game_state = GameState.WaitingOnClients;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsClient && !base.IsHost)
        {
            GetComponent<GameManager>().enabled = false;
        }
        return;
    }

    // Update is called once per frame
    void Update()
    {
        if (base.IsServer)
        {
            switch (game_state)
            { 
                case GameState.WaitingOnClients:
                    WaitForPlayers();
                    break;
                default:
                    return;
            }
        }
    }

    void WaitForPlayers()
    {

        //num_clients = GameObject.FindGameObjectsWithTag("Player").Length;
        num_clients = InstanceFinder.ServerManager.Clients.Count;
        NetworkManager.Log("Number of connected players: " + num_clients);
        if (num_clients == max_clients)
        {
            StartPhaseOne(); // First phase.
        }
        return;
    }

    void StartPhaseOne()
    {
        spawn_barriers.BarriersOff();
        NetworkManager.Log("Barriers off!");
        game_state++;
        return;
    }
}
