using UnityEngine;

public class PauseInput : MonoBehaviour
{
    [SerializeField] private PausePanel pausePanel;

    private void Update()
    {
        // ESC 键切换暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.Toggle();
        }
    }
}
