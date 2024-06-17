using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public static Dictionary<int, Bullet> Bullets = new Dictionary<int, Bullet>();
    [HideInInspector] public List<State> PastStates = new List<State>();

    private Vector3 _direction;
    [SerializeField] private float _speed;
    public int Identification;
    public int OwnerID;

    [Header("Debugging")]
    [SerializeField]
    private bool drawGizmos = true;

    private void Awake()
    {
        if (InstanceFinder.IsServer)
            InstanceFinder.TimeManager.OnTick += OnTick;
    }
    // Start is called before the first frame update
    public void Initialize(Vector3 direction, float speed, int bulletID, int ownerID)
    {
        _direction = direction;
        //_speed = speed;
        Bullets.Add(bulletID, this);
        Identification = bulletID;
        OwnerID = ownerID;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _direction * Time.deltaTime * _speed;    
    }

    private void OnTick()
    {
        if (PastStates.Count > InstanceFinder.TimeManager.TickRate)
        {
            PastStates.RemoveAt(0);
        }
/*
        PastStates.Add(new State() { Position - transform.position });

        foreach (var player in PlayerCollisionRollback.Players.Values)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > 3f) continue;

            if (player.CheckPastCollisions(this))
            {
                // This bullet hit a player.
            }
  
        }
*/
    }

    public void DestroyBullet()
    {
        if (InstanceFinder.IsServer)
            InstanceFinder.TimeManager.OnTick -= OnTick;

        Bullets.Remove(Identification);
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        // Handle collision logic here
        Debug.Log("Bullet collided with " + collision.gameObject.name);

        // Optionally, destroy the bullet after collision
        Destroy(gameObject);
    }

    public class State
    {
        public Vector3 Position;
    }
}
