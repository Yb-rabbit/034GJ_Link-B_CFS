using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public AudioClip collisionSound; // 撞击时播放的音频剪辑
    public string targetTag = "Target"; // 目标物体的标签
    private AudioSource audioSource; // 用于播放音频的AudioSource组件

    void Start()
    {
        // 确保当前物体上有AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // 如果没有，自动添加
        }

        // 设置AudioSource的音频剪辑
        audioSource.clip = collisionSound;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 当物体发生碰撞时，检查碰撞对象的标签
        if (collision.gameObject.CompareTag(targetTag))
        {
            // 播放音频
            if (collisionSound != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("未分配音频剪辑！");
            }
        }
    }
}
