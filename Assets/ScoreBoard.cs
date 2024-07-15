using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : NetworkBehaviour
{

    [SyncObject]
    private readonly SyncDictionary<NetworkConnection, ScoreboardEntry> scores_dictionary = new();

    private Dictionary<NetworkConnection, GameObject> UI_elements = new();
    private Dictionary<NetworkConnection, GameObject> table_elements = new();

    [SerializeField]
    private GameObject player_score_prefab;

    [SerializeField]
    Transform scoreboard_parent;

    [SerializeField]
    GameObject full_scoreboard;

    [SerializeField]
    Transform full_scoreboard_entries;

    [SerializeField]
    private GameObject full_score_prefab;

    private float points_to_win;


    //int scoreboard_count = 0;
    //bool full_sync = false;

    [SerializeField]
    List<UnityEngine.Color> colors;

    private void Awake()
    {
        scores_dictionary.OnChange += Scoreboard_OnChange;
        full_scoreboard.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            full_scoreboard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            full_scoreboard.SetActive(false);
        }
    }


    private void Scoreboard_OnChange(SyncDictionaryOperation op,
    NetworkConnection key, ScoreboardEntry value, bool asServer)
    {
        switch (op)
        {
            //Adds key with value.
            case SyncDictionaryOperation.Add:
                spawnUiElement(key, value);
                break;
            //Removes key.
            case SyncDictionaryOperation.Remove:
                local_despawnPlayerScore(key, value);
                break;
            //Sets key to a new value.
            case SyncDictionaryOperation.Set:
                local_updateScore(key, value);
                local_reorderScores();
                break;
            //Clears the dictionary.
            case SyncDictionaryOperation.Clear:
                break;
            //Like SyncList, indicates all operations are complete.
            case SyncDictionaryOperation.Complete:
                break;
        }
    }

    private void local_reorderScores()
    {
        Transform[] children = scoreboard_parent.Cast<Transform>().ToArray();
        Transform[] sorted_children = children.OrderByDescending(child => child.GetComponentInChildren<Slider>().value).ToArray();
        for (int i = 0; i < sorted_children.Length; i++)
        {
            sorted_children[i].SetSiblingIndex(i);
        }
        
        children = full_scoreboard_entries.Cast<Transform>().ToArray();
        sorted_children = children.OrderByDescending(entry => entry.GetComponentInChildren<DetailedScoreEntry>().score_value).ToArray();
        for (int i = 0; i < sorted_children.Length; i++)
        {
            sorted_children[i].SetSiblingIndex(i);
        }
    }

    private void local_despawnPlayerScore(NetworkConnection key, ScoreboardEntry value)
    {
        Destroy(UI_elements[key]);
        UI_elements.Remove(key);
        
        Destroy(table_elements[key]);
        table_elements.Remove(key);
    }

    private void local_updateScore(NetworkConnection key, ScoreboardEntry value)
    {
        GameObject table_ui_element = table_elements[key];
        GameObject ui_element = UI_elements[key];
        ui_element.GetComponentInChildren<Slider>().value = value.percentage;
        DetailedScoreEntry entry = table_ui_element.GetComponentInChildren<DetailedScoreEntry>();
        entry.SetScore(value.score);
        entry.SetRoundsWon(value.rounds_won);
    }

    public void spawnPlayerScore(NetworkConnection client, string player_name)
    {
        if (client.IsHost)
        {
            addPlayerEntry(client, player_name);
        }
        else
        {
            SRPC_addPlayerEntry(client, player_name);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void SRPC_addPlayerEntry(NetworkConnection client, string player_name)
    {
        addPlayerEntry(client, player_name);
    }

    private void addPlayerEntry(NetworkConnection client, string player_name)
    {
        ScoreboardEntry new_entry = new ScoreboardEntry();
        new_entry.background_color = colors[scores_dictionary.Count];
        new_entry.percentage = 0;
        new_entry.score = 0;
        new_entry.rounds_won = 0;
        new_entry.player_name = player_name;
        scores_dictionary.Add(client, new_entry);
    }

    private void spawnUiElement(NetworkConnection key, ScoreboardEntry value)
    {
        if (UI_elements.ContainsKey(key)) return;

        GameObject UI_element = Instantiate(player_score_prefab, scoreboard_parent);
        UI_element.GetComponentInChildren<Slider>().value = value.percentage;
        UI_element.transform.Find("Background").GetComponent<Image>().color = value.background_color;

        GameObject UI_table_entry = Instantiate(full_score_prefab, full_scoreboard_entries.transform);
        DetailedScoreEntry table_entry = UI_table_entry.GetComponent<DetailedScoreEntry>();
        table_entry.SetScore(0f);
        table_entry.SetName(value.player_name);
        table_entry.SetRoundsWon(0);


        if (key.ClientId == InstanceFinder.ClientManager.Connection.ClientId)
        {
            UI_element.GetComponentInChildren<Outline>().enabled = true;
            table_entry.HighlightPlayer();
        }

        UI_elements.Add(key, UI_element);
        table_elements.Add(key, UI_table_entry);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateScore(float percentage, float score, NetworkConnection client)
    {
        ScoreboardEntry tmp = scores_dictionary[client];
        tmp.percentage = percentage;
        tmp.score = score;
        scores_dictionary[client] = tmp;

        if(tmp.percentage > 1.0f)
        {
            FindAnyObjectByType<GameManager>().PlayerWonRound(client);
        }
        //scores_syncdictionary.Dirty(client);
    }

    [ServerRpc(RequireOwnership = false)]
    public void addPoints(float points, NetworkConnection client)
    {
        ScoreboardEntry tmp = scores_dictionary[client];
        tmp.score += points;
        if (tmp.score < 0)
        {
            tmp.score = 0;
            tmp.percentage = 0;
        }
        else
        {
            tmp.percentage = tmp.score / points_to_win;
        }
        scores_dictionary[client] = tmp;

        if (tmp.percentage > 1.0f)
        {
            FindAnyObjectByType<GameManager>().PlayerWonRound(client);
        }
    }

    [Server]
    public void SetUpRoundScore(float points)
    {
        this.points_to_win = points;
    }


    [Server]
    public NetworkConnection getWinner()
    {
        if (scores_dictionary.Count == 0)
            return null;

        NetworkConnection winner = null;
        float highestScore = float.MinValue;
        int parity_counter = 0;

        foreach (var entry in scores_dictionary)
        {
            if (entry.Value.score > highestScore)
            {
                highestScore = entry.Value.score;
                winner = entry.Key;
                parity_counter = 0;
            }
            else if (entry.Value.score == highestScore)
            {
                parity_counter++;
            }
        }


        return (parity_counter > 0 ? null : winner);
    }

    [Server]
    public bool AwardRoundPoint(NetworkConnection round_winner, int rounds_to_win) 
    {
        ScoreboardEntry tmp = scores_dictionary[round_winner];
        tmp.rounds_won++;
        scores_dictionary[round_winner] = tmp;
        if (tmp.rounds_won >= rounds_to_win)
        {
            return true;
        }
        else
        {
            return false;
        }


    }

    [Server]
    public void ResetScores()
    {
        /*
        NetworkConnection[] keys = scores_dictionary.Keys.ToArray();
        foreach (var key in keys)
        {
            ScoreboardEntry tmp = scores_dictionary[key];
            tmp.score = 0;
            scores_dictionary[key] = tmp;
        }
        */
        foreach (var entry in scores_dictionary)
        {
            entry.Value.score = 0;
            entry.Value.percentage = 0;
            scores_dictionary.Dirty(entry.Key);
        }
    }

    [System.Serializable]
    public class ScoreboardEntry
    {
        public UnityEngine.Color background_color;
        public string player_name;
        public float percentage;
        public float score;
        public int rounds_won;
    }


}
