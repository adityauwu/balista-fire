using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public float moveSpeed;
    public DropWeapon dropWeapon;
    // Update is called once per frame
    void Update () {
        transform.Translate(- transform.forward * moveSpeed * Time.deltaTime);
	}

    private void OnDisable() {
        dropWeapon.DropSword();
    }
}
