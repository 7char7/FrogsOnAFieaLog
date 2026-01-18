using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private AudioClip enemyHitSE;
    [SerializeField] private AudioClip enemyAttackSE;
    [SerializeField] private AudioClip[] playerDamagedSounds;
    [SerializeField] private AudioClip shotgunSE;
    [SerializeField] private AudioClip footstepSE;
    [SerializeField] private AudioClip miningSound;
    [SerializeField] private AudioClip crystalCollectSound;
    [SerializeField] private AudioClip crystalShatterSound;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;
    
    [Header("Footstep Settings")]
    [SerializeField][Range(0f, 1f)] private float footstepVolume = 0.5f;
    [SerializeField] private float footstepPitchMin = 0.9f;
    [SerializeField] private float footstepPitchMax = 1.1f;
    
    private int lastPlayerDamagedIndex = -1;

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

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }

        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.volume = sfxVolume;
    }

    public void PlayEnemyHitSound()
    {
        if (enemyHitSE != null)
        {
            sfxAudioSource.PlayOneShot(enemyHitSE, sfxVolume);
        }
    }

    public void PlayEnemyAttackSound()
    {
        if (enemyAttackSE != null)
        {
            sfxAudioSource.PlayOneShot(enemyAttackSE, sfxVolume);
        }
    }

    public void PlayPlayerDamagedSound()
    {
        if (playerDamagedSounds == null || playerDamagedSounds.Length == 0)
            return;
        
        int newIndex;
        
        if (playerDamagedSounds.Length == 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, playerDamagedSounds.Length);
            }
            while (newIndex == lastPlayerDamagedIndex);
        }
        
        lastPlayerDamagedIndex = newIndex;
        
        if (playerDamagedSounds[newIndex] != null)
        {
            sfxAudioSource.PlayOneShot(playerDamagedSounds[newIndex], sfxVolume);
        }
    }

    public void PlayShotgunSound()
    {
        if (shotgunSE != null)
        {
            sfxAudioSource.PlayOneShot(shotgunSE, sfxVolume);
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepSE != null)
        {
            float randomPitch = Random.Range(footstepPitchMin, footstepPitchMax);
            sfxAudioSource.pitch = randomPitch;
            sfxAudioSource.PlayOneShot(footstepSE, footstepVolume);
            sfxAudioSource.pitch = 1f;
        }
    }

    public void PlayMiningSound()
    {
        if (miningSound != null)
        {
            sfxAudioSource.PlayOneShot(miningSound, sfxVolume);
        }
    }

    public void PlayCrystalCollectSound()
    {
        if (crystalCollectSound != null)
        {
            sfxAudioSource.PlayOneShot(crystalCollectSound, sfxVolume);
        }
    }

    public void PlayCrystalShatterSound()
    {
        if (crystalShatterSound != null)
        {
            sfxAudioSource.PlayOneShot(crystalShatterSound, sfxVolume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxAudioSource.volume = sfxVolume;
    }
}
