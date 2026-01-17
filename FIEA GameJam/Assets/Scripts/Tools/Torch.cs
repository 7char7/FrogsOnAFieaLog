using UnityEngine;

public class Torch : MonoBehaviour
{
    public Stats torchStatsScriptableObject;
    [SerializeField] private GameObject torch;

    void Awake()
    {
        torchStatsScriptableObject = Instantiate(torchStatsScriptableObject);
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
