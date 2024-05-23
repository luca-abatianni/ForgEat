using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class PowerBehavior : NetworkBehaviour
{
    [SerializeField] PowerType _powerType;
    [SerializeField] GameObject _impact;
    [SerializeField] GameObject _muzzle;

    GameObject _spawner;
    float _speed = 20f;
    private Vector3 _direction;
    private GameObject _initialPosition;
    public enum PowerType
    {
        IceBullet=0,
    }
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        if (_muzzle != null)
            SRPC_Muzzle(this, gameObject, _muzzle, _initialPosition);
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
                transform.position += _direction * (_speed * Time.deltaTime);
                break;
        }
    }

    public void SetParent(GameObject pos)
    {
        _initialPosition=pos;
    }
    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
    public void SetSpawner(GameObject spawner)
    {
        _spawner = spawner;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == _spawner.GetComponent<Collider>())
            return;
        Debug.Log("Ice collision");
        _speed = 0f;
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        if (_impact != null)
        {
            ORPC_OnImpact(this, _impact, pos, rot);
        }
        else
            return;
        ServerManager.Despawn(gameObject);
        Destroy(gameObject);
    }

  
    [ObserversRpc]
    public void SRPC_Muzzle(PowerBehavior script, GameObject bulletObj, GameObject muzzle, GameObject parent)
    {
        var obj = Instantiate(muzzle, parent.transform.position, Camera.main.transform.rotation);
        obj.transform.forward = bulletObj.transform.forward;
        ServerManager.Spawn(obj);
        //obj.transform.SetParent(parent.transform);  
    }
    [ObserversRpc]
    public void ORPC_OnImpact(PowerBehavior script, GameObject _impact, Vector3 pos, Quaternion rot)
    {
        var obj = Instantiate(_impact, pos, rot);
        ServerManager.Spawn(obj);
    }
}
