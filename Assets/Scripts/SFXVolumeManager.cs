using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SFXVolumeManager : MonoBehaviour
{
    TMP_Text lenText;
    void Start() {
        lenText = GetComponent < TMP_Text > ();
    }

    public void TextUpdate(float value) {
        lenText.SetText(Mathf.RoundToInt(value*100) + "%");
    }
}
