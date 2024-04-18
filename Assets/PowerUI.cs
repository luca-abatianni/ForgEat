using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using FishNet.Object;

public class PowerUI : NetworkBehaviour
{
    public TextMeshProUGUI tmpro;
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            GetComponent<PowerUI>().enabled = false;
    }


    // Update is called once per frame
    public void cooldown()
    {
        UpdateText("ON COOLDOWN");
    }

    public void ready()
    {
        UpdateText("READY");
    }

    public void frozen ()
    {
        UpdateText("FROZEN");
    }

    void UpdateText(string text)
    {
        tmpro.text = "POWER: " + text;
    }

}
