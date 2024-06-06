using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonAdd : MonoBehaviour
{
    public Transform Parent;
    public GameObject PrefabServerButton;
    public Dictionary<string, GameObject> buttonAdded;

    void Start()
    {
        buttonAdded = new Dictionary<string, GameObject>();
    }

    void LateUpdate()
    {
        foreach (KeyValuePair<string, string> server in ClientScript.serversFound)
        {
            if (!buttonAdded.ContainsKey(server.Key))
            {
                AddButton(server.Value, server.Key);
            }
        }

        // foreach (KeyValuePair<string, GameObject> button in buttonAdded)
        // {
        //     if (!ClientScript.serversFound.ContainsKey(button.Key))
        //     {
        //         // Destroy object
        //     }
        // }  
    }

    void AddButton(string name, string ip)
    {
        GameObject newButton = Instantiate(PrefabServerButton);
        newButton.GetComponentInChildren<TMP_Text>().text = name + " - " + ip;
        
        SelectServerButton script = newButton.GetComponentInChildren<SelectServerButton>();
        script.Initialize(ip);
        
        newButton.transform.SetParent(Parent);

        buttonAdded.Add(ip, newButton);
    }

}
