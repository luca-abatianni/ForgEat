using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConnectingText : MonoBehaviour
{
    TMP_Text text;
    int i = 0;
    int j = 0;
    string[] items = { "Connecting", "Connecting.", "Connecting..", "Connecting..." };
    void Start() 
    {
        text = GetComponent < TMP_Text > ();
        ChangeText();
    }

    private async void ChangeText()
    {
        while(true)
        {
            text.SetText(items[i]);
            i++;
            if(i==4)
                i=0;

            await Task.Delay(200);
            j++;
            if (j==20)
                break;
        }

        text.SetText("");
        gameObject.SetActive(false);
        
    }
}
