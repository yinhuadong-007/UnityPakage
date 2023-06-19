using UnityEngine;

public class DragCom_Move : DragCom_Base {
    //物体和鼠标的屏幕坐标之差
    private Vector3 _objMinusPos;
    private DragCom_Data data;
    private void Start() {
         _objMinusPos = Camera.main.WorldToScreenPoint(this.transform.position) - Input.mousePosition;
         data=this.GetComponent<DragCom_Data>();
    }

    private void CalcMovingPos()
    {

        Vector3 currPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + _objMinusPos);
        Vector3 Apos = Camera.main.transform.position;
        Vector3 Bpos = currPos;
        var k = (Bpos.y - Apos.y) / (Bpos.z - Apos.z);
        var b = Apos.y - k * Apos.z;

        float newY = currPos.y;
        if(data!=null&&data.highY!=0){
            newY= currPos.y*data.highY;
        }
        // if (Bpos.y >= objOriPos.y + 0.1f)
        // {
        //     newY = objOriPos.y + 0.1f;
        // }
        float newZ = (newY - b) / k;

        this.transform.position = new Vector3(currPos.x, newY, newZ);
    }

    private void Update()
    {
        if(this.GetComponent<DragCom_Assemble>().bl_allowMove==false){
            this.enabled=false;
        }
        CalcMovingPos(); 
    }
}