using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldPower : NetworkBehaviour
{
    [SerializeField] public GameObject _shieldPrefab;
    [HideInInspector] public bool _isShielded = false;
    [HideInInspector] bool _init = true;
    [HideInInspector] public ShieldCollision _shieldObj;
    [SerializeField] ManaController _manaController;
    private float initTimer = 10f;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<ShieldPower>().enabled = false;
            return;
        }
        //StartCoroutine(SpawnShield());

    }

    // Update is called once per frame
    void Update()
    {
        if (_shieldObj == null)
        {
            _shieldObj = gameObject.GetComponentInChildren<ShieldCollision>();
            StartCoroutine(SpawnShield());
        }
        else
        {
            if (_init)
            {
                if (initTimer > 0)
                    initTimer -= Time.deltaTime;
                if (initTimer <= 0)
                {
                    SRPC_ActivateShield(_shieldObj, false);
                    _init = false;
                }
            }

            if (_manaController.playerMana <= 0f)
            {
                _manaController.NotEnoughMana();
                _isShielded = false;
                SRPC_ActivateShield(_shieldObj, false);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (_manaController.playerMana > _manaController.shieldCost)
                {

                    _isShielded = true;
                    SRPC_ActivateShield(_shieldObj, true);
                }
                else
                {
                    _manaController.NotEnoughMana();
                    _isShielded = false;
                    SRPC_ActivateShield(_shieldObj, false);
                }
            }
            else if (_isShielded && Input.GetKeyUp(KeyCode.Mouse1))
            {
                _isShielded = false;
                SRPC_ActivateShield(_shieldObj, false);
            }
            //else if (_init && !_isShielded && !Input.GetKey(KeyCode.Mouse1))
            //{
            //    if (_shieldObj.GetComponent<MeshRenderer>().enabled)
            //        SRPC_ActivateShield(_shieldObj, false);
            //    else
            //        _init = false;
            //}
        }
        _manaController.isShielding = _isShielded;
    }
    private IEnumerator SpawnShield()
    {
        yield return new WaitForSeconds(1);
        //SRPC_ActivateShield(_shieldObj, false);

        yield return null;
    }
    [ObserversRpc]
    void ORPC_ActivateShield(ShieldCollision script, bool bActivate)
    {
        if (script != null)
        {
            script.ActivateShield(bActivate);
        }
        else
        {
            Debug.LogError("ERR ORPC ShieldPower - Missing script");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void SRPC_ActivateShield(ShieldCollision script, bool bActivate)
    {
        if (script != null)
        {
            script.ActivateShield(bActivate);
            ORPC_ActivateShield(_shieldObj, bActivate);
        }
        else
        {
            Debug.LogError("ERR SRPC ShieldPower - Missing script");
        }
    }
}
