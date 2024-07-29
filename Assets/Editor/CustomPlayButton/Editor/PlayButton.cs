using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

static class ToolbarStyles
{
    public static readonly GUIStyle commandButtonStyle;

    static ToolbarStyles()
    {
        commandButtonStyle = new GUIStyle("Command")
        {
            fontSize = 10,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageLeft,
            fontStyle = FontStyle.Bold,
            fixedWidth = 70,
            wordWrap = false,
        };
    }
}

[InitializeOnLoad]
public class SceneSwitchLeftButton
{
    public static bool splash = false;

    static SceneSwitchLeftButton()
    {
        UnityToolbarExtender.ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        var tex = EditorGUIUtility.IconContent(@"d_PlayButton").image;

        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode);

        if (GUILayout.Button(new GUIContent("Splash", tex, "从Splash页启动"), ToolbarStyles.commandButtonStyle))
        {
            splash = true;
            EditorApplication.EnterPlaymode();
            SceneSwitchLeftButton.splash = false;
        }
        EditorGUI.EndDisabledGroup();
    }
}

[InitializeOnLoad]
public class RewritePlayButton
{
    static RewritePlayButton()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state is PlayModeStateChange.ExitingEditMode)
            {
                if (SceneSwitchLeftButton.splash)
                {
                    SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
                    EditorSceneManager.playModeStartScene = scene;
                }
                else
                {
                    EditorSceneManager.playModeStartScene = null;
                }
            }
        };
    }
}