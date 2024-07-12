using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameConfigurator : MonoBehaviour
{
    public static TMP_InputField inputField;
    public static string playerName;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        playerName = "";
    }

    public static void UpdateField(string newPlayerName)
    {
        inputField.SetTextWithoutNotify(newPlayerName);
        playerName = newPlayerName;
    }
}
