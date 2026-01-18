using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NarratorManager : MonoBehaviour
{
    public static NarratorManager Instance { get; private set; }

    [Header("Shop Voice Lines")]
    [SerializeField] private AudioClip shopWelcome1;
    [SerializeField] private AudioClip shopWelcome2;
    [SerializeField] private AudioClip shopWelcome3;

    [Header("In-Level Messages")]
    [SerializeField] private AudioClip inLevelMessage1;
    [SerializeField] private AudioClip inLevelMessage2;
    [SerializeField] private AudioClip inLevelMessage3;
    [SerializeField] private AudioClip inLevelMessage4;

    [Header("Audio Source")]
    [SerializeField] private AudioSource narratorAudioSource;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float narratorVolume = 0.8f;
    [SerializeField] private float inLevelMessageCooldown = 60f;
    [SerializeField] private int maxInLevelMessagesPerRun = 2;

    private List<AudioClip> availableInLevelMessages;
    private int messagesPlayedThisRun;
    private float lastMessageTime;
    private bool isInMainScene;
    private Coroutine portraitCheckCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (narratorAudioSource == null)
        {
            narratorAudioSource = gameObject.AddComponent<AudioSource>();
        }

        narratorAudioSource.playOnAwake = false;
        narratorAudioSource.volume = narratorVolume;

        InitializeInLevelMessages();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool wasInMainScene = isInMainScene;
        isInMainScene = scene.name == "MainScene";

        if (wasInMainScene && !isInMainScene)
        {
            StopAllNarration();
        }

        if (!wasInMainScene && isInMainScene)
        {
            StopAllNarration();
        }

        if (isInMainScene)
        {
            ResetForNewRun();
        }
    }

    private void Update()
    {
        if (!isInMainScene || narratorAudioSource.isPlaying)
            return;

        if (messagesPlayedThisRun >= maxInLevelMessagesPerRun)
            return;

        if (Time.time - lastMessageTime >= inLevelMessageCooldown)
        {
            PlayRandomInLevelMessage();
        }
    }

    private void LateUpdate()
    {
        if (isInMainScene && NarratorPortraitUI.Instance != null)
        {
            if (narratorAudioSource.isPlaying && portraitCheckCoroutine == null)
            {
                portraitCheckCoroutine = StartCoroutine(MonitorAudioPlayback());
            }
        }
    }

    private IEnumerator MonitorAudioPlayback()
    {
        if (NarratorPortraitUI.Instance != null)
        {
            NarratorPortraitUI.Instance.ShowPortrait();
        }

        while (narratorAudioSource.isPlaying)
        {
            yield return null;
        }

        if (NarratorPortraitUI.Instance != null)
        {
            NarratorPortraitUI.Instance.HidePortrait();
        }

        portraitCheckCoroutine = null;
    }

    private void InitializeInLevelMessages()
    {
        availableInLevelMessages = new List<AudioClip>();
        
        if (inLevelMessage1 != null) availableInLevelMessages.Add(inLevelMessage1);
        if (inLevelMessage2 != null) availableInLevelMessages.Add(inLevelMessage2);
        if (inLevelMessage3 != null) availableInLevelMessages.Add(inLevelMessage3);
        if (inLevelMessage4 != null) availableInLevelMessages.Add(inLevelMessage4);
    }

    public void PlayShopWelcome()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager not found. Cannot determine run number for shop welcome.");
            return;
        }

        int currentRun = GameManager.Instance.CurrentRun;
        AudioClip clipToPlay = null;

        switch (currentRun)
        {
            case 1:
                clipToPlay = shopWelcome1;
                break;
            case 2:
                clipToPlay = shopWelcome2;
                break;
            case 3:
            default:
                clipToPlay = shopWelcome3;
                break;
        }

        if (clipToPlay != null)
        {
            narratorAudioSource.PlayOneShot(clipToPlay, narratorVolume);
        }
        else
        {
            Debug.LogWarning($"Shop welcome clip for run {currentRun} is not assigned.");
        }
    }

    private void PlayRandomInLevelMessage()
    {
        if (availableInLevelMessages.Count == 0)
        {
            Debug.LogWarning("No in-level messages available to play.");
            return;
        }

        int randomIndex = Random.Range(0, availableInLevelMessages.Count);
        AudioClip selectedMessage = availableInLevelMessages[randomIndex];

        if (selectedMessage != null)
        {
            narratorAudioSource.PlayOneShot(selectedMessage, narratorVolume);
            messagesPlayedThisRun++;
            lastMessageTime = Time.time;
        }
    }

    private void ResetForNewRun()
    {
        messagesPlayedThisRun = 0;
        lastMessageTime = -inLevelMessageCooldown;
    }

    public void SetNarratorVolume(float volume)
    {
        narratorVolume = Mathf.Clamp01(volume);
        narratorAudioSource.volume = narratorVolume;
    }

    public void StopNarrator()
    {
        if (narratorAudioSource.isPlaying)
        {
            narratorAudioSource.Stop();
        }
    }

    public void CompleteReset()
    {
        StopAllNarration();
        messagesPlayedThisRun = 0;
        lastMessageTime = -inLevelMessageCooldown;
        isInMainScene = false;
    }

    private void StopAllNarration()
    {
        if (narratorAudioSource.isPlaying)
        {
            narratorAudioSource.Stop();
        }

        if (portraitCheckCoroutine != null)
        {
            StopCoroutine(portraitCheckCoroutine);
            portraitCheckCoroutine = null;
        }

        if (NarratorPortraitUI.Instance != null)
        {
            NarratorPortraitUI.Instance.HidePortrait();
        }
    }
}
