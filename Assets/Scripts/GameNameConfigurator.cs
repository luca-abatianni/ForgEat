using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameNameConfigurator : MonoBehaviour
{
    public static TMP_InputField inputField;
    public static string gameName;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public static void UpdateField(string newGameName)
    {
        inputField.SetTextWithoutNotify(newGameName);
        gameName = newGameName;
    }
}
