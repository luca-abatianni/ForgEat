using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayersNumberManager : MonoBehaviour
{
    // prova
    TMP_Text lenText;
    void Start() {
        lenText = GetComponent < TMP_Text > ();
    }

    public void TextUpdate(float value) {
        lenText.SetText(Mathf.RoundToInt(value).ToString());
    }
}
