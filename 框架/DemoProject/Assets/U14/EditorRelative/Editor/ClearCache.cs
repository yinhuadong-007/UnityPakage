using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ClearCache : MonoBehaviour
{
    [MenuItem("Tool/ClearCache")]
    static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }
}
