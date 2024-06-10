using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private float cameraYOffset = 0f;
    private Camera playerCamera;
    //private static PlayerManager instance;
    //private Dictionary<int, Player> _players = new Dictionary<int, Player>();

    /*public static void InitializeNewPlayer(int clientID)
    {
        instance._players.Add(clientID, new Player());
    }*/
    /*public static void UpdateScore(int player, int amount)
    {
        if (instance._players.TryGetValue(player, out Player thisPlayer))
        {
            thisPlayer.Score += amount;
        }

    }*/
    public override void OnStartClient()
    {
        base.OnStartClient();
        //_players.Add(OwnerId, this.GetComponent<Player>());
        //InitializeNewPlayer(OwnerId);

        if (base.IsOwner)
        {
            var joint = transform.Find("Joint");
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(joint);

            var fpc = GetComponent<FirstPersonController>();
            fpc.joint = playerCamera.transform;
            fpc.playerCamera = Camera.main;
            fpc.enabled = true;

        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    /*class Player
    {
        public int Score = 0;
    }*/

    // Update is called once per frame
    void Update()
    {

    }
}
