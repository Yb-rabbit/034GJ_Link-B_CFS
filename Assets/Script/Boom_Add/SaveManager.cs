using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    // 定义一个存档的键名
    private const string SCENE_SAVE_KEY = "SavedSceneName";

    /// <summary>
    /// 保存当前所在的场景名字
    /// </summary>
    public static void SaveCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SCENE_SAVE_KEY, currentSceneName);
        PlayerPrefs.Save(); // 强制立即写入，防止退出太快来不及存
        Debug.Log("存档成功，当前场景：" + currentSceneName);
    }

    /// <summary>
    /// 读取存档并跳转（如果没有存档，就去默认的主菜单）
    /// </summary>
    public static void LoadSavedScene(string defaultSceneName = "Menu1")
    {
        // 尝试读取存档的场景名，如果没找到，就返回第二个参数（默认主菜单）
        string sceneToLoad = PlayerPrefs.GetString(SCENE_SAVE_KEY, defaultSceneName);
        
        Debug.Log("正在加载场景：" + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// 清除进度存档（比如玩家看完了结局，或者重新开始游戏时调用）
    /// </summary>
    public static void ClearSave()
    {
        PlayerPrefs.DeleteKey(SCENE_SAVE_KEY);
        Debug.Log("进度存档已清除");
    }
}
