using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    private GameLobby lobby;
    private void Awake()
    {
        Locator.RegisterService<LobbyScene>(this);
    }

    public void InitClient(GameLobby lobby)
    {
        Debug.Log("We enter initclient in game lobby");
        this.lobby = lobby;

        lobby.SignIn();
    }

    #region Navigation
    public void ChangeCanvas(string target)
    {

    }

    public void EnterGame()
    {

    }
    public void EnterLobby()
    {

    }

    public void SignInSuccess(string username)
    {
        Debug.Log("SUCCESS: we signed in as " + username);
    }
    public void SignInFailed(string failedReason)
    {
        Debug.Log("FAILURE: we didn't sign in");
    }

    public void OnCreateRoom(Room r)
    {

    }
    public void OnJoinRoom(Room r)
    {
        
    }
    public void OnLeaveRoom()
    {
        
    }
    #endregion

    #region Buttons
    public void OnCreateRoomClick()
    {

    }
    public void OnJoinRoomClick(string roomName)
    {
        
    }
    public void OnLeaveRoomClick()
    {
        
    }
    public void OnStartGameClick()
    {
        
    }
    public void OnRefreshLobbyClick()
    {
        
    }
    #endregion
}
