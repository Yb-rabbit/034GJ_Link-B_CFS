using UnityEngine;
using UnityEngine.Audio;

public class PlayAudioSequence : MonoBehaviour
{
    public AudioClip audioClipA; // 音频A
    public AudioClip audioClipB; // 音频B
    private AudioSource audioSource;

    void Start()
    {
        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 播放音频A
        audioSource.clip = audioClipA;
        audioSource.Play();

        // 在音频A播放完成后播放音频B
        Invoke("PlayAudioB", audioClipA.length + 0.8f);
    }

    void PlayAudioB()
    {
        // 播放音频B
        audioSource.clip = audioClipB;
        audioSource.loop = true; // 设置音频B循环播放
        audioSource.Play();
    }
}
