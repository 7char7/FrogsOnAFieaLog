using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Shotgun : Gun
{

    public override void Shoot()
    {
        if (Time.time < nextFireTime || currentAmmo <= 0)
            return;

        currentAmmo--;
        for (int i = 0; i < gunStatsScriptableObject.GetStat(Stat.bulletsPerShot); i++)
        {
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
        }

        nextFireTime = Time.time + (1f / gunStatsScriptableObject.GetStat(Stat.fireRate));
    }
}
