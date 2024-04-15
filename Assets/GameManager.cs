using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using System;

public class GameManager : NetworkBehaviour
{
    enum GameState
    {
        WaitingOnClients,
        FirstPhase,
        WaitingSecondPhase,
        SecondPhase,
        End
    };

    [SerializeField]
    private int num_clients;
    [SerializeField]
    private int max_clients;

    [SerializeField]
    private GameObject player_spawns;

    [SerializeField]
    private float phase_one_period;

    [SerializeField]
    private float phase_two_period;

    private bool phase_timer;

    private GameState game_state;

    [SerializeField] private SpawnBarriers spawn_barriers; 


    public override void OnStartServer()
    {
        base.OnStartServer();
        phase_timer = false;
        game_state = GameState.WaitingOnClients;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsClient && !base.IsServer)
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
                case GameState.FirstPhase:
                    PhaseOne();
                    break;
                case GameState.WaitingSecondPhase:
                    WaitingSecondPhase();
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
        //FoodSpawner fs = FindAnyObjectByType<FoodSpawner>();
        //NetworkManager.Log("FoodSpawner is null? -> " + (fs == null));
        //fs.SpawnFoodTrash();
        spawn_barriers.BarriersOff();
        NetworkManager.Log("Barriers off!");
        game_state++;
        return;
    }

    void PhaseOne()
    {
        if (phase_timer == false)
        {
            phase_timer = true;
            StartCoroutine(PhaseTimer(phase_one_period));
            return;
        }
    }

    void WaitingSecondPhase()
    {
        if (phase_timer == false)
        {
            phase_timer = true;
            FindAnyObjectByType<FoodSpawner>().Server_TransformTrashInFood();
            FindAnyObjectByType<FoodSpawner>().Observer_TransformTrashInFood();
            PlayersBackToSpawn();
            StartCoroutine(PhaseTimer(phase_two_period));
        }
    }

    IEnumerator PhaseTimer(float time)
    {
        yield return new WaitForSeconds(time);
        phase_timer = false;
        NetworkManager.Log("Timer finished.");
        game_state++;
        yield return null;
    }

    void PlayersBackToSpawn()
    {
        GameObject[] player_list = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;

        if (player_list.Length <= 0) return;
        foreach (Transform spawn in player_spawns.transform)
        {
            if (i < player_list.Length)
            {
                player_list[i].GetComponent<PlayerController>().TransportPlayerToPosition(spawn.position);
                i++;
            }
            else break;
        }
    }
}
