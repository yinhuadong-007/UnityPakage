namespace U14.Event
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EventMono : MonoBehaviour
    {
        private void Awake()
        {
            EventUtil.Clear();
        }
    }
}