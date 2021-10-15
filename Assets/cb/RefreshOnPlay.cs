using UnityEditor;

[InitializeOnLoad]
class RefreshOnPlay
{
    static RefreshOnPlay()
    {
        EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
    }

    static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
    {
        if (obj == PlayModeStateChange.ExitingEditMode)
            AssetDatabase.Refresh();
    }
}