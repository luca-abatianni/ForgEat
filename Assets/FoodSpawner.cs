using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

//Made by Bobsi Unity - Youtube
public class FoodSpawner : NetworkBehaviour
{
    public GameObject food_prefab;
    public GameObject trash_prefab;
    [HideInInspector] public List<GameObject> spawnedObject = new List<GameObject>();
    List<GameObject> food_list = new List<GameObject>();
    List<GameObject> trash_list = new List<GameObject>();


    public override void OnStartServer()
    {
        Debug.Log("Starting food spawner?");
        SpawnFoodTrash();
    }

    private void Update()
    {

    }

    public void SpawnFoodTrash()
    {
        foreach (Transform spawn_point in transform)
        {
            SpawnObject(Random.value > 0.5, spawn_point.position, spawn_point.rotation, this);
        }
    }

    //[ServerRpc]
    public void SpawnObject(bool food_or_trash, Vector3 position, Quaternion rotation, FoodSpawner script)
    {
        Debug.Log("Spawning " +  (food_or_trash ? "food" : "trash") + "(" + food_or_trash + ")");
        GameObject spawned = Instantiate(food_or_trash ? food_prefab : trash_prefab, position, rotation);
        ServerManager.Spawn(spawned);
        SetSpawnedObject(spawned, script, food_or_trash);
    }

    //[ObserversRpc]
    public void SetSpawnedObject(GameObject spawned, FoodSpawner script, bool food_or_trash)
    {
        if (food_or_trash)
        {
            script.food_list.Add(spawned);
        }
        else
        {
            script.trash_list.Add(spawned);
        }
        script.spawnedObject.Add(spawned);
    }

    //[ServerRpc(RequireOwnership = false)]
    public void DespawnObject(GameObject obj)
    {
        ServerManager.Despawn(obj);
    }
}