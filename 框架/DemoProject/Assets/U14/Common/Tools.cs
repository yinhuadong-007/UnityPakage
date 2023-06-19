namespace U14.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class Tools
    {
        /// <summary>
        /// 获得animator下某个动画片段的时长方法
        /// </summary>
        /// <param animator="animator">Animator组件</param> 
        /// <param name="name">要获得的动画片段名字</param>
        /// <returns></returns>
        public static float GetAnimatorLength(Animator animator, string name)
        {
            //动画片段时间长度
            float length = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                // Print.Log(clip.name);
                if (clip.name.Equals(name))
                {
                    length = clip.length;
                    break;
                }
            }
            return length;
        }

        public static void PlayTriggerAni(Animator ani, string triggerName)
        {
            Tools.ResetAllTriggers(ani);
            ani.SetTrigger(triggerName);
        }

        // 清除所有的激活中的trigger缓存
        public static void ResetAllTriggers(Animator animator)
        {
            AnimatorControllerParameter[] aps = animator.parameters;
            for (int i = 0; i < aps.Length; i++)
            {
                AnimatorControllerParameter paramItem = aps[i];
                if (paramItem.type == AnimatorControllerParameterType.Trigger)
                {
                    string triggerName = paramItem.name;
                    bool isActive = animator.GetBool(triggerName);
                    if (isActive)
                    {
                        animator.ResetTrigger(triggerName);
                    }
                }
            }
        }

        /// <summary>
        /// 清除父物体下面的所有子物体
        /// </summary>
        /// <param name="parent"></param>
        public static void ClearChildren(Transform parent)
        {
            if (parent.childCount > 0)
            {
                for (int i = 0, len = parent.childCount; i < len; i++)
                {
                    GameObject.Destroy(parent.GetChild(i).gameObject);
                }
            }
        }

        /// <summary>
        /// 判断触摸点是否有点击事件遮挡
        /// </summary>
        /// <param name="screenPosition">触摸点</param>
        /// <returns>是否被遮挡</returns>
        public static bool IsPointerOverUIObject(Vector2 screenPosition)
        {
            UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
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
        /// 移除点击事件
        /// </summary>
        /// <param name="UIObject">点击对象</param>
        /// <param name="cbk">侦听函数</param>
        public static void RemoveButtonEvent(GameObject UIObject, UnityEngine.Events.UnityAction cbk)
        {
            if (UIObject == null) return;
            UnityEngine.UI.Button btn = UIObject.GetComponent<UnityEngine.UI.Button>();
            if (btn == null)
            {
                // Debug.LogError("未添加 UnityEngine.UI.Button 脚本");
                return;
            }
            btn.onClick.RemoveListener(cbk);
        }

        /// <summary>
        /// 移除所有点击事件
        /// </summary>
        /// <param name="UIObject">点击对象</param>
        /// <param name="cbk">侦听函数</param>
        public static void RemoveAllButtonEvent(GameObject UIObject)
        {
            if (UIObject == null) return;
            UnityEngine.UI.Button btn = UIObject.GetComponent<UnityEngine.UI.Button>();
            if (btn == null)
            {
                // Debug.LogError("未添加 UnityEngine.UI.Button 脚本");
                return;
            }
            btn.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 递归查找子节点
        /// </summary>
        /// <param name="parent">树节点</param>
        /// <param name="name">查找路径或节点名</param>
        public static Transform FindChildRecursively(Transform parent, string name)
        {
            Transform t = null;
            t = parent.Find(name);
            if (t == null)
            {
                foreach (Transform tran in parent)
                {
                    t = FindChildRecursively(tran, name);
                    if (t != null)
                    {
                        return t;
                    }
                }
            }
            return t;
        }
        /// <summary>
        /// 递归查找子节点
        /// </summary>
        /// <param name="parent">树节点</param>
        /// <param name="name">查找路径或节点名</param>
        public static GameObject FindChildRecursively(GameObject parent, string name)
        {
            Transform t = FindChildRecursively(parent.transform, name);
            return t == null ? null : t.gameObject;
        }

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

        //设置分辨率
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
            float ratioW = scWidth/designWidth;
            // float ratioH = scHeight/designHeight;
            
            Screen.SetResolution(designWidth, (int)(scHeight/ratioW), false);
        }else{
            Screen.SetResolution(scWidth, scHeight, false);
        }
#endif
        }

        public static Vector3 VectorMult(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        //浮点判断是否为零
        public static bool IsEqualZero(float value)
        {
            return Mathf.Abs(value) < 1e-5;
        }

        /// <summary>
        /// 一个点绕另一个点旋转
        /// </summary>
        /// <param name="point">要旋转的点</param>
        /// <param name="pivot">中心点</param>
        /// <param name="euler">旋转的角度</param>
        /// <returns>返回点旋转后的位置</returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 euler)
        {
            Vector3 direction = point - pivot;
            Vector3 rotatedDirection = Quaternion.Euler(euler) * direction;// 这句代码实现了旋转向量的功能
            Vector3 rotatedPoint = rotatedDirection + pivot;
            return rotatedPoint;
        }
    }


    /// <summary>
    /// 作者：Foldcc
    /// </summary>
    public class BezierMath
    {
        /// <summary>
        /// 二次贝塞尔
        /// </summary>
        public static Vector3 Bezier_2(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        }
        public static void Bezier_2ref(ref Vector3 outValue, Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            outValue = (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
        }

        /// <summary>
        /// 三次贝塞尔
        /// </summary>
        public static Vector3 Bezier_3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
        }
        public static void Bezier_3ref(ref Vector3 outValue, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            outValue = (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
        }
    }


    namespace CommonLibrary
    {
        /// <summary>
        /// Sorter 排序类
        /// </summary>
        public class Sorter
        {
            /// <summary>
            /// 两个值的比较委托
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="value1">值1</param>
            /// <param name="value2">值2</param>
            /// <returns>返回值,值1大于值2返回1,值1小于值2返回-1,值1等于值2返回0</returns>
            public delegate int Compare<T>(T value1, T value2);

            /// <summary>
            /// 二分排序法
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="myList">要进行排序的集合</param>
            /// <param name="myCompareMethod">两个值的比较方法</param>
            public static void DimidiateSort<T>(IList<T> myList, Compare<T> myCompareMethod)
            {
                DimidiateSort<T>(myList, 0, myList.Count - 1, myCompareMethod);
            }
            /// <summary>
            /// 二分排序法
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="myList">要进行排序的集合</param>
            /// <param name="left">起始位置</param>
            /// <param name="right">结束位置</param>
            /// <param name="myCompareMethod">两个值的比较方法</param>
            public static void DimidiateSort<T>(IList<T> myList, int left, int right, Compare<T> myCompareMethod)
            {
                if (left < right)
                {
                    T s = myList[(right + left) / 2];
                    int i = left - 1;
                    int j = right + 1;
                    T temp = default(T);
                    while (true)
                    {
                        do
                        {
                            i++;
                        }
                        while (i < right && myCompareMethod(myList[i], s) == -1);
                        do
                        {
                            j--;
                        }
                        while (j > left && myCompareMethod(myList[j], s) == 1);
                        if (i >= j)
                            break;
                        temp = myList[i];
                        myList[i] = myList[j];
                        myList[j] = temp;
                    }
                    DimidiateSort(myList, left, i - 1, myCompareMethod);
                    DimidiateSort(myList, j + 1, right, myCompareMethod);
                }
            }
        }
    }
}

