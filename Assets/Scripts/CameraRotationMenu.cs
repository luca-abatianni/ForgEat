using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationMenu : MonoBehaviour
{
    public float speed = 1.0f;

    public void RotateCamera()
    {
        transform.Rotate(new Vector3(0.0f, 7.0f, 0.0f));
        transform.position += new Vector3 (0.0f, 0.0f, 80.0f);
    }

    public void RotateBackCamera()
    {
        transform.Rotate(new Vector3(0.0f, -7.0f, 0.0f));
        transform.position -= new Vector3 (0.0f, 0.0f, 80.0f);
    }

    IEnumerator LerpRotateCamera()
    {
        float timeSinceStarted = 0f;
        Vector3 newPosition = transform.position + new Vector3 (0.0f, 0.0f, 80.0f);
        Quaternion newRotation = transform.rotation * Quaternion.Euler(0.0f, 7.0f, 0.0f);
        
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, newPosition, timeSinceStarted), Quaternion.Lerp(transform.rotation, newRotation, timeSinceStarted));

            // If the object has arrived, stop the coroutine
            if (transform.position == newPosition)
            {
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }

    public void LerpRotateCameraWrapper()
    {
        StartCoroutine(LerpRotateCamera());
    }

    IEnumerator LerpRotateBackCamera()
    {
        float timeSinceStarted = 0f;
        Vector3 newPosition = transform.position - new Vector3 (0.0f, 0.0f, 80.0f);
        Quaternion newRotation = transform.rotation * Quaternion.Euler(0.0f, -7.0f, 0.0f);
        
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, newPosition, timeSinceStarted), Quaternion.Lerp(transform.rotation, newRotation, timeSinceStarted));

            // If the object has arrived, stop the coroutine
            if (transform.position == newPosition)
            {
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }

    public void LerpRotateBackCameraWrapper()
    {
        StartCoroutine(LerpRotateBackCamera());
    }
}
