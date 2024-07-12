using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondPhaseLenManager: MonoBehaviour {
  TMP_Text lenText;
  void Start() {
    lenText = GetComponent < TMP_Text > ();
  }

  public void TextUpdate(float value) {
    lenText.SetText(Mathf.RoundToInt(value) + " m");
  }
}