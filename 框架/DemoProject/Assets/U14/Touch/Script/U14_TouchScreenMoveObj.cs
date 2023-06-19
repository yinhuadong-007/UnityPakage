namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using U14.Common;
    using U14.Event;

    public enum EDragAxis
    {
        None,
        XY,
        XZ,
        YZ,
        XZ_Circle,
    }

    public class U14_TouchScreenMoveObj : MonoBehaviour
    {
        // [Header("拖拽射线位置定位层, bux所有层")]
        // public LayerMask layerMask;
        [Header("移动的速度")]
        public float followMoveSpeed = 10;
        [Header("移动距离映射")]
        public float moveDistanceMap = 1;

        [Header("使用通用配置")]
        public bool useCommonConf = true;

        [Header("拖拽移动范围限制层")]
        public LayerMask limitLayerMask = ~0;
        [Header("拖拽移动平面")]
        public EDragAxis dragAxis = EDragAxis.XZ;
        [Header("中心点 dragAxis等于XZ_Circle有效")]
        public Transform centerTrans;
        [Header("半径 dragAxis等于XZ_Circle有效")]
        public float radius = -1f;

        private GameObject handleObj;
        private Transform moveTrans;

        private Transform targetTrans;
        private Vector3 lastPosition;

        /// <summary> 物体update移动控制 </summary>
        public bool canMove = true;
        protected MoveTargetData m_moveTargetData;
        protected MoveTargetData m_moveCurData;

        protected EDragStatus m_curDragStatus = EDragStatus.None;
        public EDragStatus curDragStatus
        {
            get { return m_curDragStatus; }
        }
        LayerMask everyThingMask = ~0;

        private void Start()
        {
            //配置数据优先
            SetConfData();
            if (moveTrans == null) moveTrans = this.transform;
            if (handleObj == null) handleObj = this.gameObject;


            canMove = true;

            targetTrans = this.transform;
            lastPosition = targetTrans.position;
            ResetMoveTargetData();
        }

        protected virtual void SetConfData()
        {
            if (U14_TouchScreenMoveData.instance != null && useCommonConf)
            {
                // layerMask = U14_TouchScreenMoveData.instance.layerMask;
                followMoveSpeed = U14_TouchScreenMoveData.instance.followMoveSpeed;
                limitLayerMask = U14_TouchScreenMoveData.instance.limitLayerMask;
                dragAxis = U14_TouchScreenMoveData.instance.dragAxis;
                centerTrans = U14_TouchScreenMoveData.instance.centerTrans;
                moveDistanceMap = U14_TouchScreenMoveData.instance.moveDistanceMap;
            }

            LayerMask mask = 1;
            int i = 30;
            while (i > 0)
            {
                mask |= 1 << i;
                i--;
            }
            everyThingMask = mask;
            if (centerTrans != null && radius <= 0)
            {
                radius = centerTrans.GetComponent<MeshFilter>().mesh.bounds.size.x / 2 * centerTrans.lossyScale.x * 0.9f;
            }
        }

        public virtual void SetCircleData(EDragAxis dragAxis, Transform centerTrans, float radius = 0)
        {
            this.dragAxis = dragAxis;
            this.centerTrans = centerTrans;
            this.radius = radius;
            if (this.centerTrans != null && this.radius <= 0)
            {
                this.radius = this.centerTrans.GetComponent<MeshFilter>().mesh.bounds.size.x * this.centerTrans.lossyScale.x / 2 * 0.9f;
            }
        }

        public void CanNotMove()
        {
            canMove = false;
        }

        public void CanMove()
        {
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

        Vector3 disScreenPos;
        Vector3 initPos;

        private void Update()
        {
            if (!canMove) return;

            TouchInput();

            if (m_moveCurData.position != m_moveTargetData.position)
            {
                // m_moveTargetData.lerpTime += Time.deltaTime;
                // float factor = m_moveTargetData.lerpTime / m_moveTargetData.totalTime;
                // m_moveCurData.position = Vector3.Lerp(m_moveTargetData.startPos, m_moveTargetData.position, factor);
                m_moveCurData.position = m_moveTargetData.position;
                moveTrans.position = m_moveCurData.position;
            }
        }

        protected virtual void TouchInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 t_pos = moveTrans.position;
                if (dragAxis == EDragAxis.XZ)
                {
                    t_pos = new Vector3(t_pos.x, t_pos.z, 0);
                }
                else if (dragAxis == EDragAxis.XY)
                {
                    t_pos = new Vector3(t_pos.x, t_pos.y, 0);
                }
                else if (dragAxis == EDragAxis.YZ)
                {
                    t_pos = new Vector3(t_pos.z, t_pos.y, 0);
                }

                disScreenPos = Camera.main.WorldToScreenPoint(t_pos) - Input.mousePosition;
                ResetMoveTargetData();
                OnBeginDragDelegate(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_moveTargetData = m_moveCurData;
                OnEndDragDelegate(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + disScreenPos);
                Vector3 t_pos = pos;
                if (dragAxis == EDragAxis.XZ)
                {
                    t_pos = new Vector3(pos.x, initPos.y, pos.y);
                }
                else if (dragAxis == EDragAxis.XY)
                {
                    t_pos = new Vector3(pos.x, pos.y, initPos.z);
                }
                else if (dragAxis == EDragAxis.YZ)
                {
                    t_pos = new Vector3(initPos.x, pos.y, pos.x);
                }
                t_pos *= moveDistanceMap;
                SetMoveTargetPosition(t_pos);
                OnDragDelegate(Input.mousePosition);
            }
        }

        protected virtual void OnBeginDragDelegate(Vector3 pos)
        {
            if (HitPos(pos))
            {
                m_curDragStatus = EDragStatus.BeginDrag;
                EventUtil.DispatchEvent(DragMoveEventType.DragBegin, moveTrans.gameObject, pos);

                ZFrame.EventSystem.goEvent(EventName.Drag_Begin, moveTrans.gameObject);

            }
        }

        protected virtual void OnDragDelegate(Vector3 pos)
        {
            if (HitPos(pos))
            {
                m_curDragStatus = EDragStatus.MoveDrag;
                EventUtil.DispatchEvent(DragMoveEventType.DragMoving, moveTrans.gameObject, pos);
                ZFrame.EventSystem.goEvent(EventName.Drag_Move, moveTrans.gameObject);
            }
        }

        protected virtual void OnEndDragDelegate(Vector3 pos)
        {
            m_curDragStatus = EDragStatus.EndDrag;
            EventUtil.DispatchEvent(DragMoveEventType.DragEnd, moveTrans.gameObject, pos);
            ZFrame.EventSystem.goEvent(EventName.Drag_End, moveTrans.gameObject);
        }

        bool HitPos(Vector3 pos)
        {
            return true;
            // Ray ray = Camera.main.ScreenPointToRay(pos);
            // RaycastHit raycastHit;
            // if (Physics.Raycast(ray, out raycastHit, 1000, layerMask))
            // {
            //     Vector3 hitPos = raycastHit.point;
            //     Debug.DrawLine(Camera.main.transform.position, hitPos);
            //     SetMoveTargetPosition(hitPos);
            //     return true;
            // }
            // return false;
        }

        Vector3 lastTargetPos;
        protected virtual void SetMoveTargetPosition(Vector3 targetPos)
        {
            if (m_curDragStatus == EDragStatus.MoveDrag)
            {
                Vector3 subPos = targetPos - lastTargetPos;

                Vector3 rayDir = Vector3.zero;

                if (dragAxis == EDragAxis.XZ || dragAxis == EDragAxis.XZ_Circle)
                {
                    subPos.y = 0;
                    rayDir = Vector3.down;
                }
                else if (dragAxis == EDragAxis.XY)
                {
                    subPos.z = 0;
                    rayDir = Vector3.forward;
                }
                else if (dragAxis == EDragAxis.YZ)
                {
                    subPos.x = 0;
                    rayDir = Vector3.right;
                }
                // Debug.Log("subPos:" + subPos);
                Vector3 lastMoveTargetPos = m_moveTargetData.position;
                Vector3 nextPos = lastMoveTargetPos + subPos;
                Vector3 nextPosCircle = nextPos;

                Vector3 subPos1 = subPos;
                Vector3 subPos2 = subPos;
                if (dragAxis == EDragAxis.XZ)
                {
                    subPos1.x = 0;
                    subPos2.z = 0;
                }
                else if (dragAxis == EDragAxis.XY)
                {
                    subPos1.x = 0;
                    subPos2.y = 0;
                }
                else if (dragAxis == EDragAxis.YZ)
                {
                    subPos1.y = 0;
                    subPos2.z = 0;
                }

                Vector3 nextPos1 = lastMoveTargetPos + subPos1;
                Vector3 nextPos2 = lastMoveTargetPos + subPos2;

                RaycastHit hit;
                bool isHit = false;

                if (limitLayerMask == everyThingMask)
                {
                    isHit = true;
                }
                else if (dragAxis == EDragAxis.XZ_Circle)
                {
                    var yy = nextPos.y;
                    var xx = nextPos.x;
                    var zz = nextPos.z;
                    var _radius = radius;
                    var _centerPos = centerTrans.transform.position;
                    var dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_centerPos.x, _centerPos.z));
                    // Debug.Log("dist1:" + dist + " _radius1:" + _radius);
                    if (dist >= _radius)
                    {
                        var dir = nextPos - new Vector3(_centerPos.x, transform.position.y, _centerPos.z);
                        dir.Normalize();
                        xx = _centerPos.x + dir.x * (_radius);
                        zz = _centerPos.z + dir.z * (_radius);
                        nextPosCircle = new Vector3(xx, yy, zz);
                        nextPos = nextPosCircle;
                    }
                    Debug.DrawLine(nextPos, new Vector3(_centerPos.x, lastMoveTargetPos.y, _centerPos.z), Color.red);
                    isHit = true;
                }
                else if (Physics.Raycast(nextPos, rayDir, out hit, 100, limitLayerMask))
                {
                    Debug.DrawLine(nextPos, hit.point, Color.red);
                    isHit = true;
                }

                else if (Physics.Raycast(nextPos1, rayDir, out hit, 100, limitLayerMask))
                {
                    Debug.DrawLine(nextPos1, hit.point, Color.red);
                    isHit = true;
                    nextPos = nextPos1;
                }
                else if (Physics.Raycast(nextPos2, rayDir, out hit, 100, limitLayerMask))
                {
                    Debug.DrawLine(nextPos2, hit.point, Color.red);
                    isHit = true;
                    nextPos = nextPos2;
                }

                if (isHit)
                {
                    m_moveTargetData.position = nextPos;
                    // Debug.Log("m_moveTargetData.position:" + m_moveTargetData.position);
                    // m_moveTargetData.position = Tools.VectorMult(new Vector3(lastMoveTargetPos.x + targetPos.x, y_h, targetPos.z), moveDirection);
                    if (m_moveTargetData.position != lastMoveTargetPos)
                    {
                        m_moveTargetData.startPos = m_moveCurData.position;
                        m_moveTargetData.totalTime = Vector3.Distance(m_moveTargetData.position, m_moveTargetData.startPos) / followMoveSpeed;
                        m_moveTargetData.lerpTime = 0;
                    }
                }
            }

            lastTargetPos = targetPos;
        }

    }
}

