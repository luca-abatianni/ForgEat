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
        EndRound,
        EndMatch
    };

    [SerializeField]
    private int num_clients;
    [SerializeField]
    private int max_clients;

    [SerializeField]
    private int rounds_to_win;

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
    [SerializeField] private ScoreBoard scoreboard;
    [SerializeField] private GameAnnouncement announcement;

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
        Debug.Log("Phase: " +  game_state);
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
                case GameState.EndRound:
                    EndRound();
                    break;
                default:
                    return;
            }
        }
    }

    void WaitForPlayers()
    {
        int current_num = InstanceFinder.ServerManager.Clients.Count;
        if (num_clients != current_num)
        {
            string s = "Waiting for players... " + current_num.ToString() + "/" + max_clients.ToString();
            announcement.ORPC_Announcement(s, -1);
            num_clients = current_num;
        }
        NetworkManager.Log("Number of connected players: " + num_clients);
        if (num_clients == max_clients)
        {
            phase_timer = true;
            StartCoroutine(PhaseTimer(5, false));
            announcement.ORPC_Announcement("Starting match!", -1);
            //game_state++;
        }
        return;
    }



    void WaitingFirstPhase() // memory phase
    {
        phase_timer = true;
        announcement.ORPC_Announcement("Look for food! Remember where it is!", between_phase_period);
        timer_coroutine = StartCoroutine(PhaseTimer(between_phase_period, true));
        return;
    }


    void PhaseOne()
    {
        phase_timer = true;
        food_spawner.SpawnFoodTrash();

        spawn_barriers.BarriersOff();
        EnableFoodPicking(false);
        timer_coroutine = StartCoroutine(PhaseTimer(phase_one_period, true));
        return;
    }

    void WaitingSecondPhase()
    {
        phase_timer = true;
        food_spawner.Server_TransformTrashInFood();
        food_spawner.Observer_TransformTrashInFood();
        announcement.ORPC_Announcement("Get to your food before the others!", between_phase_period);
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
        spawn_barriers.BarriersOn();

        ScoreBoard sb = FindAnyObjectByType<ScoreBoard>();
        NetworkConnection winner = sb.getWinner();

        if (winner == null)
        {
            announcement.ORPC_Announcement("It's a draw! Prepare for the next round!", between_phase_period);
            PrepareNextRound();
        }
        else if (sb.AwardRoundPoint(winner, rounds_to_win)) // will return true when a player reached winning number of rounds.
        {
            announcement.ORPC_DifferentiatedAnnouncement("You lost the match!", "You won the match!", winner, 10f);
            game_state++;
        }
        else
        {
            announcement.ORPC_DifferentiatedAnnouncement("You lost the round... Prepare for the next!", "You won the round! Prepare for the next!", winner, between_phase_period);
            PrepareNextRound();
        }

    }

    private void PrepareNextRound()
    {
        timer_coroutine = StartCoroutine(PhaseTimer(between_phase_period, false, GameState.WaitingFirstPhase));
        scoreboard.ResetScores();
        food_spawner.DespawnAll();
        game_state = GameState.WaitingFirstPhase;
    }

    public void PlayerWonRound(NetworkConnection winner_client)
    {
        StopCoroutine(timer_coroutine);
        game_state = GameState.EndRound;
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

    IEnumerator PhaseTimer(int time, bool show_clients, GameState next_state)
    {
        if (show_clients) GetComponent<Timer>().ORPC_StartTimer(time, TimeManager.Tick);
        yield return new WaitForSeconds(time);
        phase_timer = false;
        game_state = next_state;
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
        scoreboard.SetUpRoundScore(food_spawner.total_food_points/8);
    }

    [ObserversRpc]
    public void ORPC_GMSpawn(GameObject go, Vector3 pos, Quaternion rot)
    {
        GameObject spawned = Instantiate(go, pos, rot);
        ServerManager.Spawn(spawned);
    }
}
