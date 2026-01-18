using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake(float duration, float magnitude, float rotationMagnitude = 0f)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude, rotationMagnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude, float rotationMagnitude)
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            if (rotationMagnitude > 0f)
            {
                float rotationX = Random.Range(-1f, 1f) * rotationMagnitude;
                float rotationY = Random.Range(-1f, 1f) * rotationMagnitude;
                float rotationZ = Random.Range(-1f, 1f) * rotationMagnitude;

                transform.localRotation = originalRotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    public IEnumerator ShakeImpulseCoroutine(float duration, float magnitude, float rotationMagnitude)
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            if (rotationMagnitude > 0f)
            {
                float rotationX = Random.Range(-1f, 1f) * rotationMagnitude;
                float rotationY = Random.Range(-1f, 1f) * rotationMagnitude;
                float rotationZ = Random.Range(-1f, 1f) * rotationMagnitude;

                transform.localRotation = originalRotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
            }

            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(1f);
            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}
