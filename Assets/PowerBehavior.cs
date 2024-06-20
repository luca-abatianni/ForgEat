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

    void Start()
    {
        //SpawnSigil();
        //SpawnSigilFoot();
    }



    public void SetPowerType(PowerBehavior.PowerType type)
    {
        _powerType = type;
        Debug.Log("Set " + _powerType);
    }
    public void SetSpawner(GameObject spawner)
    {
        _spawner = spawner;
    }
    public void CollisionEvent(GameObject bullet, GameObject player)
    {
        Debug.Log("P1 collision");
        var powerEffect = bullet.gameObject.GetComponent<PowerEffect>();
        Vector3 pos = bullet.transform.position;
        if (player != null && player.GetComponent<NetworkBehaviour>().IsOwner && powerEffect != null)
        {
            PowerEffectHit(powerEffect, _powerType);
            Debug.Log("Collision " + _powerType);

        }
        if (_impactEffect != null)
        {
            OnImpact(_impactEffect, pos, bullet.transform.rotation);
        }
    }


    //[ObserversRpc]
    public void PowerEffectHit(PowerEffect script, PowerType type)
    {
        script.Hit(type);
    }
    //[ObserversRpc]
    public void Muzzle(PowerBehavior script, GameObject bulletObj, GameObject muzzle, GameObject parent)
    {
        var obj = Instantiate(muzzle, parent.transform.position, Camera.main.transform.rotation);
        obj.transform.forward = bulletObj.transform.forward;
        //obj.transform.SetParent(parent.transform);  
    }

   // [ObserversRpc]
    public void OnImpact(GameObject _impact, Vector3 pos, Quaternion rot)
    {
        Debug.Log("Impact effect!");
        Instantiate(_impact, pos, rot);
    }
    /*
    //[ServerRpc]
    public void SRPC_OnImpact(GameObject _impact, Vector3 pos, Quaternion rot)
    {
        var obj = Instantiate(_impact, pos, rot);
        Instantiate(obj);
    }
    */
}
