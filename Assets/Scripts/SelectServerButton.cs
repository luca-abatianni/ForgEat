using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectServerButton : MonoBehaviour
{
    public TMP_Text text;

    public void OnClickServerOption()
    {
        if (ClientScript.serverText != null){
            SetupMenu.SetServerIP(text.text);
            Debug.Log("Correctly selected server " + text.text);
        }
            
    }
}
