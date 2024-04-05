using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

//Made by Bobsi Unity - Youtube
public class PlayerSpawnObject : NetworkBehaviour
{
    public GameObject objToSpawn;
    [HideInInspector] public List<GameObject>  spawnedObject = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            GetComponent<PlayerSpawnObject>().enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Transform camera = transform.Find("Main Camera");
            if (camera != null)
            {
                SpawnObject(objToSpawn, camera.position + camera.forward*2.0f, camera.rotation, this);
            }
        }
    }

    [ServerRpc]
    public void SpawnObject(GameObject obj, Vector3 position, Quaternion rotation, PlayerSpawnObject script)
    {
        GameObject spawned = Instantiate(obj, position, rotation);
        ServerManager.Spawn(spawned);
        SetSpawnedObject(spawned, script);
    }

    [ObserversRpc]
    public void SetSpawnedObject(GameObject spawned, PlayerSpawnObject script)
    {
        script.spawnedObject.Add(spawned);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnObject(GameObject obj)
    {
        ServerManager.Despawn(obj);
    }
}