using UnityEngine;
using System.Collections;

public class BallManager : MonoBehaviour
{
    [Header("设置")]
    public Vector3 startPos = new Vector3(0, 0.5f, 0);
    public float resetDelay = 1f;
    [Tooltip("场地大小边界")]
    public float boundaryX = 15f;
    public float boundaryZ = 10f;

    private Rigidbody rb;
    private bool isResetting = false; // 【关键】防止重复触发的开关

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 确保刚开始时球是活着的
        rb.isKinematic = false;
    }

    void Update()
    {
        // 如果正在重置中，就不要检测了，防止死循环
        if (isResetting)
            return;

        // 检测是否出界
        bool isOut = (Mathf.Abs(transform.position.x) > boundaryX || Mathf.Abs(transform.position.z) > boundaryZ || transform.position.y < -5f);
        // 只有掉到很深的地方才重置
        if (isOut)
        {
            ResetBall();
        }
    }

    public void ResetBall()
    {
        if (isResetting)
            return; // 如果已经在重置了，就别再来了
        StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        isResetting = true; // 锁住

        // 1. 冻结球，防止它在重置倒计时里乱跑
        rb.isKinematic = true;

        // 2. 等待时间（进球欢呼等）
        yield return new WaitForSeconds(resetDelay);

        // 3. 强制清除物理状态
        rb.velocity = Vector3.zero;
        // 【关键修改】Kinematic状态下通过transform.rotation重置旋转（替代angularVelocity）
        transform.rotation = Quaternion.identity; 

        // 4. 强制归位
        transform.position = startPos;

        // 5. 【关键】解冻，恢复物理
        rb.isKinematic = false;
        // 给一点点向上的微力，防止卡在地里
        // rb.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);

        isResetting = false; // 解锁
    }
}
