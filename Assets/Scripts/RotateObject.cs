using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private float RotationSpeed = 100;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float x = Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -x);
        }
    }
}
