namespace U14.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    public static class Tools
    {
        /// <summary> 修正角度旋转不超过180度 </summary>
        public static void FixRotationIn180(Vector3 fromAngle, ref Vector3 targetAngle)
        {
            bool isFix = false;
            float fixX = 0f;
            float fixY = 0f;
            float fixZ = 0f;
            Vector3 toRotation = targetAngle;
            isFix = false;
            fixX = toRotation.x;
            fixY = toRotation.y;
            fixZ = toRotation.z;
            if (Mathf.Abs(toRotation.x - fromAngle.x) > 180f)
            {
                fixX = toRotation.x + (toRotation.x > fromAngle.x ? -360f : 360f);
                isFix = true;
            }
            if (Mathf.Abs(toRotation.y - fromAngle.y) > 180f)
            {
                fixY = toRotation.y + (toRotation.y > fromAngle.y ? -360f : 360f);
                isFix = true;
            }
            if (Mathf.Abs(toRotation.z - fromAngle.z) > 180f)
            {
                fixZ = toRotation.z + (toRotation.z > fromAngle.z ? -360f : 360f);
                isFix = true;
            }

            if (isFix)
            {
                targetAngle = new Vector3(fixX, fixY, fixZ);
            }
        }

        /// <summary>
        /// 添加点击事件监听
        /// </summary>
        /// <param name="UIObject">点击对象</param>
        /// <param name="cbk">侦听函数</param>
        public static void AddButtonEvent(GameObject UIObject, UnityEngine.Events.UnityAction cbk)
        {
            if (UIObject == null) return;
            UnityEngine.UI.Button btn = UIObject.GetComponent<UnityEngine.UI.Button>();
            if (btn == null)
            {
                // Debug.LogError("未添加 UnityEngine.UI.Button 脚本");
                btn = UIObject.AddComponent<UnityEngine.UI.Button>();
            }
            UnityEngine.UI.Button.ButtonClickedEvent e_t = new UnityEngine.UI.Button.ButtonClickedEvent();
            e_t.AddListener(cbk);
            btn.onClick = e_t;
        }

        /// <summary>
        /// 将UI Canvas上的世界坐标转化到一个同一UI Canvas的某一个节点下
        /// </summary>
        /// <param name="canvas">ui Canvas</param>
        /// <param name="worldPos">屏幕坐标</param>
        /// <param name="node">UI节点</param>
        /// <returns></returns>
        public static Vector2 ScreenPositionConvertToUINode(Canvas canvas, Vector2 screenPos, Transform node)
        {
            float scaleFactor = GetCanvasScaleFactor(canvas);
            return ScreenPositionConvertToUINode(scaleFactor, screenPos, node);
        }

        /// <summary>
        /// 获取屏幕宽高缩放比例因子
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static float GetCanvasScaleFactor(Canvas canvas)
        {
            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
            float scaleFactor;
            if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                float matchWidthOrHeight = canvasScaler.matchWidthOrHeight;
                if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
                {
                    matchWidthOrHeight = canvasScaler.matchWidthOrHeight;
                }
                else if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
                {
                    canvasScaler.matchWidthOrHeight = 1;
                }
                else if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
                {
                    canvasScaler.matchWidthOrHeight = 0;
                }
                scaleFactor = Screen.width / canvasScaler.referenceResolution.x * (1 - matchWidthOrHeight)
                + Screen.height / canvasScaler.referenceResolution.y * matchWidthOrHeight;
            }
            else
            {
                scaleFactor = canvasScaler.scaleFactor;
            }
            // Print.Log("Screen= " + Screen.width + "x" + Screen.height + ", referenceResolution= " + canvasScaler.referenceResolution + ", scaleFactor= " + scaleFactor);
            return scaleFactor;
        }

        /// <summary>
        /// 将UI Canvas上的世界坐标转化到一个同一UI Canvas的某一个节点下
        /// </summary>
        ///  <param name="canvas">ui Canvas 缩放尺寸</param>
        /// <param name="worldPos">屏幕坐标</param>
        /// <param name="node">UI节点</param>
        /// <returns></returns>
        public static Vector2 ScreenPositionConvertToUINode(float scaleFactor, Vector2 screenPos, Transform node)
        {
            Transform parent = node.parent;
            Canvas canvas = parent.GetComponent<Canvas>();
            if (parent == null || canvas != null)
            {
                RectTransform t_node = node as RectTransform;
                Vector2 vec = new Vector2((screenPos.x - Screen.width / 2) / scaleFactor, (screenPos.y - Screen.height / 2) / scaleFactor);
                return ToNode(vec, node);
            }
            else
            {
                // Print.Log("WorldUIConvertToNode s -- > " + node.name);
                return ToNode(ScreenPositionConvertToUINode(scaleFactor, screenPos, parent), node);
            }
        }

        /// <summary>
        /// 把父节点中的位置转化到父节点中的一个子节点下
        /// </summary>
        /// <param name="value"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Vector2 ToNode(Vector2 value, Transform node)
        {
            RectTransform t_node = node as RectTransform;
            float v_x = value.x;
            float v_y = value.y;

            Vector2 pos = new Vector2(
                (v_x - t_node.localPosition.x) / t_node.localScale.x,
                (v_y - t_node.localPosition.y) / t_node.localScale.y
            );
            Vector3 direction = pos - Vector2.zero;
            Vector3 rotatedDirection = Quaternion.Euler(-t_node.localEulerAngles) * direction;
            pos = rotatedDirection + Vector3.zero;
            // Print.Log("ToNode s -- > " + node.name + ", pos = " + pos + ", t_node.localPosition = " + t_node.localPosition);
            return pos;
        }

        /// <summary>
        /// 将UI Canvas上的一个局部节点转化到一个同一UI Canvas的屏幕坐标下
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Vector2 UINodeConvertToScreenPosition(Canvas canvas, Transform node)
        {
            float scaleFactor = GetCanvasScaleFactor(canvas);
            return UINodeConvertToScreenPosition(scaleFactor, node.localPosition, node);
        }

        /// <summary>
        /// 将UI Canvas上的一个局部节点转化到一个同一UI Canvas的屏幕坐标下
        /// </summary>
        /// <param name="scaleFactor"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Vector2 UINodeConvertToScreenPosition(float scaleFactor, Transform node)
        {
            return UINodeConvertToScreenPosition(scaleFactor, node.localPosition, node);
        }

        /// <summary>
        /// 将UI Canvas上的一个局部节点转化到一个同一UI Canvas的屏幕坐标下
        /// </summary>
        /// <param name="scaleFactor"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Vector2 UINodeConvertToScreenPosition(float scaleFactor, Vector2 localPos, Transform node)
        {
            Transform parent = node.parent;
            Canvas canvas = parent.GetComponent<Canvas>();
            if (parent == null || canvas != null)
            {
                RectTransform t_node = node as RectTransform;
                Vector2 vec = new Vector2(localPos.x * scaleFactor + Screen.width / 2, localPos.y * scaleFactor + Screen.height / 2);
                return vec;
            }
            else
            {
                return UINodeConvertToScreenPosition(scaleFactor, ToParent(localPos, parent), parent);
            }
        }
        /// <summary>
        /// 把子节点的位置转化到父节点的父节点下
        /// </summary>
        /// <param name="value"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Vector2 ToParent(Vector2 value, Transform parent)
        {
            RectTransform t_parent = parent as RectTransform;

            Vector2 pos = new Vector2(
               value.x * t_parent.localScale.x + t_parent.localPosition.x,
               value.y * t_parent.localScale.y + t_parent.localPosition.y
            );
            // 从子节点向父节点转时不需要考虑旋转
            // Vector3 direction = pos - Vector2.zero;
            // Vector3 rotatedDirection = Quaternion.Euler(t_parent.localEulerAngles) * direction;
            // pos = rotatedDirection + Vector3.zero;
            // Debug.Log("ToParent s -- > " + t_parent.name + ", pos = " + pos + ", t_parent.localPosition = " + t_parent.localPosition);
            return pos;
        }


        public static void SetResolution(bool isScale)
        {
#if !UNITY_EDITOR

        int scWidth = Screen.width;
        int scHeight = Screen.height;
        if(isScale){
            int designWidth = 360; //这个是设计分辨率
            int designHeight = 720;

            if (scWidth <= designWidth || scHeight <= designHeight)
                return;

            Screen.SetResolution(designWidth, designHeight, false);
        }else{
            Screen.SetResolution(scWidth, scHeight, false);
        }
#endif
        }

        public static Vector3 VectorMult(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }
        // public static Vector3 VectorDivision(Vector3 v1, Vector3 v2)
        // {
        //     return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        // }
    }
}
