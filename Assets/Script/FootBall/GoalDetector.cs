using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public AudioClip cheerSound;
    
    // 【新增】把 BallManager 拖进来，或者自动找到
    public BallManager ballManager; 

    void Start()
    {
        if(ballManager == null)
            ballManager = FindObjectOfType<BallManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // 播放声音
            AudioSource.PlayClipAtPoint(cheerSound, transform.position);
            
            // 【修改】调用重置方法
            ballManager.ResetBall();
        }
    }
}
