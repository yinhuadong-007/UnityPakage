namespace U14.Click
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class U14_ClickData : MonoBehaviour
    {
        public static U14_ClickData instance;

        [ChineseLabel("允许的点击偏移值")] public float moveOffset = 0.2f;
        [ChineseLabel("是否使用上拖 点击")] public bool useDragUp = true;
        [ChineseLabel("使用上拖点击 偏移值")] public float successOffset = 0.5f;



        protected virtual void Awake()
        {
            instance = this;
        }

        protected virtual void OnEnable()
        {
            instance = this;
        }
    }


}
