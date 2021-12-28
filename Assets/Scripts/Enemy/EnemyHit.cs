using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour {

    public Animator animator;
    public EnemyMovement mov;
    public GameObject bloodEffect;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Arrow") {
            animator.enabled = false;
            mov.enabled = false;
            Instantiate(bloodEffect, this.transform, false);
        }
    }
}
