using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailedScoreEntry : MonoBehaviour
{

    private TextMeshPro name;
    private TextMeshPro rounds_won;
    private TextMeshPro score;

    public Color highlight_colour;
    //public Color base_colour;

    private void Awake()
    {
        name = transform.Find("Name").GetComponent<TextMeshPro>();
        score = transform.Find("Score").GetComponent<TextMeshPro>();
        rounds_won = transform.Find("Rounds won").GetComponent<TextMeshPro>();
    }
    public void SetRoundsWon(string rounds_won)
    {
        this.rounds_won.text = rounds_won;
    }

    public void SetScore(string rounds_won)
    {
        this.rounds_won.text = rounds_won;
    }

    public void SetName(string rounds_won)
    {
        this.rounds_won.text = rounds_won;
    }

    public void HighlightPlayer()
    {
        Color color = Color.white;
        color.a = 0.5f;
        this.GetComponent<Image>().color = highlight_colour;
    }

}
