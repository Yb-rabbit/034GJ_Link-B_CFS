using UnityEngine;
using UnityEngine.UI;

public class KeyButton_UIShow : MonoBehaviour
{
    [Header("设置对应的按键")]
    public KeyCode targetKey; // 在Inspector面板选择 A, D, W, S

    private Image buttonImage;
    private Color originalColor;
    
    [Header("按下时的颜色")]
    public Color pressColor = Color.gray; // 按下变灰

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color; // 记录原始颜色
    }

    void Update()
    {
        // 检测按键按下
        if (Input.GetKeyDown(targetKey))
        {
            buttonImage.color = pressColor;
            
            // 可选：触发按钮点击事件（如果有挂载Button组件）
            GetComponent<Button>()?.onClick.Invoke();
        }

        // 检测按键抬起
        if (Input.GetKeyUp(targetKey))
        {
            buttonImage.color = originalColor;
        }
    }
}
