using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameAnnouncement : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI announcement_ui;
    [SerializeField]
    float fadeout_duration;
    Coroutine announcement_coroutine = null;
    bool showing_announcement;

    [ObserversRpc(BufferLast = true)]
    public void ORPC_RoundWinner(NetworkConnection winner)
    {
        ResetText();
        if (winner.ClientId == InstanceFinder.ClientManager.Connection.ClientId)
        {
            announcement_coroutine = StartCoroutine(timedAnnouncement("You won the round!", 10f));
        }
        else
        {
            announcement_coroutine = StartCoroutine(timedAnnouncement("You lost the round!", 10f));
        }
    }

    [TargetRpc]
    public void TRPC_Announcement(NetworkConnection client, string message, float timeout)
    {
        ResetText();
        announcement_coroutine = StartCoroutine(timedAnnouncement(message, timeout));
    }

    [ObserversRpc(BufferLast = true)]
    public void ORPC_Announcement(string message, float timeout)
    {
        ResetText();
        announcement_coroutine = StartCoroutine(timedAnnouncement(message, timeout));
    }

    [ObserversRpc(BufferLast = true)]
    public void ORPC_DifferentiatedAnnouncement(string message, string alt_message, NetworkConnection alt_messasge_target, float timeout)
    {
        ResetText();
        if (alt_messasge_target.ClientId == InstanceFinder.ClientManager.Connection.ClientId)
        {
            announcement_coroutine = StartCoroutine(timedAnnouncement(alt_message, timeout));
        }
        else
        {
            announcement_coroutine = StartCoroutine(timedAnnouncement(message, 10f));
        }
    }

    [ObserversRpc(BufferLast = true)]
    public void ORPC_ResetText()
    {
        ResetText();
    }

    public void ResetText()
    {
        if (showing_announcement && announcement_coroutine != null) StopCoroutine(announcement_coroutine);
        announcement_ui.text = "";
        announcement_ui.color = Color.white;
    }

    IEnumerator timedAnnouncement(string message, float timeout)
    {
        float t = 1f;
        float fadeout_counter = 0f;
        showing_announcement = true;
        announcement_ui.text = message;

        if (timeout > 0f)
        {

            yield return new WaitForSeconds(timeout); // Showing text for set amount of time.

            while (t > 0)
            {
                t = 1f - (fadeout_counter / fadeout_duration);
                announcement_ui.color = Color.Lerp(Color.clear, Color.white, t);
                yield return new WaitForSeconds(0.1f);
                fadeout_counter += 0.1f;
            }

            showing_announcement = false;
            ResetText();
        }

        yield return null;
    }
}
