using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [Header("场景设置")]
    [Tooltip("第一关的场景名字")]
    [SerializeField] private string firstLevelName = "Level_01"; 

    [Header("全局黑屏设置")]
    [SerializeField] private Image fadeImage;       
    [SerializeField] private float defaultFadeDuration = 1.5f; // 默认黑屏时间
    [SerializeField] private float delayBeforeExit = 0.5f; 

    /// <summary>
    /// 继续游戏（可自定义黑屏时间）
    /// </summary>
    public void OnContinueGameClicked(float customFadeTime = -1f)
    {
        float targetDuration = customFadeTime < 0 ? defaultFadeDuration : customFadeTime;
        
        string savedScene = PlayerPrefs.GetString("SavedSceneName", "");
        if (!string.IsNullOrEmpty(savedScene))
        {
            StartCoroutine(FadeAndLoadScene(savedScene, targetDuration));
        }
        else
        {
            StartNewGame(targetDuration); 
        }
    }

    /// <summary>
    /// 新游戏（可自定义黑屏时间）
    /// </summary>
    public void StartNewGame(float customFadeTime = -1f)
    {
        float targetDuration = customFadeTime < 0 ? defaultFadeDuration : customFadeTime;
        
        SaveManager.ClearSave();
        StartCoroutine(FadeAndLoadScene(firstLevelName, targetDuration));
    }

    /// <summary>
    /// 退出游戏（可自定义黑屏时间）
    /// </summary>
    public void OnQuitGameClicked(float customFadeTime = -1f)
    {
        float targetDuration = customFadeTime < 0 ? defaultFadeDuration : customFadeTime;
        StartCoroutine(FadeAndExit(targetDuration));
    }

    // --- 下方为内部逻辑，不需要手动调用 ---

    private IEnumerator FadeAndLoadScene(string sceneName, float duration)
    {
        yield return StartCoroutine(PlayFadeOutAnimation(duration));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeAndExit(float duration)
    {
        yield return StartCoroutine(PlayFadeOutAnimation(duration));
        yield return new WaitForSeconds(delayBeforeExit);
        
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private IEnumerator PlayFadeOutAnimation(float duration)
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float elapsedTime = 0f;
            Color color = fadeImage.color;
            color.a = 0f; 
            fadeImage.color = color;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / duration);
                fadeImage.color = color;
                yield return null;
            }
        }
        else
        {
            yield return null;
        }
    }
}
