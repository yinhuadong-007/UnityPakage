namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using U14.Event;
    using U14.Common;

    public class U14_MoveScaleDragObj : U14_MoveDragObj
    {
        [Header("拖动时进行缩放 - 基于当前尺寸进行的缩放值")]
        public Vector3 mulScale = Vector3.one;
        [Header("缩放速度 - 每秒变化的缩放值")]
        public float scaleSpeed = 1f;

        protected Vector3 m_initScale;

        Vector3 subScaleSpeed;
        Vector3 targetScale;
        public bool canScale;


        // Start is called before the first frame update
        protected override void Start()
        {
            //配置数据优先
            SetConfData();

            base.Start();
            m_initPosition = transform.position;
            m_initScale = transform.localScale;
            subScaleSpeed = (mulScale - Vector3.one) * scaleSpeed;
            targetScale = Tools.VectorMult(m_initScale, mulScale);
            Debug.Log("subScaleSpeed = " + subScaleSpeed);
            canScale = true;
        }

        protected override void SetConfData()
        {
            Debug.Log("SetConfData 1 ");
            base.SetConfData();
            if (U14_DragData.instance != null && useCommonConf)
            {
                Debug.Log("SetConfData 2");
                mulScale = ((U14_DragData.instance) as U14_DragScaleData).mulScale;
                scaleSpeed = ((U14_DragData.instance) as U14_DragScaleData).scaleSpeed;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!canScale) return;
            // Debug.Log("m_curDragStatus = " + m_curDragStatus);
            if (m_curDragStatus == EDragStatus.MoveDrag && transform.localScale != targetScale)
            {
                Vector3 scl = transform.localScale;
                Vector3 sub1 = targetScale - scl;
                scl += subScaleSpeed * Time.deltaTime;
                Vector3 sub2 = targetScale - scl;
                Vector3 sub3 = targetScale - m_initScale;


                if ((Mathf.Sign(sub1.x) * Mathf.Sign(sub2.x) <= 0 && Mathf.Sign(sub1.y) * Mathf.Sign(sub2.y) <= 0 && Mathf.Sign(sub1.z) * Mathf.Sign(sub2.z) <= 0)
                || (Mathf.Sign(sub3.x) * Mathf.Sign(sub2.x) <= 0 && Mathf.Sign(sub3.y) * Mathf.Sign(sub2.y) <= 0 && Mathf.Sign(sub3.z) * Mathf.Sign(sub2.z) <= 0))
                {
                    transform.localScale = targetScale;
                }
                else
                {
                    transform.localScale = scl;
                }
            }
            else if (m_curDragStatus == EDragStatus.EndDrag && transform.localScale != m_initScale)
            {
                Vector3 scl = transform.localScale;
                Vector3 sub1 = scl - m_initScale;
                scl -= subScaleSpeed * Time.deltaTime;
                Vector3 sub2 = scl - m_initScale;
                Vector3 sub3 = targetScale - m_initScale;
                if ((Mathf.Sign(sub1.x) * Mathf.Sign(sub2.x) <= 0 && Mathf.Sign(sub1.y) * Mathf.Sign(sub2.y) <= 0 && Mathf.Sign(sub1.z) * Mathf.Sign(sub2.z) <= 0)
                || (Mathf.Sign(sub3.x) * Mathf.Sign(sub2.x) <= 0 && Mathf.Sign(sub3.y) * Mathf.Sign(sub2.y) <= 0 && Mathf.Sign(sub3.z) * Mathf.Sign(sub2.z) <= 0))
                {
                    transform.localScale = m_initScale;
                }
                else
                {
                    transform.localScale = scl;
                }
            }
        }
    }
}

