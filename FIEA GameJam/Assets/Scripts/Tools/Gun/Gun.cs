using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.XR;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] private Image circleIndicator;
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
    protected Coroutine reloadCoroutine;
    protected bool canReload = true;

    protected bool canShoot = true;

    protected virtual void Awake()
    {
        gunStatsScriptableObject = Instantiate(gunStatsScriptableObject);
        nextFireTime = 0f;
    }

    void Start()
    {
        currentAmmo = gunStatsScriptableObject.GetStat(Stat.maxAmmo);
    }

    protected void TriggerAmmoChanged()
    {
        OnAmmoChanged?.Invoke();
    }

    protected virtual void OnDisable()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            canShoot = true;
            if (currentAmmo != gunStatsScriptableObject.GetStat(Stat.maxAmmo))
                canReload = true;
        }

        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    public virtual void Shoot()
    {
        if (Time.time < nextFireTime || currentAmmo <= 0 || !canShoot)
            return;

        currentAmmo--;
        TriggerAmmoChanged();
        Vector3 shootDirection = GetShootingDirection();
        HandleBullets(shootDirection, gunStatsScriptableObject.GetStat(Stat.damage));

        nextFireTime = Time.time + (gunStatsScriptableObject.GetStat(Stat.fireRate));
    }

    protected void HandleBullets(Vector3 shootDirection, float damageAmount)
    {
        if (Physics.Raycast(cameraTransform.position, shootDirection, out RaycastHit hit, gunStatsScriptableObject.GetStat(Stat.range)))
        {
            CreateBulletTrail(hit.point);
            
            if (hit.transform.CompareTag("Enemy"))
            {
                EnemyHitbox hitbox = hit.transform.GetComponent<EnemyHitbox>();
                if (hitbox != null && hitbox.Enemy != null)
                {
                    hitbox.Enemy.TakeDamage(damageAmount);
                    BloodParticleEffect.PlayBloodEffect(hit.point, hit.normal, hitbox.Enemy.Type);
                }
            }
        }
        else
        {
            CreateBulletTrail(cameraTransform.position + shootDirection.normalized * gunStatsScriptableObject.GetStat(Stat.range));
        }
    }

    public void ReloadGun(InputAction.CallbackContext context)
    {
        if (context.performed && this.gameObject.activeSelf && canReload)
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    protected virtual IEnumerator Reload()
    {
        if (currentAmmo == gunStatsScriptableObject.GetStat(Stat.maxAmmo))
            yield return null;

        Debug.Log("Reloading...");
        canReload = false;
        float elapsedTime = 0f;
        canShoot = false;
        circleIndicator.gameObject.SetActive(true);
        circleIndicator.fillAmount = 0f;
        while (elapsedTime < gunStatsScriptableObject.GetStat(Stat.reloadSpeed))
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / gunStatsScriptableObject.GetStat(Stat.reloadSpeed);
            if (circleIndicator != null)
            {
                circleIndicator.fillAmount = progress;
            }
            yield return null;
        }
        canShoot = true;
        canReload = true;
        circleIndicator.gameObject.SetActive(false);
        currentAmmo = gunStatsScriptableObject.GetStat(Stat.maxAmmo);
        reloadCoroutine = null;

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