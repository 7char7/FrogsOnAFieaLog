using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Crystal Properties")]
    [SerializeField] private CrystalType crystalType;
    [SerializeField] private int baseValue = 10;
    [SerializeField] private float crystalHealth = 100f;
    [SerializeField] private float miningTime = 1f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject wispParticlePrefab;
    [SerializeField] private float rotationSpeed = 30f;

    public CrystalType Type => crystalType;
    public int Value => baseValue;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void MineCrystal(float damage)
    {
        if (crystalHealth >= 0f)
        {
            crystalHealth -= damage;

            if (crystalHealth <= 0f)
            {
                if (wispParticlePrefab != null)
                {
                    Instantiate(wispParticlePrefab, transform.position, Quaternion.identity);
                }
                
                Destroy(this.gameObject);
            }
        }
    }

    public void OnValidate()
    {
        switch (crystalType)
        {
            case CrystalType.Common:
                baseValue = 10;
                crystalHealth = 20f;
                miningTime = 0.5f;
                break;
            case CrystalType.Uncommon:
                baseValue = 25;
                crystalHealth = 50f;
                miningTime = 1f;
                break;
            case CrystalType.Rare:
                baseValue = 50;
                crystalHealth = 100f;
                miningTime = 1.5f;
                break;
            case CrystalType.Legendary:
                baseValue = 100;
                crystalHealth = 200f;
                miningTime = 2f;
                break;
        }
    }
}

public enum CrystalType
{
    Common,
    Uncommon,
    Rare,
    Legendary
}
