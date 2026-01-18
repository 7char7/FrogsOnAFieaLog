using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Shotgun : Gun
{
    [Header("Shell VFX")]
    [SerializeField] private ParticleSystem shellParticles;
    [SerializeField] private ParticleSystem shellParticlesLeft;
    
    [Header("Muzzle Flash VFX")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Light muzzleFlashLight;
    [SerializeField] private float muzzleFlashDuration = 0.05f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.15f;
    [SerializeField] private float shakeRotationMagnitude = 2f;

    public override void Shoot()
    {
        if (Time.time < nextFireTime || currentAmmo <= 0 || !canShoot)
            return;

        currentAmmo--;
        TriggerAmmoChanged();

        int pelletsPerShot = (int)gunStatsScriptableObject.GetStat(Stat.bulletsPerShot);
        for (int i = 0; i < pelletsPerShot; i++)
        { 
            Vector3 shootDirection = GetShootingDirection();
            HandleBullets(shootDirection, gunStatsScriptableObject.GetStat(Stat.damage) / pelletsPerShot);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShotgunSound();
        }

        if (shellParticles != null)
        {
            shellParticles.Play();
        }
        
        if (shellParticlesLeft != null)
        {
            shellParticlesLeft.Play();
        }
        
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        if (muzzleFlashLight != null)
        {
            StartCoroutine(FlashMuzzleLight());
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude, shakeRotationMagnitude);
        }

        nextFireTime = Time.time + (1f / gunStatsScriptableObject.GetStat(Stat.fireRate));
    }
    
    private IEnumerator FlashMuzzleLight()
    {
        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(muzzleFlashDuration);
        muzzleFlashLight.enabled = false;
    }
}
