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
using FishNet.Component.Spawning;
using Cursor = UnityEngine.Cursor;

public class GameManager : NetworkBehaviour
{
    public enum GameState
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
    public static int max_clients = 1;

    [SerializeField]
    private int rounds_to_win;

    [SerializeField]
    private GameObject player_spawns;

    [SerializeField]
    public static float phase_one_period;

    [SerializeField]
    private int between_phase_period;

    [SerializeField]
    public static float phase_two_period;

    public static bool phase_timer;

    private Coroutine timer_coroutine;

    public GameState game_state;

    [SerializeField] private SpawnBarriers spawn_barriers;
    [SerializeField] private FoodSpawner food_spawner;
    [SerializeField] private ScoreBoard scoreboard;
    [SerializeField] private GameAnnouncement announcement;

    public static bool isPaused = false;
    public GameObject pauseMenuCanvas;
    private string playerName = "";
    private int playerSkin = 0;
    public int roundNumber = 0;

    [SerializeField] public GameObject networkManager;

    public void RetrievePlayerSettings()
    {
        Debug.Log("PLAYER: " + MenuChoices.playerName + " with skin " + MenuChoices.playerSkin);
        playerName = MenuChoices.playerName;
        playerSkin = MenuChoices.playerSkin;
        PlayerSpawner playerSpawner = networkManager.GetComponent<PlayerSpawner>();
        playerSpawner._selectedSkin = playerSkin;
        //prova
    }

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

    public void RetrievePlayersNumber()
    {
        Debug.Log("Number of players: " + MenuChoices.playersNumber);
        // From minutes to seconds
        max_clients = MenuChoices.playersNumber;
    }

    [HideInInspector]
    public Dictionary<int, GameObject> player_dictionary = new Dictionary<int, GameObject>();

    public override void OnStartServer()
    {
        RetrieveFirstPhaseLen();
        RetrieveSecondPhaseLen();
        RetrievePlayersNumber();
        RetrievePlayerSettings();
        base.OnStartServer();
        roundNumber = 0;
        phase_timer = false;
        game_state = GameState.WaitingOnClients;
    }

    public override void OnStartClient()
    {
        RetrievePlayerSettings();
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
        //Debug.Log("Phase: " + game_state);
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            ShowPauseMenu();
        }


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
        //NetworkManager.Log("Number of connected players: " + num_clients);
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
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        announcement.ORPC_Announcement("Look for food! Remember where it is!", between_phase_period);
        timer_coroutine = StartCoroutine(PhaseTimer(between_phase_period, true));
        return;
    }

    void PhaseOne()
    {
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        food_spawner.SpawnFoodTrash();

        spawn_barriers.BarriersOff();
        EnableFoodPicking(false);
        timer_coroutine = StartCoroutine(PhaseTimer((int)phase_one_period, true));
        return;
    }

    void WaitingSecondPhase()
    {
        Debug.Log(game_state.ToString() + " phase started");
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
        Debug.Log(game_state.ToString() + " phase started");
        phase_timer = true;
        spawn_barriers.BarriersOff();
        EnableFoodPicking(true);
        timer_coroutine = StartCoroutine(PhaseTimer((int)phase_two_period, true));
    }

    void EndRound()
    {
        Debug.Log(game_state.ToString() + " phase started");
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
            UpdateGameState(game_state + 1);
        }
        else
        {
            announcement.ORPC_DifferentiatedAnnouncement("You lost the round... Prepare for the next!", "You won the round! Prepare for the next!", winner, between_phase_period);
            PrepareNextRound();
        }

    }

    //commento
    [ObserversRpc]
    private void ORPC_UpdateGameState(GameState state)
    {
        game_state = state;
    }
    private void UpdateGameState(GameState state)
    {
        game_state = state;
        ORPC_UpdateGameState(state);
    }
    private void PrepareNextRound()
    {
        timer_coroutine = StartCoroutine(PhaseTimer(between_phase_period, false, GameState.WaitingFirstPhase));
        scoreboard.ResetScores();
        food_spawner.DespawnAll();
        UpdateGameState(GameState.WaitingFirstPhase);
        roundNumber++;
    }

    public void PlayerWonRound(NetworkConnection winner_client)
    {
        StopCoroutine(timer_coroutine);
        UpdateGameState(GameState.EndRound);
        EndRound();
    }

    IEnumerator PhaseTimer(int time, bool show_clients)
    {
        if (show_clients) GetComponent<Timer>().ORPC_StartTimer(time, TimeManager.Tick);
        yield return new WaitForSeconds(time);
        phase_timer = false;
        UpdateGameState(game_state + 1);
        yield return null;
    }

    IEnumerator PhaseTimer(int time, bool show_clients, GameState next_state)
    {
        if (show_clients) GetComponent<Timer>().ORPC_StartTimer(time, TimeManager.Tick);
        yield return new WaitForSeconds(time);
        phase_timer = false;
        UpdateGameState(next_state);
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
        scoreboard.SetUpRoundScore(food_spawner.total_food_points / 8);
    }

    [ObserversRpc]
    public void ORPC_GMSpawn(GameObject go, Vector3 pos, Quaternion rot)
    {
        GameObject spawned = Instantiate(go, pos, rot);
        ServerManager.Spawn(spawned);
    }

    public void ShowPauseMenu()
    {
        PauseMenu pauseMenu = pauseMenuCanvas.GetComponent<PauseMenu>();

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenuCanvas.SetActive(true);
            pauseMenu.PlayerCanMove(false);
        }
        else if (!isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenuCanvas.SetActive(false);
            pauseMenu.PlayerCanMove(true);
        }
    }

    public void ResumeFromPauseMenu()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
    }

}
