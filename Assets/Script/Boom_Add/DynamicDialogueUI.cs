using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DynamicDialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    public Text dialogueText;
    public DialogueData currentData; // 允许在面板直接拖拽赋值
    public AudioSource audioSource;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public float fadeOutDuration = 1.0f;
    public float autoNextDelay = 1.5f;

    // 内部状态
    private CanvasGroup canvasGroup;
    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        // 1. 核心修复：自动获取或添加 CanvasGroup，防止空引用报错
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 初始状态设为隐藏
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        // 2. 方便测试：如果在 Inspector 里直接拖了 SO 数据，游戏开始就自动播放
        if (currentData != null)
        {
            StartDialogue(currentData);
        }
    }

    /// <summary>
    /// 外部调用接口1：启动对话（传入 ScriptableObject）
    /// </summary>
    public void StartDialogue(DialogueData data)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentData = data; // 记录数据
        currentIndex = 0;

        // 激活显示
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        PlayCurrentLine();
    }

    /// <summary>
    /// 外部调用接口2：无参启动（配合 UI 按钮事件使用，直接读取面板拖拽的 currentData）
    /// </summary>
    public void StartDialogue()
    {
        if (currentData != null)
        {
            StartDialogue(currentData);
        }
        else
        {
            Debug.LogWarning("没有赋值 DialogueData！");
        }
    }

    /// <summary>
    /// 外部调用接口3：跳过当前打字 / 强制下一段
    /// </summary>
    public void SkipOrNextLine()
    {
        if (isTyping)
        {
            isTyping = false;
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            if (currentData != null && currentIndex < currentData.dialogueLines.Length)
            {
                dialogueText.text = currentData.dialogueLines[currentIndex].text;
            }
        }
        else
        {
            if (currentData != null && currentIndex < currentData.dialogueLines.Length)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                NextLine();
            }
        }
    }

    private void PlayCurrentLine()
    {
        if (currentData == null || currentIndex >= currentData.dialogueLines.Length) return;

        DialogueData.DialogueLine line = currentData.dialogueLines[currentIndex];
        dialogueText.text = "";

        if (audioSource != null)
        {
            audioSource.Stop();
            if (line.audioClip != null)
            {
                audioSource.clip = line.audioClip;
                audioSource.Play();
            }
        }

        typingCoroutine = StartCoroutine(TypeTextCoroutine(line.text));
    }

    private IEnumerator TypeTextCoroutine(string fullText)
    {
        isTyping = true;
        int currentCharIndex = 0;

        while (currentCharIndex < fullText.Length)
        {
            if (!isTyping) yield break;

            currentCharIndex++;
            dialogueText.text = fullText.Substring(0, currentCharIndex);
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        yield return new WaitForSeconds(autoNextDelay);
        NextLine();
    }

    private void NextLine()
    {
        currentIndex++;
        if (currentIndex >= currentData.dialogueLines.Length)
        {
            StartCoroutine(FadeOutCoroutine());
        }
        else
        {
            PlayCurrentLine();
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        dialogueText.text = "";
        typingCoroutine = null;
    }
}
