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

    [Header("当前状态")]
    public bool isRed = true; // 外部可读，供车辆查询

    private Renderer redRend, yellowRend, greenRend;
    public Material offMaterial; // 灯光关闭时的材质（如黑色）
    public Material onMaterial;  // 灯光开启时的材质（需在Inspector中分别赋值或通过代码改色）

    void Start()
    {
        // 获取渲染器组件
        redRend = redLight.GetComponent<Renderer>();
        yellowRend = yellowLight.GetComponent<Renderer>();
        greenRend = greenLight.GetComponent<Renderer>();

        // 开始信号灯循环
        StartCoroutine(LightCycle());
    }

    IEnumerator LightCycle()
    {
        while (true)
        {
            // 红灯阶段
            SetLight("Red");
            isRed = true;
            yield return new WaitForSeconds(redDuration);

            // 绿灯阶段
            SetLight("Green");
            isRed = false;
            yield return new WaitForSeconds(greenDuration);

            // 黄灯阶段
            SetLight("Yellow");
            isRed = false; // 黄灯通常允许通行或准备停车，这里设为非红灯
            yield return new WaitForSeconds(yellowDuration);
        }
    }

    void SetLight(string state)
    {
        // 简单的材质开关逻辑
        // 注意：实际项目中建议使用 Material.SetColor("_EmissionColor", color) 实现发光效果
        redRend.material.color = state == "Red" ? Color.red : Color.black;
        yellowRend.material.color = state == "Yellow" ? Color.yellow : Color.black;
        greenRend.material.color = state == "Green" ? Color.green : Color.black;
    }
}
