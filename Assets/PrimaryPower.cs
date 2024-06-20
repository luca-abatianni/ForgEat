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
    [SerializeField]
    private PerformantShoot performant_shoot;

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

            _cooldown = Time.time + 1f;

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

    private IEnumerator SpawnMagic()
    {
        animator.SetBool("attackFreeze", true);
        //yield return new WaitForSeconds(.3f);
        //SRPC_SpawnMagic(this, _effectToSpawn, _firePoint, gameObject, spawn_forward, spawn_rot);
        GetComponent<PerformantShoot>().Shoot(0.5f);
        yield return null;
    }

          
    [ObserversRpc]
    void ORPC_SetParent(GameObject parent, GameObject spawned)
    {
        spawned.transform.SetParent(parent.transform);
    }
    [ServerRpc]
    void SRPC_SpawnMagic(PrimaryPower script, GameObject _effectToSpawn, GameObject _firePoint, GameObject player, Vector3 spawn_forward, Quaternion spawn_rot)
    {
        GameObject spawned;
        if (_firePoint != null)
        {
            spawned = Instantiate(_effectToSpawn, _firePoint.transform.position, _firePoint.transform.rotation);
            ServerManager.Spawn(spawned);
            //GetComponent<PowerBehavior>().SetDirection(spawn_forward);
            //spawned.GetComponent<PowerBehavior>().SetParent(_firePoint);
            spawned.GetComponent<PowerBehavior>().SetSpawner(player);
            //spawned.GetComponent<PowerBehavior>().SetPowerType(_primaryPower);
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
}
