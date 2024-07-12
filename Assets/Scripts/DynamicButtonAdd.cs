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
    public GameObject Loading;
    public Dictionary<string, GameObject> buttonAdded;

    void Start()
    {
        Debug.Log("DynamicButtonAdd started");
        Loading.SetActive(true);
        buttonAdded = new Dictionary<string, GameObject>();
    }

    void LateUpdate()
    {
        if (buttonAdded.Count > 0)
            Loading.SetActive(false);
            
        
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

        if (buttonAdded.Count == 0 && !Loading.activeSelf)
            Loading.SetActive(true);  
    }

    void AddButton(string name, string ip)
    {
        GameObject newButton = Instantiate(PrefabServerButton);
        newButton.GetComponentInChildren<TMP_Text>().text = name + " - " + ip;
        
        SelectServerButton script = newButton.GetComponentInChildren<SelectServerButton>();
        script.Initialize(ip);
        
        newButton.transform.SetParent(Parent);
        newButton.transform.position = Parent.transform.position;
        newButton.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        newButton.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

        buttonAdded.Add(ip, newButton);
    }

}
