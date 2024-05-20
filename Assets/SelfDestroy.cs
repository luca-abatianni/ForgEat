using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : NetworkBehaviour
{
    public float timeout = 1f;
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
            Destroy(gameObject);
        }
        if (Time.time > timer)
        {
            Despawn(gameObject);
            Destroy(gameObject);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void Despawn(GameObject obj)
    {
        ServerManager.Despawn(obj);
    }
}
