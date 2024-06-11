using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : NetworkBehaviour
{
    public float _timeout = 5f;
    float _timer = 0f;
    public bool _WithTimer = false, _effectSpawned = false;
    public GameObject _trickSigil;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsServer)
        {
            GetComponent<SelfDestroy>().enabled = false;
            return;
        }
    }
    private void Start()
    {
        if (GetComponent<Food>() != null)
            _timer += 1f;
        _timer = Time.time + _timeout;

    }
    void Update()
    {
        if (transform.childCount == 0)
        {
            ORPC_Despawn(gameObject);
            SRPC_Despawn(gameObject);
            //Destroy(gameObject);
        }
        float now = Time.time;
        if (_WithTimer && now > (_timer - .8f) && _trickSigil != null && !_effectSpawned)//circa un secondo prima parte l'effetto
        {
            SRPC_SpawnSigilFoot(_trickSigil, gameObject);
            _effectSpawned = true;
        }
        if (_WithTimer && now > _timer)
        {
            ORPC_Despawn(gameObject);
            SRPC_Despawn(gameObject);

        }
    }

    [ServerRpc]
    public void SRPC_Despawn(GameObject obj)
    {
        Debug.Log("Despawn VFX");
        ServerManager.Despawn(obj);
    }
    [ObserversRpc]
    public void ORPC_Despawn(GameObject obj)
    {
        Debug.Log("Despawn VFX");
        ServerManager.Despawn(obj);
    }
    [ObserversRpc]
    void SRPC_SpawnSigilFoot(GameObject _effectToSpawn, GameObject _spawnPoint)
    {
        var p = _spawnPoint.transform.position;
        var spawned = Instantiate(_effectToSpawn, new Vector3(p.x, p.y, p.z), Quaternion.LookRotation(Vector3.up));
        ServerManager.Spawn(spawned);
        spawned.transform.SetParent(_spawnPoint.transform);
    }
}
