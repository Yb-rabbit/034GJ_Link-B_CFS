using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class BmTypingEffect : MonoBehaviour
{
    [Header("打字设置")]
    [Tooltip("完成整个文本打字所需的总时间（秒）")]
    public float totalDuration = 2f;          // 打字总时长

    [Header("完成后颜色渐变")]
    [Tooltip("打字完成后，文字渐变到的目标颜色")]
    public Color targetColor = Color.white;    // 目标颜色
    [Tooltip("颜色渐变过程耗时（秒）")]
    public float fadeDuration = 0.5f;          // 渐变时长

    private Text textComponent;                // Text 组件引用
    private string originalText;               // 原始完整文本
    private Coroutine typingCoroutine;          // 打字协程引用

    private void Awake()
    {
        textComponent = GetComponent<Text>();
        originalText = textComponent.text;      // 保存原始文本
    }

    private void OnEnable()
    {
        // 每次启用时，重置并开始打字效果
        StartTypingEffect();
    }

    private void OnDisable()
    {
        // 禁用时停止协程，避免残留
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    /// <summary>
    /// 开始打字效果（重置文本，启动协程）
    /// </summary>
    private void StartTypingEffect()
    {
        // 停止当前正在进行的协程（如果有）
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 重置文本为空，颜色恢复为初始颜色（Text组件设定的颜色）
        textComponent.text = "";
        textComponent.color = textComponent.color;  // 保留原色（也可以通过属性获取默认颜色）
        // 注意：初始颜色应该是 Text 组件在 Inspector 中设置的颜色，我们之后会基于它渐变

        // 开始打字协程
        typingCoroutine = StartCoroutine(TypeText());
    }

    /// <summary>
    /// 打字协程：逐字显示，完成后渐变颜色
    /// </summary>
    private IEnumerator TypeText()
    {
        if (string.IsNullOrEmpty(originalText))
        {
            // 如果没有文本，直接跳转到渐变
            yield return StartCoroutine(FadeColor());
            yield break;
        }

        int totalChars = originalText.Length;
        float charDelay = totalDuration / totalChars;   // 每个字符的平均显示间隔

        for (int i = 0; i <= totalChars; i++)
        {
            // 显示前 i 个字符
            textComponent.text = originalText.Substring(0, i);
            // 最后一个字符显示后无需再等待，直接跳出循环开始渐变
            if (i < totalChars)
                yield return new WaitForSecondsRealtime(charDelay);
        }

        // 打字完成，开始颜色渐变
        yield return StartCoroutine(FadeColor());

        typingCoroutine = null; // 协程结束，清空引用
    }

    /// <summary>
    /// 颜色渐变协程：从当前颜色渐变到 targetColor
    /// </summary>
    private IEnumerator FadeColor()
    {
        Color startColor = textComponent.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            textComponent.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        textComponent.color = targetColor; // 确保最终颜色精确
    }
}