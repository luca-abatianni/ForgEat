using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickImpact : NetworkBehaviour
{
    [SerializeField] public GameObject _trash_prefab;
    private bool _hasSpawned=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_hasSpawned)
        {
            ORPC_SpawnTrash(_trash_prefab,gameObject.transform.position,gameObject.transform.rotation);
            SRPC_SpawnTrash(_trash_prefab,gameObject.transform.position,gameObject.transform.rotation);
        }
        _hasSpawned = true;
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!_hasSpawned)
        {
            ORPC_SpawnTrash(_trash_prefab, gameObject.transform.position, gameObject.transform.rotation);
            SRPC_SpawnTrash(_trash_prefab, gameObject.transform.position, gameObject.transform.rotation);
        }
        _hasSpawned = true;
    }
    [ServerRpc]
    void SRPC_SpawnTrash(GameObject trash,Vector3 position, Quaternion rotation)
    {
        GameObject spawned = Instantiate(_trash_prefab, position, rotation);
        spawned.GetComponent<Food>().SetFood();
        
        ServerManager.Spawn(spawned);

    }
    [ObserversRpc]
    void ORPC_SpawnTrash(GameObject trash,Vector3 position, Quaternion rotation)
    {
        GameObject spawned = Instantiate(_trash_prefab, position, rotation);
        spawned.GetComponent<Food>().SetFood();
        
        ServerManager.Spawn(spawned);

    }
}
