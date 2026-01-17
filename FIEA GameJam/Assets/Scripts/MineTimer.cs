using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MineTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float mineTimeLimit = 120f;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    private float currentTime;
    private bool isTimerRunning;
    
    private void Start()
    {
        currentTime = mineTimeLimit;
        isTimerRunning = true;
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
        }
        else if (currentTime <= 10f)
        {
            timerText.color = Color.red;
        }
    }
    
    private void OnTimerExpired()
    {
        Debug.Log("Time's up! Returning to menu...");
        SceneManager.LoadScene("MainMenu");
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
}
