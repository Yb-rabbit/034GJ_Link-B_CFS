using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIDiogram : MonoBehaviour
{
    #region 显示设置
    [Header("显示")]
    public Color lineColor = Color.green;
    [Range(1, 10)]
    public float lineWidth = 2f;
    #endregion

    #region 波形参数
    [Header("波形（心搏参数）")]
    [Tooltip("一个完整心搏+静息段占用的像素宽度")]
    public float cycleWidth = 200f;
    [Tooltip("波幅（像素）")]
    public float amplitude = 100f;
    [Tooltip("滚动速度（像素/秒）")]
    public float scrollSpeed = 100f;
    #endregion

    #region 心搏形状微调
    [Header("心搏形状微调")]
    [Range(0, 1)] public float qrsPosition = 0.2f;
    [Range(0, 1)] public float qrsWidth = 0.08f;
    [Range(0, 1)] public float tWavePosition = 0.6f;
    [Range(0, 1)] public float tWaveWidth = 0.2f;
    [Range(0, 0.2f)] public float pWaveAmplitude = 0.2f;
    [Range(0, 1)] public float pWavePosition = 0.05f;
    [Range(0, 0.1f)] public float pWaveWidth = 0.06f;
    [Range(0, 0.5f)] public float flatRatio = 0.3f;   // 周期末尾平直线比例
    #endregion

    #region 淡入淡出
    [Header("淡入淡出")]
    [Range(0, 1)]
    public float globalAlpha = 1f;
    public float defaultFadeDuration = 2.5f;
    #endregion

    #region 运行时状态
    private RawImage rawImage;
    private Texture2D texture;
    private Color32[] pixels;
    private int width, height;
    private float offset = 0f;              // 滚动偏移（像素）
    private bool scrollingEnabled = true;    // 滚动是否允许
    private Coroutine activeFadeCoroutine;
    #endregion

    #region Unity 生命周期
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        Rect rect = rawImage.rectTransform.rect;
        width = Mathf.RoundToInt(rect.width);
        height = Mathf.RoundToInt(rect.height);

        if (width <= 0 || height <= 0)
        {
            width = Screen.width > 0 ? Screen.width : 1920;
            height = Screen.height > 0 ? Screen.height : 1080;
            Debug.LogWarning($"RawImage 尺寸无效，使用备用尺寸 {width}x{height}");
        }

        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        rawImage.texture = texture;
        pixels = new Color32[width * height];
        ClearTexture();
    }

    void Update()
    {
        if (texture == null) return;

        if (scrollingEnabled)
        {
            offset += scrollSpeed * Time.deltaTime;
            if (offset >= cycleWidth)
                offset -= cycleWidth;
        }

        DrawWaveform();
        texture.SetPixels32(pixels);
        texture.Apply();
    }
    #endregion

    #region 公共接口
    /// <summary>淡出（波形逐渐透明，可选择停止滚动）</summary>
    public void FadeOut(float duration = -1, bool stopScrolling = true)
    {
        if (duration <= 0) duration = defaultFadeDuration;
        StopCurrentFade();
        activeFadeCoroutine = StartCoroutine(FadeGlobalAlpha(1f, 0f, duration, stopScrolling));
    }

    /// <summary>淡入（波形逐渐恢复可见，并恢复滚动）</summary>
    public void FadeIn(float duration = -1)
    {
        if (duration <= 0) duration = defaultFadeDuration;
        StopCurrentFade();
        scrollingEnabled = true;
        activeFadeCoroutine = StartCoroutine(FadeGlobalAlpha(globalAlpha, 1f, duration, false));
    }

    /// <summary>供 Inspector 按钮调用的淡出（使用默认时长与停止滚动）</summary>
    public void FadeOutDefault() => FadeOut(defaultFadeDuration, true);
    
    /// <summary>供 Inspector 按钮调用的淡入（使用默认时长）</summary>
    public void FadeInDefault() => FadeIn(defaultFadeDuration);

    /// <summary>立即关闭（完全透明，停止滚动）</summary>
    public void TurnOffImmediately()
    {
        globalAlpha = 0f;
        scrollingEnabled = false;
        DrawWaveform();
        texture.Apply();
    }
    #endregion

    #region 私有方法
    private void StopCurrentFade()
    {
        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);
    }

    private IEnumerator FadeGlobalAlpha(float from, float to, float duration, bool disableScrollAtEnd)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            globalAlpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        globalAlpha = to;
        if (disableScrollAtEnd && to == 0f)
            scrollingEnabled = false;
        activeFadeCoroutine = null;
    }

    private float GetHeartbeatValue(float t)
    {
        // 末尾平直线
        if (t > 1f - flatRatio)
            return 0f;

        float value = 0f;
        value += ComputePWave(t);
        value += ComputeQRS(t);
        value += ComputeTWave(t);
        return Mathf.Clamp(value, -0.5f, 1.2f);
    }

    private float ComputePWave(float t)
    {
        float center = pWavePosition;
        float halfWidth = pWaveWidth * 0.5f;
        float dist = Mathf.Abs(t - center);
        if (dist < halfWidth)
        {
            float factor = 1f - (dist / halfWidth);
            return pWaveAmplitude * factor * factor;
        }
        return 0f;
    }

    private float ComputeQRS(float t)
    {
        float center = qrsPosition;
        float halfWidth = qrsWidth * 0.5f;
        float dist = Mathf.Abs(t - center);
        float value = 0f;

        if (dist < halfWidth)
        {
            float spike = 1f - (dist / halfWidth);
            spike = spike * spike;
            value += spike;
        }
        else
        {
            // 轻微负向 Q 波
            if (t > center - halfWidth - 0.03f && t < center - halfWidth)
                value -= 0.15f;
        }
        return value;
    }

    private float ComputeTWave(float t)
    {
        float center = tWavePosition;
        float halfWidth = tWaveWidth * 0.5f;
        float dist = Mathf.Abs(t - center);
        if (dist < halfWidth)
        {
            float factor = 1f - (dist / halfWidth);
            factor = Mathf.Pow(factor, 0.7f);
            return 0.4f * factor;
        }
        return 0f;
    }

    private void DrawWaveform()
    {
        // 清空为完全透明
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color32(0, 0, 0, 0);

        float centerY = height / 2f;
        int halfLineWidth = Mathf.Max(1, Mathf.RoundToInt(lineWidth * 0.5f));
        byte colorR = (byte)(lineColor.r * 255);
        byte colorG = (byte)(lineColor.g * 255);
        byte colorB = (byte)(lineColor.b * 255);
        byte alphaByte = (byte)(255 * globalAlpha);

        for (int x = 0; x < width; x++)
        {
            float samplePos = (x + offset) % cycleWidth;
            float t = samplePos / cycleWidth;
            float yValue = GetHeartbeatValue(t) * amplitude;
            int yPos = Mathf.RoundToInt(centerY - yValue);

            if (yPos >= 0 && yPos < height)
            {
                for (int dy = -halfLineWidth; dy <= halfLineWidth; dy++)
                {
                    int ny = yPos + dy;
                    if (ny >= 0 && ny < height)
                    {
                        pixels[ny * width + x] = new Color32(colorR, colorG, colorB, alphaByte);
                    }
                }
            }
        }
    }

    private void ClearTexture()
    {
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color32(0, 0, 0, 0);
    }
    #endregion
}