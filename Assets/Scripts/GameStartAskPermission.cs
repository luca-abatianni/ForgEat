using System.Collections;
using System.Collections.Generic;
using FishNet.Example;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartAskPermission : MonoBehaviour
{
    private UdpClient Server;
    private const int port = 8899;

    // Start is called before the first frame update
    void Awake()
    {
        Server = new UdpClient(port);
        Server.BeginReceive(OnReceive, null);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(WaitABitThenCloseFakeServer(20));
    }

    IEnumerator WaitABitThenCloseFakeServer(float duration)
    {
        Debug.Log("Waiting " + duration);
        yield return new WaitForSeconds(duration);
        Debug.Log("Closing server after " + duration);
        Server.Close();
    }
}
