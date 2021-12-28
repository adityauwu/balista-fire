using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraZoom : MonoBehaviour {

    private Camera cameraMain;
    public Vector3 defaultPosition;

    public Vector3 newPosition;
    private float newFOV;

    public AnimationCurve cameraMoveForwardCurve;

    // Use this for initialization
    void Awake () {
        cameraMain = transform.GetComponent<Camera>();
        defaultPosition = transform.position;
    }

    public IEnumerator MoveCameraIn(float timeTakenDuringLerp) {
        float timeStartedLerping = Time.time;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float startFOV = cameraMain.fieldOfView;
        float _pullBackAmount = 1.0f - (Input.mousePosition.y / Screen.height);
        Vector3 position = defaultPosition;
        position.z += cameraMoveForwardCurve.Evaluate(_pullBackAmount);
        float newRotation = 40 - (25 * _pullBackAmount);
        newFOV = startFOV - Mathf.Abs(40 * _pullBackAmount);

        float percentageComplete = 0f;
        while (percentageComplete < 1.0f)
        {
            float timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            transform.position = Vector3.Lerp(startPosition, position, percentageComplete);
            transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(newRotation, 0,0), percentageComplete);
            cameraMain.fieldOfView = Mathf.Lerp(startFOV, newFOV, percentageComplete);
            yield return null;
        }
        transform.position = position;
        transform.rotation = Quaternion.Euler(newRotation, 0, 0);
        cameraMain.fieldOfView = newFOV;

    }
}
