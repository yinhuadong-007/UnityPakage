using UnityEngine;

public class DragCom_DropDown : DragCom_Base
{
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //物体不再允许被控制移动
            this.GetComponent<DragCom_Assemble>().bl_allowMove = false;
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            this.enabled = false;
        }
    }
}