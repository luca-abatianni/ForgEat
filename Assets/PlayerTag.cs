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

    private string player_name = null;
    private Color color = Color.white;
    public override void OnStartClient()
    {
        base.OnStartClient();
        setName("");
        setColor(color);
        StartCoroutine(GetData());

        if (base.IsOwner)
        {
            this.enabled = false;
        }
    }

    [Client]
    IEnumerator GetData()
    {
        ScoreBoard sb = null;
        while(sb == null)
        {
            sb = FindAnyObjectByType<ScoreBoard>();
            yield return null;
        }
        while (player_name == null)
        {
            player_name = sb.getPlayerName(base.Owner);
            yield return null;
        }
        setName(player_name);

        while (color == Color.white) 
        {
            color = sb.getPlayerColor(base.Owner);
            yield return null;
        }
        setColor(color);

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

    [Client]
    void setName(string name)
    {
        if (name == null) this.name = "";
        else this.name_tag.text = name;
        return;
    }

    [Client]
    void setColor(Color color)
    {
        this.name_tag.color = color;
        return;
    }

}
