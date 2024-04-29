using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour
{
    public static TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public static void UpdateField()
    {
        string serverIp = SetupMenu.serverIP;
        inputField.SetTextWithoutNotify(serverIp);
        inputField.interactable = false;
    }

    public static void ResetField()
    {
        inputField.interactable = true;
        inputField.SetTextWithoutNotify("");
    }
}
