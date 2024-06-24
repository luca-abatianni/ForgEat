using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Score : NetworkBehaviour
{
    // Start is called before the first frame update

    private int points_to_win;
    public int current_score;
    public int rounds_won;
    private float current_percentage;
    bool set;

    // Update is called once per frame
    public override void OnStartClient()
    {
        if (base.IsOwner)
        {
            points_to_win = 0;
            current_score = 0;
            rounds_won = 0;
            current_percentage = 0;
            set = false;
        }
        base.OnStartClient();

    }
    
    public void AddPoints(int points)
    {
        current_score += points;
        if (current_score < 0)
        {
            current_percentage = 0;
            current_score = 0;
        }
        else
        {
            current_percentage = (current_score / points);
        }
    }

    [TargetRpc] 
    public void SetUpRoundScore(NetworkConnection net_connection, int winning_points)
    {
        points_to_win = winning_points;
        current_score = 0;
        current_percentage = 0;
    }
}
