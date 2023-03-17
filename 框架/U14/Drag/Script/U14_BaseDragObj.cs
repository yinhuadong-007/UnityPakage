namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class U14_BaseDragObj : MonoBehaviour
    {
        [Header("拖拽射线位置定位层, -1所有层")]
        public List<int> maskLayerIdList;
        [Header("移动的速度")]
        public float followMoveSpeed = 5;

        [Header("事件监听节点")]
        public GameObject handleObj;
        [Header("移动节点")]
        public Transform moveTrans;


        /// <summary> 物体update移动控制 </summary>
        public bool canMove = false;
        protected MoveTargetData m_moveTargetData;
        protected MoveTargetData m_moveCurData;
        protected int layerMaskRay = -1;

        // Start is called before the first frame update
        protected EDragStatus m_curDragStatus = EDragStatus.None;
        public EDragStatus curDragStatus
        {
            get { return m_curDragStatus; }
        }

        protected virtual void Start()
        {
            if (moveTrans == null) moveTrans = this.transform;
            if (handleObj == null) handleObj = this.gameObject;

            InitInput();
            SetLayerMask();
            ResetMoveTargetData();
            canMove = true;
        }

        protected void ResetMoveTargetData()
        {
            m_moveTargetData = new MoveTargetData()
            {
                position = transform.position,
                startPos = transform.position,
            };
            m_moveCurData = m_moveTargetData;
        }

        void SetLayerMask()
        {
            if (maskLayerIdList.Count > 0)
            {
                maskLayerIdList.Remove(handleObj.layer);
                layerMaskRay = 0;
                for (int i = 0, length = maskLayerIdList.Count; i < length; i++)
                {
                    int maskLayerId = maskLayerIdList[i];
                    layerMaskRay |= 1 << maskLayerId;
                }
            }
            else
            {
                layerMaskRay = ~(1 << handleObj.layer);
            }
        }

        void InitInput()
        {
            EventTrigger trigger = GetComponent<EventTrigger>();
            if (trigger == null) trigger = handleObj.AddComponent<EventTrigger>();
            while (trigger.triggers.Count > 0)
            {
                trigger.triggers.RemoveAt(0);
            }

            EventTrigger.Entry m_entry = new EventTrigger.Entry();
            m_entry.eventID = EventTriggerType.Drag;
            m_entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
            trigger.triggers.Add(m_entry);

            EventTrigger.Entry b_entry = new EventTrigger.Entry();
            b_entry.eventID = EventTriggerType.BeginDrag;
            b_entry.callback.AddListener((data) => { OnBeginDragDelegate((PointerEventData)data); });
            trigger.triggers.Add(b_entry);

            EventTrigger.Entry e_entry = new EventTrigger.Entry();
            e_entry.eventID = EventTriggerType.EndDrag;
            e_entry.callback.AddListener((data) => { OnEndDragDelegate((PointerEventData)data); });
            trigger.triggers.Add(e_entry);

            EventTrigger.Entry c_entry = new EventTrigger.Entry();
            c_entry.eventID = EventTriggerType.Cancel;
            c_entry.callback.AddListener((data) => { OnEndDragDelegate((PointerEventData)data); });
            trigger.triggers.Add(c_entry);

            PhysicsRaycaster physicsRaycaster = Camera.main.gameObject.GetComponent<PhysicsRaycaster>();
            if (physicsRaycaster == null)
            {
                physicsRaycaster = Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
            }
        }

        protected virtual void OnBeginDragDelegate(PointerEventData data)
        {
            m_curDragStatus = EDragStatus.BeginDrag;
        }

        protected virtual void OnDragDelegate(PointerEventData data)
        {
            Vector2 pos = data.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(handleObj.transform.position);
            Vector3 dragPos = new Vector3(pos.x, pos.y, screenPos.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(dragPos);

            Ray ray = Camera.main.ScreenPointToRay(data.position);
            RaycastHit raycastHit;
            if (layerMaskRay != -1)
            {
                if (Physics.Raycast(ray, out raycastHit, 1000, layerMaskRay))
                {
                    Vector3 hitPos = raycastHit.point;
                    SetMoveTargetPosition(hitPos);
                }
            }
            else
            {
                if (Physics.Raycast(ray, out raycastHit))
                {
                    Vector3 hitPos = raycastHit.point;
                    SetMoveTargetPosition(hitPos);
                }
            }
            m_curDragStatus = EDragStatus.MoveDrag;
        }

        protected virtual void OnEndDragDelegate(PointerEventData data)
        {
            m_curDragStatus = EDragStatus.EndDrag;
        }

        protected virtual void SetMoveTargetPosition(Vector3 targetPos)
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (!canMove) return;

            if (m_moveCurData.position != m_moveTargetData.position)
            {
                m_moveTargetData.lerpTime += Time.deltaTime;
                float factor = m_moveTargetData.lerpTime / m_moveTargetData.totalTime;
                m_moveCurData.position = Vector3.Lerp(m_moveTargetData.startPos, m_moveTargetData.position, factor);
                moveTrans.position = m_moveCurData.position;
            }
        }
        public enum EDragStatus
        {
            /// <summary> 初始状态 </summary>
            None,
            /// <summary> 拖拽开始 </summary>
            BeginDrag,
            /// <summary> 移动中 </summary>
            MoveDrag,
            /// <summary> 拖拽结束 </summary>
            EndDrag,
            /// <summary> 拖拽完成 </summary>
            CompleteDrag,
        }

        public struct MoveTargetData
        {
            public Vector3 position;

            //插值移动相关
            public Vector3 startPos;
            public float totalTime;
            public float lerpTime;
        }
    }
}

