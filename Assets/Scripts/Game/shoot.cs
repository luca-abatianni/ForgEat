using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using TMPro;
using FishNet;

public class shoot : MonoBehaviour
{
    [SerializeField] float force;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(force * this.transform.forward);
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        this.gameObject.SetActive(false);
        GameObject collided_obj = collision.gameObject;
        Debug.Log("Collided object name: " + collided_obj.name);
        if (collided_obj.tag == "Player")
        {
            GameObject go = GameObject.FindGameObjectWithTag("Score");
            ScoreCounter score_counter = go.GetComponent<ScoreCounter>();
            if (score_counter)
            {
                score_counter.AddPoints(1);
            }
        }
        InstanceFinder.ServerManager.Despawn(this.gameObject);
    }
}
