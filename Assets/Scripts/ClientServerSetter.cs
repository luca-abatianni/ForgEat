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
        StartCoroutine(WaitABitThenSet(1));
    }

    IEnumerator WaitABitThenSet(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetServer();
        yield return new WaitForSeconds(duration);
        SetClient();
    }

    void SetServer()
    {
        canvas = GameObject.FindFirstObjectByType<NetworkHudCanvases>();
        if (canvas != null)
        {
            if (MainManager.amIServer)
                canvas.OnClick_Server();
        }
    }

    void SetClient()
    {
        if (canvas != null)
        {
            canvas.OnClick_Client();
        }
    }
}
