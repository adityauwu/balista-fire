using System;
using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour {

    private bool _orientToVelocity;
    private Rigidbody _rigidbody;
    private Collider[] colliders;

    public event Func<IEnumerator> OnHit;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
    }

    void Update() {
        if (!_orientToVelocity) return;

        // set the rotation to the velocity
        transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);

        // rotate 90 additional degress to make the cyliner orient along the longer side
        Vector3 transformEulerAngles = transform.eulerAngles;
        transformEulerAngles.x += 90;
        transform.eulerAngles = transformEulerAngles;
    }

    public void OnFired() {
        _orientToVelocity = true;
    }

    private void OnCollisionEnter(Collision collision) {
        // make the object 'stick' when it hits an object
        //_rigidbody.isKinematic = true;
        _orientToVelocity = false;
        if (OnHit != null) {
            StartCoroutine(OnHit());
            OnHit = null;
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Ground") {
            _rigidbody.isKinematic = true;
            transform.SetParent(other.transform, true);
            foreach (Collider collider in colliders) {
                collider.enabled = false; // disable all colliders on the arrow.
            }
            _orientToVelocity = false;
            if (OnHit != null) {
                StartCoroutine(OnHit());
                OnHit = null;
            }
        }
    }
}
