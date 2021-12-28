using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {
    private Transform target;
    private bool followTarget;

    public float smoothSpeed = 0.300f;
    public Vector3 offset;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private float defaultFOV;
    private Camera cameraMain;

    public Transform Target {
        private get {
            return target;
        }

        set {
            if (target != value) {
                target = value;
            }
        }
    }

    public bool FollowTarget {
        private get {
            return followTarget;
        }
        set {
            if (followTarget != value) {
                followTarget = value;
            }
        }
    }

    private void Awake() {
        cameraMain = transform.GetComponent<Camera>();
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
        defaultFOV = cameraMain.fieldOfView;
    }

    void FixedUpdate() {
        if (followTarget && target != null) {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(target);
        }

    }

    public IEnumerator SmoothlyMoveCameraToDefaultPosition(float timeTakenDuringLerp) {
        float timeStartedLerping = Time.time;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float startFOV = cameraMain.fieldOfView;
        float percentageComplete = 0f;
        while (percentageComplete < 1.0f) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            transform.position = Vector3.Lerp(startPosition, defaultPosition, percentageComplete);
            transform.rotation = Quaternion.Lerp(startRotation, defaultRotation, percentageComplete);
            cameraMain.fieldOfView = Mathf.Lerp(startFOV, defaultFOV, percentageComplete);
            yield return null;
        }
        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
        cameraMain.fieldOfView = defaultFOV;
    }
}
