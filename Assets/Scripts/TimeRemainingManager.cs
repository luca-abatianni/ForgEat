using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class TimeRemainingManager : NetworkBehaviour
{
    private float TimeRemaining = GameManager.phase_one_period;
    private bool TimerIsRunning = false;
    public TextMeshProUGUI tmpro;

    void Start()
    {
        TimerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerIsRunning)
        {
            if (TimeRemaining > 0)
            {
                TimeRemaining -= Time.deltaTime;
                UpdateText();
            }
            else
            {
                Debug.Log("Time has run out!");
                TimeRemaining = GameManager.phase_one_period;
                TimerIsRunning = false;
            }
        }
    }

    void UpdateText()
    {
        tmpro.text = "Time left: " +  TimeRemaining;
    }
}
