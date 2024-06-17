using FishNet;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public static Dictionary<int, Bullet> Bullets = new Dictionary<int, Bullet>();
    [HideInInspector] public List<State> PastStates = new List<State>();

    private Vector3 _direction;
    [SerializeField] private float _speed;
    public int Identification;
    public int OwnerID;

    [SerializeField]
    private LayerMask exclude_mask; // Excluding player from raycast

    //private float distance_to_collision;
    [SerializeField]
    private float max_distance;


    [Header("Debugging")]
    [SerializeField]
    private bool drawGizmos = true;

    private void Awake()
    {
        if (InstanceFinder.IsServer)
            InstanceFinder.TimeManager.OnTick += OnTick;
        exclude_mask = ~exclude_mask;
    }
    // Start is called before the first frame update
    public void Initialize(Vector3 direction, float speed, int bulletID, int ownerID)
    {
        _direction = direction;
        //_speed = speed;
        Bullets.Add(bulletID, this);
        Identification = bulletID;
        OwnerID = ownerID;
        //distance_to_collision = DistanceToCollision();
        //Debug.Log($"[ID: {bulletID}] Distance to collision: {distance_to_collision}");
    }

    // Update is called once per frame
    void Update()
    {
        float increment = Time.deltaTime * _speed;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _direction, out hit, increment, exclude_mask))
        {
            Debug.Log($"Collision with {hit.collider.transform.name}");
            OnCollision();
        }
        else transform.position += _direction * increment;
    }

    private void OnCollision()
    {
        DestroyBullet();
    }

    private void OnTick()
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
                // This bullet hit a player.
                Debug.Log("Player Collision!");
                DestroyBullet();
            }
  
        }
    }

    //private float DistanceToCollision()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, _direction, out hit))
    //    {
    //        return hit.distance;
    //    }
    //    return max_distance;
    //}


    public void DestroyBullet()
    {
        if (InstanceFinder.IsServer)
            InstanceFinder.TimeManager.OnTick -= OnTick;

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
