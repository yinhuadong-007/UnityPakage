using UnityEngine;

public class DragCom_JustClick : DragCom_Base {
        private void Start() {
            Destroy(this.gameObject);
            // ZFrame.EventSystem.goEvent("拖拽物体仅点击后销毁");
        }
}