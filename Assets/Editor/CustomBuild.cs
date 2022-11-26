using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomBuild
{
    private static string outputProjectsFolder =  Environment.GetEnvironmentVariable("PROJECT_BUILD_PATH");

    static void BuildWindows()
    {
        // TMP Get scenes list
        List<string> scenesToBuild = new List<string>();
        scenesToBuild.Add("Assets/Scenes/Main.unity");
            
        BuildTarget target = BuildTarget.StandaloneWindows;
        EditorUserBuildSettings.SwitchActiveBuildTarget(target);
        PlayerSettings.applicationIdentifier = Environment.GetEnvironmentVariable("PROJECT_BUILD_APP_ID");
        BuildPipeline.BuildPlayer(scenesToBuild.ToArray(), string.Format("{0}/{1}.exe" , outputProjectsFolder, PlayerSettings.applicationIdentifier), target, BuildOptions.None);
    }
    static void BuildAndroid()
    {
        // TMP Get scenes list
        List<string> scenesToBuild = new List<string>();
        scenesToBuild.Add("Assets/Scenes/Main.unity");
            
        BuildTarget target = BuildTarget.Android;
        EditorUserBuildSettings.SwitchActiveBuildTarget(target);
        PlayerSettings.applicationIdentifier = Environment.GetEnvironmentVariable("PROJECT_BUILD_APP_ID");
        BuildPipeline.BuildPlayer(scenesToBuild.ToArray(), string.Format("{0}/{1}.apk" , outputProjectsFolder, PlayerSettings.applicationIdentifier), target, BuildOptions.None);
    }
    static string[] GetScenes()
    {
        var projectScenes = EditorBuildSettings.scenes;
        List<string> scenesToBuild = new List<string>();
        for (int i = 0; i < projectScenes.Length; i++)
        {
            if (projectScenes[i].enabled) {
                scenesToBuild.Add(projectScenes[i].path);
            }
        }
        return scenesToBuild.ToArray();
    }
}