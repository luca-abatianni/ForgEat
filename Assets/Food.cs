using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;
using FishNet;
using System;

public class Food : NetworkBehaviour
{
    // Points that will be awarded to the player.
    private float value;

    // Editable from editor. Values awarded to player when object is set as trash or set as food.
    [SerializeField] private int value_as_trash;
    [SerializeField] private int value_as_food;


    // Trash is not food but can appear as such.
    [SerializeField]
    private bool appears_food;

    //Is this ACTUALLY food?
    public bool is_food;

    [SerializeField]
    public GameObject food_model;
    [SerializeField]
    public GameObject trash_model;


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
        appears_food = false;
        is_food = true;
        value = value_as_food;
        SetFoodModel();
    }

    public void InitAsTrash()
    {
        appears_food = false;
        is_food = false;
        value = value_as_trash;
        SetTrashModel();
    }

    public void SetFoodModel()
    {
        if (appears_food || is_food) return;
        appears_food = true;
        DisableTrash();
        //food_model.GetComponent<BoxCollider>().enabled = true;
        food_model.SetActive(true);
    }

    public void SetTrashModel()
    {
        if (!appears_food) return; // is trash
        DisableFood();
        appears_food = false;
        //trash_model.GetComponent<BoxCollider>().enabled = true;
        trash_model.SetActive(true);
    }

    private void DisableTrash()
    {
        //trash_model.GetComponent<BoxCollider>().enabled = false;
        trash_model.SetActive(false);
    }

    private void DisableFood()
    {
        //food_model.GetComponent<BoxCollider>().enabled = false;
        food_model.SetActive(false);
    }

    public float getValue()
    {
        return this.value;
    }
}