using FishNet;
using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static PowerBehavior;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public static Dictionary<int, Bullet> Bullets = new Dictionary<int, Bullet>();
    [HideInInspector] public List<State> PastStates = new List<State>();

    private Vector3 _direction;
    [SerializeField] private float _speed;
    public int Identification;
    public int OwnerID;

    [SerializeField]
    private LayerMask layer_mask; // Excluding player from raycast on server side.

    //private float distance_to_collision;
    [SerializeField]
    private float max_distance;
    PowerType _powerType;
    private void Awake()
    {
        if (InstanceFinder.IsServer) // if server
        {
            Debug.Log("I'm server side bullet!");
            layer_mask = ~(1 << 3); // raycast on everything but player which has collider rollback instead.
            InstanceFinder.TimeManager.OnTick += OnTick;
        }
        else // if client
        {
            Debug.Log("I'm client side bullet!");
            layer_mask = ~0; // raycast on everything
        }
    }
    // Start is called before the first frame update
    public void Initialize(Vector3 direction, int bulletID, int ownerID)
    {
        _direction = direction;
        Bullets.Add(bulletID, this);
        Identification = bulletID;
        OwnerID = ownerID;
        //Debug.Log($"Initializing bullet {bulletID} of {ownerID}.");
    }
    public void SetPowerType(PowerBehavior.PowerType type)
    {
        _powerType = type;
    }


    // Update is called once per frame
    void Update()
    {
        float increment = Time.deltaTime * _speed;
        max_distance -= increment;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _direction, out hit, increment, layer_mask))
        {
            PowerBehavior pb = GetComponent<PowerBehavior>();
            pb.CollisionEvent(this.gameObject, null);
            DestroyBullet(true);
        }
        if (max_distance < 0) DestroyBullet(false);
        else transform.position += _direction * increment;
    }

    private void OnTick() // SERVER SIDE ONLY
    {
        if (PastStates.Count > InstanceFinder.TimeManager.TickRate)
        {
            PastStates.RemoveAt(0);
        }
        PastStates.Add(new State() { Position = transform.position });

        foreach (var player in PlayerCollisionRollback.Players.Values)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > 3f) continue;

            if (player.CheckPastCollisions(this))
            {
                this.GetComponent<PowerBehavior>().CollisionEvent(this.gameObject, player.transform.gameObject);
            }

        }
    }

    public void DestroyBullet(bool spawn_effect)
    {
        if (InstanceFinder.IsServer)
            InstanceFinder.TimeManager.OnTick -= OnTick;

        if (spawn_effect)
        {
            if (_powerType == PowerType.IceBullet || _powerType == PowerType.MindBullet)
                GetComponent<PowerBehavior>().OnImpact(this.transform.position, this.transform.rotation);
            else
                GetComponent<PowerBehavior>().OnImpact_SRPC(this.transform.position, this.transform.rotation);
        }
        Bullets.Remove(Identification);
        Destroy(gameObject);
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        // Handle collision logic here
        Debug.Log("Bullet collided with " + collision.gameObject.name);

        // Optionally, destroy the bullet after collision
        Destroy(gameObject);
    }
    */

    public class State
    {
        public Vector3 Position;
    }
}
