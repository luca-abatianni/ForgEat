using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailedScoreEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI player_name;

    [SerializeField]
    private TextMeshProUGUI rounds_won;

    [SerializeField]
    private TextMeshProUGUI score;

    public float score_value = 0;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Color highlight_colour;


    public void SetRoundsWon(int value)
    {
        rounds_won.text = value.ToString();
    }

    public void SetScore(float value)
    {
        score.text = value.ToString();
        this.score_value = value;
    }

    public void SetName(string name)
    {
        player_name.text = name;
    }

    public void HighlightPlayer()
    {
        this.image.color = highlight_colour;    
    }

}
