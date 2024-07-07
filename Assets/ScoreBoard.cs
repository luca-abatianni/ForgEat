using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.UI.GridLayoutGroup;

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
    GameObject full_scoreboard_parent;

    [SerializeField]
    private GameObject full_score_prefab;


    //int scoreboard_count = 0;
    //bool full_sync = false;

    [SerializeField]
    List<UnityEngine.Color> colors;

    private void Awake()
    {
        scores_dictionary.OnChange += Scoreboard_OnChange;
        full_scoreboard_parent.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            full_scoreboard_parent.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            full_scoreboard_parent.SetActive(false);
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
        Transform[] table_children = full_scoreboard_parent.transform.Cast<Transform>().ToArray();
        Transform[] sortedChildren = children.OrderByDescending(child => child.GetComponentInChildren<Slider>().value).ToArray();
        for (int i = 0; i < sortedChildren.Length; i++)
        {
            sortedChildren[i].SetSiblingIndex(i);
        }

        children = full_scoreboard_parent.transform.Cast<Transform>().ToArray();
        sortedChildren = children.OrderByDescending(child => child.GetComponentInChildren<DetailedScoreEntry>().score_value).ToArray();
        for (int i = 0; i < sortedChildren.Length; i++)
        {
            sortedChildren[i].SetSiblingIndex(i);
        }
    }

    private void local_despawnPlayerScore(NetworkConnection key, ScoreboardEntry value)
    {
        Destroy(UI_elements[key]);
        UI_elements.Remove(key);
    }

    private void local_updateScore(NetworkConnection key, ScoreboardEntry value)
    {
        GameObject table_ui_element = table_elements[key];
        GameObject ui_element = UI_elements[key];
        ui_element.GetComponentInChildren<Slider>().value = value.percentage;

        table_ui_element.GetComponent<DetailedScoreEntry>().SetScore(value.score);
    }

    public void spawnPlayerScore(NetworkConnection client)
    {
        if (client.IsHost)
        {
            addPlayerEntry(client);
        }
        else
        {
            SRPC_addPlayerEntry(client);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void SRPC_addPlayerEntry(NetworkConnection client)
    {
        addPlayerEntry(client);
    }

    private void addPlayerEntry(NetworkConnection client)
    {
        ScoreboardEntry new_entry = new ScoreboardEntry();
        new_entry.background_color = colors[scores_dictionary.Count];
        new_entry.percentage = 0;
        new_entry.score = 0;
        new_entry.rounds_won = 0;
        scores_dictionary.Add(client, new_entry);
    }

    private void spawnUiElement(NetworkConnection key, ScoreboardEntry value)
    {
        if (UI_elements.ContainsKey(key)) return;

        GameObject UI_element = Instantiate(player_score_prefab, scoreboard_parent);
        UI_element.GetComponentInChildren<Slider>().value = value.percentage;
        UI_element.transform.Find("Background").GetComponent<Image>().color = value.background_color;

        GameObject UI_table_entry = Instantiate(full_score_prefab, full_scoreboard_parent.transform);
        DetailedScoreEntry table_entry = UI_table_entry.GetComponent<DetailedScoreEntry>();
        table_entry.SetScore(0f);
        table_entry.SetName("Player_" + key.ClientId);
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

        
        //scores_syncdictionary.Dirty(client);
    }

    [System.Serializable]
    public class ScoreboardEntry
    {
        public UnityEngine.Color background_color;
        public float percentage;
        public float score;
        public int rounds_won;
    }

}
