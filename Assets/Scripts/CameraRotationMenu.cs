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
}
