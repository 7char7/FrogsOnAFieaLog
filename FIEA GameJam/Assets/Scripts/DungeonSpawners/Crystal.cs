using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Crystal Properties")]
    [SerializeField] private CrystalType crystalType;
    [SerializeField] private int baseValue = 10;
    [SerializeField] private float miningTime = 1f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject wispParticlePrefab;
    [SerializeField] private float rotationSpeed = 30f;

    private bool isMined = false;

    public CrystalType Type => crystalType;
    public int Value => baseValue;
    public bool IsMined => isMined;

    private void Update()
    {
        if (!isMined)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void MineCrystal()
    {
        if (isMined) return;

        isMined = true;

        if (wispParticlePrefab != null)
        {
            Instantiate(wispParticlePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void OnValidate()
    {
        switch (crystalType)
        {
            case CrystalType.Common:
                baseValue = 10;
                miningTime = 0.5f;
                break;
            case CrystalType.Uncommon:
                baseValue = 25;
                miningTime = 1f;
                break;
            case CrystalType.Rare:
                baseValue = 50;
                miningTime = 1.5f;
                break;
            case CrystalType.Legendary:
                baseValue = 100;
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
