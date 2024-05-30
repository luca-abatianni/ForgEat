using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Component.Animating;
using UnityEngine.VFX;
using Unity.VisualScripting;
public class PrimaryPower : NetworkBehaviour
{
    [SerializeField] public float _fireOffset;
    [SerializeField] public GameObject _sigilPoint;
    [SerializeField] public List<GameObject> _listEffects = new List<GameObject>();
    [SerializeField] public List<GameObject> _listSigils = new List<GameObject>();
    [SerializeField] public PowerBehavior.PowerType _primaryPower = PowerBehavior.PowerType.IceBullet;
    private GameObject _effectToSpawn;
    private GameObject _sigilToSpawn;

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
        if (_listEffects.Count == 0)
            return;
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time > _cooldown && Input.GetKeyDown(KeyCode.Mouse0) && _listEffects.Count > 0)
        {

            _cooldown = Time.time + 1f;
            _effectToSpawn = _listEffects[(int)_primaryPower];//Left click - Power 1
            _sigilToSpawn = _listSigils[(int)_primaryPower];
            StartCoroutine(SpawnMagic());
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
            _primaryPower = PowerBehavior.PowerType.IceBullet;
            SRPC_SwitchPower(this, PowerBehavior.PowerType.IceBullet);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _primaryPower = PowerBehavior.PowerType.MindBullet;
            SRPC_SwitchPower(this, PowerBehavior.PowerType.MindBullet);
        }
    }
    [ServerRpc (RequireOwnership =false)]
    void SRPC_SwitchPower(PrimaryPower script,PowerBehavior.PowerType type)
    {
        script._primaryPower = type;
    }
    private IEnumerator SpawnMagic()
    {
        Vector3 _firePoint = Camera.main.transform.position + Camera.main.transform.forward*_fireOffset;
        Quaternion rotation = Camera.main.transform.rotation;
        SRPC_SpawnSigil(_sigilToSpawn, _firePoint, rotation);
        SRPC_SpawnSigilFoot(_sigilToSpawn, _firePoint);
        animator.SetBool("attackFreeze", true);
        yield return new WaitForSeconds(.3f);
        GetComponent<PerformantBullet>().Shoot(_effectToSpawn, _fireOffset, _primaryPower);

        yield return null;
    }

    [ServerRpc]
    void SRPC_SpawnSigil(GameObject _effectToSpawn, Vector3 _firePoint, Quaternion rotation)
    {
        var spawned = Instantiate(_effectToSpawn, _firePoint, rotation);
        ServerManager.Spawn(spawned);
        spawned.transform.SetParent(transform);
    }
    [ServerRpc]
    void SRPC_SpawnSigilFoot(GameObject _effectToSpawn, Vector3 _firePoint)
    {
        var spawned = Instantiate(_effectToSpawn, _firePoint, Quaternion.LookRotation(Vector3.up));
        ServerManager.Spawn(spawned);
        spawned.transform.SetParent(transform);
    }

    /*
    [ServerRpc]
    void SRPC_SpawnMagic(PrimaryPower script, GameObject _effectToSpawn, GameObject _firePoint, GameObject player, Vector3 spawn_forward, Quaternion spawn_rot)
    {
        GameObject spawned;
        if (_firePoint != null)
        {
            spawned = Instantiate(_effectToSpawn, _firePoint.transform.position, _firePoint.transform.rotation);
            ServerManager.Spawn(spawned);
            spawned.GetComponent<PowerBehavior>().SetDirection(spawn_forward);
            spawned.GetComponent<PowerBehavior>().SetParent(_firePoint);
            spawned.GetComponent<PowerBehavior>().SetSpawner(player);
            spawned.GetComponent<PowerBehavior>().SetPowerType(_primaryPower);
            Physics.IgnoreCollision(player.GetComponent<Collider>(), spawned.GetComponent<Collider>(), true);
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
    */
}
