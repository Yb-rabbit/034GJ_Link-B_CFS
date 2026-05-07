using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PausePanel : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeValueText;

    [Header("音量设置")]
    [SerializeField] private float defaultVolume = 0.8f;
    [SerializeField] private float minVolume = 0f;
    [SerializeField] private float maxVolume = 1f;
    private const string VOLUME_KEY = "GameVolume";

    [Header("退出渐变设置")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool isQuittingToMenu = false; 

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        float currentVolume = AudioListener.volume;
        volumeSlider.minValue = minVolume;
        volumeSlider.maxValue = maxVolume;
        volumeSlider.value = currentVolume;
        UpdateVolumeText(currentVolume);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        
        if (!isQuittingToMenu)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResumeClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
        AudioListener.volume = savedVolume;
    }

    public void Show() { gameObject.SetActive(true); }
    public void Hide() { gameObject.SetActive(false); }
    public void Toggle() { if (gameObject.activeSelf) Hide(); else Show(); }

    private void OnResumeClicked()
    {
        Hide(); // 此时 isQuittingToMenu 是 false，OnDisable 会正常锁定鼠标
    }

    private void OnQuitClicked()
    {
        isQuittingToMenu = true; 

        SaveManager.SaveCurrentScene();
        Time.timeScale = 1f; 
        StartCoroutine(FadeAndLoadMenu());
    }

    private IEnumerator FadeAndLoadMenu()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0f;
            Color color = fadeImage.color;
            color.a = 0f; 
            fadeImage.color = color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
        }

        SceneManager.LoadScene("MenuNew");
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        UpdateVolumeText(value);
    }

    private void UpdateVolumeText(float value)
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }
}
