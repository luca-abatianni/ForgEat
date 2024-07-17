using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CharacterSpawn : NetworkBehaviour
{
    public List<GameObject> playerSkins = new List<GameObject>();
    public GameObject spawnObject;

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Spawn(playerSkin, LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Spawn(int skinIndex, NetworkConnection conn)
    {
        Debug.Log("!!!!!-----Spawn Called");
        List<GameObject> spawnPoints = new List<GameObject>();
        for (int i=0; i < spawnObject.transform.childCount; i++)
            spawnPoints.Add(spawnObject.transform.GetChild(i).gameObject);
            
        GameObject player = Instantiate(playerSkins[skinIndex], spawnPoints[0].transform.position, Quaternion.identity);
        Spawn(player, conn);
    }
}
