using UnityEngine;
using UnityEngine.Events; // 必须引入这个命名空间才能使用UnityEvent

public class LookAtPlayerWhenNear : MonoBehaviour
{
    [Header("玩家对象")]
    public Transform player;

    [Header("检测参数")]
    public float detectionDistance = 5f;
    public bool smoothRotation = false;
    public float rotationSpeed = 5f;
    
    [Header("事件触发")]
    [Space]
    [Tooltip("当玩家进入检测范围时触发，可在Inspector中拖拽赋值")]
    public UnityEvent OnPlayerDetected; // 改用 UnityEvent，替代 Action

    private Quaternion originalRotation;
    private bool isEventTriggered = false; // 防止每帧重复触发事件

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null) Debug.LogError("未找到玩家对象！");
        }
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (player == null) return;

        float distanceSqr = (player.position - transform.position).sqrMagnitude;
        float thresholdSqr = detectionDistance * detectionDistance;

        if (distanceSqr <= thresholdSqr)
        {
            // 面向玩家逻辑
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                if (smoothRotation)
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                else
                    transform.rotation = targetRotation;
            }

            // 触发事件（仅在首次进入时触发，避免重复）
            if (!isEventTriggered)
            {
                OnPlayerDetected?.Invoke(); // 触发 Inspector 中配置的事件
                isEventTriggered = true;
            }
        }
        else
        {
            // 离开范围：恢复旋转，并重置触发状态
            if (smoothRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
            else
                transform.rotation = originalRotation;

            isEventTriggered = false; // 离开后重置，允许下次再次触发
        }
    }
}
