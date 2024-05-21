using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Component.Animating;
using FishNet.Component.Transforming;

public class AnimationStateController : NetworkBehaviour
{
    Animator animator;
    NetworkAnimator netAnim;
    public FirstPersonController fpc;
    void Awake()
    {
        animator = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }
    void Update()
    {
        if (fpc.isWalking)
        {
            animator.SetBool("isWalking", true);
            Debug.Log("Porco");
        }
        if (!fpc.isWalking)
        {
            animator.SetBool("isWalking", false);
            Debug.Log("Dio");
        }
        if (fpc.isMoonwalking)
        {
            animator.SetBool("isMoonwalking", true);
        }
        if (!fpc.isMoonwalking)
        {
            animator.SetBool("isMoonwalking", false);
        }
        if (fpc.isWalkingLeft)
        {
            animator.SetBool("isWalkingLeft", true);
        }
        if (!fpc.isWalkingLeft)
        {
            animator.SetBool("isWalkingLeft", false);
        }
        if (fpc.isWalkingRight)
        {
            animator.SetBool("isWalkingRight", true);
        }
        if (!fpc.isWalkingRight)
        {
            animator.SetBool("isWalkingRight", false);
        }
    }
}
