using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using FishNet.Connection;

public class FoodSpawner : NetworkBehaviour
{
    public GameObject food_prefab;
    public GameObject trash_prefab;
    public GameObject food_parent;
    public GameObject spawn_points;
    public List<GameObject> spawnedObject = new List<GameObject>();
    public List<GameObject> food_list = new List<GameObject>();     // Set private later... Public for testing.
    public List<GameObject> trash_list = new List<GameObject>();    // Set private later... Public for testing.
    public override void OnStartServer()
    {
        SpawnFoodTrash();
    }
    public override void OnStartClient()
    {
        if (!base.IsOwner)
        {
            GetComponent<FoodSpawner>().enabled = false;
            return;
        }
        SetClientsFoodTrashList();
    }

    public void SpawnFoodTrash()
    {
        //GlobalConsole.Instance.Log("Spawning food!");
        NetworkManager.Log("Spawning food!");
        foreach (Transform spawn_point in spawn_points.transform)
        {
            SpawnObject(Random.value > 0.5, spawn_point.position, spawn_point.rotation, this);
        }
    }

    
    private void SpawnObject(bool food_or_trash, Vector3 position, Quaternion rotation, FoodSpawner script)
    {
        Debug.Log("Spawning " +  (food_or_trash ? "food" : "trash") + "(" + food_or_trash + ")");
        GameObject spawned = Instantiate((food_or_trash ? food_prefab : trash_prefab), position, rotation, food_parent.transform);
        ServerManager.Spawn(spawned);
        if (food_or_trash)
        {
            script.food_list.Add(spawned);
        }
        else
        {
            script.trash_list.Add(spawned);
        }
    }

    private void SetClientsFoodTrashList()
    {
        FoodSpawner script = GetComponent<FoodSpawner>();
        foreach (Transform t in food_parent.transform)
        {
            if(t.GetComponent<Food>().is_food)
            {
                food_list.Add(t.gameObject);
            }
            else
            {
                trash_list.Add(t.gameObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveObject(GameObject obj)
    {
        NetworkManager.Log("Despawning object: " + obj);
        if (obj.GetComponent<Food>().value > 0)
        {
            food_list.Remove(obj);
        }
        else
        {
            trash_list.Remove(obj);
        }
        spawnedObject.Remove(obj);
        ServerManager.Despawn(obj);
    }

    //[ObserversRpc]
    public void Server_TransformTrashInFood()
    {
        foreach (var trash in trash_list)
        {
            NetworkManager.Log("Changing trash into food." + trash.name);
            trash.GetComponent<Food>().SetFood();
        }
    }

    [ObserversRpc]
    public void Observer_TransformTrashInFood()
    {
        if (trash_list.Count == 0) SetClientsFoodTrashList();
        foreach (var trash in trash_list)
        {
            NetworkManager.Log("Changing trash into food." + trash.name);
            trash.GetComponent<Food>().SetFood();
        }
    }
}