using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float _timeout = 5f;
    float _timer = 0f;
    public bool _WithTimer = false, _effectSpawned = false;
    public GameObject _trickSigil;

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
            Despawn(gameObject);
            //SRPC_Despawn(gameObject);
            //Destroy(gameObject);
        }
        float now = Time.time;
        if (_WithTimer && now > (_timer - .8f) && _trickSigil != null && !_effectSpawned)//circa un secondo prima parte l'effetto
        {
            SpawnSigilFoot(_trickSigil, gameObject);
            _effectSpawned = true;
        }
        if (_WithTimer && now > _timer)
        {
            Despawn(gameObject);
            //SRPC_Despawn(gameObject);

        }
    }

    public void Despawn(GameObject obj)
    {
        Destroy(obj);
    }
    void SpawnSigilFoot(GameObject _effectToSpawn, GameObject _spawnPoint)
    {
        var p = _spawnPoint.transform.position;
        var spawned = Instantiate(_effectToSpawn, new Vector3(p.x, p.y, p.z), _spawnPoint.transform.rotation);
        spawned.transform.SetParent(_spawnPoint.transform);
    }
}
