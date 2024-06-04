using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonAdd : MonoBehaviour
{
    public Transform Parent;
    public GameObject PrefabServerButton;
    public bool buttonAdded = false;

    void Update()
    {
        if (ClientScript.serverText != null && ClientScript.serverText != "" && ClientScript.gameName != null &&!buttonAdded)
        {
            AddButton(ClientScript.gameName, ClientScript.serverText);
            buttonAdded = true;
        }    
    }

    void AddButton(string name, string ip)
    {
        if (name == "")
            name = GetRandomGameName();
        
        GameObject newButton = Instantiate(PrefabServerButton);
        newButton.GetComponentInChildren<TMP_Text>().text = name + " - " + ip;
        
        SelectServerButton script = newButton.GetComponentInChildren<SelectServerButton>();
        script.Initialize(ip);
        
        newButton.transform.SetParent(Parent);
    }

    string GetRandomGameName()
    {
        var random = new System.Random();
        var list = new List<string>{"MazzAglio", "MazZuccaGlia", "BatteGazzosa", "BatTheGazzorre"};

        return list[random.Next(list.Count)];
    }   
}
