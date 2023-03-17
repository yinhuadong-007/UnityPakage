namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class U14_DragData : MonoBehaviour
    {
        public static U14_DragData instance;

        [Header("拖拽射线位置定位层, -1所有层")]
        public List<int> maskLayerIdList;
        [Header("移动的速度")]
        public float followMoveSpeed = 30;
        [Header("移动高度是否使用偏移值")]
        public bool useHeightOffset = true;
        [Header("移动高度偏移值 - 勾选useHeightOffset生效")]
        public float moveHeightOffset = 2;

        [Header("移动高度绝对值 - 不勾选useHeightOffset生效")]
        public float moveHeightValue = 3;

        protected virtual void Awake()
        {
            instance = this;
        }
    }

    public static class DragMoveEventType
    {
        /// <summary> 通知开始拖拽 </summary>
        public const string DragBegin = "Drag_Begin";
        /// <summary> 通知拖拽中 </summary>
        public const string DragMoving = "Drag_Moving";
        /// <summary> 通知结束拖拽 </summary>
        public const string DragEnd = "Drag_End";
    }

    ///使用
    ///在其他脚本中监听事件
    // void Start()
    // {
    //     EventUtil.AddListener(DragMoveEventType.DragBegin, this.OnDragBegin);
    //     EventUtil.AddListener(DragMoveEventType.DragBegin, this.OnDragMoving);
    //     EventUtil.AddListener(DragMoveEventType.DragBegin, this.OnDragEnd);
    // }
    // void OnDragBegin(EventArgs eventArgs) {GameObject obj = eventArgs.args[0] as GameObject;}
    // void OnDragMoving(EventArgs eventArgs) {GameObject obj = eventArgs.args[0] as GameObject;}
    // void OnDragEnd(EventArgs eventArgs) {GameObject obj = eventArgs.args[0] as GameObject;}
}
