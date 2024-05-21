using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Component.Animating;
using UnityEngine.VFX;
public class PrimaryPower : NetworkBehaviour
{
    public GameObject _firePoint;
    public List<GameObject> _listVFX = new List<GameObject>();
    private GameObject _effectToSpawn;

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
        if (_listVFX.Count == 0)
            return;
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time > _cooldown && Input.GetKeyDown(KeyCode.Mouse0))
        {
            _cooldown = Time.time + 1f;
            _effectToSpawn = _listVFX[0];//Left click - Power 1
            SRPC_SpawnMagic(this, _effectToSpawn, _firePoint, gameObject);
            //SRPC_SpawnSigil(_castVFX, _firePoint);
            animator.SetBool("attackFreeze", true);
        }
        else
        {
            animator.SetBool("attackFreeze", false);
        }
    }

    [ServerRpc]
    void SRPC_SpawnSigil(GameObject _effectToSpawn, GameObject _firePoint)
    {
        var spawned = Instantiate(_effectToSpawn);//, _firePoint.transform.position, _firePoint.transform.rotation);
        spawned.transform.SetParent(_firePoint.transform);
        ServerManager.Spawn(spawned);
    }
    [ServerRpc]
    void SRPC_SpawnMagic(PrimaryPower script, GameObject _effectToSpawn, GameObject _firePoint, GameObject player)
    {
        GameObject spawned;
        if (_firePoint != null)
        {
            //var camera = player.GetComponentInChildren<Camera>();
            spawned = Instantiate(_effectToSpawn, _firePoint.transform.position, _firePoint.transform.rotation);
            spawned.GetComponent<PowerBehavior>().SetDirection(_firePoint.transform.forward);
            spawned.GetComponent<PowerBehavior>().SetParent(_firePoint);
            spawned.GetComponent<PowerBehavior>().SetSpawner(player);
            Physics.IgnoreCollision(player.GetComponent<Collider>(), spawned.GetComponent<Collider>(), true);
            ServerManager.Spawn(spawned);
        }
        else
        {
            Debug.LogError("ERR PrimaryPower - Missing FirePoint");
        }
    }
    //[ObserversRpc]
    //public void SetSpawnedMagic(PrimaryPower script, GameObject spawned)
    //{
    //    script._effectToSpawn = spawned;
    //}
}
