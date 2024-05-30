using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformantBullet : NetworkBehaviour
{
    [SerializeField] private float bullet_speed;

    private List<Bullet> spawned_bullets = new List<Bullet>();
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        // This part is run by everyon
        foreach (Bullet m_bullet in spawned_bullets)
        {
            m_bullet.bullet_transform.position += m_bullet.direction * Time.deltaTime * bullet_speed;
        }

        if (!IsOwner) return;

        // This part is run only by owner
    }

    public void Shoot(GameObject effect_to_spawn, float fire_offset, PowerBehavior.PowerType primary_power)
    {
        Transform camera_transform = Camera.main.transform;
        Vector3 start_position = camera_transform.position + camera_transform.forward * fire_offset;
        Quaternion rotation = camera_transform.rotation;
        Vector3 direction = camera_transform.forward;

        SpawnLocal(start_position, rotation, effect_to_spawn, primary_power);
        SRPC_SpawnMagicBullet(start_position, rotation, direction, effect_to_spawn, primary_power, TimeManager.Tick);
    }

    private void SpawnLocal(Vector3 start_position, Quaternion rotation, GameObject effect_to_spawn, PowerBehavior.PowerType primary_power)
    {
        Spawn(start_position, rotation, effect_to_spawn, primary_power);
    }


    [ServerRpc]
    private void SRPC_SpawnMagicBullet(Vector3 start_position, Quaternion rotation, Vector3 direction, GameObject effect_to_spawn, PowerBehavior.PowerType primary_power, uint start_tick)
    {
        ORPC_SpawnBulletObserver(start_position, rotation, direction, effect_to_spawn, primary_power, start_tick);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ORPC_SpawnBulletObserver(Vector3 start_position, Quaternion rotation, Vector3 direction, GameObject effect_to_spawn, PowerBehavior.PowerType primary_power, uint start_tick)
    {
        float time_diff = (float)(TimeManager.Tick - start_tick) / TimeManager.TickRate;
        Vector3 spawn_position = start_position + direction * bullet_speed * time_diff;
        Spawn(spawn_position, rotation, effect_to_spawn, primary_power);
    }


    private void Spawn(Vector3 spawn_position, Quaternion spawn_rotation, GameObject effect_to_spawn, PowerBehavior.PowerType primary_power)
    {

        GameObject spawned = Instantiate(effect_to_spawn, spawn_position, spawn_rotation);
        //spawned.GetComponent<PowerBehavior>().SetSpawner(gameObject);
        //spawned.GetComponent<PowerBehavior>().SetPowerType(primary_power);
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), spawned.GetComponent<Collider>(), true);
        spawned_bullets.Add(new Bullet() { bullet_transform = spawned.transform, direction = spawned.transform.forward });

    }
    private class Bullet
    {
        public Transform bullet_transform;
        public Vector3 direction;
    }


}
