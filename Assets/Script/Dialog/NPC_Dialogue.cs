using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject dialogueUI;     // 对话面板
    public DialogueManager dialogueManager; // 对话管理器
    public DialogueData dialogueData; // 这里的对话数据

    // 这个方法将被 LookAtPlayerWhenNear 的 Inspector 事件调用
    public void ActivateDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true);
        }
        
        // 启动对话管理器，传入数据
        if (dialogueManager != null && dialogueData != null)
        {
            dialogueManager.StartDialogue(dialogueData);
        }
    }
}
