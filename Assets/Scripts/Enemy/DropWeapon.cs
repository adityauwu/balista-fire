using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapon : MonoBehaviour {

    public void DropSword() {
        transform.SetParent(null, true);
        gameObject.AddComponent<BoxCollider>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}
