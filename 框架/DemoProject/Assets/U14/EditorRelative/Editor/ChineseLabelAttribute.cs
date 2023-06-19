using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ChineseLabel))]
public class ChineseLabelDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = attribute as ChineseLabel;
        EditorGUI.PropertyField(position, property, new GUIContent(att.header), true);
    }
}