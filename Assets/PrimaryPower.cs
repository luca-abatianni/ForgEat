using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Component.Animating;
using UnityEngine.VFX;
using Unity.VisualScripting;
using System.Threading;

public class PrimaryPower : NetworkBehaviour
{
    [SerializeField]
    private PerformantShoot performant_shoot;
    [SerializeField]

    private float _cooldown = 0f;
    public Animator animator;
    public NetworkAnimator netAnim;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<PrimaryPower>().enabled = false;
            return;
        }
        if (performant_shoot._listEffects.Count == 0)
            return;
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time > _cooldown && Input.GetKeyDown(KeyCode.Mouse0) && performant_shoot._listEffects.Count > 0)
        {
            float offset = 0.5f;
            var player_controller = GetComponent<PlayerController>();
            _cooldown = Time.time + 1f;
            animator.SetBool("attackFreeze", true);
            if (player_controller.isRunning)
            {
                offset *= 3;
            }
            else if (player_controller.isWalking)
            {
                offset *= 2;
            }

            Debug.Log("Offset: " + offset);
            performant_shoot.Shoot(offset);
        }
        else
        {
            animator.SetBool("attackFreeze", false);
        }
        SwitchPower();
    }
    void SwitchPower()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || performant_shoot._effectToSpawn == null)
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.IceBullet;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.MindBullet;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.WindBullet;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.TrickBullet;
        }
    }
}
