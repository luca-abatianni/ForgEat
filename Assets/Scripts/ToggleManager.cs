using System.Net;
using AddressFamily=System.Net.Sockets.AddressFamily;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Transporting.Tugboat;

public class ToggleManager : MonoBehaviour
{
    public Toggle m_Toggle;

    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<Toggle>();
        //Add listener for when the state of the Toggle changes, and output the state
        m_Toggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(m_Toggle);
            });
    }

    //Output the new state of the Toggle into Text when the user uses the Toggle
    void ToggleValueChanged(Toggle change)
    {
        if(m_Toggle.isOn){
            SetupMenu.SetServerIP(GetLocalAddress());
            InputFieldManager.UpdateField();
        } else {
            InputFieldManager.ResetField();
        } 
    }

    public string GetLocalAddress()
    {
        string serverIP = "";

        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList) {
            if (ip.AddressFamily==AddressFamily.InterNetwork) {
                serverIP=ip.ToString();
                break;
            } 
        } 

        return serverIP;
    }
}
