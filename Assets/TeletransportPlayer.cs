using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeletransportPlayer : NetworkBehaviour
{
    CharacterController cc;

    private void Start()
    {
        cc = this.GetComponent<CharacterController>();
    }

    [ObserversRpc]
    public void TransportPlayerToPosition (Vector3 position)
    {
        if (!base.IsOwner) return;
        if (cc == null) cc = this.GetComponent<CharacterController>();
        cc.Move(position);
        Debug.Log("TELEPORTING");
    }
}
