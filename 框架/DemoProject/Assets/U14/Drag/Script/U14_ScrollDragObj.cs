namespace U14.Drag
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using U14.Common;
    using DG.Tweening;

    /// <summary>
    /// 自动设置位置模式
    /// </summary>
    public enum EAutoResetPositionType
    {
        [Header("不自动化")]
        /// <summary> 不自动化 </summary>
        None,
        [Header("自动化 - 固定间距")]
        /// <summary> 自动化 - 间距 </summary>
        IntervalDistance,
        [Header("自动化 - 传入宽度")]
        /// <summary> 自动化 - 传入宽度 </summary>
        InputWidth,
    }
    public class U14_ScrollDragObj : MonoBehaviour
    {
        [Header("拖拽射线位置定位层")]
        public LayerMask layerMask;
        [Header("移动的速度")]
        public float followMoveSpeed = 5;
        [Header("移动的距离放大系数")]
        public float followMoveDistanceRatio = 5;
        [Header("衰减缓动 - 每秒衰减系数")]
        public float tweenFactor = 0;

        [Header("事件监听节点")]
        public GameObject handleObj;
        [Header("移动节点")]
        public Transform moveTrans;
        [Header("容器尺寸")]
        public Transform containTrans;
        [Header("移动的是不是世界坐标")]
        public bool isWorldPos = false;
        [Header("自动设置位置模式")]
        [EnumLabel("自动设置位置模式")] public EAutoResetPositionType autoResetPosition = EAutoResetPositionType.InputWidth;

        [Header("间距 - 自动化生效")]
        public Vector3 IntervalDistance = new Vector3(1, 0, 0);

        // [Header("子对象列表")]
        // public List<GameObject> objList;
        [Header("滑动方向")]
        public Vector3 moveDirection = Vector3.left;

        public bool isMoving = false;
        /// <summary> 物体update移动控制 </summary>
        protected bool _canMove = false;
        // public bool canMove
        // {
        //     get { return _canMove; }
        //     set
        //     {
        //         _canMove = value;
        //         if (_canMove)
        //         {
        //             ResetMoveTargetData();
        //         }
        //     }
        // }
        protected MoveTargetData m_moveTargetData;
        protected MoveTargetData m_moveCurData;

        // Start is called before the first frame update
        protected EDragStatus m_curDragStatus = EDragStatus.None;
        public EDragStatus curDragStatus
        {
            get { return m_curDragStatus; }
        }

        Vector3 lastTargetPos;
        bool first;
        [Header("展示数据 - 配置无效")]
        public Vector3 moveNodeInitPos;
        public Vector3 moveNodeMinPos;
        public Vector3 moveNodeMaxPos;
        bool isStart = false;

        List<float> widthList;
        Dictionary<Transform, float> itemsDir;

        float failTweenFactor = 1;

        private void Awake()
        {
            isWorldPos = false;
        }
        // Start is called before the first frame update
        protected virtual void Start()
        {
            if (moveTrans == null) moveTrans = this.transform;
            if (handleObj == null) handleObj = this.gameObject;
            if (containTrans == null) containTrans = this.transform;

            ResetMoveTargetData();
            _canMove = true;
            isWorldPos = false;
            failTweenFactor = 1;
            enterTween = false;
            tweenLastTime = 0;
            if (!isStart)
            {
                SetInitMoveData();
                if (autoResetPosition != EAutoResetPositionType.InputWidth)
                {
                    ResetObjList();
                }
            }
        }

        /// <summary>
        /// 设置滑动类型
        /// </summary>
        /// <param name="type">None 手动排列、IntervalDistance 等间距、InputWidth 传入物品宽度</param>
        /// <param name="list">InputWidth 有效</param>
        /// <returns></returns>
        public U14_ScrollDragObj SetScrollType(EAutoResetPositionType type = EAutoResetPositionType.IntervalDistance, List<float> list = null)
        {
            autoResetPosition = type;
            widthList = list;
            return this;
        }

        /// <summary>
        /// InputWidth 有效 设置宽度列表
        /// </summary>
        /// <param name="list"> 宽度列表</param>
        /// <returns></returns>
        public U14_ScrollDragObj SetSizeList(List<float> list)
        {
            widthList = list;
            return this;
        }

        protected void ResetMoveTargetData()
        {
            m_moveTargetData = new MoveTargetData()
            {
                position = isWorldPos ? moveTrans.position : moveTrans.localPosition,
                startPos = isWorldPos ? moveTrans.position : moveTrans.localPosition,
            };
            m_moveCurData = m_moveTargetData;
        }

        void SetInitMoveData()
        {
            if (!isStart)
            {
                isStart = true;
                moveNodeInitPos = moveTrans.localPosition;
                moveDirection = moveDirection.normalized;
            }
        }

        Vector3 downPos;
        protected virtual void TouchInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 1000, layerMask))
                {
                    downPos = raycastHit.point;
                    ResetMoveTargetData();
                    m_curDragStatus = EDragStatus.None;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_curDragStatus = EDragStatus.EndDrag;
                if (m_moveCurData.position == m_moveTargetData.position)
                {
                    cor_MoveEnd = StartCoroutine(MoveEnd());
                }
            }
            else if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 1000, layerMask))
                {
                    Vector3 hitPos = raycastHit.point;

                    if (m_curDragStatus == EDragStatus.None)
                    {
                        float dis = 999;
                        if (moveDirection.x != 0)
                        {
                            dis = downPos.x - hitPos.x;
                        }
                        else if (moveDirection.y != 0)
                        {
                            dis = downPos.y - hitPos.y;
                        }
                        else if (moveDirection.z != 0)
                        {
                            dis = downPos.z - hitPos.z;
                        }
                        if (Mathf.Abs(dis) > 0.1f)
                        {
                            SetMoveTargetPosition(hitPos);
                            m_curDragStatus = EDragStatus.MoveDrag;
                            failTweenFactor = 1;
                            enterTween = false;
                            tweenLastTime = 0;
                        }
                    }
                    else if (m_curDragStatus == EDragStatus.MoveDrag)
                    {
                        SetMoveTargetPosition(hitPos);
                    }
                }
            }
        }

        bool enterEnd = false;
        protected virtual void Update()
        {
            if (!_canMove) return;
            //操作
            TouchInput();
            //移动
            Move();
        }

        bool enterTween = false;
        float tweenLastTime = 0;
        void Move()
        {
            if (m_moveCurData.position != m_moveTargetData.position)
            {
                if (cor_MoveEnd != null) StopCoroutine(cor_MoveEnd);
                isMoving = true;

                float dis = (m_moveCurData.position - m_moveTargetData.position).sqrMagnitude;

                float factor = m_moveTargetData.lerpTime / m_moveTargetData.totalTime;
                if (m_curDragStatus != EDragStatus.MoveDrag && !enterTween)
                {
                    enterTween = true;
                    m_moveTargetData.startPos = m_moveCurData.position;
                    m_moveTargetData.totalTime -= m_moveTargetData.lerpTime;
                    m_moveTargetData.lerpTime = 0;
                }

                if (enterTween)
                {
                    failTweenFactor -= Time.deltaTime * tweenFactor;
                    if (failTweenFactor < 0.1f)
                    {
                        failTweenFactor = 0.1f;
                        tweenLastTime = 0.5f;
                    }

                    if (tweenLastTime > 0)
                    {
                        tweenLastTime -= Time.deltaTime;
                        if (tweenLastTime <= 0)
                        {
                            failTweenFactor = 0;
                            m_moveTargetData.totalTime = m_moveTargetData.lerpTime;
                            m_moveTargetData.position = m_moveCurData.position;
                        }
                    }
                }

                m_moveTargetData.lerpTime += Time.deltaTime * failTweenFactor;
                factor = m_moveTargetData.lerpTime / m_moveTargetData.totalTime;

                // Debug.Log("factor = " + factor + ", failTweenFactor " + failTweenFactor);
                m_moveCurData.position = Vector3.Lerp(m_moveTargetData.startPos, m_moveTargetData.position, factor);
                if (isWorldPos)
                {
                    moveTrans.position = m_moveCurData.position;
                }
                else
                {
                    moveTrans.localPosition = m_moveCurData.position;
                }
            }
            else if (isMoving && !enterEnd)
            {
                if (m_curDragStatus != EDragStatus.MoveDrag)
                {
                    enterEnd = true;
                    if (cor_MoveEnd != null) StopCoroutine(cor_MoveEnd);
                    cor_MoveEnd = StartCoroutine(MoveEnd());
                }
            }
            // Debug.Log("isMoving = " + isMoving);
        }

        Coroutine cor_MoveEnd;
        IEnumerator MoveEnd()
        {
            yield return null;
            isMoving = false;
            enterEnd = false;
        }

        protected virtual void SetMoveTargetPosition(Vector3 targetPos)
        {
            // Debug.Log("targetPos= " + targetPos + ", lastTargetPos= " + lastTargetPos);
            if (m_curDragStatus == EDragStatus.MoveDrag)
            {
                Vector3 subPos = Tools.VectorMult(targetPos - lastTargetPos, moveDirection);
                Vector3 lastMoveTargetPos = m_moveTargetData.position;
                // float y_h = lastMoveTargetPos.y;
                m_moveTargetData.position = lastMoveTargetPos + subPos * followMoveDistanceRatio;
                if (autoResetPosition != EAutoResetPositionType.None) FixMoveTargetPosition();
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

        /// <summary> 修正位置 </summary>
        /// <returns></returns>
        protected void FixMoveTargetPosition()
        {
            if (m_moveTargetData.position.x > moveNodeMaxPos.x)
            {
                m_moveTargetData.position.x = moveNodeMaxPos.x;
            }
            else if (m_moveTargetData.position.x < moveNodeMinPos.x)
            {
                m_moveTargetData.position.x = moveNodeMinPos.x;
            }
            if (m_moveTargetData.position.y > moveNodeMaxPos.y)
            {
                m_moveTargetData.position.y = moveNodeMaxPos.y;
            }
            else if (m_moveTargetData.position.y < moveNodeMinPos.y)
            {
                m_moveTargetData.position.y = moveNodeMinPos.y;
            }
            if (m_moveTargetData.position.z > moveNodeMaxPos.z)
            {
                m_moveTargetData.position.z = moveNodeMaxPos.z;
            }
            else if (m_moveTargetData.position.z < moveNodeMinPos.z)
            {
                m_moveTargetData.position.z = moveNodeMinPos.z;
            }
        }

        void SetMoveRangePos(List<float> widthList = null)
        {
            SetInitMoveData();
            moveNodeMinPos = moveNodeInitPos;
            moveNodeMaxPos = moveNodeInitPos;
            float disPos = 1;
            float contentLength = 0;
            if (autoResetPosition == EAutoResetPositionType.IntervalDistance)
            {
                if (moveDirection.x != 0)
                {
                    contentLength = Mathf.Abs(IntervalDistance.x) * (moveTrans.childCount) - containTrans.lossyScale.x / 2;
                }
                if (moveDirection.y != 0)
                {
                    contentLength = Mathf.Abs(IntervalDistance.y) * (moveTrans.childCount) - containTrans.lossyScale.y / 2;
                }
                if (moveDirection.z != 0)
                {
                    contentLength = Mathf.Abs(IntervalDistance.z) * (moveTrans.childCount) - containTrans.lossyScale.z / 2;
                }
            }
            else if (autoResetPosition == EAutoResetPositionType.InputWidth && widthList != null)
            {
                for (int i = 0; i < widthList.Count; i++)
                {
                    contentLength += widthList[i];
                }
                float halfWidth = 0;
                if (moveDirection.x != 0)
                {
                    halfWidth = containTrans.lossyScale.x / 2;
                }
                if (moveDirection.y != 0)
                {
                    halfWidth = containTrans.lossyScale.y / 2;
                }
                if (moveDirection.z != 0)
                {
                    halfWidth = containTrans.lossyScale.z / 2;
                }
                contentLength -= halfWidth;
            }

            if (moveDirection.x > 0)
            {
                float x = moveNodeInitPos.x - contentLength;
                x = x >= moveNodeInitPos.x ? moveNodeInitPos.x : x;
                moveNodeMinPos.x = x;
                moveNodeMaxPos.x = moveNodeInitPos.x + disPos;
            }
            else if (moveDirection.x < 0)
            {
                float x = moveNodeInitPos.x + contentLength;
                x = x <= moveNodeInitPos.x ? moveNodeInitPos.x : x;
                moveNodeMinPos.x = moveNodeInitPos.x - disPos;
                moveNodeMaxPos.x = x;
            }
            if (moveDirection.y > 0)
            {
                float y = moveNodeInitPos.y + contentLength;
                y = y <= moveNodeInitPos.y ? moveNodeInitPos.y : y;
                moveNodeMinPos.y = moveNodeInitPos.y - disPos;
                moveNodeMaxPos.y = y;
            }
            else if (moveDirection.y < 0)
            {
                float y = moveNodeInitPos.y - contentLength;
                y = y >= moveNodeInitPos.y ? moveNodeInitPos.y : y;
                moveNodeMinPos.y = y;
                moveNodeMaxPos.y = moveNodeInitPos.y + disPos;
            }
            if (moveDirection.z > 0)
            {
                float z = moveNodeInitPos.z - contentLength;
                z = z >= moveNodeInitPos.z ? moveNodeInitPos.z : z;
                moveNodeMinPos.z = z;
                moveNodeMaxPos.z = moveNodeInitPos.z + disPos;
            }
            else if (moveDirection.z > 0)
            {
                float z = moveNodeInitPos.z + contentLength;
                z = z <= moveNodeInitPos.z ? moveNodeInitPos.z : z;
                moveNodeMinPos.z = moveNodeInitPos.z - disPos;
                moveNodeMaxPos.z = z;
            }
        }

        public void ResetObjList()
        {
            if (autoResetPosition == EAutoResetPositionType.IntervalDistance)
            {
                ResetIntervalDistanceObjList();
            }
            else if (autoResetPosition == EAutoResetPositionType.InputWidth)
            {
                if (widthList == null)
                {
                    Debug.LogError("ResetObjList 配置表中物品尺寸未传入！！！");
                    return;
                }
                ResetInputWidthObjList(widthList);
            }
        }

        public void UpdateObjList(Action act = null)
        {
            if (autoResetPosition == EAutoResetPositionType.IntervalDistance)
            {
                UpdateIntervalDistanceObjList();
            }
            else if (autoResetPosition == EAutoResetPositionType.InputWidth)
            {
                widthList.Clear();
                for (int i = 0, length = moveTrans.childCount; i < length; i++)
                {
                    Transform obj = moveTrans.GetChild(i);
                    if (itemsDir.ContainsKey(obj))
                    {
                        widthList.Add(itemsDir[obj]);
                    }
                }
                UpdateInputWidthObjList(widthList, act);
            }
        }

        /// <summary>
        /// 间距 重置位置
        /// </summary>
        public void ResetIntervalDistanceObjList()
        {
            Vector3 cubeOffset = Tools.VectorMult(containTrans.lossyScale, IntervalDistance.normalized);
            Vector3 startPos = (-cubeOffset + IntervalDistance) / 2;

            List<int> removeObj = new List<int>();
            for (int i = 0, length = moveTrans.childCount; i < length; i++)
            {
                Transform obj = moveTrans.GetChild(i);
                if (obj.gameObject.activeSelf)
                {
                    Vector3 pos = startPos;
                    if (IntervalDistance.x == 0)
                    {
                        pos.x = obj.localPosition.x;
                    }
                    if (IntervalDistance.y == 0)
                    {
                        pos.y = obj.localPosition.y;
                    }
                    if (IntervalDistance.z == 0)
                    {
                        pos.z = obj.localPosition.z;
                    }
                    obj.localPosition = pos;
                    startPos += IntervalDistance;
                }
                else
                {
                    removeObj.Add(i);
                }
            }
            for (int i = removeObj.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(moveTrans.GetChild(removeObj[i]).gameObject);
            }
            SetMoveRangePos();
        }

        /// <summary>
        /// 间距 更新位置
        /// </summary>
        /// <param name="act"></param>
        public void UpdateIntervalDistanceObjList(Action act = null)
        {
            Vector3 cubeOffset = Tools.VectorMult(containTrans.lossyScale, IntervalDistance.normalized);
            Vector3 startPos = (-cubeOffset + IntervalDistance) / 2;

            int endCount = 0;

            for (int i = 0, length = moveTrans.childCount; i < length; i++)
            {
                Transform obj = moveTrans.GetChild(i);
                if (obj.gameObject.activeSelf)
                {
                    Vector3 pos = startPos;
                    if (IntervalDistance.x == 0)
                    {
                        pos.x = obj.localPosition.x;
                    }
                    if (IntervalDistance.y == 0)
                    {
                        pos.y = obj.localPosition.y;
                    }
                    if (IntervalDistance.z == 0)
                    {
                        pos.z = obj.localPosition.z;
                    }
                    obj.transform.DOLocalMove(pos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        endCount++;
                        if (endCount >= length && act != null)
                        {
                            act();
                        }
                    });
                    startPos += IntervalDistance;
                }
            }
            SetMoveRangePos();
        }

        /// <summary> 传入宽度列表 重置位置 </summary>
        /// <param name="widthList"></param>
        public void ResetInputWidthObjList(List<float> widthList)
        {
            Vector3 cubeOffset = Tools.VectorMult(containTrans.lossyScale, moveDirection.normalized);
            Vector3 startPos = (-cubeOffset + moveDirection * widthList[0]) / 2;

            List<int> removeObj = new List<int>();
            if (itemsDir == null)
            {
                itemsDir = new Dictionary<Transform, float>();
            }
            else
            {
                itemsDir.Clear();
            }
            int widthIndex = 0;
            for (int i = 0, length = moveTrans.childCount; i < length; i++)
            {
                Transform obj = moveTrans.GetChild(i);
                if (obj.gameObject.activeSelf)
                {
                    Vector3 pos = startPos;
                    obj.localPosition = pos;
                    itemsDir.Add(obj, widthList[widthIndex]);
                    if (i < length - 1)
                    {
                        startPos += moveDirection * (widthList[widthIndex] + widthList[widthIndex + 1]) / 2;
                        widthIndex++;
                    }
                }
                else
                {
                    removeObj.Add(i);
                }
            }
            for (int i = removeObj.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(moveTrans.GetChild(removeObj[i]).gameObject);
            }
            SetMoveRangePos(widthList);
        }

        /// <summary>
        ///  传入宽度列表 更新位置
        /// </summary>
        /// <param name="act"></param>
        public void UpdateInputWidthObjList(List<float> widthList, Action act = null)
        {
            Vector3 cubeOffset = Tools.VectorMult(containTrans.lossyScale, moveDirection.normalized);
            Vector3 startPos = (-cubeOffset + moveDirection * widthList[0]) / 2;

            int endCount = 0;
            int widthIndex = 0;

            for (int i = 0, length = moveTrans.childCount; i < length; i++)
            {
                Transform obj = moveTrans.GetChild(i);
                if (obj.gameObject.activeSelf)
                {
                    Vector3 pos = startPos;
                    obj.transform.DOLocalMove(pos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        endCount++;
                        if (endCount >= length && act != null)
                        {
                            act();
                        }
                    });
                    if (i < length - 1)
                    {
                        startPos += moveDirection * (widthList[widthIndex] + widthList[widthIndex + 1]) / 2;
                        widthIndex++;
                    }
                }
            }
            SetMoveRangePos(widthList);
        }
    }
}