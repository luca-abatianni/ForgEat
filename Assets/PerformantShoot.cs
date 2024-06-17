using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformantShoot : NetworkBehaviour
{
    [SerializeField] private float bullet_speed;
    [SerializeField] private GameObject bullet_prefab;

    private List<Bullet> spawned_bullets = new List<Bullet>();
    // Start is called before the first frame update
    //
    // Update is called once per frame

    public void Shoot(float fire_offset)
    {
        Transform camera_transform = Camera.main.transform;
        Vector3 start_position = camera_transform.position + camera_transform.forward * fire_offset;
        Quaternion rotation = camera_transform.rotation;
        Vector3 direction = camera_transform.forward;

        int bulletID = Random.Range(0, 999999);

        SpawnLocal(start_position, rotation, bulletID, LocalConnection.ClientId);
        SRPC_SpawnMagicBullet(start_position, rotation, direction, TimeManager.Tick, bulletID, OwnerId);
    }

    private void SpawnLocal(Vector3 start_position, Quaternion rotation, int bulletID, int ownerID)
    {
        Vector3 direction = rotation * Vector3.forward;
        GameObject bullet = Instantiate(bullet_prefab, start_position, rotation);
        bullet.GetComponent<Bullet>().Initialize(direction, bullet_speed, bulletID, ownerID);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SRPC_SpawnMagicBullet(Vector3 start_position, Quaternion rotation, Vector3 direction, uint start_tick, int bulletID, int ownerID)
    {
        ORPC_SpawnBulletObserver(start_position, rotation, direction, start_tick, bulletID, ownerID);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ORPC_SpawnBulletObserver(Vector3 start_position, Quaternion rotation, Vector3 direction, uint start_tick, int bulletID, int ownerID)
    {
        float time_diff = (float)(TimeManager.Tick - start_tick) / TimeManager.TickRate;
        Vector3 spawn_position = start_position + direction * bullet_speed * time_diff;
        SpawnLocal(spawn_position, rotation, bulletID, ownerID);
    }


}
