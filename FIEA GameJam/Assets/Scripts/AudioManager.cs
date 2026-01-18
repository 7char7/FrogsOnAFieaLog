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
    [SerializeField][Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float enemyVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float playerVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float weaponVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float footstepVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float miningVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float crystalVolume = 1f;
    
    [Header("Footstep Settings")]
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
        sfxAudioSource.volume = masterVolume;
    }

    public void PlayEnemyHitSound()
    {
        if (enemyHitSE != null)
        {
            sfxAudioSource.PlayOneShot(enemyHitSE, masterVolume * enemyVolume);
        }
    }

    public void PlayEnemyAttackSound()
    {
        if (enemyAttackSE != null)
        {
            sfxAudioSource.PlayOneShot(enemyAttackSE, masterVolume * enemyVolume);
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
            sfxAudioSource.PlayOneShot(playerDamagedSounds[newIndex], masterVolume * playerVolume);
        }
    }

    public void PlayShotgunSound()
    {
        if (shotgunSE != null)
        {
            sfxAudioSource.PlayOneShot(shotgunSE, masterVolume * weaponVolume);
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepSE != null)
        {
            float randomPitch = Random.Range(footstepPitchMin, footstepPitchMax);
            sfxAudioSource.pitch = randomPitch;
            sfxAudioSource.PlayOneShot(footstepSE, masterVolume * footstepVolume);
            sfxAudioSource.pitch = 1f;
        }
    }

    public void PlayMiningSound()
    {
        if (miningSound != null)
        {
            sfxAudioSource.PlayOneShot(miningSound, masterVolume * miningVolume);
        }
    }

    public void PlayCrystalCollectSound()
    {
        if (crystalCollectSound != null)
        {
            sfxAudioSource.PlayOneShot(crystalCollectSound, masterVolume * crystalVolume);
        }
    }

    public void PlayCrystalShatterSound()
    {
        if (crystalShatterSound != null)
        {
            sfxAudioSource.PlayOneShot(crystalShatterSound, masterVolume * crystalVolume);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        sfxAudioSource.volume = masterVolume;
    }

    public void SetEnemyVolume(float volume)
    {
        enemyVolume = Mathf.Clamp01(volume);
    }

    public void SetPlayerVolume(float volume)
    {
        playerVolume = Mathf.Clamp01(volume);
    }

    public void SetWeaponVolume(float volume)
    {
        weaponVolume = Mathf.Clamp01(volume);
    }

    public void SetFootstepVolume(float volume)
    {
        footstepVolume = Mathf.Clamp01(volume);
    }

    public void SetMiningVolume(float volume)
    {
        miningVolume = Mathf.Clamp01(volume);
    }

    public void SetCrystalVolume(float volume)
    {
        crystalVolume = Mathf.Clamp01(volume);
    }
}
