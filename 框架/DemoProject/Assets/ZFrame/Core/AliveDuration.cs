namespace ZFrame
{
    using UnityEngine;
    public class AliveDuration : MonoBehaviour
    {
        public float duration;
        public AliveDuration setDuration(float value){
            this.duration=value;
            return this;
        }
        private void Update() {
            if(this.duration<=0){
                Destroy(this.gameObject);
            }
        }
    }

}