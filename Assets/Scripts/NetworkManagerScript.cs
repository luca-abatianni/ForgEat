using FishNet.Transporting.Tugboat;
using UnityEngine;

public class NetworkManagerScript : MonoBehaviour
{
    public static Tugboat _tugboat;
    string serverIP = SetupMenu.serverIP;

    private void Start() 
    {
        if (TryGetComponent(out Tugboat _t)) {
            _tugboat = _t;
        } else {
            Debug.LogError("Tugboat not found");
            return;
        }

        Debug.Log("Server selected in menu: " + serverIP);
        _tugboat.SetClientAddress(serverIP);
    }

}
