using System.Collections.Generic;
using System.Net.Mime;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

/**
 * @author vic
 * @time 2022/5/31
 * @description 扩展工具类
 */
public class ToolTagLayerMaker : MonoBehaviour
{
    [MenuItem("Tool/TagLayerMaker")]
    static void TagLayerMaker()
    {
        //如果不存在Scripts文件夹，就自动创建
        if (!Directory.Exists(Application.dataPath + "/Scripts"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Scripts");
        }
        //如果不存在/Scripts/Provide 文件夹，就自动创建
        if (!Directory.Exists(Application.dataPath + "/Scripts/Provide"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Scripts/Provide");
        }
        //标签层级文件
        string TagManagerPath = Application.dataPath.Replace("Assets", "ProjectSettings/TagManager.asset");
        //逐行读取标签层级文件
        string[] TagManagerLines = File.ReadAllLines(TagManagerPath);
        List<string> tagNames = new List<string>();
        //内置标签
        tagNames.Add("Untagged");
        tagNames.Add("Respawn");
        tagNames.Add("Finish");
        tagNames.Add("EditorOnly");
        tagNames.Add("MainCamera");
        tagNames.Add("Player");
        tagNames.Add("GameController");
        List<string> layerNames = new List<string>();
        bool isTagStart = false;
        bool isLayerStart = false;
        for (int i = 0; i < TagManagerLines.Length; i++)
        {
            string line = TagManagerLines[i];
            if (line.IndexOf("tags:") >= 0)
            {
                isTagStart = true;
                isLayerStart = false;
                continue;
            }
            else if (line.IndexOf("layers:") >= 0)
            {
                isLayerStart = true;
                isTagStart = false;
                continue;
            }
            else if (line.IndexOf("-") == -1)
            {
                isTagStart = false;
                isLayerStart = false;
                continue;
            }
            else if (isTagStart)
            {
                line = line.Replace(" ", "").Replace("-", "");
                if (line != "" && layerNames.IndexOf(line) == -1) tagNames.Add(line);
            }
            else if (isLayerStart)
            {
                line = line.Replace(" ", "").Replace("-", "");
                if (line != "" && layerNames.IndexOf(line) == -1) layerNames.Add(line);
            }
        }
        //文件信息
        string fileInfo = "using UnityEngine;\n" +
          "/**\n" +
          " * 标签 层级 保存\n" +
          " */\n" +
          "public class TagLayer\n" +
          "{\n";
        //标签内容
        for (int i = 0; i < tagNames.Count; i++)
        {
            fileInfo += "    public const string tag_" + tagNames[i] + " = \"" + tagNames[i] + "\";\n";
        }
        //层级内容
        for (int i = 0; i < layerNames.Count; i++)
        {
            // fileInfo+="   public static string layerName_"+layerNames[i]+" = \""+layerNames[i]+"\";\n"+
            fileInfo += "\n" +
              "    private static int _layer_" + layerNames[i] + " = -1;\n" +
              "    public static int layer_" + layerNames[i] + " { get { if (_layer_" + layerNames[i] + " == -1){_layer_" + layerNames[i] + " = LayerMask.NameToLayer(\"" + layerNames[i] + "\");}  return _layer_" + layerNames[i] + "; } }\n";
        }
        fileInfo += "}\n";
        //写入文件
        File.WriteAllText(Application.dataPath + "/Scripts/Provide/TagLayer.cs", fileInfo);
        AssetDatabase.Refresh();
        Debug.Log("文件已更新:" + Application.dataPath + "/Scripts/Provide/TagLayer.cs");
    }
}