using System.Collections;
using UnityEngine;

public class EnemyPartFreeze : MonoBehaviour {

    Rigidbody rig;
    public Rigidbody hip;
    public Rigidbody spine;


    // Use this for initialization
    void Start() {
        rig = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Arrow") {
            rig.isKinematic = true;
            rig.constraints = RigidbodyConstraints.FreezeAll;
            if (gameObject.name == "Head") {
                hip.isKinematic = true;
                hip.constraints = RigidbodyConstraints.FreezeAll;
                spine.isKinematic = true;
                spine.constraints = RigidbodyConstraints.FreezeAll;
            } else if (gameObject.name == "Spine_01") {
                hip.isKinematic = true;
                hip.constraints = RigidbodyConstraints.FreezeAll;
            } else {
                StartCoroutine(DisableAllRigidBodies());
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Arrow") {
            StopAllCoroutines();
            rig.isKinematic = false;
            rig.constraints = RigidbodyConstraints.None;
            hip.isKinematic = false;
            hip.constraints = RigidbodyConstraints.None;
            spine.isKinematic = false;
            spine.constraints = RigidbodyConstraints.None;
            StartCoroutine(DisableAllRigidBodies());
        }
    }

    private IEnumerator DisableAllRigidBodies() {
        yield return new WaitForSeconds(3f);
        hip.isKinematic = true;
        hip.constraints = RigidbodyConstraints.FreezeAll;
        spine.isKinematic = true;
        spine.constraints = RigidbodyConstraints.FreezeAll;
    }
}
