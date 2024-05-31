using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldPower : NetworkBehaviour
{
    [SerializeField] public GameObject _shieldPrefab;
    [SerializeField] public bool _isShielded = false;
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
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _isShielded = true;
                _shieldObj.ActivateShield(true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                _isShielded = false;
                _shieldObj.ActivateShield(false);
            }
        }
    }
    private IEnumerator SpawnShield()
    {
        //animator.SetBool("attackFreeze", true);
        SRPC_SpawnShield(this, _shieldPrefab, gameObject);

        yield return null;
    }
    [ObserversRpc]
    void ORPC_AssignShield(ShieldPower script, GameObject spawned)
    {
        script._shieldObj = spawned.GetComponent<ShieldCollision>();
    }
    [ServerRpc]
    void SRPC_SpawnShield(ShieldPower script, GameObject shield, GameObject player)
    {
        GameObject spawned;
        if (player != null && shield != null)
        {
            spawned = Instantiate(shield, player.transform.position, player.transform.rotation);
            ServerManager.Spawn(spawned);
            spawned.transform.SetParent(player.transform);
            ORPC_AssignShield(script, spawned);
        }
        else
        {
            Debug.LogError("ERR ShieldPower - Missing FirePoint");
        }
    }
}
