using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuShopMusic;

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] private float fadeSpeed = 1f;

    private bool shouldPlayMusic;
    private float targetVolume;

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

        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }

        musicAudioSource.playOnAwake = false;
        musicAudioSource.loop = true;
        musicAudioSource.volume = 0f;
        targetVolume = musicVolume;
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
        CheckSceneForMusic(scene.name);
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        CheckSceneForMusic(currentScene.name);
    }

    private void Update()
    {
        if (shouldPlayMusic && !musicAudioSource.isPlaying && menuShopMusic != null)
        {
            musicAudioSource.clip = menuShopMusic;
            musicAudioSource.volume = 0f;
            musicAudioSource.Play();
        }

        float currentVolume = musicAudioSource.volume;
        float desiredVolume = shouldPlayMusic ? targetVolume : 0f;

        if (Mathf.Abs(currentVolume - desiredVolume) > 0.01f)
        {
            musicAudioSource.volume = Mathf.Lerp(currentVolume, desiredVolume, fadeSpeed * Time.deltaTime);
        }
        else
        {
            musicAudioSource.volume = desiredVolume;
            
            if (!shouldPlayMusic && musicAudioSource.isPlaying && musicAudioSource.volume <= 0.01f)
            {
                musicAudioSource.Stop();
            }
        }
    }

    private void CheckSceneForMusic(string sceneName)
    {
        shouldPlayMusic = sceneName == "MainMenu" || sceneName == "Shop";
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        targetVolume = musicVolume;
    }

    public void StopMusic()
    {
        shouldPlayMusic = false;
    }

    public void PlayMusic()
    {
        shouldPlayMusic = true;
    }
}
