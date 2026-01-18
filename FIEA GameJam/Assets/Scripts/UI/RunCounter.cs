using UnityEngine;

public class RunCounter : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI runText;
    
    private void OnEnable()
    {
        GameManager.Instance.OnRunCompleted += UpdateRunText;
        UpdateRunText();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRunCompleted -= UpdateRunText;
        }
    }

    private void UpdateRunText()
    {
        int currentRun = GameManager.Instance.CurrentRun;
        int maxRuns = GameManager.Instance.MaxRuns;
        runText.text = $"Run: {currentRun} / {maxRuns}";
    }
}
