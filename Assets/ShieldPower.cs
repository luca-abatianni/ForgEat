using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldPower : NetworkBehaviour
{
    [SerializeField] public GameObject _shieldPrefab;
    [HideInInspector] public bool _isShielded = false;
    [HideInInspector] public ShieldCollision _shieldObj;
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

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _isShielded = true;
                SRPC_ActivateShield(_shieldObj, true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                _isShielded = false;
                SRPC_ActivateShield(_shieldObj, false);
            }
        }
    }
    private IEnumerator SpawnShield()
    {
        yield return new WaitForSeconds(1);
        //SRPC_ActivateShield(_shieldObj, false);

        yield return null;
    }
    //[ObserversRpc]
    //void ORPC_AssignShield(ShieldPower script, GameObject spawned)
    //{
    //    script._shieldObj = spawned.GetComponent<ShieldCollision>();
    //    script._shieldObj.ActivateShield(false);
    //}
    //[ServerRpc]
    //void SRPC_SpawnShield(ShieldPower script, GameObject shield, GameObject player)
    //{
    //    GameObject spawned;
    //    if (player != null && shield != null)
    //    {
    //        spawned = Instantiate(shield, player.transform.position, player.transform.rotation);
    //        ServerManager.Spawn(spawned);
    //        spawned.transform.SetParent(player.transform);
    //        ORPC_AssignShield(script, spawned);
    //    }
    //    else
    //    {
    //        Debug.LogError("ERR ShieldPower - Missing FirePoint");
    //    }
    //}
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
    [ServerRpc]
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
