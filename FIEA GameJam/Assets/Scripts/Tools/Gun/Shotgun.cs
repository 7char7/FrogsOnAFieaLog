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

    public override void Shoot()
    {
        if (Time.time < nextFireTime || currentAmmo <= 0)
            return;

        currentAmmo--;
        
        int pelletsPerShot = (int)gunStatsScriptableObject.GetStat(Stat.bulletsPerShot);
        for (int i = 0; i < pelletsPerShot; i++)
        {
            RaycastHit hit;
            Vector3 shootDirection = GetShootingDirection();
            
            if (Physics.Raycast(cameraTransform.position, shootDirection, out hit, gunStatsScriptableObject.GetStat(Stat.range)))
            {
                CreateBulletTrail(hit.point);
                
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int damagePerPellet = Mathf.CeilToInt(gunStatsScriptableObject.GetStat(Stat.damage) / pelletsPerShot);
                    enemy.TakeDamage(damagePerPellet);
                }
            }
            else
            {
                CreateBulletTrail(cameraTransform.position + shootDirection * gunStatsScriptableObject.GetStat(Stat.range));
            }
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

        nextFireTime = Time.time + (1f / gunStatsScriptableObject.GetStat(Stat.fireRate));
    }
    
    private IEnumerator FlashMuzzleLight()
    {
        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(muzzleFlashDuration);
        muzzleFlashLight.enabled = false;
    }
}
