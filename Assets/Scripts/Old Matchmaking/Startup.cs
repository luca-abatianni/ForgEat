using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [SerializeField] private NetworkObject lobbyNetworkPrefab;
    private NetworkManager networkManager;
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();

        Multipass mp = GetComponent<Multipass>();

        mp.SetClientTransport<Tugboat>();

        // Register events (connections of servers, clients...)
        networkManager.ServerManager.OnServerConnectionState += ServerStateChanged;
    }

    void ServerStateChanged(ServerConnectionStateArgs obj)
    {
        if (obj.ConnectionState != LocalConnectionState.Started)
            return;
        if (!networkManager.ServerManager.OneServerStarted())
            return;

        Debug.Log("Instantiating networked lobby");

        NetworkObject nob = Instantiate(lobbyNetworkPrefab);
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Main");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(nob.gameObject, scene);
        networkManager.ServerManager.Spawn(nob.gameObject);
    }
}
