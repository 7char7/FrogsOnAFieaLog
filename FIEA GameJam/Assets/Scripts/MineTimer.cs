using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MineTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float mineTimeLimit = 120f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.03f;
    [SerializeField] private float shakeRotationMagnitude = 2f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Scene Settings")]
    [SerializeField] private string shopSceneName = "Shop";
    
    private float currentTime;
    private bool isTimerRunning;
    private ResourceManager resourceManager;
    private GameManager gameManager;
    
    private void Start()
    {
        currentTime = mineTimeLimit;
        isTimerRunning = true;
        resourceManager = ResourceManager.Instance;
        gameManager = GameManager.Instance;
        
        if (gameManager != null)
        {
            gameManager.StartNewRun();
        }
    }
    
    private void Update()
    {
        if (!isTimerRunning)
            return;
        
        currentTime -= Time.deltaTime;
        
        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerRunning = false;
            OnTimerExpired();
        }
        
        UpdateTimerDisplay();
    }
    
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (currentTime <= 30f && currentTime > 10f)
        {
            timerText.color = Color.yellow;
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(mineTimeLimit, shakeMagnitude, shakeRotationMagnitude);
            }
        }
        else if (currentTime <= 10f)
        {
            timerText.color = Color.red;
        }
    }
    
    private void OnTimerExpired()
    {
        if (resourceManager == null || gameManager == null)
        {
            Debug.LogWarning("ResourceManager or GameManager not found!");
            
            if (SceneFadeManager.Instance != null)
            {
                SceneFadeManager.Instance.FadeToScene(shopSceneName, true);
            }
            else
            {
                RestoreCursor();
                SceneManager.LoadScene(shopSceneName);
            }
            return;
        }
        
        gameManager.CompleteRunFailure();
        
        if (gameManager.HasLost)
        {
            gameManager.LoadLossScene();
        }
        else
        {
            if (SceneFadeManager.Instance != null)
            {
                SceneFadeManager.Instance.FadeToScene(shopSceneName, true);
            }
            else
            {
                RestoreCursor();
                SceneManager.LoadScene(shopSceneName);
            }
        }
    }
    
    public void PauseTimer()
    {
        isTimerRunning = false;
    }
    
    public void ResumeTimer()
    {
        isTimerRunning = true;
    }
    
    public void AddTime(float seconds)
    {
        currentTime += seconds;
    }
    
    private void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
