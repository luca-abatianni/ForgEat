using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ServerScript : MonoBehaviour
{
    private UdpClient Server;
    private const int port = 8888;

    public void OnServerStart()
    {
        Server = new UdpClient(port);
        Server.BeginReceive(OnReceive, null);
        Debug.Log("Started server");
    }

    void OnReceive(IAsyncResult result)
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, port);
        byte[] data = Server.EndReceive(result, ref sender);
        string message = Encoding.UTF8.GetString(data);

        if (message == "FORGEAT-CLIENT-BROADCAST")
        {
            Debug.Log($"Client {sender.Address} has reached the server");
            string responseMessage = "FORGEAT-SERVER-RESPONSE/" + GameNameConfigurator.gameName;
            byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
            Server.Send(responseData, responseData.Length, sender);
        } 
        
        Server.BeginReceive(OnReceive, null);
    }

    void OnApplicationQuit()
    {
        Server.Close();
    }
}