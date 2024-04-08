using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using FishNet.Object;

public class ScoreCounter : NetworkBehaviour
{
    private int score;
    public TextMeshProUGUI tmpro;
    private GameObject player;
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            GetComponent<ScoreCounter>().enabled = false;
    }


    // Update is called once per frame
    public void AddPoints(int amount)
    {

        score += amount;
        UpdateText();
    }

    public void SetPoints(int amount)
    {
        score = amount;
        UpdateText();
    }

    void UpdateText()
    {
        tmpro.text = "Score: " +  score;
    }

}
