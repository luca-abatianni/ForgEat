using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectServerButton : MonoBehaviour
{
    public TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        if (ClientScript.serverText != null)
            text.text = ClientScript.serverText;
        else 
            text.text = "No server found";
    }

    public void onClickServerOption()
    {
        if (ClientScript.serverText != null){
            SetupMenu.SetServerIP(text.text);
            Debug.Log("Correctly selected server " + text.text);
        }
            
    }
}
