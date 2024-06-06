using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeletransportPlayer : MonoBehaviour
{
    Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    public void TransportPlayerToPosition (Vector3 position)
    {
        if (rb == null) rb = this.GetComponent<Rigidbody>();
        rb.position = position;
        return;
    }
}
