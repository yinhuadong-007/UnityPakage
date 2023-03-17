namespace U14.Drag
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class U14_DragScaleData : U14_DragData
    {
        [Header("基于当前尺寸进行的缩放值")]
        public Vector3 mulScale = Vector3.one;
        [Header("缩放速度")]
        public float scaleSpeed = 1f;

        protected override void Awake()
        {
            instance = this;
        }
    }
}
