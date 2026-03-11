using UnityEngine;

public class Kicker : MonoBehaviour
{
    public float kickPower = 10f; // 踢球力度
    
    // 当有东西进入这个隐形区域时
    private void OnTriggerStay(Collider other)
    {
        // 如果碰到的是球（假设球的 Tag 设为了 "Ball"）
        if (other.CompareTag("Ball"))
        {
            Rigidbody ballRb = other.GetComponent<Rigidbody>();
            
            // 获取父物体（即方块球员）的朝向
            Vector3 kickDirection = transform.parent.forward;
            
            // 给球一个爆发力
            ballRb.AddForce(kickDirection * kickPower, ForceMode.Impulse);
        }
    }
}
