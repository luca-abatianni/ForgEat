using System.Net;
using AddressFamily=System.Net.Sockets.AddressFamily;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Transporting.Tugboat;
using FishNet.Example;

public class ServerStartScript : MonoBehaviour
{
    public void OnServerClickStartGame()
    {
        string serverIP = "";

        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList) {
            if (ip.AddressFamily==AddressFamily.InterNetwork) {
                serverIP=ip.ToString();
                Debug.Log("We are starting with server " + serverIP);
                break;
            } 
        } 

        if (serverIP != "" && serverIP != null) {
            MenuChoices.serverIP = serverIP;
            MenuChoices.SetMeAsServer();
        }
    }
}
