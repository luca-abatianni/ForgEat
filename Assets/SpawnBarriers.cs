using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;

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
        if (base.IsOwner)
        {
            GetComponent<SpawnBarriers>().enabled = false;
        }
    }
    
    public void BarriersOn()
    {
        if (!barriers_on) SetBarriers(true);
    }

    public void BarriersOff()
    {
        if (barriers_on) SetBarriers(false);
    }

    private void SetBarriers(bool setting)
    {
        foreach (GameObject barrier in barriers)
        {
            barrier.SetActive(setting);
        }
    }
}
