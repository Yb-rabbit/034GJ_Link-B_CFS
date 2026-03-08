using UnityEngine;
using System.Collections;

public class SmartBreathingPursuer : MonoBehaviour
{
    [Header("=== 引用设置 ===")]
    [Tooltip("将玩家（通常是FirstPersonController挂载的物体）拖到这里")]
    public Transform playerTarget;

    [Header("=== 行为参数 ===")]
    [Tooltip("停止移动的安全距离（米）")]
    public float safeDistance = 3f;
    [Tooltip("追踪时的移动速度")]
    public float moveSpeed = 3f;
    [Tooltip("转向时的平滑速度")]
    public float turnSpeed = 5f;

    [Header("=== 呼吸效果设置 ===")]
    public BreathMode breathMode = BreathMode.Random; // 呼吸模式选择
    [Tooltip("呼吸循环的速度")]
    public float breathSpeed = 2f;

    [Header("浮动设置 (仅Float模式有效)")]
    [Tooltip("上下浮动的最小幅度")]
    public float floatMinAmplitude = 0.1f;
    [Tooltip("上下浮动的最大幅度")]
    public float floatMaxAmplitude = 0.5f;

    [Header("缩放设置 (仅Scale模式有效)")]
    [Tooltip("缩放的最小幅度 (1.0为原始大小)")]
    public float scaleMinAmplitude = 0.1f;
    [Tooltip("缩放的最大幅度 (1.0为原始大小)")]
    public float scaleMaxAmplitude = 0.3f;

    // 枚举定义呼吸类型
    public enum BreathMode
    {
        Float,      // 上下浮动
        Scale,      // 缩放
        Random      // 随机选择
    }

    private float currentAmplitude;     // 当前呼吸幅度
    private BreathMode activeMode;      // 实际生效的模式
    private Vector3 originalScale;      // 记录初始大小
    private float randomTimeOffset;     // 随机时间偏移，防止所有物体同步呼吸

    void Start()
    {
        // 1. 自动寻找玩家
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }

        // 2. 初始化呼吸参数
        originalScale = transform.localScale;
        randomTimeOffset = Random.Range(0f, 100f); // 随机相位

        // 确定呼吸模式
        if (breathMode == BreathMode.Random)
        {
            activeMode = (Random.value > 0.5f) ? BreathMode.Float : BreathMode.Scale;
        }
        else
        {
            activeMode = breathMode;
        }

        // 根据模式随机生成幅度
        if (activeMode == BreathMode.Float)
        {
            currentAmplitude = Random.Range(floatMinAmplitude, floatMaxAmplitude);
        }
        else
        {
            currentAmplitude = Random.Range(scaleMinAmplitude, scaleMaxAmplitude);
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 1. 计算与玩家的距离
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // 2. 判断逻辑
        if (distance < safeDistance)
        {
            // 【警戒模式】距离过近 -> 停止移动 -> 表现“呼吸”效果
            PerformBreathing();
        }
        else
        {
            // 【追踪模式】距离较远 -> 追踪玩家 -> 面向移动方向
            ChasePlayer(distance);
        }
    }

    // 追踪逻辑：移动 + 面向
    void ChasePlayer(float distance)
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        
        // 移动
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        // 面向移动方向 (平滑旋转)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // 仅绕Y轴旋转，防止物体倾斜
            targetRotation.eulerAngles = new Vector3(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    // 呼吸效果逻辑
    void PerformBreathing()
    {
        // 使用正弦波计算循环值
        float cycle = Mathf.Sin((Time.time + randomTimeOffset) * breathSpeed);

        switch (activeMode)
        {
            case BreathMode.Float:
                // 上下浮动：利用 Cos 计算速度积分效果，或直接使用 Sin 控制位置偏移
                // 这里使用速度积分法使运动更平滑
                float verticalSpeed = Mathf.Cos((Time.time + randomTimeOffset) * breathSpeed) * currentAmplitude;
                transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.World);
                break;

            case BreathMode.Scale:
                // 缩放：在 1.0 基础上进行正弦波动
                float scaleValue = 1.0f + (cycle * currentAmplitude);
                transform.localScale = originalScale * scaleValue;
                break;
        }
    }

    // 辅助调试：在Scene视图中画出安全距离范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
