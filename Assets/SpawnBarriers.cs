using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using System.Threading;

public class SpawnBarriers : NetworkBehaviour
{
    private List<GameObject> barriers = new List<GameObject>(); 
    private bool barriers_on;
    // Start is called before the first frame update

    public override void OnStartServer()
    {
        base.OnStartServer();
        barriers_on = true;
        foreach (Transform t in transform)
        {
            barriers.Add(t.gameObject);
        }

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        barriers_on = true;
        foreach (Transform t in transform)
        {
            barriers.Add(t.gameObject);
        }
        if (!base.IsOwner)
        {
            GetComponent<SpawnBarriers>().enabled = false;
        }

    }

    [ObserversRpc(BufferLast = true)]
    public void BarriersOn()
    {
        if (!barriers_on) SetBarriers(true);
    }

    [ObserversRpc(BufferLast = true)]
    public void BarriersOff()
    {
        if (barriers_on) SetBarriers(false);
    }

    private void SetBarriers(bool setting)
    {
        barriers_on = setting;
        foreach (GameObject barrier in barriers)
        {
            barrier.GetComponent<BoxCollider>().enabled = setting;
        }
    }
}
