using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.VFX;
public class PrimaryPower : NetworkBehaviour
{
    [SerializeField] GameObject _muzzle;
    public GameObject _firePoint;
    public List<GameObject> _listVFX = new List<GameObject>();
    private GameObject _effectToSpawn;


    private float _cooldown = 0f;
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
            SRPC_Muzzle(this,gameObject, _muzzle, _firePoint.transform.position);
        }
    }
    [ServerRpc]
    public void SRPC_Muzzle(PrimaryPower script, GameObject GO, GameObject muzzle, Vector3 pos)
    {
        var obj = Instantiate(muzzle, pos, Camera.main.transform.rotation);
        obj.transform.forward = GO.transform.forward;
        ServerManager.Spawn(obj);
    }
    [ServerRpc]
    void SRPC_SpawnMagic(PrimaryPower script, GameObject _effectToSpawn, GameObject _firePoint, GameObject player)
    {
        GameObject spawned;
        if (_firePoint != null)
        {
            spawned = Instantiate(_effectToSpawn, _firePoint.transform.position, Camera.main.transform.rotation);
            spawned.GetComponent<PowerBehavior>().SetDirection(Camera.main.transform.forward);
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
