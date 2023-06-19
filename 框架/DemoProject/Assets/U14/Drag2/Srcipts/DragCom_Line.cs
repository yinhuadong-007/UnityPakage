using UnityEngine;
//抓取物下方射出光柱
public class DragCom_Line : DragCom_Base {
    private GameObject _line;

     public void showLine(){
        // _line=ZFrame.Res.createObj("Assets/Game/Prefab/Line.prefab");
        _line.transform.localScale*=0.2f;
        _line.transform.SetParent(transform);
        var mesh=_line.GetComponent<MeshFilter>().mesh;

        var scale= _line.transform.localScale;
        _line.transform.localPosition=new Vector3(-mesh.bounds.size.x*scale.x/2,-mesh.bounds.size.y*scale.y/2,mesh.bounds.size.z*scale.z/2);
    }
}