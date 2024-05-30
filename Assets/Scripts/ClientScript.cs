using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientScript : MonoBehaviour
{
    private bool startSearch = false;

    private UdpClient Client;
    private IPEndPoint broadcast_endpoint;
    private const int port = 8888;
    private const float broadcast_interval = 1f;
    private float broadcast_timer;

    public static string serverText;
    public static string gameName;

    public void OnClientSearchStart()
    {
        startSearch = true;

        Client = new UdpClient();
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
            serverText = sender.Address.ToString();
            int substringLen = "FORGEAT-SERVER-RESPONSE/".Length;
            gameName = message.Remove(0, substringLen);
        } 
        
        Client.BeginReceive(OnReceive, null);
    }

    void OnApplicationQuit()
    {
        Client.Close();
    }
}