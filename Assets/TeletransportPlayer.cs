using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeletransportPlayer : NetworkBehaviour
{
    Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    [ObserversRpc]
    public void TransportPlayerToPosition (Vector3 position)
    {
        if (!base.IsOwner) return;
        if (rb == null) rb = this.GetComponent<Rigidbody>();
        rb.position = position;
        return;
    }
}
