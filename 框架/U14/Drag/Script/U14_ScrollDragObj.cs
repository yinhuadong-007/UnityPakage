namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using U14.Common;

    public class U14_ScrollDragObj : U14_BaseDragObj
    {
        [Header("是否自动化重置位置")]
        public bool autoResetPosition = true;
        [Header("间距 - 自动化生效")]
        public Vector3 IntervalDistance = new Vector3(1, 0, 0);

        // [Header("子对象列表")]
        // public List<GameObject> objList;
        [Header("滑动方向")]
        public Vector3 moveDirection = Vector3.left;

        /// <summary> 物体触摸拖动控制 </summary>
        protected bool m_canDrag = true;

        Vector3 lastTargetPos;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            moveDirection = moveDirection.normalized;
            ResetObjList();
        }

        protected override void OnBeginDragDelegate(PointerEventData data)
        {
            if (!m_canDrag) return;
            base.OnBeginDragDelegate(data);
            OnDragDelegate(data);
        }

        protected override void OnDragDelegate(PointerEventData data)
        {
            if (!m_canDrag) return;
            base.OnDragDelegate(data);
        }

        protected override void OnEndDragDelegate(PointerEventData data)
        {
            if (!m_canDrag) return;
            base.OnEndDragDelegate(data);
            m_canDrag = true;
        }

        protected override void SetMoveTargetPosition(Vector3 targetPos)
        {
            Debug.Log("targetPos= " + targetPos + ", lastTargetPos= " + lastTargetPos);
            if (m_curDragStatus == EDragStatus.MoveDrag)
            {
                Vector3 subPos = Tools.VectorMult(targetPos - lastTargetPos, moveDirection);
                Vector3 lastMoveTargetPos = m_moveTargetData.position;
                float y_h = lastMoveTargetPos.y;
                m_moveTargetData.position = lastMoveTargetPos + subPos;
                // m_moveTargetData.position = Tools.VectorMult(new Vector3(lastMoveTargetPos.x + targetPos.x, y_h, targetPos.z), moveDirection);
                if (m_moveTargetData.position != lastMoveTargetPos)
                {
                    m_moveTargetData.startPos = m_moveCurData.position;
                    m_moveTargetData.totalTime = Vector3.Distance(m_moveTargetData.position, m_moveTargetData.startPos) / followMoveSpeed;
                    m_moveTargetData.lerpTime = 0;
                }
            }

            lastTargetPos = targetPos;
        }

        private void ResetObjList()
        {
            if (!autoResetPosition) return;
            Vector3 cubeOffset = Tools.VectorMult(handleObj.transform.lossyScale, IntervalDistance.normalized);
            Vector3 startPos = (-cubeOffset + IntervalDistance) / 2 + moveTrans.position;

            for (int i = 0, length = moveTrans.childCount; i < length; i++)
            {
                Transform obj = moveTrans.GetChild(i);
                if (obj.gameObject.activeSelf)
                {
                    Vector3 pos = startPos;
                    if (IntervalDistance.x == 0)
                    {
                        pos.x = obj.position.x;
                    }
                    if (IntervalDistance.y == 0)
                    {
                        pos.y = obj.position.y;
                    }
                    if (IntervalDistance.z == 0)
                    {
                        pos.z = obj.position.z;
                    }
                    obj.position = pos;
                    startPos += IntervalDistance;
                }
            }
        }
    }
}