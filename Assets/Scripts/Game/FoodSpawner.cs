using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using FishNet.Connection;

public class FoodSpawner : NetworkBehaviour
{
    public int food_count = 0;
    public int trash_count = 0;
    public GameObject food_parent;
    public GameObject trash_parent;
    public GameObject spawn_points;
    public List<Food> food_spawnlist = new List<Food>();
    public List<GameObject> trash_spawnlist = new List<GameObject>();
    public List<GameObject> spawnedObject = new List<GameObject>();
    public List<GameObject> food_list = new List<GameObject>();     // Set private later... Public for testing.
    public List<GameObject> trash_list = new List<GameObject>();    // Set private later... Public for testing.



    //[ObserversRpc]
    public void SpawnFoodTrash() // Called only by game manager (server only).
    {
        //GlobalConsole.Instance.Log("Spawning food!");
        foreach (Transform spawn_point in spawn_points.transform)
        {
            NetworkManager.Log("Spawning food!");
            SpawnObject(Random.value > 0.5, spawn_point.position, spawn_point.rotation, this);
        }
    }

    private void SpawnObject(bool food_or_trash, Vector3 position, Quaternion rotation, FoodSpawner script)
    {
        if (food_count == 24) food_or_trash = false;
        if (trash_count == 24) food_or_trash = true;
        if (food_or_trash)
        {
            food_count++;
        }
        else
        {
            trash_count++;
        }

        Debug.Log("Spawning " + (food_or_trash ? "food" : "trash") + "(" + food_or_trash + ")");
        GameObject spawned = Instantiate(SelectFoodList(), position, rotation);
        ServerManager.Spawn(spawned);
        SetClientFoodTrash_ORPC(food_or_trash, spawned);
    }
    private GameObject SelectFoodList()
    {
        int itemIndex = Random.Range (0,(food_spawnlist.Count));
        return food_spawnlist[itemIndex].gameObject;
    }

    [ObserversRpc]
    void SetClientFoodTrash_ORPC (bool food_or_trash, GameObject spawned)
    {
        if (food_or_trash)
        {
            Debug.Log("Spawned food");
            spawned.transform.SetParent(food_parent.transform);
            spawned.GetComponent<Food>().InitAsFood();
            food_list.Add(spawned);
        }
        else
        {
            Debug.Log("Spawned trash");
            spawned.transform.SetParent(trash_parent.transform);
            spawned.GetComponent<Food>().InitAsTrash();
            trash_list.Add(spawned);
        }
    }

    //[ObserversRpc]
    public void Server_TransformTrashInFood()
    {
        foreach (var trash in trash_list)
        {
            NetworkManager.Log("Changing trash into food." + trash.name);
            trash.GetComponent<Food>().SetFoodModel();
        }
    }

    [ObserversRpc]
    public void Observer_TransformTrashInFood()
    {
        //if (trash_list.Count == 0) SetClientsFoodTrashList();
        foreach (var trash in trash_list)
        {
            NetworkManager.Log("Changing trash into food." + trash.name);
            trash.GetComponent<Food>().SetFoodModel();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveObject(GameObject obj)
    {
        NetworkManager.Log("Despawning object: " + obj);
        if (obj.GetComponent<Food>().is_food)
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
}