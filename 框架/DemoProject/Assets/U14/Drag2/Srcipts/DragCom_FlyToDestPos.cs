using System.Collections;
using UnityEngine;
using System;

public class DragCom_FlyToDestPos : DragCom_Base
{
    public Vector3 _destPos;
    public Vector3 _destEulerAngle = Vector3.zero;
    // public Action func_complete;


    private bool _hasBegin = false;

    public DragCom_FlyToDestPos SetDestPos(Vector3 pos)
    {
        _destPos = pos;
        return this;
    }
    public DragCom_FlyToDestPos SetDestEnlerAngle(Vector3 eulerAngle)
    {
        if (eulerAngle != null)
        {
            _destEulerAngle = eulerAngle;
        }
        return this;
    }

    private void Start()
    {
        StartCoroutine(IEnum_Tween());
    }

    IEnumerator IEnum_Tween()
    {
        Vector3 oriPos = transform.position;
        Quaternion oriRotation = transform.rotation;

        float time = 0;
        while (time < 1)
        {
            yield return null;
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(oriPos, _destPos, time);

            if (_destEulerAngle != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(oriRotation, Quaternion.Euler(_destEulerAngle), time);
            }
        }

        gameObject.AddComponent<DragCom_Move>();

        // func_complete?.Invoke();
    }
}