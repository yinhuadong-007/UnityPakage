using System.Runtime.InteropServices;
using System;
using System.Net.Mime;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
#if UNITY_EDITOR
public struct exportResNameFieldData
{
    public string fieldName;
    public string primaryKeyStr;
}

public class AutoGenerateScript : Editor
{
    private static string path_src = Application.dataPath + "/ZFrame/AutoGenerateScript/";
    private static string rootFolderName = "Game";
    private static string path_DynamicRes = Application.dataPath + "/" + rootFolderName + "/";
    [MenuItem("ZFrame/AutoGenerateScript/CreateFileIndex")]
    static void GenerateFileIndex()
    {
        //如果不存在Scripts文件夹，就自动创建
        if (!Directory.Exists(path_src))
        {
            Directory.CreateDirectory(path_src);
        }
        // createExampleCode();
        generateResName();
        generateEventName();
    }
    /// <summary>创建示例ExampleClass.cs </summary>
    static void createExampleCode()
    {
        //准备一个代码编译器单元
        CodeCompileUnit unit = new CodeCompileUnit();
        //设置命名空间（这个是指要生成的类的空间）
        CodeNamespace myNamespace = new CodeNamespace("AutoGenerateScriptSpace");
        //导入必要的命名空间引用
        myNamespace.Imports.Add(new CodeNamespaceImport("System"));
        myNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));

        string className = "ExampleClass";
        //Code:代码体
        CodeTypeDeclaration myClass = new CodeTypeDeclaration(className);
        //指定为类
        myClass.IsClass = true;
        //设置类的访问类型
        myClass.TypeAttributes = TypeAttributes.Public;// | TypeAttributes.Sealed;
                                                       //把这个类放在这个命名空间下
        myNamespace.Types.Add(myClass);
        //把该命名空间加入到编译器单元的命名空间集合中
        unit.Namespaces.Add(myNamespace);


        //添加字段
        CodeMemberField field = new CodeMemberField(typeof(System.String), "str");
        //设置访问类型
        field.Attributes = MemberAttributes.Public;
        ///添加到myClass类中
        myClass.Members.Add(field);

        //添加属性
        CodeMemberProperty property = new CodeMemberProperty();
        //设置访问类型
        property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        //对象名称
        property.Name = "Str";
        //有get
        property.HasGet = true;
        //有set
        property.HasSet = true;
        //设置property的类型            
        property.Type = new CodeTypeReference(typeof(System.String));
        //添加注释
        property.Comments.Add(new CodeCommentStatement("this is Str"));
        //get
        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "str")));
        //set
        property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "str"), new CodePropertySetValueReferenceExpression()));
        ///添加到Customerclass类中
        myClass.Members.Add(property);

        //添加方法
        CodeMemberMethod method = new CodeMemberMethod();
        //方法名
        method.Name = "Function";
        //访问类型
        method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
        //添加一个参数
        method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "number"));
        //设置返回值类型：int/不设置则为void
        method.ReturnType = new CodeTypeReference(typeof(int));
        //设置返回值
        method.Statements.Add(new CodeSnippetStatement(" return number+1; "));
        ///将方法添加到myClass类中
        myClass.Members.Add(method);
        //添加构造方法
        CodeConstructor constructor = new CodeConstructor();
        constructor.Attributes = MemberAttributes.Public;
        ///将构造方法添加到myClass类中
        myClass.Members.Add(constructor);

        //添加特特性
        myClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));

        //生成C#脚本("VisualBasic"：VB脚本)
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        //代码风格:大括号的样式{}
        options.BracingStyle = "C";
        //是否在字段、属性、方法之间添加空白行
        options.BlankLinesBetweenMembers = true;

        //输出文件路径
        string outputFile = path_src + className + ".cs";
        //保存
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
        {
            //为指定的代码文档对象模型(CodeDOM) 编译单元生成代码并将其发送到指定的文本编写器，使用指定的选项。(官方解释)
            //将自定义代码编译器(代码内容)、和代码格式写入到sw中
            provider.GenerateCodeFromCompileUnit(unit, sw, options);
        }
    }
    /// <summary>创建ResName.cs====>对应资源地址的脚本</summary>
    static void generateResName()
    {
        //准备一个代码编译器单元
        CodeCompileUnit unit = new CodeCompileUnit();
        //设置命名空间（这个是指要生成的类的空间）
        CodeNamespace myNamespace = new CodeNamespace("");
        //类名
        string className = "ResName";
        //Code:代码体
        CodeTypeDeclaration myClass = new CodeTypeDeclaration(className);
        //指定为类
        myClass.IsClass = true;
        //设置类的访问类型
        myClass.TypeAttributes = TypeAttributes.Public;// | TypeAttributes.Sealed;
                                                       //把这个类放在这个命名空间下
        myNamespace.Types.Add(myClass);
        //把该命名空间加入到编译器单元的命名空间集合中
        unit.Namespaces.Add(myNamespace);


        List<exportResNameFieldData> strList = new();
        checkDir(ref strList, path_DynamicRes);

        for (int z = 0; z < strList.Count; z++)
        {
            //添加字段
            CodeMemberField field = new CodeMemberField(typeof(System.String), strList[z].fieldName);
            //设置访问类型
            field.Attributes = (int)MemberAttributes.Const + MemberAttributes.Public;
            field.InitExpression = new CodePrimitiveExpression(strList[z].primaryKeyStr);
            ///添加到myClass类中
            myClass.Members.Add(field);
        }

        //生成C#脚本("VisualBasic"：VB脚本)
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        //代码风格:大括号的样式{}
        options.BracingStyle = "C";
        //是否在字段、属性、方法之间添加空白行
        options.BlankLinesBetweenMembers = false;

        //输出文件路径
        string outputFile = path_src + className + ".cs";
        //保存
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
        {
            //为指定的代码文档对象模型(CodeDOM) 编译单元生成代码并将其发送到指定的文本编写器，使用指定的选项。(官方解释)
            //将自定义代码编译器(代码内容)、和代码格式写入到sw中
            provider.GenerateCodeFromCompileUnit(unit, sw, options);
            UnityEngine.Debug.Log("generate " + className + ".cs success!");
        }
    }

    /// <summary>创建EventName.cs====>具有特性EventReceiveAttribute的方法</summary>
    static void generateEventName()
    {
        //字段列表
        List<exportResNameFieldData> fieldDataList = new();
        //程序集 列表
        List<Assembly> assList = new();
        //加入默认程序集
        assList.Add(Assembly.GetExecutingAssembly());
        //外部输入的程序集列表
        var assemblyNameStr = EditorUserSettings.GetConfigValue("eventAssemblyDefineName");
        if (!string.IsNullOrEmpty(assemblyNameStr))
        {
            var assNameList = assemblyNameStr.Split(",");
            foreach (var assName in assNameList)
            {
                try
                {
                    assList.Add(Assembly.Load(assName));
                }
                catch (Exception ex)
                {
                    Debug.Log("输入的程序集名称不存在:" + ex.ToString());
                    continue;
                }
            }
        }
        Debug.Log("Assembly.GetCallingAssembly():"+Assembly.GetCallingAssembly());
        foreach (var ass in assList)
        {
            
            // foreach (var t in ass.GetExportedTypes())
            foreach (var t in ass.GetTypes())
            {
                // Debug.Log("t:"+t.Name);
                foreach (var method in t.GetMethods())
                {
                    foreach (var attr in Attribute.GetCustomAttributes(method))
                    {
                        if (attr is ZFrame.EventReceiveAttribute)
                        {
                            
                            string methodDesc = (attr as ZFrame.EventReceiveAttribute).methodDesc;
                            fieldDataList.Add(new exportResNameFieldData() { fieldName = methodDesc, primaryKeyStr = methodDesc });
                        }
                    }
                }
            }
        }

        //准备一个代码编译器单元
        CodeCompileUnit unit = new CodeCompileUnit();
        //设置命名空间（这个是指要生成的类的空间）
        CodeNamespace myNamespace = new CodeNamespace("");
        //类名
        string className = "EventName";
        //Code:代码体
        CodeTypeDeclaration myClass = new CodeTypeDeclaration(className);
        //指定为类
        myClass.IsClass = true;
        //设置类的访问类型
        myClass.TypeAttributes = TypeAttributes.Public;// | TypeAttributes.Sealed;
                                                       //把这个类放在这个命名空间下
        myNamespace.Types.Add(myClass);
        //把该命名空间加入到编译器单元的命名空间集合中
        unit.Namespaces.Add(myNamespace);

        for (int z = 0; z < fieldDataList.Count; z++)
        {
            //添加字段
            CodeMemberField field = new CodeMemberField(typeof(System.String), fieldDataList[z].fieldName);
            //设置访问类型
            field.Attributes = (int)MemberAttributes.Const + MemberAttributes.Public;
            field.InitExpression = new CodePrimitiveExpression(fieldDataList[z].primaryKeyStr);
            ///添加到myClass类中
            myClass.Members.Add(field);
        }

        //生成C#脚本("VisualBasic"：VB脚本)
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        //代码风格:大括号的样式{}
        options.BracingStyle = "C";
        //是否在字段、属性、方法之间添加空白行
        options.BlankLinesBetweenMembers = false;

        //输出文件路径
        string outputFile = path_src + className + ".cs";
        //保存
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
        {
            //为指定的代码文档对象模型(CodeDOM) 编译单元生成代码并将其发送到指定的文本编写器，使用指定的选项。(官方解释)
            //将自定义代码编译器(代码内容)、和代码格式写入到sw中
            provider.GenerateCodeFromCompileUnit(unit, sw, options);
            UnityEngine.Debug.Log("generate " + className + ".cs success!");
        }
    }

    /// <summary>获得目标文件夹下的全部文件名称（处理）</summary>
    /// <param name="strList"></param>
    /// <param name="path"></param>
    private static void checkDir(ref List<exportResNameFieldData> strList, string path)
    {
        DirectoryInfo DI = new DirectoryInfo(path);
        foreach (FileInfo file in DI.GetFiles())
        {
            //文件名中带有meta和~的文件都不予自动生成
            if (!file.Name.Contains(".meta") && !file.Name.Contains("~") && !file.Name.Contains("-"))
            {
                string[] list = file.FullName.Split('\\', '.');
                var exportFieldName = list[list.Length - 1];
                var exportPrimaryKeyStr = "Assets/" + rootFolderName;
                bool isInRoot = false;
                for (int z = 0; z < list.Length; z++)
                {
                    if (isInRoot == false)
                    {
                        if (list[z] == rootFolderName)
                        {
                            isInRoot = true;
                        }
                        continue;
                    }
                    if (z <= list.Length - 2)
                    {
                        exportFieldName += "_" + list[z];
                        exportPrimaryKeyStr += "/" + list[z];
                    }
                    else
                    {
                        exportPrimaryKeyStr += "." + list[z];
                    }
                }
                strList.Add(new exportResNameFieldData() { fieldName = exportFieldName, primaryKeyStr = exportPrimaryKeyStr });
            }
        }
        foreach (DirectoryInfo root in DI.GetDirectories())
        {
            checkDir(ref strList, root.FullName);
        }
    }
}
#endif