using UnityEngine;

public class HeartbeatPulse : MonoBehaviour
{
    public Material material;
    public float bpm = 70f;          // 心跳频率（次/分钟）
    private float lastTime;

    void Update()
    {
        if (material == null) return;
        // 计算脉冲值：每周期一个尖峰
        float period = 60f / bpm;
        float t = (Time.time % period) / period; // 0..1 一个周期内
        // 产生窄尖峰
        float pulse = Mathf.Clamp01(1f - Mathf.Abs(t - 0.15f) / 0.1f); // 尖峰在 0.15 位置
        material.SetFloat("_Pulse", pulse);
        // 同时可以根据 pulse 改变振幅或颜色闪烁
        // material.SetFloat("_Amplitude", 0.1f + pulse * 0.05f);
    }
}