using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class TimeRemainingManager : NetworkBehaviour
{
    private float TimeRemaining = GameManager.phase_one_period;
    //private bool TimerIsRunning = false;
    public TMP_Text text;

    void Start()
    {
        //TimerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool TimerIsRunning = GameManager.phase_timer;
        if (!TimerIsRunning)
        {
            if (TimeRemaining > 0)
            {
                Debug.Log("ENTERED IF and time remaining");
                TimeRemaining -= Time.deltaTime;
                UpdateText();
            }
            else
            {
                Debug.Log("Time has run out!");
                TimeRemaining = GameManager.phase_two_period;
                //TimerIsRunning = false;
            }
        }
    }

    void UpdateText()
    {
        text.SetText("Time left: " +  TimeRemaining.ToString());
    }
}
