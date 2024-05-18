using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class PowerBehavior : MonoBehaviour
{
    [SerializeField] PowerType _powerType;
    [SerializeField] GameObject _impact;
    [SerializeField] GameObject _muzzle;
    float _speed = 20f;
    private Vector3 _direction;
    enum PowerType
    {
        IceBullet,
    }
    // Start is called before the first frame update
    void Start()
    {
        if (_muzzle != null)
        {
            var muzzle = Instantiate(_muzzle, transform.position, Quaternion.identity);
            muzzle.transform.forward = gameObject.transform.forward;
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
    private void OnCollisionEnter(Collision collision)
    {
        _speed = 0f;
        Destroy(gameObject);
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        if (_impact != null)
        {
            Instantiate(_impact, pos, rot);
        }
    }
}
