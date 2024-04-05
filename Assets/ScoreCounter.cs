using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScoreCounter : MonoBehaviour
{
    private int score;
    public TextMeshProUGUI tmpro;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        tmpro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void AddPoints(int amount)
    {
        score += amount;
        UpdateText();
    }

    public void RemovePoints(int amount)
    {
        if (score > 0)
        {
            score -= amount;
        }
        UpdateText();
    }

    void UpdateText()
    {
        tmpro.text = "Score: " +  score;
    }

}
