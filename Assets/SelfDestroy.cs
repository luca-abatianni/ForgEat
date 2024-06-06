using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : NetworkBehaviour
{
    public float timeout = 5f;
    float timer;
    public override void OnStartClient()
    {
        timer = Time.time + timeout;
    }
    void Update()
    {
        if (transform.childCount == 0)
        {
            Despawn(gameObject);
            //Destroy(gameObject);
        }
        //float now = Time.time;
        //if (now > timer)
        //{
        //    Despawn(gameObject);
        //    //Destroy(gameObject);
        //}
    }
    //[ServerRpc(RequireOwnership = false)]
    [ObserversRpc]
    public void Despawn(GameObject obj)
    {
        Debug.Log("Despawn VFX");
        ServerManager.Despawn(obj);
    }
}
