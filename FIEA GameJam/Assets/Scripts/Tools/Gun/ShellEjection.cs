using UnityEngine;

public class ShellEjection : MonoBehaviour
{
    [Header("Shell Settings")]
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private Transform ejectionPoint;
    
    [Header("Ejection Physics")]
    [SerializeField] private Vector3 ejectionDirection = new Vector3(1f, 1f, 0f);
    [SerializeField] private float ejectionForce = 3f;
    [SerializeField] private float ejectionForceRandomness = 0.5f;
    [SerializeField] private float torqueAmount = 5f;
    
    [Header("Shell Lifetime")]
    [SerializeField] private float shellLifetime = 5f;
    
    public void EjectShell()
    {
        if (shellPrefab == null || ejectionPoint == null)
            return;
        
        GameObject shell = Instantiate(shellPrefab, ejectionPoint.position, ejectionPoint.rotation);
        
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomizedDirection = ejectionDirection.normalized;
            randomizedDirection += new Vector3(
                Random.Range(-ejectionForceRandomness, ejectionForceRandomness),
                Random.Range(-ejectionForceRandomness, ejectionForceRandomness),
                Random.Range(-ejectionForceRandomness, ejectionForceRandomness)
            );
            
            Vector3 ejectionVelocity = ejectionPoint.TransformDirection(randomizedDirection) * ejectionForce;
            rb.linearVelocity = ejectionVelocity;
            
            rb.angularVelocity = Random.insideUnitSphere * torqueAmount;
        }
        
        Destroy(shell, shellLifetime);
    }
}
