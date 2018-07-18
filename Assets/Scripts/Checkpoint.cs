using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public PlayerPhysics player;
     Animator animator;
    public bool thisCheckpoint = false;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            animator.SetBool("Activated", true);
        }
    }

}
