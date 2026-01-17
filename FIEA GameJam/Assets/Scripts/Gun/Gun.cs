using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using Unity.VisualScripting;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected Transform cameraTransform;

    [Header("Gun Settings")]
    public float range;
    public float damage;
    public float fireRate;
    WaitForSeconds fireRateWait;
    public int maxAmmo;
    public int currentAmmo;
    public float reloadTime;
    WaitForSeconds reloadWait;
    public float spreadAngle;

    [Header("Bullet Trail")]
    [SerializeField] GameObject trailPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float fadeDuration;
    protected Coroutine fireCoroutine;

    protected virtual void Awake()
    {
        fireRateWait = new WaitForSeconds(1f / fireRate);
        reloadWait = new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
    }

    public virtual void Shoot()
    {
        currentAmmo--;
        RaycastHit hit;
        Vector3 shootDirection = GetShootingDirection();
        if (Physics.Raycast(cameraTransform.position, GetShootingDirection(), out hit, range))
        {
            CreateBulletTrail(hit.point);
            Debug.Log("Hit " + hit.transform.name);
        }
        else
        {
            CreateBulletTrail(cameraTransform.position + shootDirection.normalized * range);
        }
    }

    protected bool CanShoot()
    {
        return currentAmmo > 0;
    }

    protected virtual IEnumerator Reload()
    {
        if (currentAmmo == maxAmmo)
            yield return null;

        Debug.Log("Reloading...");
        yield return reloadWait;
        currentAmmo = maxAmmo;
        Debug.Log("Reloaded");
    }

    protected Vector3 GetShootingDirection()
    {
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * range;

        targetPosition = new Vector3(
            targetPosition.x + Random.Range(-spreadAngle, spreadAngle),
            targetPosition.y + Random.Range(-spreadAngle, spreadAngle),
            targetPosition.z + Random.Range(-spreadAngle, spreadAngle)
        );

        return targetPosition - cameraTransform.position;
    }

    public virtual IEnumerator FireGun()
    {
        while (true)
        {
            Shoot();
            yield return fireRateWait;
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
