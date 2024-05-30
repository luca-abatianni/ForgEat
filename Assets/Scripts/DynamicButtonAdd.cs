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
    public bool buttonAdded = false;

    void Update()
    {
        if (ClientScript.serverText != null && ClientScript.serverText != "" && !buttonAdded)
        {
            AddButton(ClientScript.serverText);
            buttonAdded = true;
        }    
    }

    void AddButton(string text)
    {
        GameObject newButton = Instantiate(PrefabServerButton);
        newButton.GetComponentInChildren<TMP_Text>().text = text;
        
        newButton.transform.SetParent(Parent);
    }   
}
