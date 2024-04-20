using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System;
using UnityEngine.UI;

public class MagicPower : NetworkBehaviour
{
    [SerializeField]
    private float magic_effect_period, cooldown_period, max_target_distance;

    PowerUI powerUI;

    private bool on_cooldown;
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<MagicPower>().enabled = false;
            return;
        }
        on_cooldown = false;
        if (!this.enabled) this.enabled = true;
    }

    private void Start()
    {
        powerUI = FindObjectOfType<PowerUI>();
        if (powerUI)
        {
            Debug.Log("Power UI found at Start()");
            powerUI.ready();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (on_cooldown)
        {
            Debug.Log("On cooldown...");
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Transform pov_t = Camera.main.transform;
            if (base.IsHost)
            {
                ShootMagic(pov_t.position, pov_t.forward);
            }
            else
            {
                Server_ShootMagic(pov_t.position, pov_t.forward);
            }
            StartCoroutine(CooldownTimer());
        }
    }

    private void ShootMagic(Vector3 position, Vector3 direction)
    {
        int players_layer = 1 << 3;
        RaycastHit hit;
        PlayerController player_controller;
        if (!Physics.Raycast(position, direction, out hit, max_target_distance, players_layer)) return;
        player_controller = hit.transform.gameObject.GetComponent<PlayerController>();
        if (player_controller != null)
        {
            if (player_controller.canMove)
            {
                StartCoroutine(MagicEffectTimer(magic_effect_period, player_controller));
            }
        }
        return;
    }

    [ServerRpc(RequireOwnership = false)]
    private void Server_ShootMagic(Vector3 position, Vector3 direction)
    {
        int players_layer = 1 << 3;
        RaycastHit hit;
        PlayerController player_controller;
        if (!Physics.Raycast(position, direction, out hit, max_target_distance, players_layer)) return;
        player_controller = hit.transform.gameObject.GetComponent<PlayerController>();
        if (player_controller != null )
        {
            if (player_controller.enabled)
            {
                StartCoroutine(MagicEffectTimer(magic_effect_period, player_controller));
            }
        }
        return;
    }

    IEnumerator MagicEffectTimer(float time, PlayerController player_controller)
    {
        NetworkConnection net_connection = player_controller.GetComponent<NetworkObject>().Owner;
        Debug.Log("Magic effect timer started");
        player_controller.SetCanMove(net_connection, false);
        yield return new WaitForSeconds(time);
        Debug.Log("Magic effet timer stopped.");
        player_controller.SetCanMove(net_connection, true);
        yield return null;
    }

    IEnumerator CooldownTimer() 
    {
        on_cooldown = true;
        powerUI.cooldown();
        yield return new WaitForSeconds(cooldown_period);
        on_cooldown = false;
        powerUI.ready();
        yield return null;
    }
}
