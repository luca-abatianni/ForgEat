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
    private ScoreBoard scoreboard;

    public override void OnStartClient()
    {
        if (!base.IsOwner)
        {
            this.enabled = false;
        }
        else
        {
            StartCoroutine(locateScoreboard());
        }
        base.OnStartClient();
    }

    public void AddPoints(float points)
    {
        scoreboard.addPoints(points, base.Owner);
    }

    IEnumerator locateScoreboard()
    {

        scoreboard = FindAnyObjectByType<ScoreBoard>();
        while (scoreboard == null)
        {
            yield return null;
            scoreboard = FindAnyObjectByType<ScoreBoard>();
        }
        scoreboard.spawnPlayerScore(base.Owner, MenuChoices.playerName);
    }

    // Start is called before the first frame update


}
