using UnityEngine;
using System;
using System.Collections;
namespace ZFrame
{
    /// <summary>Mono的替身 </summary>
    public class MonoSubstitute : MonoBehaviour
    {
        public static MonoSubstitute instance;

        private void Awake() {
            instance=this;
        }
        public void SStartCoroutine(IEnumerator IEnum){
            this.StartCoroutine(IEnum);
        }


        public void waitForSeconds(float duration,Action action){
            StartCoroutine(IEnum_waitForSeconds(duration,action));
        }
        IEnumerator IEnum_waitForSeconds(float duration,Action action){
            yield return new WaitForSeconds(duration);
            action();
        }

    }
}