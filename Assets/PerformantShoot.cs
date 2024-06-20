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

    [SerializeField]
    public List<GameObject> _listEffects = new List<GameObject>();
    [SerializeField]
    public List<GameObject> _listSigils = new List<GameObject>();

    public GameObject _effectToSpawn;
    public GameObject _sigilToSpawn;


    public void Shoot(float fire_offset)
    {
        Transform camera_transform = Camera.main.transform;
        Vector3 start_position = camera_transform.position + camera_transform.forward * fire_offset;
        Quaternion rotation = camera_transform.rotation;
        Vector3 direction = camera_transform.forward;

        int bulletID = Random.Range(0, 999999);
        SpawnLocal(start_position, rotation, _firePoint.transform, _primaryPower, bulletID, LocalConnection.ClientId);
        SRPC_SpawnMagicBullet(start_position, rotation, direction, _firePoint.transform, _primaryPower, TimeManager.Tick, bulletID, OwnerId);
    }

    private void SpawnLocal(Vector3 start_position, Quaternion rotation, Transform sigil_point, PowerBehavior.PowerType power, int bulletID, int ownerID)
    {
        Vector3 direction = rotation * Vector3.forward;

        _effectToSpawn = _listEffects[(int) power];
        GameObject bullet = Instantiate(_effectToSpawn, start_position, rotation);

        bullet.GetComponent<Bullet>().Initialize(direction, bulletID, ownerID);
        bullet.GetComponent<PowerBehavior>().SetPowerType(power);

        _sigilToSpawn = _listSigils[(int) power];
        SpawnSigil(_sigilToSpawn, _firePoint);
        SpawnSigilFoot(_sigilToSpawn, _firePoint);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SRPC_SpawnMagicBullet(Vector3 start_position, Quaternion rotation, Vector3 direction, Transform sigil_point, PowerBehavior.PowerType power, uint start_tick, int bulletID, int ownerID)
    {
        ORPC_SpawnBulletObserver(start_position, rotation, direction, sigil_point, power, start_tick, bulletID, ownerID);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void ORPC_SpawnBulletObserver(Vector3 start_position, Quaternion rotation, Vector3 direction, Transform sigil_point, PowerBehavior.PowerType power, uint start_tick, int bulletID, int ownerID)
    {
        float time_diff = (float)(TimeManager.Tick - start_tick) / TimeManager.TickRate;
        Vector3 spawn_position = start_position + direction * bullet_speed * time_diff;
        SpawnLocal(spawn_position, rotation, sigil_point, power, bulletID, ownerID);
    }


    void SpawnSigil(GameObject _effectToSpawn, GameObject _firePoint)
    {
        var spawned = Instantiate(_effectToSpawn, _firePoint.transform.position, _firePoint.transform.rotation);
        spawned.transform.SetParent(_firePoint.transform);
    }
    void SpawnSigilFoot(GameObject _effectToSpawn, GameObject _firePoint)
    {
        var p = _firePoint.transform.position;
        var spawned = Instantiate(_effectToSpawn, new Vector3(p.x, p.y, p.z), Quaternion.LookRotation(Vector3.up));
        spawned.transform.SetParent(_firePoint.transform);
    }


}
