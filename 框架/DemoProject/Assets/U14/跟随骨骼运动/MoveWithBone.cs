using System.Collections;
using UnityEngine;

//跟随骨骼移动的物体
public class MoveWithBone : MonoBehaviour {

    public BendBoneSurface BendBoneSurface;

    //当前绑定的骨头节点
    private Transform _stickBone;

    // Use this for initialization
    void Start() {
        _stickBone = null;

        Stick();
    }

    //立即绑定到要跟随的表面
    public void Stick() {
        _stickBone = BendBoneSurface.GetNearesBone(transform.position);
        transform.SetParent(_stickBone);
    }


    //解绑
    public void UnStick() {
        if (_stickBone) {
            transform.SetParent(null);
            _stickBone = null;
        }
    }

}