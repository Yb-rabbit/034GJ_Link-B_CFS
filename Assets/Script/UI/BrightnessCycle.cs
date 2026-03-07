using UnityEngine;
using UnityEngine.UI; // 如果使用Text组件
using TMPro; // 如果使用TextMeshPro组件

public class BrightnessCycle : MonoBehaviour
{
    public Text textComponent; // 如果使用Text组件
    // public TextMeshProUGUI textComponent; // 如果使用TextMeshPro组件

    public float duration = 2.0f; // 明度变化的周期时间（秒）
    private float elapsedTime = 0.0f;

    private Color baseColor; // 基础颜色
    public float minBrightness = 0.5f; // 最小明度
    public float maxBrightness = 1.0f; // 最大明度

    private int baseFontSize; // 基础字体大小
    public int minFontSize = 20; // 最小字体大小
    public int maxFontSize = 30; // 最大字体大小

    public bool keepCurrentFontSize = false; // 是否保持当前字体大小

    void Start()
    {
        baseColor = textComponent.color; // 获取初始颜色
        baseFontSize = textComponent.fontSize; // 获取初始字体大小
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 计算当前明度
        float brightness = Mathf.Sin((elapsedTime / duration) * Mathf.PI * 2) * 0.5f + 0.5f;

        // 将明度映射到最小和最大明度之间
        brightness = Mathf.Lerp(minBrightness, maxBrightness, brightness);

        // 更新颜色
        textComponent.color = new Color(
            baseColor.r * brightness,
            baseColor.g * brightness,
            baseColor.b * brightness,
            baseColor.a // 保持透明度不变
        );

        // 如果不保持当前字体大小，则更新字体大小
        if (!keepCurrentFontSize)
        {
            // 计算当前字体大小
            float fontSize = Mathf.Sin((elapsedTime / duration) * Mathf.PI * 2) * 0.5f + 0.5f;

            // 将字体大小映射到最小和最大字体大小之间
            fontSize = Mathf.Lerp(minFontSize, maxFontSize, fontSize);

            // 更新字体大小
            textComponent.fontSize = Mathf.RoundToInt(fontSize);
        }
    }
}
