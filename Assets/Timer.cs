using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    private bool running = false;
    private Coroutine t_coroutine;

    [SerializeField]
    private TextMeshProUGUI timer_ui;

    [ObserversRpc]
    public void ORPC_StartTimer(int time, uint start_tick)
    {
        if (running)
        {
            StopCoroutine(t_coroutine);
            running = false;
        } 
        t_coroutine = StartCoroutine(TimerCoroutine(time, start_tick));
    }

    IEnumerator TimerCoroutine(int time, uint start_tick)
    {
        string str_min = "00", str_sec = "00";
        running = true;
        timer_ui.color = Color.white;

        float total_time_diff = (float)(TimeManager.Tick - start_tick) / TimeManager.TickRate;
        int seconds_diff = (int)total_time_diff;
        float decimal_diff = total_time_diff - seconds_diff;

        time -= seconds_diff;

        int minutes = time / 60;
        int seconds = time % 60;

        Debug.Log($"Seconds diff {seconds_diff}, decimal_diff {decimal_diff}. Total diff {total_time_diff}");
        yield return new WaitForSeconds(decimal_diff);

        while (minutes >= 0)
        {
            while (seconds > 0)
            {
                if (seconds < 20)
                {
                    if (minutes == 0)
                    {
                        float t = 1f - (seconds / 20f); // Normalized value (0 to 1) as timeRemaining goes from 20 to 0
                        timer_ui.color = Color.Lerp(Color.white, Color.red, t);
                    }
                }

                if (seconds < 10) str_sec = "0" + seconds.ToString();
                else str_sec = seconds.ToString();
                timer_ui.text = str_min + ":" + str_sec;
                yield return new WaitForSeconds(1f);
                seconds--;
            }
            minutes -= 1;
            if (minutes < 0)
            {
                timer_ui.text = "00:00";
                break;
            }
            seconds = 59;
            if (minutes < 10) str_min = "0" + minutes.ToString();
            else str_min = minutes.ToString();
            timer_ui.text = str_min + ":" + str_sec;
        }

        timer_ui.color = Color.white;
        yield return null;
    }

    /*
    IEnumerator ttttt(int time, uint start_tick)
    {
        string str_min, str_sec;
        running = true;
        float time_diff = (float)(TimeManager.Tick - start_tick) / TimeManager.TickRate;

        int minutes = time / 60;
        int seconds = time % 60;
        if (minutes < 10) str_min = "0" + minutes.ToString();
        else str_min = minutes.ToString();
        if (seconds < 10) str_sec = "0" + seconds.ToString();
        else str_sec = seconds.ToString();
        if (seconds < 20)
        {
            if (minutes == 0)
            {
                float t = 1f - (seconds / 20f); // Normalized value (0 to 1) as timeRemaining goes from 20 to 0
                timer_ui.color = Color.Lerp(Color.white, Color.red, t);
            }
        }
        timer_ui.text = str_min + ":" + str_sec;
        yield return new WaitForSeconds(1f);
        while (minutes >= 0)
        {
            while (seconds > 0)
            {
                yield return new WaitForSeconds(1f);
                seconds--;
                if (seconds < 20)
                {
                    if (minutes == 0)
                    {
                        float t = 1f - (seconds / 20f); // Normalized value (0 to 1) as timeRemaining goes from 20 to 0
                        timer_ui.color = Color.Lerp(Color.white, Color.red, t);
                    }
                }
                if (seconds < 10) str_sec = "0" + seconds.ToString();
                else str_sec = seconds.ToString();
                timer_ui.text = str_min + ":" + str_sec;
            }
            minutes -= 1;
            seconds = 59;
            if (minutes < 0)
            {
                timer_ui.text = "00:00";
                break;
            }
            if (minutes < 10) str_min = "0" + minutes.ToString();
            else str_min = minutes.ToString();
            timer_ui.text = str_min + ":" + str_sec;
        }
        timer_ui.color = Color.white;
        //yield return new WaitForSeconds(time);
        running = false;
        NetworkManager.Log("Timer finished.");
        yield return null;
    }
    */
}
