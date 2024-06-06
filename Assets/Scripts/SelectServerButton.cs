using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectServerButton : MonoBehaviour
{
    public TMP_Text text;
    private string ip;

    public void Initialize(string tmpIp)
    {
        ip = tmpIp;
    }

    public void OnClickServerOption()
    {
        if (ClientScript.serversFound.ContainsKey(ip)){
            MenuChoices.SetServerIP(ip);
            Debug.Log("Correctly selected server " + ip);
        }
            
    }
}
