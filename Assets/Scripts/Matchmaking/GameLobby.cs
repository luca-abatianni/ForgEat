using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class GameLobby : NetworkBehaviour
{
    #region Init & Update
    protected virtual void Awake()
    {
        Locator.RegisterService<GameLobby>(this);

        // Register connection states events
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerStateChanged;
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientStateChanged;
        InstanceFinder.ClientManager.OnClientConnectionState += OnLocalClientStateChanged;
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("We're in override onstartclient");
        Locator.GetService<LobbyScene>().InitClient(this);
    }
    public override void OnStartServer()
    {
        // We subscribe to the scene manager (to scene-related events)
        // Unsubscribe firts in case OnStopServer is not called
        Debug.Log("We're in override onstartserver");
        ChangeSubscriptions(false);
        ChangeSubscriptions(true);
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        ChangeSubscriptions(false);
    }
    #endregion

    #region Events
    // Scenes
    private void OnClientLoadedScene(SceneLoadEndEventArgs obj)
    {

    }
    private void OnClientSceneState(ClientPresenceChangeEventArgs obj)
    {

    }
    private void ChangeSubscriptions(bool subscribe)
    {
        if (base.NetworkManager == null)
            return;

        if (subscribe)
        {
            base.NetworkManager.SceneManager.OnLoadEnd += OnClientLoadedScene;
            base.NetworkManager.SceneManager.OnClientPresenceChangeEnd += OnClientSceneState;
        }
        else
        {
            base.NetworkManager.SceneManager.OnLoadEnd -= OnClientLoadedScene;
            base.NetworkManager.SceneManager.OnClientPresenceChangeEnd -= OnClientSceneState;
        }
    }

    // Connection states
    private void OnServerStateChanged(ServerConnectionStateArgs obj)
    {

    }
    private void OnClientStateChanged(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
    {

    }
    private void OnLocalClientStateChanged(FishNet.Transporting.ClientConnectionStateArgs obj)
    {

    }
    #endregion

    #region SignIn
    [Client] // Ensures that the method is running on client only
    public void SignIn()
    {
        ServerSignIn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSignIn(NetworkConnection sender = null)
    {
        // Assign a username to this new connection
        bool success = OnSignIn(sender.ClientId, out string username, out string failedReason);

        if (success)
        {
            TargetSignInSuccess(sender, username);
        }
        else 
        {
            TargetSignInFailed(sender, failedReason);
        }
    }
    private bool OnSignIn(int clientId, out string username, out string failedReason)
    {
        username = "Mago " + clientId;
        failedReason = "This can't fail";
        return true;
    }

    // We send back the sign-in result, as a success or as a failure
    [TargetRpc]
    private void TargetSignInSuccess(NetworkConnection conn, string username)
    {
        Locator.GetService<LobbyScene>().SignInSuccess(username);
    }
    [TargetRpc]
    private void TargetSignInFailed(NetworkConnection conn, string failedReason)
    {
        Locator.GetService<LobbyScene>().SignInFailed(failedReason);
    }
    #endregion
}
