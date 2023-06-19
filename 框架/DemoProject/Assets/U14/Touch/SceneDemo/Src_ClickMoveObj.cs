namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using U14.Event;
    using U14.Click;
    using U14.Common;
    using DG.Tweening;

    using UnityEngine.EventSystems;

    public class Src_ClickMoveObj : MonoBehaviour
    {
        [ChineseLabel("移动到目标位置")] public Transform targetMoveTrans;
        [ChineseLabel("移动时间")] public float moveTime = 0.5f;
        [ChineseLabel("基于当前尺寸进行的缩放值")] public Vector3 mulScale = Vector3.one;

        protected GameObject moveObj;
        protected bool operateEnd = false;

        protected virtual void OnEnable()
        {
            operateEnd = false;

            EventUtil.RemoveListener(this);
            EventUtil.AddListener(ClickEventType.ClickDown, this.OnClickDown, this);
            EventUtil.AddListener(ClickEventType.ClickUp, this.OnClickUp, this);
            EventUtil.AddListener(ClickEventType.ClickSuccess, this.OnClickSuccess, this);
        }

        protected virtual void OnDisable()
        {
            EventUtil.RemoveListener(this);
        }

        protected virtual void OnClickDown(EventArgs eventArgs)
        {
            // PointerEventData pointData = eventArgs.args[1] as PointerEventData;
            GameObject obj = eventArgs.args[0] as GameObject;
            if (moveObj != null && moveObj != obj) return;
            moveObj = obj;
        }
        protected virtual void OnClickUp(EventArgs eventArgs)
        {
            GameObject obj = eventArgs.args[0] as GameObject;
            if (moveObj != obj) return;
            moveObj = null;
        }
        protected virtual void OnClickSuccess(EventArgs eventArgs)
        {
            GameObject obj = eventArgs.args[0] as GameObject;
            if (moveObj != obj) return;
            float t1 = moveTime > 0.2f ? moveTime - 0.2f : 0.2f;
            obj.transform.DOScale(Tools.VectorMult(mulScale, obj.transform.localScale), t1);
            obj.transform.DOMove(targetMoveTrans.position, moveTime).OnComplete(() =>
                {
                    U14_TouchScreenMoveObj u14_TouchScreenMoveObj = obj.GetComponent<U14_TouchScreenMoveObj>();
                    u14_TouchScreenMoveObj.CanMove();
                    this.enabled = false;
                    operateEnd = true;
                    moveObj = null;
                });
            obj.transform.parent = targetMoveTrans.parent;
        }

        private void Update()
        {
            if (operateEnd) return;
        }

    }
}
