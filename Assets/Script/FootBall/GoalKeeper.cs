using UnityEngine;

public class GoalKeeper : MonoBehaviour
{
    public Transform ball;
    public float moveSpeed = 5f;
    public float leftLimit = -5f; // 球门左边界
    public float rightLimit = 5f; // 球门右边界

    void Update()
    {
        // 守门员只关心球的 X 轴（假设球门在 Z 轴两端）
        // 它只在球门线上左右移动
        
        Vector3 targetPos = new Vector3(ball.position.x, transform.position.y, transform.position.z);
        
        // 限制范围，别跑出球门了
        targetPos.x = Mathf.Clamp(targetPos.x, leftLimit, rightLimit);
        
        // 平滑移动过去
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }
}
