using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SFX Klipleri")]
    public AudioClip swordSwing;
    public AudioClip shockwave;
    public AudioClip enemyHit;
    public AudioClip death;

    [Header("Müzik Klipleri")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;

    [Header("Ses Ayarları")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Sahne Adları")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "SampleScene";

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

           
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;

          
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;

           
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
      
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        if (sceneName == mainMenuSceneName)
        {
            PlayMusic(menuMusic);
        }
        else if (sceneName == gameSceneName)
        {
            PlayMusic(gameMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayGameOverMusic()
    {
        PlayMusic(gameOverMusic);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("SfxVolume", value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

   
    public void PlaySwordSwing() => PlaySfx(swordSwing);
    public void PlayShockwave() => PlaySfx(shockwave);
    public void PlayEnemyHit() => PlaySfx(enemyHit);
    public void PlayDeath() => PlaySfx(death);

    void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}