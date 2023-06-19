using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OperateWindow : EditorWindow
{

    private string eventAssemblyDefineName;

    [MenuItem("ZFrame/OperateWindow")]
    private static void ShowWindow()
    {
        var window = GetWindow<OperateWindow>();
        window.titleContent = new GUIContent("OperateWindow");
        window.Show();


    }
    private void OnGUI()
    {
        if (string.IsNullOrEmpty(this.eventAssemblyDefineName))
        {
            this.eventAssemblyDefineName = EditorUserSettings.GetConfigValue("eventAssemblyDefineName");
        }
        this.eventAssemblyDefineName = EditorGUILayout.TextField("eventAssemblyDefineName", this.eventAssemblyDefineName);
        if (GUILayout.Button("Save EventAssemblyDefineName"))
        {
            saveData();
        }
    }

    private void saveData()
    {
        EditorUserSettings.SetConfigValue("eventAssemblyDefineName", this.eventAssemblyDefineName);
        Debug.Log($"assemblyDefineNameStr :{EditorUserSettings.GetConfigValue("eventAssemblyDefineName")}");
    }

}