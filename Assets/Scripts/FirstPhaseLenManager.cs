using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstPhaseLenManager: MonoBehaviour 
{
  TMP_Text lenText;
  void Start() {
    lenText = GetComponent < TMP_Text > ();
  }

  public void TextUpdate(float value) {
    lenText.SetText(Mathf.RoundToInt(value) + " s");
  }
}