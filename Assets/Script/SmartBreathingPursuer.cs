using UnityEngine;

public class SmartBreathingPursuer : MonoBehaviour
{
    [Header("=== 引用设置 ===")]
    public Transform playerTarget;

    [Header("=== 行为参数 ===")]
    [Tooltip("停止移动的安全距离（米）")]
    public float safeDistance = 1.5f; // 建议稍微调小一点，防止卡住
    [Tooltip("追踪时的移动速度")]
    public float moveSpeed = 5f;
    [Tooltip("转向时的平滑速度")]
    public float turnSpeed = 5f;

    [Header("=== 防抖动与电梯设置 ===")]
    [Tooltip("Y轴跟随的平滑度（值越大越快，越小越慢）")]
    public float verticalSmoothTime = 0.3f; 

    [Header("=== 呼吸效果设置 ===")]
    public BreathMode breathMode = BreathMode.Random;
    public float breathSpeed = 2f;

    [Header("浮动设置")]
    public float floatMinAmplitude = 0.1f;
    public float floatMaxAmplitude = 0.5f;

    [Header("缩放设置")]
    public float scaleMinAmplitude = 0.1f;
    public float scaleMaxAmplitude = 0.3f;

    public enum BreathMode { Float, Scale, Random }

    private float currentAmplitude;
    private BreathMode activeMode;
    private Vector3 originalScale;
    private float randomTimeOffset;
    
    // 平滑阻尼的速度缓存
    private float verticalVelocityY = 0f; 

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }

        originalScale = transform.localScale;
        randomTimeOffset = Random.Range(0f, 100f);

        if (breathMode == BreathMode.Random)
            activeMode = (Random.value > 0.5f) ? BreathMode.Float : BreathMode.Scale;
        else
            activeMode = breathMode;

        if (activeMode == BreathMode.Float)
            currentAmplitude = Random.Range(floatMinAmplitude, floatMaxAmplitude);
        else
            currentAmplitude = Random.Range(scaleMinAmplitude, scaleMaxAmplitude);
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 1. 计算距离（只用水平距离判断是否到达，防止Y轴呼吸干扰）
        Vector3 currentPos = transform.position;
        Vector3 targetPos = playerTarget.position;
        
        Vector3 horizontalOffset = new Vector3(targetPos.x, currentPos.y, targetPos.z) - currentPos;
        float horizontalDistance = horizontalOffset.magnitude;

        // 2. 计算目标位置
        Vector3 finalPosition = transform.position;

        if (horizontalDistance > safeDistance)
        {
            // --- 追踪模式 (水平) ---
            // 水平移动保持刚性，直接移动
            Vector3 moveDir = horizontalOffset.normalized;
            finalPosition += moveDir * moveSpeed * Time.deltaTime;
        }

        // --- 垂直处理 (关键点) ---
        // 无论是否在安全距离内，Y轴都要平滑过渡。
        // 这样既能跟上电梯(因为目标Y变了)，又能过滤掉微小的抖动。
        float targetY = targetPos.y;
        
        // 如果处于警戒范围且呼吸模式是Float，给目标Y加上呼吸高度
        if (horizontalDistance < safeDistance && activeMode == BreathMode.Float)
        {
            // 计算当前呼吸应该处于的高度偏移
            float breathOffset = Mathf.Sin((Time.time + randomTimeOffset) * breathSpeed) * currentAmplitude;
            targetY += breathOffset;
        }

        // 使用 SmoothDamp 进行垂直平滑
        // 这会让渲染体在电梯启动时平滑跟上，在呼吸时平滑浮动，且不会突然卡顿
        finalPosition.y = Mathf.SmoothDamp(currentPos.y, targetY, ref verticalVelocityY, verticalSmoothTime);

        // 应用最终位置
        transform.position = finalPosition;

        // 3. 旋转逻辑
        if (horizontalDistance > safeDistance)
        {
            Vector3 flatDirection = new Vector3(horizontalOffset.x, 0, horizontalOffset.z);
            if (flatDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        // 4. 缩放呼吸 (Scale模式)
        if (activeMode == BreathMode.Scale)
        {
            float cycle = Mathf.Sin((Time.time + randomTimeOffset) * breathSpeed);
            float scaleValue = 1.0f + (cycle * currentAmplitude);
            transform.localScale = originalScale * scaleValue;
        }
    }
}
