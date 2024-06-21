using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowerBehavior;

public class WindImpact : NetworkBehaviour
{//Dovrebbe essere Server Owned
    [SerializeField] public float _pushForce = 1000f;
    private PowerBehavior.PowerType _powerType = PowerBehavior.PowerType.WindBullet;


    // Update is called once per frame
    void Update()
    {

    }
    [ObserversRpc]
    void ORPC_WindImpact(GameObject other)
    {
        var plyer = other.GetComponent<PlayerController>();
        Vector3 direction = other.transform.position - transform.position;
        direction.y = 0; // Ensure the force is horizontal (you can remove this line if you want a vertical component too)
        direction.Normalize();

        // Apply an upward and away force
        Vector3 pushDirection = (direction).normalized;
        plyer.AddForce(pushDirection * _pushForce);
    }
    private void OnTriggerEnter(Collider other)
    {
        ORPC_WindImpact(other.gameObject);
        //other.gameObject.GetComponent<PowerEffect>().Hit(PowerType.WindBullet);
    }
    private void OnTriggerStay(Collider other)
    {
    }
}
