using System.Collections;
using System.Collections.Generic;
using FishNet.Example;
using Unity.VisualScripting;
using UnityEngine;

public class ClientServerSetter : MonoBehaviour
{
    NetworkHudCanvases canvas;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitABit(1));
    }

    IEnumerator WaitABit(float duration)
    {
        Debug.Log("We are waiting a bit");
        yield return new WaitForSeconds(duration);
        Debug.Log("We ended waiting");
        SetClientAndServer();
    }

    void SetClientAndServer()
    {
        canvas = GameObject.FindFirstObjectByType<NetworkHudCanvases>();
        if (canvas != null)
        {
            Debug.Log("We have a canvas game object, maybe");

            if (MainManager.amIServer)
                canvas.OnClick_Server();
            canvas.OnClick_Client();

            return;
        }
    }
}
