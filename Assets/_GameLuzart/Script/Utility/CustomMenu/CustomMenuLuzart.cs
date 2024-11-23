#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomMenuLuzart : EditorWindow
{
    [MenuItem("Luzart/Game")]
    public static void Game()
    {
        // Tên của scene bạn muốn chuyển đến
        string sceneName = "Game";

        // Kiểm tra xem scene có tồn tại trong Build Settings hay không
        if (IsSceneInBuildSettings(sceneName))
        {
            // Chuyển scene
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(GetBuildIndex(sceneName)));
        }
        else
        {
            AddSceneToBuildSettings(sceneName);
            // Chuyển scene
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(GetBuildIndex(sceneName)));
        }
        
    }
    [MenuItem("Luzart/Gameplay")]
    public static void Gameplay()
    {
        // Tên của scene bạn muốn chuyển đến
        string sceneName = "GamePlay";

        // Kiểm tra xem scene có tồn tại trong Build Settings hay không
        if (IsSceneInBuildSettings(sceneName))
        {
            // Chuyển scene
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(GetBuildIndex(sceneName)));
        }

    }
    // Kiểm tra xem scene có tồn tại trong Build Settings hay không
    static bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneFileName == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    static int GetBuildIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneFileName == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

    // Thêm scene vào Build Settings
    static void AddSceneToBuildSettings(string sceneName)
    {
        // Lấy đường dẫn của scene
        string scenePath = "Assets/_GameLuzart/Scenes/" + sceneName + ".unity"; // Đường dẫn của scene trong thư mục Assets

        // Tạo một danh sách mới với tất cả các scene hiện tại trong Build Settings
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        // Tạo một scene mới và đặt nó là enabled
        EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(scenePath, true);
        scenes.Add(newScene);

        // Cập nhật Build Settings với danh sách mới
        EditorBuildSettings.scenes = scenes.ToArray();

        GameUtil.Log("Scene " + sceneName + " đã được thêm vào Build Settings.");
    }

    [MenuItem("Luzart/Play")]
    public static void Play()
    {
        Game();
        EditorApplication.isPlaying = true;
    }
}
#endif
