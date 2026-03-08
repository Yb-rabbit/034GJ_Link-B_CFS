using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float maxTravelDistance = 100f;

    [Header("检测设置")]
    public float rayDistance = 5f;

    private bool shouldStop = false;
    private Vector3 startPosition;
    private bool isDestroyed = false; // 防止重复销毁

    // 静态变量
    public static int TotalCarCount = 0;
    public static int MaxCarLimit = 10;

    void Start()
    {
        startPosition = transform.position;
        TotalCarCount++;
    }

    void Update()
    {
        if (isDestroyed) return;

        // 1. 行驶距离限制
        if (Vector3.Distance(transform.position, startPosition) > maxTravelDistance)
        {
            DestroyCar();
            return;
        }

        // 2. 射线检测（跟车/红绿灯）
        CheckObstacles();

        // 3. 移动
        if (!shouldStop)
        {
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void CheckObstacles()
    {
        shouldStop = false;
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f + transform.forward * 1f;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, rayDistance + moveSpeed * 0.5f))
        {
            if (hit.collider.CompareTag("Car"))
            {
                shouldStop = true;
            }
            else if (hit.collider.CompareTag("StopLine"))
            {
                if (FindObjectOfType<TrafficLightController>()?.isRed == true)
                {
                    shouldStop = true;
                }
            }
        }
    }

    // ================== 核心碰撞逻辑 ==================
    
    // 使用 Trigger 检测，车辆不会产生物理碰撞反弹，只会触发事件
    void OnTriggerEnter(Collider other)
    {
        if (isDestroyed) return;

        // 如果撞到了其他车辆
        if (other.CompareTag("Car"))
        {
            // 销毁对方
            CarAI otherCar = other.GetComponent<CarAI>();
            if (otherCar != null && !otherCar.isDestroyed)
            {
                otherCar.DestroyCar();
            }
            // 销毁自己
            DestroyCar();
        }
    }

    public void DestroyCar()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        
        TotalCarCount--;
        Destroy(gameObject); // 车辆直接消失
    }
}
