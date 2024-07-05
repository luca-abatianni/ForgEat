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
    [SerializeField] private PerformantShoot performant_shoot;
    [SerializeField] private float _cooldown = 0f;
    [SerializeField] ManaController _manaController;
    public Animator animator;
    public NetworkAnimator netAnim;
    int _powerCost = 0;
    [SerializeField] private CanvasGroup _powerCanvasGroup = null;
    private bool _powerCanvasInit = true;
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
        if (_powerCanvasGroup == null)
            _powerCanvasGroup = GameObject.FindGameObjectWithTag("PowerSelectionGroup").GetComponent<CanvasGroup>();
        else if (_powerCanvasGroup != null && _powerCanvasInit)
        {
            _powerCanvasInit = false;
            UpdatePowerHUD(PowerBehavior.PowerType.IceBullet);
        }
        if (Time.time > _cooldown && Input.GetKeyDown(KeyCode.Mouse0) && performant_shoot._listEffects.Count > 0)
        {
            if (_manaController.playerMana > _powerCost)
            {
                _manaController.playerMana -= _powerCost;
                float offset = 1f;
                var player_controller = GetComponent<PlayerController>();
                _cooldown = Time.time + .5f;
                if (player_controller.isRunning)
                {
                    offset = 3.5f;
                }
                else if (player_controller.isWalking)
                {
                    offset = 2.5f;
                }


                Debug.Log("Offset: " + offset);
                performant_shoot.Shoot(offset);
                animator.SetBool("attackFreeze", true);
            }
            else
            {
                _manaController.NotEnoughMana();
                animator.SetBool("attackFreeze", false);
            }
        }
        else
        {
            animator.SetBool("attackFreeze", false);
        }
        SwitchPower();
    }
    void SwitchPower()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.IceBullet;
            UpdatePowerHUD(PowerBehavior.PowerType.IceBullet);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.MindBullet;
            UpdatePowerHUD(PowerBehavior.PowerType.MindBullet);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.WindBullet;
            UpdatePowerHUD(PowerBehavior.PowerType.WindBullet);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            performant_shoot._primaryPower = PowerBehavior.PowerType.TrickBullet;
            UpdatePowerHUD(PowerBehavior.PowerType.TrickBullet);
        }
        _powerCost = PowerBehavior.vecPowerCost[(int)performant_shoot._primaryPower];
    }

    void UpdatePowerHUD(PowerBehavior.PowerType powerType)
    {
        int nType = (int)powerType;
        for (int i = 0; i < 4; i++)//Disattivo tutti gli altri e attivo il selezionato
            _powerCanvasGroup.transform.GetChild(i).transform.Find("BackgroundSel").gameObject.SetActive(false);
        var selectedPower = _powerCanvasGroup.transform.GetChild(nType);
        selectedPower.transform.Find("BackgroundSel").gameObject.SetActive(true);
    }
}
