using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea] // 让文本框大一点，方便编辑
        public string text;         // 对话文本
        public AudioClip audioClip; // 对应音频（可选）
    }

    // 直接使用数组，代码会自动按顺序索引 0, 1, 2...
    public DialogueLine[] dialogueLines;
}
