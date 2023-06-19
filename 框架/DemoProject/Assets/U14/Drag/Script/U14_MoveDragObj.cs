namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using U14.Event;

    public class U14_MoveDragObj : U14_BaseDragObj
    {
        [Header("移动高度是否使用偏移值")]
        public bool useHeightOffset = true;
        [Header("移动高度偏移值 - 勾选useHeightOffset生效")]
        public float moveHeightOffset = 2;

        [Header("移动高度绝对值 - 不勾选useHeightOffset生效")]
        public float moveHeightValue = 2;
        [Header("使用通用配置")]
        public bool useCommonConf = true;


        /// <summary> 物体触摸拖动控制 </summary>
        protected bool m_canDrag = true;

        protected Vector3 m_initPosition;


        // Start is called before the first frame update
        protected override void Start()
        {
            //配置数据优先
            SetConfData();

            base.Start();
            m_initPosition = transform.position;
        }

        protected virtual void SetConfData()
        {
            if (U14_DragData.instance != null && useCommonConf)
            {
                layerMask = U14_DragData.instance.layerMask;
                followMoveSpeed = U14_DragData.instance.followMoveSpeed;
                useHeightOffset = U14_DragData.instance.useHeightOffset;
                moveHeightOffset = U14_DragData.instance.moveHeightOffset;
                moveHeightValue = U14_DragData.instance.moveHeightValue;
            }
        }

        protected override void OnBeginDragDelegate(PointerEventData data)
        {
            if (!m_canDrag) return;

            m_initPosition = transform.position;
            ResetMoveTargetData();

            base.OnBeginDragDelegate(data);
            OnDragDelegate(data);
            EventUtil.DispatchEvent(DragMoveEventType.DragBegin, this.gameObject, data);
           
        }

        protected override void OnDragDelegate(PointerEventData data)
        {
            if (!m_canDrag) return;
            base.OnDragDelegate(data);
            EventUtil.DispatchEvent(DragMoveEventType.DragMoving, this.gameObject, data);
        }

        protected override void OnEndDragDelegate(PointerEventData data)
        {
            if (!m_canDrag) return;
            base.OnEndDragDelegate(data);
            m_canDrag = true;
            SetMoveTargetPosition(m_initPosition);
            EventUtil.DispatchEvent(DragMoveEventType.DragEnd, this.gameObject, data);

           
        }

        protected override void SetMoveTargetPosition(Vector3 targetPos)
        {
            Vector3 lastMoveTargetData = m_moveTargetData.position;
            float y_h;
            if (m_curDragStatus == EDragStatus.EndDrag)
            {
                y_h = m_initPosition.y;
            }
            else
            {
                y_h = useHeightOffset ? targetPos.y + moveHeightOffset : moveHeightValue;
            }
            m_moveTargetData.position = new Vector3(targetPos.x, y_h, targetPos.z);
            if (m_moveTargetData.position != lastMoveTargetData)
            {
                m_moveTargetData.startPos = m_moveCurData.position;
                m_moveTargetData.totalTime = Vector3.Distance(m_moveTargetData.position, m_moveTargetData.startPos) / followMoveSpeed;
                m_moveTargetData.lerpTime = 0;
            }
        }
    }
}

