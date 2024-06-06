using System.ComponentModel.Design;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class NetworkManagerScript : MonoBehaviour
{
    public static Tugboat _tugboat;
    string serverIP = MenuChoices.serverIP;

    public GameObject GameManager;

    void Awake()
    {
        Debug.Log("Is game manager active? " + GameManager.activeSelf);
        if(!GameManager.activeSelf)
        {
            Debug.Log("Game manager is not active");
            GameManager.SetActive(true);
        }
        
    }

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

        GameManager.SetActive(true);
    }

    private void Update()
    {
        Debug.Log("Is game manager active? " + GameManager.activeSelf);
    }

}
