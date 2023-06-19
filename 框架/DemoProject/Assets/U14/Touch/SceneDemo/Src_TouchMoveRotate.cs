namespace U14.Drag
{
    using UnityEngine;
    using U14.Event;
    using U14.Drag;
    using U14.Common;
    using DG.Tweening;

    public class Src_TouchMoveRotate : MonoBehaviour
    {
        [ChineseLabel("倾倒角度")] public Vector3 targetAngle = new Vector3(0, 0, 90);

        protected GameObject moveObj;
        protected bool operateEnd = false;
        private Vector3 initAngle;

        private void Start()
        {
            initAngle = transform.eulerAngles;
        }

        protected void OnEnable()
        {
            EventUtil.RemoveListener(this);
            EventUtil.AddListener(DragMoveEventType.DragBegin, this.OnDragBegin, this);
            EventUtil.AddListener(DragMoveEventType.DragMoving, this.OnDragMoving, this);
            EventUtil.AddListener(DragMoveEventType.DragEnd, this.OnDragEnd, this);
        }

        private void OnDisable()
        {
            EventUtil.RemoveListener(this);
        }

        void OnDragBegin(EventArgs eventArgs)
        {
            if (operateEnd) return;
            GameObject obj = eventArgs.args[0] as GameObject;
            if (moveObj != null && moveObj != obj) return;

            moveObj = obj;
            moveObj.transform.DORotate(targetAngle, 0.2f);
        }

        void OnDragMoving(EventArgs eventArgs)
        {
            if (operateEnd) return;
            GameObject obj = eventArgs.args[0] as GameObject;
            if (moveObj != obj) return;
        }

        void OnDragEnd(EventArgs eventArgs)
        {
            if (operateEnd) return;
            moveObj.transform.DORotate(initAngle, 0.2f);
        }

    }
}
