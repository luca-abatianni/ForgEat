using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    //public Transform camTransform;
    private bool m_Enabled = false;
    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.5f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
    }

    public void Enable()
    {
        m_Enabled = true;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else if (m_Enabled)
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }
}