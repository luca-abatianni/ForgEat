using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.UI.GridLayoutGroup;

public class ScoreBoard : NetworkBehaviour
{
    // Start is called before the first frame update
    //public Dictionary<int, ScoreboardEntry> scores_dicrionary = new Dictionary<int, ScoreboardEntry>();

    [SyncObject]
    private readonly SyncDictionary<NetworkConnection, ScoreboardEntry> scores_syncdictionary = new();
    private Dictionary<NetworkConnection, GameObject> UI_elements = new();

    [SerializeField]
    private GameObject player_score_prefab;

    [SerializeField]
    Transform scoreboard_parent;

    //int scoreboard_count = 0;
    //bool full_sync = false;

    [SerializeField]
    List<UnityEngine.Color> colors;

    private void Awake()
    {
        scores_syncdictionary.OnChange += Scoreboard_OnChange;
    }

    private void Scoreboard_OnChange(SyncDictionaryOperation op,
    NetworkConnection key, ScoreboardEntry value, bool asServer)
    {
        switch (op)
        {
            //Adds key with value.
            case SyncDictionaryOperation.Add:
                //Debug.Log("Spawning ui element!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                spawnUiElement(key, value);
                break;
            //Removes key.
            case SyncDictionaryOperation.Remove:
                local_despawnPlayerScore(key, value);
                break;
            //Sets key to a new value.
            case SyncDictionaryOperation.Set:
                local_updateScore(key, value);
                break;
            //Clears the dictionary.
            case SyncDictionaryOperation.Clear:
                break;
            //Like SyncList, indicates all operations are complete.
            case SyncDictionaryOperation.Complete:
                break;
        }
    }

    private void local_despawnPlayerScore(NetworkConnection key, ScoreboardEntry value)
    {
        Destroy(UI_elements[key]);
        UI_elements.Remove(key);
    }

    private void local_updateScore(NetworkConnection key, ScoreboardEntry value)
    {
        GameObject ui_element = UI_elements[key];
        ui_element.GetComponentInChildren<Slider>().value = value.percentage;
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
        new_entry.background_color = colors[scores_syncdictionary.Count];
        new_entry.percentage = 0;
        scores_syncdictionary.Add(client, new_entry);
    }

    private void spawnUiElement(NetworkConnection key, ScoreboardEntry value)
    {
        if (UI_elements.ContainsKey(key)) return;
        GameObject UI_element = Instantiate(player_score_prefab, scoreboard_parent);
        UI_element.GetComponentInChildren<Slider>().value = value.percentage;
        UI_element.transform.Find("Background").GetComponent<Image>().color = value.background_color;

        UI_elements.Add(key, UI_element);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateScore(float percentage, NetworkConnection client)
    {
        ScoreboardEntry tmp = scores_syncdictionary[client];
        tmp.percentage = percentage;
        scores_syncdictionary[client] = tmp;
        //scores_syncdictionary.Dirty(client);
    }

    /*
    public void spawnPlayerScore(NetworkConnection owner)
    {
        StartCoroutine(initRoutine(owner));
    }


    IEnumerator initRoutine(NetworkConnection owner)
    {
        SRPC_syncDictiornary(owner);
        while (!full_sync)
        {
            yield return null;
        }
        local_spawnPlayerScore(owner);
        SRPC_spawnPlayerScore(owner);
    }


    [ServerRpc(RequireOwnership = false)]
    public void SRPC_syncDictiornary(NetworkConnection client)
    {
        int count = scores_dicrionary.Keys.Count;
        foreach (int ClientId in scores_dicrionary.Keys)
        {
            count--;
            Debug.Log("Sending trpc");
            ScoreboardEntry entry = scores_dicrionary[ClientId];
            TRPC_syncDictionary(client, ClientId, entry.slider.value, entry.background_color);
        }
        TRPC_syncDone(client);
    }

    [TargetRpc]
    public void TRPC_syncDone(NetworkConnection client)
    {
        full_sync = true;
    }

    [TargetRpc]
    public void TRPC_syncDictionary(NetworkConnection client, int key, float percentage, UnityEngine.Color color)
    {
        ScoreboardEntry tmp;

        
        Debug.Log($"Syncy dictionary: ClientID: {key}, percentage: {percentage}, color: {color}");
        if (scores_dicrionary.TryGetValue(key, out tmp))
        {
            tmp.slider.value = percentage;
        }
        else
        {
            tmp.UI_element = Instantiate(player_score_prefab, scoreboard_parent);
            tmp.slider = tmp.UI_element.GetComponentInChildren<Slider>();
            tmp.slider.value = percentage;
            tmp.UI_element.transform.Find("Background").GetComponent<Image>().color = color;
            scores_dicrionary.Add(key, tmp);
            if (scoreboard_count < 5) scoreboard_count++;
        }
    }

    public void local_spawnPlayerScore(NetworkConnection owner)
    {
        ScoreboardEntry new_entry = new ScoreboardEntry();
        new_entry.UI_element = Instantiate(player_score_prefab, scoreboard_parent);
        new_entry.slider = new_entry.UI_element.GetComponentInChildren<Slider>();
        new_entry.slider.value = 0f;
        new_entry.background_color = colors[scoreboard_count];
        new_entry.UI_element.transform.Find("Background").GetComponent<Image>().color = new_entry.background_color;
        scoreboard_count++;

        scores_dicrionary.Add(owner.ClientId, new_entry);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SRPC_spawnPlayerScore(NetworkConnection owner)
    {
        /*
        ScoreboardEntry new_entry = new ScoreboardEntry();
        new_entry.UI_element = null;
        new_entry.slider = null;
        new_entry.slider.value = 0f;
        new_entry.background_color = colors[scoreboard_count];

        scoreboard_count++;

        scores_dicrionary.Add(owner.ClientId, new_entry);
        
        ORPC_spawnPlayerScore(owner);
    }

    [ObserversRpc(ExcludeOwner = true)]
    public void ORPC_spawnPlayerScore(NetworkConnection owner)
    {
        if (owner == base.Owner) return;
        local_spawnPlayerScore(owner);
    }

    public void updateScore(float percentage, NetworkConnection owner)
    {
        ScoreboardEntry tmp;        
        tmp = scores_dicrionary[owner.ClientId];
        if (tmp == null)
        {
            spawnPlayerScore(owner);
            updateScore(percentage, owner);
        }
        else
        {
            tmp.slider.value = percentage;
        }
        SRPC_updateScore(percentage, owner);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SRPC_updateScore(float percentage, NetworkConnection owner)
    {
        ORPC_updateScore(percentage, owner);
    }

    [ObserversRpc(ExcludeOwner = true)]
    public void ORPC_updateScore(float percentage, NetworkConnection owner)
    {
        if (owner == base.Owner) return;
        ScoreboardEntry tmp;
        tmp = scores_dicrionary[owner.ClientId];
        if (tmp != null)
        {
            tmp.slider.value = percentage;
        }
        else
        {
            SRPC_syncDictiornary(base.Owner);
        }
    }
    */

    [System.Serializable]
    public class ScoreboardEntry
    {
        public UnityEngine.Color background_color;
        public float percentage;
    }

    /*

      public void SRPC_setScore(Score script, float percentage, float score)
      {
          script.current_percentage = percentage;
          script.GetComponentInChildren<Slider>().value = current_percentage;
          script.current_score = score;
          ORPC_setScore(script, percentage, score);
      }

      public void ORPC_setScore(Score script, float percentage, float score)
      {
          script.GetComponentInChildren<Slider>().value = current_percentage;
      }
    */

}
