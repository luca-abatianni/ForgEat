using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformantShoot : NetworkBehaviour
{
    [SerializeField] private float bullet_speed;
    [SerializeField] private GameObject bullet_prefab;

    [SerializeField] public GameObject _firePoint;
    [SerializeField] public GameObject _sigilPoint;
    [SerializeField] public PowerBehavior.PowerType _primaryPower = PowerBehavior.PowerType.IceBullet;

    [SerializeField] private float sigil_delay;
    [SerializeField] private float fire_offset;

    [SerializeField]
    public List<GameObject> _listEffects = new List<GameObject>();
    [SerializeField]
    public List<GameObject> _listSigils = new List<GameObject>();

    public GameObject _effectToSpawn;
    public GameObject _sigilToSpawn;


    public void Shoot()
    {
        //LocalSpawnSigil(_firePoint.transform, (int) _primaryPower);
        SRPC_SpawnSigil(_firePoint.transform, (int)_primaryPower);
        StartCoroutine(ShootingCoroutine());
    }

    private IEnumerator ShootingCoroutine()
    {
        Transform camera_transform = Camera.main.transform;
        Vector3 start_position = camera_transform.position + camera_transform.forward * fire_offset;
        Quaternion rotation = camera_transform.rotation;
        Vector3 direction = camera_transform.forward;

        int bulletID = Random.Range(0, 999999);

        yield return new WaitForSeconds(sigil_delay);

        SpawnLocal(start_position, rotation, _firePoint.transform, (int) _primaryPower, bulletID, LocalConnection.ClientId);
        SRPC_SpawnMagicBullet(start_position, rotation, direction, _firePoint.transform, (int) _primaryPower, TimeManager.Tick, bulletID, OwnerId);
    }

    private void SpawnLocal(Vector3 start_position, Quaternion rotation, Transform sigil_point, int power, int bulletID, int ownerID)
    {
        // Spawn bullet
        Vector3 direction = rotation * Vector3.forward;

        _effectToSpawn = _listEffects[power];
        GameObject bullet = Instantiate(_effectToSpawn, start_position, rotation);

        bullet.GetComponent<Bullet>().Initialize(direction, bulletID, ownerID);
        bullet.GetComponent<PowerBehavior>().SetPowerType((PowerBehavior.PowerType) power);

    }


    [ServerRpc(RequireOwnership = false)]
    private void SRPC_SpawnMagicBullet(Vector3 start_position, Quaternion rotation, Vector3 direction, Transform sigil_point, int power, uint start_tick, int bulletID, int ownerID)
    {
        ORPC_SpawnBulletObserver(start_position, rotation, direction, sigil_point, power, start_tick, bulletID, ownerID);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ORPC_SpawnBulletObserver(Vector3 start_position, Quaternion rotation, Vector3 direction, Transform sigil_point, int power, uint start_tick, int bulletID, int ownerID)
    {
        float time_diff = (float)(TimeManager.Tick - start_tick) / TimeManager.TickRate;
        Vector3 spawn_position = start_position + direction * bullet_speed * time_diff;
        SpawnLocal(spawn_position, rotation, sigil_point, power, bulletID, ownerID);
    }

    [ServerRpc(RequireOwnership = false)]
    void SRPC_SpawnSigil(Transform _firePoint, int power)
    {
        ORPC_SpawnSigil(_firePoint, power);
    }

    [ObserversRpc]
    void ORPC_SpawnSigil(Transform _firePoint, int power)
    {
        _sigilToSpawn = _listSigils[power];
        var spawned_sigil = Instantiate(_sigilToSpawn, _firePoint.transform.position, _firePoint.transform.rotation);
        spawned_sigil.transform.SetParent(_firePoint.transform);

        var p = _firePoint.transform.position;
        var spawned_sigil_foot = Instantiate(_sigilToSpawn, new Vector3(p.x, p.y, p.z), Quaternion.LookRotation(Vector3.up));
        spawned_sigil_foot.transform.SetParent(_firePoint.transform);
    }
}
