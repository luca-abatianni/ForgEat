using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CharacterSpawn : NetworkBehaviour
{
    public List<GameObject> playerSkins = new List<GameObject>();
    [SerializeField] public GameObject connecting;

    void Awake()
    {
        connecting = GameObject.Find("Connecting");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Spawn(playerSkin, LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Spawn(int skinIndex, NetworkConnection conn)
    {
        GameObject player = Instantiate(playerSkins[skinIndex], this.transform.position, Quaternion.identity);
        Spawn(player, conn);
        connecting.SetActive(false);
    }
}
