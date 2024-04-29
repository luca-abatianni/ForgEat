using UnityEngine;
using FishNet.Transporting.Tugboat;

public class SetupMenu : MonoBehaviour
{
    public static string serverIP;
    public static float firstPhaseLen;
    public static int secondPhaseLen;

    public static void SetServerIP (string serverIP_selected) 
    {
        Debug.Log(serverIP_selected);
        serverIP = serverIP_selected;
    }

    public void SetFirstPhaseLen(float len)
    {
        Debug.Log(len);
        firstPhaseLen = len;
    }

    public static void SetSecondPhaseLen(int len)
    {
        Debug.Log(len);
        secondPhaseLen = len;
    }
}
