using UnityEngine;

public class DragCom_Scale : DragCom_Base
{

    public Vector3 _destPos;
    private float _objOriScale;
    private float _distBetweenOriPosAndDestPos = 0;
    private float _finalScale=0;
    private DragCom_Data data;


    public DragCom_Scale SetFinalScale(float scale){
        _finalScale=scale;
        return this;
    }
     public DragCom_Scale SetDestPos(Vector3 pos)
    {
        _destPos = pos;
        return this;
    }
    private void Start()
    {
        data=this.GetComponent<DragCom_Data>();
        _objOriScale = this.transform.localScale.x;
        _distBetweenOriPosAndDestPos = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(_destPos.x, _destPos.z));
    }

    private void Update()
    {
        var currDist = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(_destPos.x, _destPos.z));

        if (data.finalScale != 0)
        {
            this.transform.localScale = _objOriScale * Vector3.one + Vector3.one * (1 - currDist / _distBetweenOriPosAndDestPos) * (_finalScale - _objOriScale);
        }
    }
}