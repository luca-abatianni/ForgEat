using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTag : NetworkBehaviour
{
    [SerializeField]
    private Transform tag_transform;
    [SerializeField]
    private TextMeshProUGUI name_tag;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            ORPC_setName(MenuChoices.playerName);
            this.enabled = false;
        }
    }

    [Client]
    void Update()
    {
        if (!base.IsOwner)
        {
            tag_transform.LookAt(Camera.main.transform);
        }
    }

    [ObserversRpc(RunLocally = false)]
    void ORPC_setName(string name)
    {
        this.name_tag.text = name;
        return;
    }
}
