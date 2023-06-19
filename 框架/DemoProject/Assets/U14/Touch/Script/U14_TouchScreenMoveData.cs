namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class U14_TouchScreenMoveData : MonoBehaviour
    {
        public static U14_TouchScreenMoveData instance;

        // [Header("拖拽射线位置定位层, bux所有层")]
        // public LayerMask layerMask;
        [Header("移动的速度")]
        public float followMoveSpeed = 10;
        [Header("移动距离映射")]
        public float moveDistanceMap = 1;
        [Header("拖拽移动范围限制层")]
        public LayerMask limitLayerMask = ~0;
        [Header("拖拽移动平面")]
        public EDragAxis dragAxis = EDragAxis.XZ;
        [Header("中心点 dragAxis等于XZ_Circle有效")]
        public Transform centerTrans;
        [Header("半径 dragAxis等于XZ_Circle有效")]
        public float radius = -1f;

        protected virtual void Awake()
        {
            instance = this;
        }

        protected virtual void OnEnable()
        {
            instance = this;
        }

        public virtual void SetCircleData(EDragAxis dragAxis, Transform centerTrans, float radius = 0)
        {
            this.dragAxis = dragAxis;
            this.centerTrans = centerTrans;
            this.radius = radius;
        }
    }
}
