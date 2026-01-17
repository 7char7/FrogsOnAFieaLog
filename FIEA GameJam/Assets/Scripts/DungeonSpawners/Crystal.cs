using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Crystal Properties")]
    [SerializeField] private CrystalType crystalType;
    [SerializeField] private int baseValue = 10;
    [SerializeField] private float maxCrystalHealth = 100f;
    [SerializeField] private float currentCrystalHealth = 100f;
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
        if (currentCrystalHealth >= 0f)
        {
            currentCrystalHealth -= damage;

            if (currentCrystalHealth <= 0f)
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
                maxCrystalHealth = 20f;
                currentCrystalHealth = maxCrystalHealth;
                miningTime = 0.5f;
                break;
            case CrystalType.Uncommon:
                baseValue = 25;
                maxCrystalHealth = 50f;
                currentCrystalHealth = maxCrystalHealth;
                miningTime = 1f;
                break;
            case CrystalType.Rare:
                baseValue = 50;
                maxCrystalHealth = 100f;
                currentCrystalHealth = maxCrystalHealth;
                miningTime = 1.5f;
                break;
            case CrystalType.Legendary:
                baseValue = 100;
                maxCrystalHealth = 200f;
                currentCrystalHealth = maxCrystalHealth;
                miningTime = 2f;
                break;
        }
    }
    public float GetCurrentCrystalHealth()
    {
        return currentCrystalHealth;
    }

    public float GetMaxCrystalHealth()
    {
        return maxCrystalHealth;
    }
}


public enum CrystalType
{
    Common,
    Uncommon,
    Rare,
    Legendary
}
