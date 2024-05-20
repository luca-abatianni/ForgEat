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
    GameObject _spawner;
    float _speed = 20f;
    private Vector3 _direction;
    enum PowerType
    {
        IceBullet,
    }
    // Start is called before the first frame update
    public override void OnStartClient()
    {

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
            OnImpact(this, _impact, pos, rot);
        }
        else
            return;
        //Destroy(gameObject);
    }
    [ServerRpc]
    public void OnImpact(PowerBehavior script, GameObject _impact, Vector3 pos, Quaternion rot)
    {
        var obj = Instantiate(_impact, pos, rot);
        ServerManager.Spawn(obj);
    }
}
