﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollComponent : MonoBehaviour
{
    [SerializeField] Collider[] myCollider = new Collider[1];
    [SerializeField] Rigidbody myRigidbody = null;
    [SerializeField] Animator anim = null;

    Bone[] myBones;

    private void Awake()
    {
        myBones = GetComponentsInChildren<Bone>();
        Ragdoll(false);
    }

    public void Ragdoll(bool active)
    {
        for (int i = 0; i < myCollider.Length; i++)
        {
            myCollider[i].enabled = !active;
        }

        myRigidbody.isKinematic = active;
        myRigidbody.detectCollisions = !active;
        anim.enabled = !active;

        for (int i = 0; i < myBones.Length; i++)
        {
            myBones[i].GetComponent<Rigidbody>().isKinematic = !active;
            myBones[i].GetComponent<Rigidbody>().detectCollisions = active;
            myBones[i].GetComponent<Collider>().enabled = active;
        }

        myBones[0].GetComponent<Rigidbody>().AddForce((-anim.transform.forward + anim.transform.up) * 40, ForceMode.Impulse);
    }
}