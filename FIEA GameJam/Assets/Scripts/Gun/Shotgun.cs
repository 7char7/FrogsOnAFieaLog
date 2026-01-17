using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class Shotgun : Gun
{
    [Header("Shotgun Settings")]
    public int pelletsPerShot;

    public override void Shoot()
    {
        currentAmmo--;
        for (int i = 0; i < pelletsPerShot; i++)
        {
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
    }
}
