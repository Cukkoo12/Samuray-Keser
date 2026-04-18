using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("Ayarlar")]
    public Slider sfxSlider;
    public Slider musicSlider;

    [Header("Oyun Sahnesi Adı")]
    public string gameSceneName = "SampleScene";

    void Start()
    {
        ShowMain();

        float savedSfx = PlayerPrefs.GetFloat("SfxVolume", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSfx;
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }
    }

    void OnSfxChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSfxVolume(value);
        }
        else
        {
            PlayerPrefs.SetFloat("SfxVolume", value);
            PlayerPrefs.Save();
        }
    }

    void OnMusicChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(value);
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            PlayerPrefs.Save();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Çıkış yapılıyor...");
        Application.Quit();
    }

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowControls()
    {
        mainPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
}