using System.Collections;
using System.Collections.Generic;
using FishNet.Example;
using Unity.VisualScripting;
using UnityEngine;

public class ClientServerSetter : MonoBehaviour
{
    GameObject GameManager;
    NetworkHudCanvases canvas;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        canvas = GameObject.FindFirstObjectByType<NetworkHudCanvases>();
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
        if (canvas != null)
        {
            if (MenuChoices.amIServer)
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
