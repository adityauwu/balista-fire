using System.Collections;
using UnityEngine;

public class Quiver : MonoBehaviour {

    public CameraFollow cameraFollow;
    public MainCameraZoom zoom;
    public Camera mainCamera;

    public GameObject arrowPrefab;
    public Vector3 maxFireVelocity = new Vector3(0, 30, 0);
    private Vector3 _fireVelocity;
    private GameObject _heldArrow;
    private LineRenderer _trajectoryLineRenderer;
    private bool canFire = true;
    private GameObject tempArrow;
    private float _pullBackAmount = 0.0f;

    private bool IsFiring {
        get { return _pullBackAmount > 0.1f; }
    }

    void Start() {
        _trajectoryLineRenderer = GetComponent<LineRenderer>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && canFire) {
            StopAllCoroutines();
            StartCoroutine(CreateArrow());
        }
        if (Input.GetMouseButtonUp(0) && tempArrow != null) {
            Destroy(tempArrow);
            tempArrow = null;
            StopAllCoroutines();
            StartCoroutine(cameraFollow.SmoothlyMoveCameraToDefaultPosition(.2f));
        }

        if (_heldArrow == null) return;

        PollTouchpad();
        SimulateTrajectory();
        CalculateFireVelocity();

        if (Input.GetMouseButtonUp(0) && _pullBackAmount >= 0.1f) {
            cameraFollow.Target = _heldArrow.transform;
            ReleaseArrow();
            cameraFollow.FollowTarget = true;
        }
        else if (Input.GetMouseButtonUp(0)) {
            Destroy(_heldArrow);
            _heldArrow = null;
            _trajectoryLineRenderer.enabled = false;
            transform.rotation = Quaternion.identity; // reset the balista rotation on cancel fire.
            StartCoroutine(cameraFollow.SmoothlyMoveCameraToDefaultPosition(.2f));
        }
    }

    private void SimulateTrajectory() {
        // only show if the arrow is being fired
        _trajectoryLineRenderer.enabled = IsFiring;

        if (IsFiring)
        {
            Vector3 initialPosition = _heldArrow.transform.position;
            Vector3 initialVelocity = _heldArrow.transform.rotation * _fireVelocity;

            const int numberOfPositionsToSimulate = 200;
            //const float timeStepBetweenPositions = 0.2f;

            // setup the initial conditions
            Vector3 simulatedPosition = initialPosition;
            Vector3 simulatedVelocity = initialVelocity;

            // update the position count
            _trajectoryLineRenderer.positionCount = numberOfPositionsToSimulate;

            for (int i = 0; i < numberOfPositionsToSimulate; i++)
            {
                // set each position of the line renderer
                _trajectoryLineRenderer.SetPosition(i, simulatedPosition);

                // change the velocity based on Gravity and the time step.
                simulatedVelocity += Physics.gravity * Time.fixedDeltaTime;

                // change the position based on Gravity and the time step.
                simulatedPosition += simulatedVelocity * Time.fixedDeltaTime;
            } 
        }
    }

    private void CalculateFireVelocity() {
        _fireVelocity = maxFireVelocity * _pullBackAmount;
    }

    private void PollTouchpad() {
        _pullBackAmount = 1.0f - (Input.mousePosition.y / Screen.height);

        PositionArrow();
    }

    private void PositionArrow() {
        // update the position of the arrow locally based on the pullback amount.
        // Since the touchpad ranges from 0(top)..1(bottom), we need to invert the amount it's coming in
        const float initialOffset = 0.25f;
        Vector3 transformLocalPosition = _heldArrow.transform.localPosition;
        transformLocalPosition.z = initialOffset + 1.0f - _pullBackAmount;
        _heldArrow.transform.localPosition = transformLocalPosition;

        Vector3 cameraPosition = zoom.defaultPosition;
        cameraPosition.z += zoom.cameraMoveForwardCurve.Evaluate(_pullBackAmount);
        mainCamera.transform.position = cameraPosition;
        float newFOV = 60 - Mathf.Abs(40 * _pullBackAmount);
        mainCamera.fieldOfView = newFOV;
        float newRotation = 40 - (25 * _pullBackAmount);
        mainCamera.transform.rotation = Quaternion.Euler(newRotation, transform.rotation.eulerAngles.y, 0); // rotate the camera with the ballista so added the y axis rotation
    }

    private void ReleaseArrow()
    {
        // change the parent to the world
        _heldArrow.transform.SetParent(null, true);
        // nullify the current velocity
        Rigidbody arrowRigidbody = _heldArrow.GetComponent<Rigidbody>();
        arrowRigidbody.velocity = Vector3.zero;
        arrowRigidbody.isKinematic = false;
        // fire the object when releasing while aiming
        arrowRigidbody.AddRelativeForce(_fireVelocity, ForceMode.VelocityChange);
        Arrow arrow = _heldArrow.GetComponent<Arrow>();
        arrow.OnFired();

        TrailRenderer trailRenderer = _heldArrow.GetComponent<TrailRenderer>();
        trailRenderer.enabled = true;

        _trajectoryLineRenderer.enabled = false;
        _heldArrow = null;
        canFire = false;
    }

    private void HoldArrow(GameObject arrow) {
        _heldArrow = arrow;
        tempArrow = null;

        // make a child of this object
        //_heldArrow.transform.SetParent(transform, false);
        //_heldArrow.transform.localPosition = new Vector3(0, 0, 0);
        //_heldArrow.transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    private IEnumerator CreateArrow() {
        // create the arrow
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.SetParent(transform, false);
        arrow.transform.localPosition = new Vector3(0, 0, 1.22f);
        arrow.transform.localEulerAngles = new Vector3(90, 0, 0);
        tempArrow = arrow;
        Arrow a = arrow.GetComponent<Arrow>();
        if (a) {
            a.OnHit += HandleOnHit;
        }
        StartCoroutine(zoom.MoveCameraIn(.2f));
        yield return new WaitForSeconds(.2f);
        // position and orient the arrow near the arm
        HoldArrow(arrow);
    }
    private IEnumerator HandleOnHit() {
        yield return new WaitForSeconds(.5f);
        cameraFollow.FollowTarget = false;
        transform.rotation = Quaternion.identity; // reset the balista rotation on hit.
        StartCoroutine(cameraFollow.SmoothlyMoveCameraToDefaultPosition(1f));
        yield return new WaitForSeconds(1f);
        
        canFire = true;
    }
}