using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using System;
using FishNet.Connection;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using TMPro;

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
    private int max_clients;

    [SerializeField]
    private GameObject player_spawns;

    [SerializeField]
    private int phase_one_period;

    [SerializeField]
    private int between_phase_period;

    [SerializeField]
    private int phase_two_period;

    private bool phase_timer;

    private Coroutine timer_coroutine;

    private GameState game_state;

    [SerializeField] private SpawnBarriers spawn_barriers;
    [SerializeField] private FoodSpawner food_spawner;

    [HideInInspector]
    public Dictionary<int, GameObject> player_dictionary = new Dictionary<int, GameObject>();

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
            GetComponent<GameManager>().enabled = false; // Update() will be run only by server.
        }
        return;
    }

    // Update is called once per frame // Executed only by server.
    void Update()
    {
        if (base.IsServer && !phase_timer) // reduntant check.
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
        num_clients = InstanceFinder.ServerManager.Clients.Count;
        NetworkManager.Log("Number of connected players: " + num_clients);
        if (num_clients == max_clients)
        {
            phase_timer = true;
            StartCoroutine(PhaseTimer(5, false));
            //game_state++;
        }
        return;
    }



    void WaitingFirstPhase()
    {
        phase_timer = true;
        timer_coroutine = StartCoroutine(PhaseTimer(between_phase_period, true));
        return;
    }


    void PhaseOne()
    {
        phase_timer = true;
        food_spawner.SpawnFoodTrash();

        spawn_barriers.BarriersOff();
        EnableFoodPicking(false);
        NetworkManager.Log("Barriers off!");
        timer_coroutine = StartCoroutine(PhaseTimer(phase_one_period, true));
        return;
    }

    void WaitingSecondPhase()
    {
        phase_timer = true;
        food_spawner.Server_TransformTrashInFood();
        food_spawner.Observer_TransformTrashInFood();
        SetUpPlayersRoundScore();
        PlayersBackToSpawn();
        spawn_barriers.BarriersOn();
        timer_coroutine = StartCoroutine(PhaseTimer(between_phase_period, true));
    }

    void SecondPhase()
    {
        phase_timer = true;
        spawn_barriers.BarriersOff();
        EnableFoodPicking(true);
        timer_coroutine = StartCoroutine(PhaseTimer(phase_two_period, true));
    }

    void EndRound()
    {
        phase_timer = true;
        PlayersBackToSpawn();

        NetworkConnection winner = FindAnyObjectByType<ScoreBoard>().getWinner();
        if (winner != null)
        {
            GetComponent<GameAnnouncement>().ORPC_Winner(winner);
        }

        spawn_barriers.BarriersOn();
    }

    public void PlayerWon(NetworkConnection winner_client)
    {
        Debug.Log($"Player_{winner_client.ClientId} [{winner_client}] won!");
        StopCoroutine(timer_coroutine);
        game_state = GameState.End;
        EndRound();
    }

    IEnumerator PhaseTimer(int time, bool show_clients)
    {
        if (show_clients) GetComponent<Timer>().ORPC_StartTimer(time, TimeManager.Tick);
        yield return new WaitForSeconds(time);
        phase_timer = false;
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
            //f_picker.enabled = setting;
        }
    }

    void SetPlayerDictionary()
    {
        GameObject[] player_list = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(player_list.Length);
        foreach (GameObject player in player_list)
        {
            int owner_id = player.GetComponent<NetworkBehaviour>().OwnerId;
            player_dictionary.Add(owner_id, player);
        }
    }

    void SetUpPlayersRoundScore()
    {
        GameObject[] player_list = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in player_list)
        {
            NetworkConnection net_connection = player.GetComponent<NetworkBehaviour>().Owner;
            Debug.Log(net_connection);
            player.GetComponent<Score>().TRPC_SetUpRoundScore(net_connection, food_spawner.total_food_points/10);
        }
    }

    [ObserversRpc]
    public void ORPC_GMSpawn(GameObject go, Vector3 pos, Quaternion rot)
    {
        GameObject spawned = Instantiate(go, pos, rot);
        ServerManager.Spawn(spawned);
    }
}
