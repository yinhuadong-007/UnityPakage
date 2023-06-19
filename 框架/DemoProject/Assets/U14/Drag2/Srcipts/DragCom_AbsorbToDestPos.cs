using System.Collections;
using UnityEngine;

public class DragCom_AbsorbToDestPos : DragCom_Base
{
    //目标位置
    private Vector3 _destPos;
    //物体初始化时的位置
    private Vector3 _objOriPos;
    public DragCom_AbsorbToDestPos SetDestPos(Vector3 pos)
    {
        _destPos = pos;
        return this;
    }
    private void Start()
    {
        _objOriPos = this.transform.position;
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        
        var currDist = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(_destPos.x, _destPos.z));
        while (currDist > 0.2f)
        {
            currDist = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(_destPos.x, _destPos.z));
            yield return null;
        }
        StartCoroutine(AbsorbToDest());
        //物体不再允许被控制移动
        this.GetComponent<DragCom_Assemble>().bl_allowMove=false;

    }


    private IEnumerator AbsorbToDest()
    {
        float time = 1;
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, new Vector3(_destPos.x, transform.position.y, _destPos.z), 1 - time);
            yield return null;
        }
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

}