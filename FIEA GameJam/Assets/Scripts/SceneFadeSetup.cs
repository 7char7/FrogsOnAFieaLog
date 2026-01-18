using UnityEngine;

public class SceneFadeSetup : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    private void Awake()
    {
        if (SceneFadeManager.Instance == null)
        {
            GameObject fadeManagerObject = new GameObject("SceneFadeManager");
            SceneFadeManager fadeManager = fadeManagerObject.AddComponent<SceneFadeManager>();
            DontDestroyOnLoad(fadeManagerObject);
            
            Debug.Log("SceneFadeManager created and ready!");
        }
    }
}
