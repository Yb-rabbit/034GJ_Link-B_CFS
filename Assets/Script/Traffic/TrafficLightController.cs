using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    [Header("信号灯物体引用")]
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    [Header("灯光持续时间(秒)")]
    public float redDuration = 5f;
    public float yellowDuration = 2f;
    public float greenDuration = 5f;

    // --- 新增部分：定义状态枚举 ---
    public enum SignalState { Red, Yellow, Green }
    // 这是一个公共变量，行人脚本会读取它
    public SignalState currentState = SignalState.Red;
    // -----------------------------

    private Renderer redRend, yellowRend, greenRend;

    void Start()
    {
        redRend = redLight.GetComponent<Renderer>();
        yellowRend = yellowLight.GetComponent<Renderer>();
        greenRend = greenLight.GetComponent<Renderer>();

        StartCoroutine(LightCycle());
    }

    IEnumerator LightCycle()
    {
        while (true)
        {
            // 红灯阶段
            SetLight(SignalState.Red); // 修改状态
            yield return new WaitForSeconds(redDuration);

            // 绿灯阶段
            SetLight(SignalState.Green); // 修改状态
            yield return new WaitForSeconds(greenDuration);

            // 黄灯阶段
            SetLight(SignalState.Yellow); // 修改状态
            yield return new WaitForSeconds(yellowDuration);
        }
    }

    // 修改后的设置方法
    void SetLight(SignalState state)
    {
        currentState = state; // 更新公共状态

        // 材质颜色控制逻辑
        redRend.material.color = state == SignalState.Red ? Color.red : Color.black;
        yellowRend.material.color = state == SignalState.Yellow ? Color.yellow : Color.black;
        greenRend.material.color = state == SignalState.Green ? Color.green : Color.black;
    }

    // 兼容旧代码：保留 isRed 属性
    public bool isRed 
    { 
        get { return currentState == SignalState.Red; } 
    }
}
