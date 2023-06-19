using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 在unity中控制组件排列、对齐的脚本
/// </summary>
public class AlignTool : Editor
{
    [MenuItem("Tool/2D对齐/水平左对齐")]
    public static void alignInHorziontalLeft()
    {
        float x = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.localPosition.x -
        ((RectTransform)obj.transform).sizeDelta.x / 2
        ).ToArray());

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.localPosition = new Vector3(x + ((RectTransform)gameObject.transform).sizeDelta.x / 2,
                gameObject.transform.localPosition.y);
            // MathUtil.setPositionX(gameObject.transform.localPosition, x);
        }
    }

    [MenuItem("Tool/2D对齐/水平右对齐")]
    public static void alignInHorziontalRight()
    {
        float x = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.localPosition.x +
        ((RectTransform)obj.transform).sizeDelta.x / 2).ToArray());
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.localPosition = new Vector3(x - ((RectTransform)gameObject.transform).sizeDelta.x / 2, gameObject.transform.localPosition.y);
            // MathUtil.setPositionX(gameObject.transform.localPosition, x);
        }
    }

    [MenuItem("Tool/2D对齐/垂直上对齐")]
    public static void alignInVerticalUp()
    {
        float y = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.localPosition.y +
        ((RectTransform)obj.transform).sizeDelta.y / 2).ToArray());

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, y - ((RectTransform)gameObject.transform).sizeDelta.y / 2);
            //MathUtil.setPositionY(gameObject.transform.localPosition, y);
        }
    }

    [MenuItem("Tool/2D对齐/垂直下对齐")]
    public static void alignInVerticalDown()
    {
        float y = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.localPosition.y -
        ((RectTransform)obj.transform).sizeDelta.y / 2).ToArray());

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, y + ((RectTransform)gameObject.transform).sizeDelta.y / 2);
            //MathUtil.setPositionY(gameObject.transform.localPosition, y);
        }
    }


    [MenuItem("Tool/2D对齐/水平均匀")]
    public static void uniformDistributionInHorziontal()
    {
        int count = Selection.gameObjects.Length;
        float firstX = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.localPosition.x).ToArray());
        float lastX = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.localPosition.x).ToArray());
        float distance = (lastX - firstX) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 position = Selection.gameObjects[i].transform.localPosition;
            Selection.gameObjects[i].transform.localPosition = new Vector3(firstX + i * distance, position.y);
            //MathUtil.setPositionX(position, firstX + i * distance);
        }
    }



    [MenuItem("Tool/2D对齐/垂直均匀")]
    public static void uniformDistributionInVertical()
    {
        int count = Selection.gameObjects.Length;
        float firstY = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.localPosition.y).ToArray());
        float lastY = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.localPosition.y).ToArray());
        float distance = (lastY - firstY) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 position = Selection.gameObjects[i].transform.localPosition;
            Selection.gameObjects[i].transform.localPosition = new Vector3(position.x, firstY + i * distance);
            //MathUtil.setPositionY(position, firstY + i * distance);
        }
    }

    [MenuItem("Tool/3D对齐/水平左对齐")]
    public static void alignInHorziontalLeft3D()
    {
        float x = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.position.x).ToArray());

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.position = new Vector3(x, gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }

    [MenuItem("Tool/3D对齐/水平右对齐")]
    public static void alignInHorziontalRight3D()
    {
        float x = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.position.x).ToArray());
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.position = new Vector3(x, gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }

    [MenuItem("Tool/3D对齐/垂直上对齐")]
    public static void alignInVerticalUp3D()
    {
        float y = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.position.y).ToArray());

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z);
        }
    }

    [MenuItem("Tool/3D对齐/垂直下对齐")]
    public static void alignInVerticalDown3D()
    {
        float y = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.position.y).ToArray());

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z);
        }
    }
    [MenuItem("Tool/3D对齐/Z轴向后对齐")]
    public static void alignInBack3D()
    {
        float z = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.position.z).ToArray());

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, z);
        }
    }

    [MenuItem("Tool/3D对齐/Z轴向前对齐")]
    public static void alignInForward3D()
    {
        float z = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.position.z).ToArray());
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, z);
        }
    }


    [MenuItem("Tool/3D对齐/水平均匀")]
    public static void uniformDistributionInHorziontal3D()
    {
        int count = Selection.gameObjects.Length;
        float firstX = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.position.x).ToArray());
        float lastX = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.position.x).ToArray());
        float distance = (lastX - firstX) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 position = Selection.gameObjects[i].transform.position;
            Selection.gameObjects[i].transform.position = new Vector3(firstX + i * distance, position.y, position.z);
        }
    }



    [MenuItem("Tool/3D对齐/垂直均匀")]
    public static void uniformDistributionInVertical3D()
    {
        int count = Selection.gameObjects.Length;
        float firstY = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.position.y).ToArray());
        float lastY = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.position.y).ToArray());
        float distance = (lastY - firstY) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 position = Selection.gameObjects[i].transform.position;
            Selection.gameObjects[i].transform.position = new Vector3(position.x, firstY + i * distance, position.z);
            //MathUtil.setPositionY(position, firstY + i * distance);
        }
    }

    [MenuItem("Tool/3D对齐/z轴均匀")]
    public static void uniformDistributionInForward3D()
    {
        int count = Selection.gameObjects.Length;
        float firstZ = Mathf.Min(Selection.gameObjects.Select(obj => obj.transform.position.z).ToArray());
        float lastZ = Mathf.Max(Selection.gameObjects.Select(obj => obj.transform.position.z).ToArray());
        float distance = (lastZ - firstZ) / (count - 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 position = Selection.gameObjects[i].transform.position;
            Selection.gameObjects[i].transform.position = new Vector3(position.x, position.y, firstZ + i * distance);
        }
    }
}


