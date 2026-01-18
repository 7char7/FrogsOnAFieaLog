using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.XR;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected Transform cameraTransform;
    public Stats gunStatsScriptableObject;
    public event Action OnAmmoChanged;
    public float currentAmmo;
    protected float nextFireTime;

    [Header("Bullet Trail")]
    [SerializeField] GameObject trailPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float fadeDuration = 0.1f;
    [SerializeField] float trailWidth = 0.02f;
    protected Coroutine fireCoroutine;

    protected virtual void Awake()
    {
        gunStatsScriptableObject = Instantiate(gunStatsScriptableObject);
        currentAmmo = gunStatsScriptableObject.GetStat(Stat.maxAmmo);
        nextFireTime = 0f;
    }

    protected void TriggerAmmoChanged()
    {
        OnAmmoChanged?.Invoke();
    }

    public virtual void Shoot()
    {
        if (Time.time < nextFireTime || currentAmmo <= 0)
            return;
        
        currentAmmo--;
        TriggerAmmoChanged();
        Vector3 shootDirection = GetShootingDirection();
        HandleBullets(shootDirection, gunStatsScriptableObject.GetStat(Stat.damage));

        nextFireTime = Time.time + (gunStatsScriptableObject.GetStat(Stat.fireRate));
    }

    protected void HandleBullets(Vector3 shootDirection, float damageAmount)
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, shootDirection, out hit, gunStatsScriptableObject.GetStat(Stat.range)))
        {
            CreateBulletTrail(hit.point);
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Hit Enemy: " + hit.transform.name);
                hit.transform.GetComponentInParent<Enemy>().TakeDamage(damageAmount);
            }
            Debug.Log("Hit " + hit.transform.name);
        }
        else
        {
            CreateBulletTrail(cameraTransform.position + shootDirection.normalized * gunStatsScriptableObject.GetStat(Stat.range));
        }
    }

    public void ReloadGun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(Reload());
        }
    }

    protected virtual IEnumerator Reload()
    {
        if (currentAmmo == gunStatsScriptableObject.GetStat(Stat.maxAmmo))
            yield return null;

        Debug.Log("Reloading...");
        yield return new WaitForSeconds(gunStatsScriptableObject.GetStat(Stat.reloadSpeed));
        currentAmmo = gunStatsScriptableObject.GetStat(Stat.maxAmmo);
        TriggerAmmoChanged();
        Debug.Log("Reloaded");
    }

    protected Vector3 GetShootingDirection()
    {
        float spreadAngle = gunStatsScriptableObject.GetStat(Stat.bulletSpread);
        
        float randomX = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        float randomY = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        
        Quaternion spreadRotation = Quaternion.Euler(randomY, randomX, 0f);
        Vector3 spreadDirection = spreadRotation * cameraTransform.forward;
        
        return spreadDirection;
    }

    public virtual IEnumerator FireGun()
    {
        while (true)
        {
            Shoot();
            yield return null;
        }
    }

    public void StartFiring(InputAction.CallbackContext context)
    {
        if (this.gameObject.activeSelf == false)
            return;
        
        if (context.started)
        {
            fireCoroutine = StartCoroutine(FireGun());
        }
    }

    public void StopFiring(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
        }
    }

    protected void CreateBulletTrail(Vector3 hitPoint)
    {
        if (trailPrefab == null)
            return;
            
        LineRenderer trail = Instantiate(trailPrefab).GetComponent<LineRenderer>();
        if (trail == null)
            return;
            
        trail.positionCount = 2;
        trail.SetPosition(0, bulletSpawnPoint.position);
        trail.SetPosition(1, hitPoint);
        
        trail.startWidth = trailWidth;
        trail.endWidth = trailWidth * 0.5f;
        
        StartCoroutine(FadeTrailBackToFront(trail, bulletSpawnPoint.position, hitPoint));
    }

    IEnumerator FadeTrailBackToFront(LineRenderer trail, Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;
        Color startColor = trail.startColor;
        Color endColor = trail.endColor;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;
            
            float newAlpha = 1f - progress;
            trail.startColor = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            trail.endColor = new Color(endColor.r, endColor.g, endColor.b, newAlpha);
            
            Vector3 newStartPos = Vector3.Lerp(startPos, endPos, progress);
            trail.SetPosition(0, newStartPos);
            
            yield return null;
        }
        
        Destroy(trail.gameObject);
    }
}
