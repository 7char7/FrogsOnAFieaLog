using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlashEffect : MonoBehaviour
{
    public static DamageFlashEffect Instance { get; private set; }

    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("References")]
    [SerializeField] private Image flashImage;

    private Canvas canvas;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetupFlashOverlay();
    }

    private void SetupFlashOverlay()
    {
        if (flashImage == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            GameObject imageObj = new GameObject("FlashImage");
            imageObj.transform.SetParent(transform, false);

            flashImage = imageObj.AddComponent<Image>();
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);

            RectTransform rectTransform = flashImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            flashImage.raycastTarget = false;
        }
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / flashDuration;
            float curveValue = flashCurve.Evaluate(normalizedTime);

            Color currentColor = flashColor;
            currentColor.a = flashColor.a * curveValue;
            flashImage.color = currentColor;

            yield return null;
        }

        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }
}
