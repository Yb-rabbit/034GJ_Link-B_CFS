using UnityEngine;

public class AIKicker : MonoBehaviour
{
    public float kickPower = 500f; // 踢球力度（可调整）

    private void OnCollisionEnter(Collision collision)
    {
        // 如果碰撞到的是球（确保球有“Ball”标签）
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // 计算撞击方向（从AI指向球）
                Vector3 kickDirection = (collision.transform.position - transform.position).normalized;
                // 抬高角度（防止球贴地摩擦）
                kickDirection.y = 0.3f; 
                // 给球施加爆发力
                ballRb.AddForce(kickDirection * kickPower, ForceMode.Impulse);
            }
        }
    }
}
