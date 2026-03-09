using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI组件")]
    public Text dialogueText;
    public Button nextButton;
    
    [Header("可选组件")]
    public AudioSource audioSource;

    private DialogueData currentData;
    private int currentIndex = 0;

    void Start()
    {
        // 确保一开始是隐藏的，或者根据需求调整
        // gameObject.SetActive(false); 
        nextButton.onClick.AddListener(OnNextClick);
    }

    // 外部调用：开始新对话
    public void StartDialogue(DialogueData data)
    {
        currentData = data;
        currentIndex = 0;
        ShowLine();
    }

    private void OnNextClick()
    {
        // 自动推进到下一行
        currentIndex++;

        // 检查是否结束
        if (currentData != null && currentIndex < currentData.dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowLine()
    {
        var line = currentData.dialogueLines[currentIndex];
        dialogueText.text = line.text;

        // 播放音频（可选）
        if (audioSource != null && line.audioClip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(line.audioClip);
        }
    }

    private void EndDialogue()
    {
        //Debug.Log("对话结束");
        // 关闭UI或重置
        gameObject.SetActive(false); 
        // 或者如果你想让 NPC_Dialogue 控制关闭，这里可以留空或发事件
    }
}
