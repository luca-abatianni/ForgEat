using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private float cameraYOffset = 0f;
    private Camera playerCamera;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            var joint=transform.Find("Joint");
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


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
