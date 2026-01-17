using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected Transform cameraTransform;
    public Stats gunStatsScriptableObject;

    public float currentAmmo;
    protected float nextFireTime;

    [Header("Bullet Trail")]
    [SerializeField] GameObject trailPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float fadeDuration;
    protected Coroutine fireCoroutine;

    protected virtual void Awake()
    {
        gunStatsScriptableObject = Instantiate(gunStatsScriptableObject);
        currentAmmo = gunStatsScriptableObject.GetStat(Stat.maxAmmo);
        nextFireTime = 0f;
    }

    public virtual void Shoot()
    {
        if (Time.time < nextFireTime || currentAmmo <= 0)
            return;
        
        currentAmmo--;
        RaycastHit hit;
        Vector3 shootDirection = GetShootingDirection();
        if (Physics.Raycast(cameraTransform.position, GetShootingDirection(), out hit, gunStatsScriptableObject.GetStat(Stat.range)))
        {
            CreateBulletTrail(hit.point);
            Debug.Log("Hit " + hit.transform.name);
        }
        else
        {
            CreateBulletTrail(cameraTransform.position + shootDirection.normalized * gunStatsScriptableObject.GetStat(Stat.range));
        }

        nextFireTime = Time.time + (1f / gunStatsScriptableObject.GetStat(Stat.fireRate));
    }

    protected virtual IEnumerator Reload()
    {
        if (currentAmmo == gunStatsScriptableObject.GetStat(Stat.maxAmmo))
            yield return null;

        Debug.Log("Reloading...");
        yield return new WaitForSeconds(1f / gunStatsScriptableObject.GetStat(Stat.reloadSpeed));
        currentAmmo = gunStatsScriptableObject.GetStat(Stat.maxAmmo);
        Debug.Log("Reloaded");
    }

    protected Vector3 GetShootingDirection()
    {
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * gunStatsScriptableObject.GetStat(Stat.range);

        targetPosition = new Vector3(
            targetPosition.x + Random.Range(-gunStatsScriptableObject.GetStat(Stat.bulletSpread), gunStatsScriptableObject.GetStat(Stat.bulletSpread)),
            targetPosition.y + Random.Range(-gunStatsScriptableObject.GetStat(Stat.bulletSpread), gunStatsScriptableObject.GetStat(Stat.bulletSpread)),
            targetPosition.z + Random.Range(-gunStatsScriptableObject.GetStat(Stat.bulletSpread), gunStatsScriptableObject.GetStat(Stat.bulletSpread))
        );

        return targetPosition - cameraTransform.position;
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
        LineRenderer trail = Instantiate(trailPrefab).GetComponent<LineRenderer>();
        trail.SetPositions(new Vector3[] { bulletSpawnPoint.position, hitPoint });
        StartCoroutine(FadeTrail(trail));
    }

    IEnumerator FadeTrail(LineRenderer trail)
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeDuration;
            trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, alpha);
            trail.endColor = new Color(trail.endColor.r, trail.endColor.g, trail.endColor.b, alpha);
            yield return null;
        }
        Destroy(trail.gameObject);
    }
}
