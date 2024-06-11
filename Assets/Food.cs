using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using System;

public class Food : NetworkBehaviour
{
    public float value;

    [SerializeField]
    private bool appears_food;

    public bool is_food;

    [SerializeField]
    private GameObject food_model;
    [SerializeField]
    private GameObject trash_model;
    private float _timer = -1f;
    private bool _hasTimer = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<Food>().enabled = false;
        }

    }

    public void InitAsFood()
    {
        SetFood();
        value = 1;
    }

    public void InitAsTrash()
    {
        SetFood();
        value = -1;
    }

    public void SetFood()
    {
        if (appears_food) return;
        DisableTrash();
        food_model.GetComponent<BoxCollider>().enabled = true;
        food_model.SetActive(true);
    }

    public void SetTrash()
    {
        if (!appears_food) return; // is trash
        DisableFood();
        trash_model.GetComponent<BoxCollider>().enabled = true;
        trash_model.SetActive(true);
    }

    private void DisableTrash()
    {
        trash_model.GetComponent<BoxCollider>().enabled = false;
        trash_model.SetActive(false);
    }

    private void DisableFood()
    {
        food_model.GetComponent<BoxCollider>().enabled = false;
        food_model.SetActive(false);
    }
    //public void ActivateTimer()
    //{//Opzione per il TrickPower
    //    _hasTimer = true;
    //    _timer = Time.time + 1f;// 60f;
    //}
    private void Update()
    {
        //if (_hasTimer && Time.time > _timer)//timer attivato e tempo scaduto
        //{
        //    SRPC_Despawn(gameObject);
        //}
    }
    [ServerRpc]
    void SRPC_Despawn(GameObject go)
    {
        ServerManager.Despawn(go);
    }
}
