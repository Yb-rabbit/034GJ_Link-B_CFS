using UnityEngine;

public class PedestrianLightController : MonoBehaviour
{
    [Header("引用汽车信号灯")]
    public TrafficLightController carTrafficLight; // 拖入场景中的汽车红绿灯物体

    [Header("行人灯物体引用")]
    public GameObject pedestrianRedLight; // 行人红灯模型
    public GameObject pedestrianGreenLight; // 行人绿灯模型

    private Renderer redRend, greenRend;

    void Start()
    {
        if (carTrafficLight == null)
        {
            Debug.LogError("未指定汽车信号灯引用！");
            return;
        }

        redRend = pedestrianRedLight.GetComponent<Renderer>();
        greenRend = pedestrianGreenLight.GetComponent<Renderer>();
    }

    void Update()
    {
        if (carTrafficLight == null) return;

        // 核心逻辑：
        // 如果汽车是红灯  -> 行人可以走 (绿灯)
        // 如果汽车是绿灯,黄灯 -> 行人必须停 (红黄灯)
        
        bool canWalk = (carTrafficLight.currentState == TrafficLightController.SignalState.Red); 

        if (canWalk)
        {
            // 行人绿灯亮，红灯灭
            SetPedestrianLight(true); 
        }
        else
        {
            // 行人红灯亮，绿灯灭
            SetPedestrianLight(false);
        }
    }

    void SetPedestrianLight(bool isGreen)
    {
        if (isGreen)
        {
            greenRend.material.color = Color.green;
            redRend.material.color = Color.black;
        }
        else
        {
            greenRend.material.color = Color.black;
            redRend.material.color = Color.red;
        }
    }
}
