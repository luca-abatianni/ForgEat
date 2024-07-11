using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Managing.Server;

public class PowerBehavior : MonoBehaviour
{
    [SerializeField] public PowerType _powerType;
    [SerializeField] GameObject _impactEffect;
    [SerializeField] GameObject _muzzle;


    GameObject _spawner;
    //float _speed = 25f;
    //private Vector3 _direction;
    private GameObject _initialPosition;
    public enum PowerType
    {
        IceBullet = 0,
        MindBullet = 1,
        WindBullet = 2,
        TrickBullet = 3,
    }
    static public int[] vecPowerCost = new int[] { 8, 20, 15, 35 };
    public void SetPowerType(PowerBehavior.PowerType type)
    {
        _powerType = type;
    }
    public void SetSpawner(GameObject spawner)
    {
        _spawner = spawner;
    }
    public void CollisionEvent(GameObject bullet, GameObject player)
    {
        if (player != null)
        {
            var powerEffect = player.GetComponent<PowerEffect>();
            if (powerEffect != null)
            {
                NetworkConnection owner = player.GetComponent<NetworkBehaviour>().Owner;
                PowerEffectHit(owner, powerEffect, _powerType);
                //Debug.Log("Collision " + _powerType);
            }

        }
    }


    public void PowerEffectHit(NetworkConnection owner, PowerEffect script, PowerType type)
    {
        script.Hit(owner, type);
    }

    public void Muzzle(PowerBehavior script, GameObject bulletObj, GameObject muzzle, GameObject parent)
    {
        var obj = Instantiate(muzzle, parent.transform.position, Camera.main.transform.rotation);
        obj.transform.forward = bulletObj.transform.forward;
    }

    public void OnImpact(Vector3 pos, Quaternion rot)
    {
        Instantiate(_impactEffect, pos, rot);
    }
    public void OnImpact_SRPC(Vector3 pos, Quaternion rot)
    {
        var GM = FindObjectOfType<GameManager>();
        if (GM != null)
        {
            GM.ORPC_GMSpawn(_impactEffect, pos, rot);
        }
    }
}
