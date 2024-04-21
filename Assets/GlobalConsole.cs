using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;

// Sadly not working... Feel free to fix.
// The idea is to have a way to print a log on every console currently connected.
public class GlobalConsole : NetworkBehaviour
{
    public static GlobalConsole instance;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
    }

    public static GlobalConsole Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GlobalConsole>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("MasterSingleton");
                    instance = singletonObject.AddComponent<GlobalConsole>();
                }
            }

            return instance;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Log(string message)
    {
        Debug.Log("[GLOBAL CONSOLE]: " + message);
        ObserversLog(message);
    }

    [ObserversRpc(BufferLast = true)]
    private void ObserversLog(string message)
    {
        Debug.Log("[GLOBAL CONSOLE]: " + message);
    }
}
