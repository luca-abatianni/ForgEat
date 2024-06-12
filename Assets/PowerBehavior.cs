using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class PowerBehavior : NetworkBehaviour
{
    [SerializeField] public PowerType _powerType;
    [SerializeField] GameObject _impactEffect;
    [SerializeField] GameObject _muzzle;

    GameObject _spawner;
    float _speed = 25f;
    private Vector3 _direction;
    private GameObject _initialPosition;
    public enum PowerType
    {
        IceBullet = 0,
        MindBullet = 1,
        WindBullet = 2,
        TrickBullet = 3,
    }
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        if (_muzzle != null)
            ORPC_Muzzle(this, gameObject, _muzzle, _initialPosition);
        base.OnStartClient();
        if (!base.IsServer)
        {
            GetComponent<PowerBehavior>().enabled = false;
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        switch (_powerType)
        {
            case PowerType.IceBullet:
            case PowerType.MindBullet:
            case PowerType.WindBullet:
                transform.position += _direction * (_speed * Time.deltaTime);
                break;
            case PowerType.TrickBullet:
                transform.position += _direction * (.4f * _speed * Time.deltaTime);
                break;
        }
    }

    public void SetParent(GameObject pos)
    {
        _initialPosition = pos;
    }
    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
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
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("P1 collision");
        var powerEffect = collision.gameObject.GetComponent<PowerEffect>();
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        if (collision.transform.tag == "Player")
        {
            _speed = 0f;
            var shield = collision.gameObject.GetComponent<ShieldPower>();
            if (shield != null)
            {
                if (powerEffect != null && !shield._isShielded)
                {
                    ORPC_PowerEffectHit(powerEffect, _powerType);
                    Debug.Log("Collision " + _powerType);
                }
            }
            else
            {
                Debug.Log("SHIELD NULL");
            }
        }
        if (_impactEffect != null)
        {
            ORPC_OnImpact(_impactEffect, pos, rot);
            SRPC_OnImpact(_impactEffect, pos, rot);
        }
        ServerManager.Despawn(gameObject);
        Destroy(gameObject);
    }


    [ObserversRpc]
    public void ORPC_PowerEffectHit(PowerEffect script, PowerType type)
    {
        script.Hit(type);
    }
    [ObserversRpc]
    public void ORPC_Muzzle(PowerBehavior script, GameObject bulletObj, GameObject muzzle, GameObject parent)
    {
        var obj = Instantiate(muzzle, parent.transform.position, Camera.main.transform.rotation);
        obj.transform.forward = bulletObj.transform.forward;
        ServerManager.Spawn(obj);
        //obj.transform.SetParent(parent.transform);  
    }

    [ObserversRpc]
    public void ORPC_OnImpact(GameObject _impact, Vector3 pos, Quaternion rot)
    {
        var obj = Instantiate(_impact, pos, rot);
        ServerManager.Spawn(obj);
    }
    [ServerRpc]
    public void SRPC_OnImpact(GameObject _impact, Vector3 pos, Quaternion rot)
    {
        var obj = Instantiate(_impact, pos, rot);
        ServerManager.Spawn(obj);
    }
}
