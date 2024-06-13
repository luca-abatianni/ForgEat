using System.Net;
using System.Net.NetworkInformation;
using AddressFamily=System.Net.Sockets.AddressFamily;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Transporting.Tugboat;
using FishNet.Example;

public class ServerStartScript : MonoBehaviour
{
    // public void OnServerClickStartGame()
    // {
    //     string serverIP = "";

    //     IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
    //     foreach (IPAddress ip in hostEntry.AddressList) {
    //         if (ip.AddressFamily==AddressFamily.InterNetwork) {
    //             serverIP=ip.ToString();
    //             Debug.Log("We are starting with server " + serverIP);
    //             break;
    //         } 
    //     } 

    //     if (serverIP != "" && serverIP != null) {
    //         MenuChoices.serverIP = serverIP;
    //         MenuChoices.SetMeAsServer();
    //     }
    // }

    public void OnServerClickStartGame()
    {
        string serverIP = "";

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            Debug.Log("ni " + ni);
            if (ni.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    Debug.Log("ip " + ip.Address);
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                    {
                        serverIP = ip.Address.ToString();
                        Debug.Log("We are starting with server " + serverIP);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(serverIP))
            {
                break;
            }
        }

        if (!string.IsNullOrEmpty(serverIP))
        {
            MenuChoices.serverIP = serverIP;
            MenuChoices.SetMeAsServer();
        }
        else
        {
            Debug.LogError("No valid IPv4 address found.");
        }
    }

}
