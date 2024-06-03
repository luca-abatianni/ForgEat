using UnityEngine;
using FishNet.Transporting.Tugboat;

public class MenuChoices : MonoBehaviour
{
    public static MenuChoices Instance;
    public static string serverIP;
    public static float firstPhaseLen = 10;
    public static float secondPhaseLen = 5;
    public static bool amIServer = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void SetServerIP (string serverIP_selected) 
    {
        Debug.Log("SetupMenu script received " + serverIP_selected + " as serverIp");
        serverIP = serverIP_selected;
    }

    public void SetFirstPhaseLen(float len)
    {
        Debug.Log(len);
        firstPhaseLen = len;
    }

    public static void SetSecondPhaseLen(float len)
    {
        Debug.Log(len);
        secondPhaseLen = len;
    }

    public static void SetMeAsServer() 
    {
        Debug.Log("You have been setted as server in MainManager");
        amIServer = true;
    }
}
