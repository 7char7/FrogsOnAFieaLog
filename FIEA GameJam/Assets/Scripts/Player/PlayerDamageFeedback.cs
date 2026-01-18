using UnityEngine;

public class PlayerDamageFeedback : MonoBehaviour
{
    [Header("Screen Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.3f;
    [SerializeField] private float shakeRotationMagnitude = 2f;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken.AddListener(OnDamageTaken);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken.RemoveListener(OnDamageTaken);
        }
    }

    private void OnDamageTaken(int damage)
    {
        TriggerDamageFeedback();
    }

    private void TriggerDamageFeedback()
    {
        if (DamageFlashEffect.Instance != null)
        {
            DamageFlashEffect.Instance.Flash();
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude, shakeRotationMagnitude);
        }
    }
}
