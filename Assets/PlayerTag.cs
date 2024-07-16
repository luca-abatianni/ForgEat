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
            SRPC_setName(MenuChoices.playerName);
            StartCoroutine(GetColor());
            this.enabled = false;
        }
    }

    IEnumerator GetColor()
    {
        ScoreBoard sb = null;
        Color color = Color.white;
        while(sb == null)
        {
            sb = FindAnyObjectByType<ScoreBoard>();
            yield return null;
        }
        while (color == Color.white)
        {
            color = sb.getPlayerColor(base.Owner);
            yield return null;
        }
        SRPC_setColor(color);
        yield return null;
    }

    [Client]
    void Update()
    {
        if (!base.IsOwner)
        {
            tag_transform.LookAt(Camera.main.transform);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SRPC_setName(string name)
    {
        ORPC_setName(name);
        return;
    }

    [ServerRpc(RequireOwnership = false)]
    void SRPC_setColor(Color color)
    {
        ORPC_setColor(color);
        return;
    }


    [ObserversRpc(RunLocally = false)]
    void ORPC_setName(string name)
    {
        this.name_tag.text = name;
        return;
    }

    [ObserversRpc(RunLocally = false)]
    void ORPC_setColor(Color color)
    {
        this.name_tag.color = color;
        return;
    }

}
