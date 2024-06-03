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
        WaitingFirstPhase,
        FirstPhase,
        WaitingSecondPhase,
        SecondPhase,
        End
    };

    [SerializeField]
    private int num_clients;
    [SerializeField]
    public static int max_clients = 1;

    [SerializeField]
    private GameObject player_spawns;

    [SerializeField]
    public static float phase_one_period;

    [SerializeField]
    private float between_phase_period;

    [SerializeField]
    public static float phase_two_period;

    public static bool phase_timer;

    private GameState game_state;

    [SerializeField] private SpawnBarriers spawn_barriers; 

    public void RetrieveFirstPhaseLen()
    {
        Debug.Log("1st phase length: " + MenuChoices.firstPhaseLen);
        phase_one_period = MenuChoices.firstPhaseLen;
    }

    public void RetrieveSecondPhaseLen()
    {
        Debug.Log("2nd phase length: " + MenuChoices.secondPhaseLen);
        // From minutes to seconds
        phase_two_period = MenuChoices.secondPhaseLen * 60;
    }

    public override void OnStartServer()
    {
        RetrieveFirstPhaseLen();
        RetrieveSecondPhaseLen();
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
        if (base.IsServer && !phase_timer)
        {
            switch (game_state)
            { 
                case GameState.WaitingOnClients:
                    WaitForPlayers();
                    break;
                case GameState.WaitingFirstPhase:
                    WaitingFirstPhase();
                    break;
                case GameState.FirstPhase:
                    PhaseOne();
                    break;
                case GameState.WaitingSecondPhase:
                    WaitingSecondPhase();
                    break;
                case GameState.SecondPhase:
                    SecondPhase();
                    break;
                case GameState.End:
                    EndRound();
                    break;
                default:
                    return;
            }
        }
    }

    void WaitForPlayers()
    {
        Debug.Log(game_state.ToString() + " phase started");
        num_clients = InstanceFinder.ServerManager.Clients.Count;
        NetworkManager.Log("Number of connected players: " + num_clients);
        if (num_clients == max_clients)
        {
            game_state++; 
        }
        return;
    }

    void WaitingFirstPhase ()
    {
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        StartCoroutine(PhaseTimer(between_phase_period));
        return;
    }


    void PhaseOne()
    {
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        spawn_barriers.BarriersOff();
        EnableFoodPicking(false);
        NetworkManager.Log("Barriers off!");
        StartCoroutine(PhaseTimer(phase_one_period));
        return;
    }

    void WaitingSecondPhase()
    {
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        FindAnyObjectByType<FoodSpawner>().Server_TransformTrashInFood();
        FindAnyObjectByType<FoodSpawner>().Observer_TransformTrashInFood();
        PlayersBackToSpawn();
        spawn_barriers.BarriersOn();
        StartCoroutine(PhaseTimer(between_phase_period));
    }

    void SecondPhase()
    {
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        spawn_barriers.BarriersOff();
        EnableFoodPicking(true);
        StartCoroutine(PhaseTimer(phase_two_period));
    }

    void EndRound()
    {
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        PlayersBackToSpawn();
        spawn_barriers.BarriersOn();
    }

    IEnumerator PhaseTimer(float time)
    {
        yield return new WaitForSeconds(time);
        phase_timer = false;
        NetworkManager.Log(game_state.ToString() + " phase finished, timer is over");
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

    void EnableFoodPicking(bool setting)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            FoodPicker f_picker = player.GetComponent<FoodPicker>();
            f_picker.Client_FoodPickerSetEnabled(setting);
            f_picker.enabled = setting;
        }
    }
}