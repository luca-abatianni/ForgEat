﻿using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollision : NetworkBehaviour
{

    [SerializeField] string[] _collisionTag;
    [SerializeField] GameObject _sphereGridObj;
    [SerializeField] GameObject _sphereInsideObj;
    float hitTime;
    Material mat;
    public void ActivateShield(bool bActivate)
    {
        gameObject.GetComponent<SphereCollider>().enabled = bActivate;
        gameObject.GetComponent<MeshRenderer>().enabled = bActivate;
        gameObject.GetComponent<AudioSource>().loop = true;
        if (bActivate)
            gameObject.GetComponent<AudioSource>().Play();
        else
            gameObject.GetComponent<AudioSource>().Stop();
        _sphereGridObj.SetActive(bActivate);
        _sphereInsideObj.SetActive(bActivate);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<ShieldCollision>().enabled = false;
            return;
        }
        if (GetComponent<Renderer>())
        {
            mat = GetComponent<Renderer>().sharedMaterial;
        }
        ActivateShield(false);
    }

    void Update()
    {

        if (hitTime > 0)
        {
            float myTime = Time.fixedDeltaTime * 1000;
            hitTime -= myTime;
            if (hitTime < 0)
            {
                hitTime = 0;
            }
            mat.SetFloat("_HitTime", hitTime);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < _collisionTag.Length; i++)
        {

            if (_collisionTag.Length > 0 || collision.transform.CompareTag(_collisionTag[i]))
            {
                //Debug.Log("hit");
                ContactPoint[] _contacts = collision.contacts;
                for (int i2 = 0; i2 < _contacts.Length; i2++)
                {
                    mat.SetVector("_HitPosition", transform.InverseTransformPoint(_contacts[i2].point));
                    hitTime = 500;
                    mat.SetFloat("_HitTime", hitTime);
                }
            }
        }
    }
}

