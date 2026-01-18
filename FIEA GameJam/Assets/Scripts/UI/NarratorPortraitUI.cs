using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NarratorPortraitUI : MonoBehaviour
{
    public static NarratorPortraitUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject portraitPanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Portrait Sprite")]
    [SerializeField] private Sprite narratorPortrait;

    [Header("Flash Settings")]
    [SerializeField] private float flashSpeed = 0.15f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    private Coroutine flashCoroutine;
    private bool isShowing;

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

        if (portraitPanel != null)
        {
            portraitPanel.SetActive(false);
        }

        if (portraitImage != null && narratorPortrait != null)
        {
            portraitImage.sprite = narratorPortrait;
        }
    }

    public void ShowPortrait()
    {
        if (isShowing || portraitPanel == null)
            return;

        isShowing = true;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(ShowPortraitCoroutine());
    }

    public void HidePortrait()
    {
        if (!isShowing || portraitPanel == null)
            return;

        isShowing = false;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(HidePortraitCoroutine());
    }

    private IEnumerator ShowPortraitCoroutine()
    {
        portraitPanel.SetActive(true);

        if (canvasGroup != null)
        {
            float elapsed = 0f;
            canvasGroup.alpha = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, maxAlpha, elapsed / fadeInDuration);
                yield return null;
            }

            canvasGroup.alpha = maxAlpha;
        }

        while (isShowing)
        {
            yield return StartCoroutine(FlashEffect());
        }
    }

    private IEnumerator HidePortraitCoroutine()
    {
        if (canvasGroup != null)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        portraitPanel.SetActive(false);
    }

    private IEnumerator FlashEffect()
    {
        if (canvasGroup == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < flashSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(maxAlpha, minAlpha, elapsed / flashSpeed);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < flashSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, elapsed / flashSpeed);
            yield return null;
        }
    }

    public void SetPortraitSprite(Sprite newSprite)
    {
        if (portraitImage != null && newSprite != null)
        {
            portraitImage.sprite = newSprite;
        }
    }
}
