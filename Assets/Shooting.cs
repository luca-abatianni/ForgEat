using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet;

public class Shooting : NetworkBehaviour
{

    [SerializeField]
    GameObject bullet_prefab;
    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    // Update is called once per frame
    public void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            if (bullet_prefab == null)
            {
                Debug.LogError("Bullet prefab not istantiated.");
                return;
            }
            else
            {
                InstantiateBullet();
            }
        }
    }

    private void InstantiateBullet()
    {
        Vector3 shoot_direction = Camera.main.transform.forward;
        Vector3 start_pos = Camera.main.transform.position + shoot_direction;
        GameObject bullet = Instantiate(bullet_prefab, start_pos, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(bullet, null);
    }
}
