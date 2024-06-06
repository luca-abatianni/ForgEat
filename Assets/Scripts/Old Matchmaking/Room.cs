using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    #region Field
    public Room() { }
    public Room(string name, string password, bool lockOnStart, int playerCount)
    {
        this.name = name;
        this.maxPlayers = playerCount;
        this.lockOnStart = lockOnStart;
        this.hasPassword = !string.IsNullOrEmpty(password);
    }

    public string name;
    public int maxPlayers;
    public bool hasPassword;
    public bool lockOnStart;
    public bool isStarted;

    public List<NetworkConnection> Members = new List<NetworkConnection>();
    public List<NetworkObject> MemberIds = new List<NetworkObject>();
    public List<NetworkObject> StartedMembers = new List<NetworkObject>();

    [System.NonSerialized] public GameManager gameManager;
    [System.NonSerialized] public string Password = String.Empty;
    [System.NonSerialized] public HashSet<Scene> Scenes = new HashSet<Scene>();
    #endregion

    #region Members
    public void AddMember(NetworkObject clientId)
    {
        if (!MemberIds.Contains(clientId))
        {
            MemberIds.Add(clientId);
            Members.Add(clientId.Owner);
        }
    }
    public void AddStartedMember(NetworkObject clientId)
    {
        if (!StartedMembers.Contains(clientId))
            StartedMembers.Add(clientId);
    }
    public bool RemoveMember(NetworkObject clientId)
    {
        int index = MemberIds.IndexOf(clientId);
        if (index != -1)
        {
            MemberIds.RemoveAt(index);
            Members.Remove(clientId.Owner);
            StartedMembers.Remove(clientId);
            return true;
        }
        else 
        {
            return false;
        }
    }
    public void ClearMembers()
    {
        MemberIds.Clear();
    }
    #endregion
}
