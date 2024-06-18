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
    public PlayerController pc;
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
        if (pc.isWalking)
        {
            animator.SetBool("isWalking", true);
            //Debug.Log("Porco");
        }
        if (!pc.isWalking)
        {
            animator.SetBool("isWalking", false);
            //Debug.Log("Dio");
        }
        if (pc.isMoonwalking)
        {
            animator.SetBool("isMoonwalking", true);
        }
        if (!pc.isMoonwalking)
        {
            animator.SetBool("isMoonwalking", false);
        }
        if (pc.isWalkingLeft)
        {
            animator.SetBool("isWalkingLeft", true);
        }
        if (!pc.isWalkingLeft)
        {
            animator.SetBool("isWalkingLeft", false);
        }
        if (pc.isWalkingRight)
        {
            animator.SetBool("isWalkingRight", true);
        }
        if (!pc.isWalkingRight)
        {
            animator.SetBool("isWalkingRight", false);
        }
        if (pc.isJumping)
        {
            animator.SetBool("isJumping", true);
        }
        if (!pc.isJumping)
        {
            animator.SetBool("isJumping", false);
        }
    }
}
