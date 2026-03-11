using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    public Transform ball; // 记得在Inspector把球拖进来！
    public Transform targetGoal; // 进攻的目标球门

    private NavMeshAgent agent;
    private Rigidbody rb;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        
        // 【关键】AI通常不需要物理旋转，由导航系统控制
        agent.updateRotation = true; 
        
        // 如果你的方块会翻倒，加上这个约束
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (ball == null) return;

        // 简单逻辑：一直追着球跑
        agent.SetDestination(ball.position);
        
        // 如果离球很近了，尝试调整朝向对着球门（方便射门）
        float distance = Vector3.Distance(transform.position, ball.position);
        if (distance < 5f && targetGoal != null)
        {
            // 计算朝向球门的方向
            Vector3 lookDir = (targetGoal.position - transform.position).normalized;
            lookDir.y = 0; // 锁定Y轴
            
            // 平滑转向
            if (lookDir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
            }
        }
    }
    
    // 【物理冲突修复】
    // 如果在移动过程中被撞飞了，AI可能会卡死，这里做一个小修正
    void FixedUpdate()
    {
        // 如果被撞倒了（Y轴不为0），强制归位
        if (Mathf.Abs(transform.position.y - 0.5f) > 0.1f) // 假设正常高度是0.5
        {
            Vector3 pos = transform.position;
            pos.y = 0.5f;
            transform.position = pos;
        }
    }
}
