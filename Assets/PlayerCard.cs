using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, scoreText;

    public void Initialize(string name)
    {
        nameText.text = name;
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
