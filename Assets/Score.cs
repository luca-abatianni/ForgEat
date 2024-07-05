using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Score : NetworkBehaviour
{
    private float points_to_win;
    public float current_score;
    public int rounds_won;
    private float current_percentage;
    bool set;

    private ScoreBoard scoreboard;

    public override void OnStartClient()
    {
        if (!base.IsOwner)
        {
            this.enabled = false;
        }
        else
        {
            points_to_win = 0;
            current_score = 0;
            rounds_won = 0;
            current_percentage = 0;
            set = false;
            scoreboard = null;
            StartCoroutine(locateScoreboard());
        }
        base.OnStartClient();
    }

    private void Update()
    {
        Debug.Log($"Points: {current_score}/{points_to_win} = {current_percentage}");
    }

    public void AddPoints(float points)
    {
        current_score += points;
        if (current_score <= 0)
        {
            current_percentage = 0;
            current_score = 0;
        }
        else
        {

            current_percentage = (current_score / points_to_win);
            scoreboard.updateScore(current_percentage, base.Owner);

        }
    }

    [TargetRpc]
    public void TRPC_SetUpRoundScore(NetworkConnection target, float winning_points)
    {
        points_to_win = winning_points;
        current_score = 0;
        current_percentage = 0;
    }

    IEnumerator locateScoreboard()
    {

        scoreboard = FindAnyObjectByType<ScoreBoard>();
        while (scoreboard == null)
        {
            yield return null;
            Debug.Log("Looking for scoreboard");
            scoreboard = FindAnyObjectByType<ScoreBoard>();
        }
        Debug.Log("Found scoreboard");
        scoreboard.spawnPlayerScore(base.Owner);
    }

    // Start is called before the first frame update


}
