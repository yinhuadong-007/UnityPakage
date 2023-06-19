using UnityEngine;

public class DragCom_Assemble : MonoBehaviour {
    //允许输入移动物体
    public bool bl_allowMove=true;



    private void OnCollisionEnter(Collision other)
    {
            // ZFrame.EventSystem.goEvent("DragObj触碰到了物体", gameObject,other.gameObject);
    }

    public void Dispose(){
        var coms=this.GetComponents<DragCom_Base>();
        foreach(var com in coms){
            Destroy(com);
        }
    }
}