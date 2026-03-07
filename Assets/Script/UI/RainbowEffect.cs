using UnityEngine;

public class RainbowEffect : MonoBehaviour
{
    public Material mat; // 指定材质
    public float speed = 1.0f; // 变化速率，默认为1.0
    private float time;

    void Update()
    {
        time += Time.deltaTime * speed; // 根据速度调整时间增量
        mat.color = CalculateRainbowColor(time);
    }

    /// <summary>
    /// 计算彩虹颜色
    /// </summary>
    /// <param name="time">时间参数</param>
    /// <returns>返回计算后的颜色</returns>
    private Color CalculateRainbowColor(float time)
    {
        // 使用 Mathf.PingPong 函数实现颜色的循环渐变
        float r = Mathf.PingPong(time, 1);
        float g = Mathf.PingPong(time + 0.33f, 1);
        float b = Mathf.PingPong(time + 0.66f, 1);
        return new Color(r, g, b, 1.0f); // 返回颜色
    }
}
