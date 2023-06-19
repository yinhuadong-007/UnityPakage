using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//通过骨骼弯曲的表面
public class BendBoneSurface : MonoBehaviour {

    [Header("骨骼根节点")]
    public Transform Transfrom_BoneRoot;

    //所有骨骼列表
    private List<Transform> _list_AllBone = new List<Transform>();
    public List<Transform> List_AllBone {
        get => _list_AllBone;
    }

    // Use this for initialization
    void Awake() {
        _list_AllBone = GetChildTransfroms(Transfrom_BoneRoot);
    }

    //通过坐标获取最靠近改坐标的骨骼对象
    public Transform GetNearesBone(Vector3 point) {
        Transform nearestObject = null;
        float nearestDistance = Mathf.Infinity;
        foreach (Transform transform in List_AllBone) {
            float distance = Vector3.Distance(transform.transform.position, point);
            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearestObject = transform;
            }
        }
        return nearestObject;
    }

    //获取所有子节点 (一直到最深的子节点)
    List<Transform> GetChildTransfroms(Transform parent) {
        List<Transform> childList = new List<Transform>();
        for (int i = 0; i < parent.transform.childCount; i++) {
            childList.Add(parent.transform.GetChild(i));
            if (parent.transform.GetChild(i).childCount > 0) {
                childList.AddRange(GetChildTransfroms(parent.transform.GetChild(i)));
            }
        }
        return childList;
    }

}