namespace U14.Click
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using U14.Event;

    public class U14_BaseClickObj : MonoBehaviour
    {
        [Header("使用通用配置")]
        public bool useCommonConf = true;
        [ChineseLabel("允许的点击偏移值")] public float moveOffset = 0.2f;
        [ChineseLabel("是否使用上拖 点击")] public bool useDragUp = true;
        [ChineseLabel("使用上拖点击 偏移值")] public float successOffset = 0.5f;
        protected GameObject handleObj;

        protected virtual void Start()
        {
            if (handleObj == null) handleObj = this.gameObject;
            SetConfData();
            InitInput();
        }

        void SetConfData()  //配置数据优先
        {
            if (U14_ClickData.instance != null && useCommonConf)
            {
                U14_ClickData data = U14_ClickData.instance;
                if (data != null)
                {
                    useDragUp = data.useDragUp;
                    moveOffset = data.moveOffset;
                    successOffset = data.successOffset;
                }
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
            b_entry.eventID = EventTriggerType.PointerDown;
            b_entry.callback.AddListener((data) => { OnBeginDragDelegate((PointerEventData)data); });
            trigger.triggers.Add(b_entry);

            EventTrigger.Entry e_entry = new EventTrigger.Entry();
            e_entry.eventID = EventTriggerType.PointerUp;
            e_entry.callback.AddListener((data) => { OnEndDragDelegate((PointerEventData)data); });
            trigger.triggers.Add(e_entry);

            PhysicsRaycaster physicsRaycaster = Camera.main.gameObject.GetComponent<PhysicsRaycaster>();
            if (physicsRaycaster == null)
            {
                physicsRaycaster = Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
            }
        }

        Vector3 downPos;
        Vector3 disScreenPos;
        bool isUp;
        protected virtual void OnBeginDragDelegate(PointerEventData data)
        {
            isUp = false;
            disScreenPos = Camera.main.WorldToScreenPoint(transform.position) - new Vector3(data.position.x, data.position.y, 0);
            downPos = transform.position;
            EventUtil.DispatchEvent(ClickEventType.ClickDown, this.gameObject, data);
            ZFrame.EventSystem.goEvent(EventName.OnClickDown, this.gameObject);
        }

        protected virtual void OnDragDelegate(PointerEventData data)
        {
            if (useDragUp)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(data.position.x, data.position.y, 0) + disScreenPos);
                if (pos.y - downPos.y > successOffset)
                {
                    DoSuccess(data);
                }
            }
        }

        protected virtual void OnEndDragDelegate(PointerEventData data)
        {
            if (isUp) return;
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(data.position.x, data.position.y, 0) + disScreenPos);
            float dis = (downPos - pos).sqrMagnitude;
            if (dis > moveOffset)
            {
                DoClickUp(data);
            }
            else
            {
                DoSuccess(data);
            }
        }

        void DoClickUp(PointerEventData data)
        {
            Debug.Log("Do ClickUp");
            EventUtil.DispatchEvent(ClickEventType.ClickUp, this.gameObject, data);
            ZFrame.EventSystem.goEvent(EventName.OnClickUp, this.gameObject);
            isUp = true;
        }

        void DoSuccess(PointerEventData data)
        {
            Debug.Log("Do Success");
            EventUtil.DispatchEvent(ClickEventType.ClickSuccess, this.gameObject, data);
            ZFrame.EventSystem.goEvent(EventName.OnClickSuccess, this.gameObject);
            EventUtil.DispatchEvent(ClickEventType.ClickUp, this.gameObject, data);
            ZFrame.EventSystem.goEvent(EventName.OnClickUp, this.gameObject);
            isUp = true;
        }



    }
    public static class ClickEventType
    {
        /// <summary> 通知点击按下 </summary>
        public const string ClickDown = "Click_Down";
        /// <summary> 通知点击抬起 </summary>
        public const string ClickUp = "Click_Up";
        /// <summary> 通知点击成功 </summary>
        public const string ClickSuccess = "Click_Success";
    }
}

