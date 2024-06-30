using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Animating;

public class FoodPicker : NetworkBehaviour
{
    [SerializeField] float pick_distance;
    ScoreCounter score_counter;

    [SerializeField]
    Score score;

    public Animator animator;
    public NetworkAnimator netAnim;
    // Start is called before the first frame update
    public override void OnStartClient()
    { // This is needed to avoid other clients controlling our character. 
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<FoodPicker>().enabled = false;
            return;
        }
    }

    private void Start()
    {
        score_counter = FindObjectOfType<ScoreCounter>();
        if (score_counter != null)
        {
            Debug.Log("Score counter found at start()");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Picking!");
            animator.SetBool("isPickingFood", true);
            Food food = CheckFoodCollision();
            if (food != null)
            {
                float points = food.GetComponent<Food>().getValue();
                score.AddPoints(Mathf.RoundToInt(points));
                NetworkManager.Log("Got some food! My score is: " + points);
                score_counter.SetPoints(Mathf.RoundToInt(points));
                FoodSpawner fs = FindObjectOfType<FoodSpawner>();
                fs.RemoveObject(food.gameObject);
            }
        }
        else
        {
            animator.SetBool("isPickingFood", false);
        }
    }


    private Food CheckFoodCollision()
    {
        RaycastHit hit;
        Transform pov_t = Camera.main.transform;
        if (Physics.Raycast(pov_t.position, pov_t.forward, out hit, pick_distance))
        {

            Debug.DrawRay(pov_t.position, pov_t.forward * hit.distance, Color.yellow);
            return hit.transform.gameObject.GetComponent<Food>();
        }
        Debug.DrawRay(pov_t.position, pov_t.forward * pick_distance, Color.white);
        return null;
    }

    public float GetScore()
    {
        return this.score.current_score;
    }

    [ObserversRpc]
    public void Client_FoodPickerSetEnabled(bool setting)
    {
        Debug.Log($"FoodPicker enabled? : {setting}");
        this.enabled = setting;
        return;
    }
}

