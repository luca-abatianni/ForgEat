using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public int ServerIP;
    public static bool amIServer = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void SetMeAsServer() 
    {
        Debug.Log("You have been setted as server in MainManager");
        amIServer = true;
    }
}
