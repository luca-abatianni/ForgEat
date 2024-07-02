using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ClientScript : MonoBehaviour
{
    private static bool startSearch = false;

    private static UdpClient Client;
    private IPEndPoint broadcast_endpoint;
    private const int port = 8888;
    private const float broadcast_interval = 1f;
    private float broadcast_timer;

    public static Dictionary<string, string> serversFound;

    private List<IPAddress> GetEndpoints()
    {
        List<IPAddress> AddressList = new List<IPAddress>();
        NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach(NetworkInterface I in Interfaces)
        {
            //I.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
            if ((I.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && I.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var Unicast in I.GetIPProperties().UnicastAddresses)
                {
                    if (Unicast.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        AddressList.Add(Unicast.Address);
                    }
                }
            }
        }
        return AddressList;
    }

    public void OnClientSearchStart()
    {
        startSearch = true;
        serversFound = new Dictionary<string, string>();

        // List<IPAddress> list = GetEndpoints();
        // foreach (IPAddress l in list)
        // {
        //     Debug.Log("Address found: " + l.ToString());
        // }

        IPAddress ipAddress = GetEndpoints()[0];
        IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 11000);

        Client = new UdpClient(ipLocalEndPoint);
        Client.EnableBroadcast = true;
        broadcast_endpoint = new IPEndPoint(IPAddress.Broadcast, port);
        Debug.Log("Broadcast to " + IPAddress.Broadcast + " at port " + port);
        broadcast_timer = broadcast_interval;
    }

    void Update()
    {
        if (startSearch) 
        {
            broadcast_timer -= Time.deltaTime;
            if (broadcast_timer <= 0f)
            {
                serversFound.Clear();
                SendMatchBroadcast();
                broadcast_timer = broadcast_interval;
            }
        }
    }

    void SendMatchBroadcast()
    {
        string message = "FORGEAT-CLIENT-BROADCAST";
        byte[] data = Encoding.UTF8.GetBytes(message);
        Client.Send(data, data.Length, broadcast_endpoint);
        Debug.Log("Broadcasting server presence...");
        Client.BeginReceive(OnReceive, null);
    }

    void OnReceive(IAsyncResult result) 
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, port);
        byte[] data = Client.EndReceive(result, ref sender);
        string message = Encoding.UTF8.GetString(data);
        Debug.Log(message);

        if (message.Contains("FORGEAT-SERVER-RESPONSE"))
        {
            Debug.Log($"Server found at {sender.Address}");
            string serverText = sender.Address.ToString();

            // Nella stringa invio sia il messaggio sia il nome della partita
            int substringLen = "FORGEAT-SERVER-RESPONSE/".Length;
            string gameName = message.Remove(0, substringLen);
            
            if(!serversFound.ContainsKey(serverText))
            {
                if (gameName == "")
                    gameName = GetRandomGameName();
                    
                serversFound.Add(serverText, gameName);
            }
        } 
        
        Client.BeginReceive(OnReceive, null);
    }

    string GetRandomGameName()
    {
        var random = new System.Random();
        var list = new List<string>{"MazzAglio", "MazZuccaGlia", "BatteGazzosa", "BatTheGazzorre"};

        return list[random.Next(list.Count)];
    }  

    // Se il client torna al menu principale ferma il broadcast
    public static void OnBackButtonClickedClient()
    {
        startSearch = false;
        Client.Close();
    }

    void OnApplicationQuit()
    {
        startSearch = false;
        Client.Close();
    }
}