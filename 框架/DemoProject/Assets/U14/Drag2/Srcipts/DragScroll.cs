using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
//之后需要通过拆分多个组件方式来优化
public class DragScroll : MonoBehaviour
{


    

    [Header("是否拖拽时消失")]
    public bool dragAndDisappear = false;
    [Header("拖拽的数量达到某数量时消失,0为不开启功能")]
    public int dragHitAndDisappearNum = 0;

    [Header("拖拽Y轴高度系数,0为不开启功能")]
    public float highY = 0;

    [Header("是否开启拖曳物体下方射线")]
    public bool bl_showLine = false;

    [Header("是否开启拖动完物体后其他物体补位")]
    public bool bl_reposition = false;

    [Header("滚动速度")]
    public float scrollSpd = 4;

    [Header("是否仅点击")]
    public bool bl_justClick=false;

      [Header("玩家控制掉落")]
    public bool bl_playDropDown=false;

    [Header("是否开启拖动")]
    public bool bl_drag=false;

    [Header("开启吸附到位置")]
    public bool bl_absorbToDest=false;
     [Header("开启飞到位置")]
    public bool bl_flyToDest=false;

    [Header("目标位置")]
    public Vector3[] destPosList;
     [Header("目标旋转")]
    public Vector3[] destRotationList;
    [Header("拖拽最终缩放比,0为不开启功能")]
    public float finalScale = 0;


    private Transform _itemLayerTs;
    private GameObject _boardObj;
    private Vector3 posOri;
    private bool isScrolling = false;
    private Vector3 objMinusPos;
    private List<Vector3> objPosList = new();
    private bool isAnimating = false;
    private int dragedNum = 0;

    void Start()
    {
        _itemLayerTs = UnityTools.FindChildName(transform, "道具层");
        _boardObj = UnityTools.FindChildName(transform, "板子").gameObject;
        RecordPos();
    }
    private void OnEnable()
    {
        StartCoroutine(appearAnima());
    }
    //拖动层出现动画
    IEnumerator appearAnima()
    {
        var ts = this.transform;
        Vector3 oriPos = ts.transform.position + Vector3.back * 0.7f;
        Vector3 desPos = ts.position;
        ts.transform.position = oriPos;

        yield return new WaitForSeconds(0.37f);


        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime * Mathf.Sqrt(1 - time) * 3;

            ts.transform.position = Vector3.Lerp(oriPos, desPos, time / 1f);
            yield return null;
        }
    }
    //拖动曾移除动画
    IEnumerator disAppearAnima()
    {
        isAnimating = true;
        var ts = this.transform;
        Vector3 oriPos = ts.position;
        Vector3 desPos = ts.position + Vector3.back * 0.7f;

        yield return new WaitForSeconds(0.00f);


        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;

            ts.transform.position = Vector3.Slerp(oriPos, desPos, time / 1);
            yield return null;
        }
        this.transform.gameObject.SetActive(false);
    }

    //检查射线是否触碰板子
    private bool checkScrollBoard(GameObject obj)
    {
        return obj.GetHashCode() == _boardObj.GetHashCode();
    }
    //检查射线是否触碰道具层上的道具
    private bool checkScrollItem(GameObject obj)
    {
        for (int z = 0; z < _itemLayerTs.childCount; z++)
        {
            if (obj.GetHashCode() == _itemLayerTs.GetChild(z).gameObject.GetHashCode())
            {
                return true;
            }
        }
        return false;
    }

    //移除拖拽物体上的文本
    private void RemoveText(GameObject dragObj)
    {
        var text = UnityTools.FindChildContainName(dragObj.transform, "Text");
        if (text != null)
        {
            text.gameObject.SetActive(false);
        }
        if (dragAndDisappear == true)
        {
            StartCoroutine(disAppearAnima());
        }
    }

    //记录道具位置-为了之后补位
    private void RecordPos()
    {
        objPosList.Clear();
        for (int z = 0; z < _itemLayerTs.childCount; z++)
        {
            var item = _itemLayerTs.GetChild(z).gameObject;
            objPosList.Add(new Vector3(item.transform.position.x, item.transform.position.y, item.transform.position.z));
        }
        objPosList = objPosList.OrderBy(x => x.x).ToList<Vector3>();
    }
    //当开启补位功能时，移走一个物体，剩下物体自动补位
    IEnumerator Reposition(GameObject obj)
    {
        if (bl_reposition == false)
        {
            yield break;
        }
        float time = 1;
        while (time > 0)
        {
            time -= Time.deltaTime * 2;
            for (int z = 0; z < _itemLayerTs.childCount; z++)
            {
                GameObject item = _itemLayerTs.GetChild(z).gameObject;
                item.transform.position = Vector3.Slerp(item.transform.position, new Vector3(objPosList[z].x, item.transform.position.y, item.transform.position.z), 1 - time);
            }
            yield return null;
        }
    }


    void Update()
    {
        if (isScrolling == true)
        {
            float minusX = Mathf.Clamp(Input.mousePosition.x - posOri.x, -1, 1);

            for (int z = 0; z < _itemLayerTs.childCount; z++)
            {
                var obj = _itemLayerTs.GetChild(z);
                obj.transform.position += new Vector3(minusX * Time.deltaTime * scrollSpd, 0, 0);
            }
            posOri = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            posOri = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(posOri);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
                //碰撞到拖动层的道具
                if (checkScrollItem(hit.collider.gameObject))
                {
                    GameObject dragObj = hit.collider.gameObject;
                    objMinusPos = Camera.main.WorldToScreenPoint(dragObj.transform.position) - posOri;
                    // ZFrame.EventSystem.goEvent("滚动条抓住物体后触发", dragObj);
                    // DragData.dragObjList.Add(dragObj);
                    dragedNum++;
                    // AddDragObj(dragObj);
                    AddDragCom(dragObj);

                    if(dragHitAndDisappearNum!=0&&dragedNum>=dragHitAndDisappearNum){
                        StartCoroutine(disAppearAnima());
                    }

                    dragObj.transform.SetParent(SceneManager.GetActiveScene().GetRootGameObjects()[0].transform.parent);
                    RemoveText(dragObj);
                    StartCoroutine(Reposition(dragObj));
                }
                //触碰到拖动层的板子
                else if (checkScrollBoard(hit.collider.gameObject))
                {
                    isScrolling = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isScrolling == true)
            {
                isScrolling = false;
                RecordPos();
            }
        }
        // //当拖动物体掉落数量达成时，隐藏拖动层
        // if (dragHitAndDisappearNum != 0 && DragData.DragObjHitGroundNum >= dragHitAndDisappearNum)
        // {
        //     this.transform.gameObject.SetActive(false);
        // }
    }

    //添加各种拖动物体功能组件
    public void AddDragCom(GameObject dragObj)
    {
       

        DragController.AddDragObjToList(dragObj);
        //抓取物体集合类
        dragObj.AddComponent<DragCom_Assemble>();
        if(bl_justClick){
              dragObj.AddComponent<DragCom_JustClick>();
        }

        if(bl_drag==true){
            dragObj.AddComponent<DragCom_Move>();
        }
        if(bl_playDropDown==true){
            dragObj.AddComponent<DragCom_DropDown>();
        }

        //显示往下的白色光柱
        if (bl_showLine == true)
        {
            dragObj.AddComponent<DragCom_Line>().showLine();
        }
        //靠近目标位置时，自动吸附过去；若有缩放需求，则根据设置进行缩放
        if (bl_absorbToDest==true&&destPosList.Length > 0)
        {
            
            dragObj.AddComponent<DragCom_AbsorbToDestPos>().SetDestPos(destPosList[dragedNum - 1]);
            if (finalScale != 0)
            {
                DragCom_Scale com_scale = dragObj.AddComponent<DragCom_Scale>();
                com_scale.SetFinalScale(finalScale).SetDestPos(destPosList[dragedNum - 1]);
            }
        }
        if(bl_flyToDest==true&&destPosList.Length>0){
            dragObj.AddComponent<DragCom_FlyToDestPos>().SetDestPos(destPosList[dragedNum - 1]).SetDestEnlerAngle(destRotationList[dragedNum-1]);
        }

        if(highY!=0){
            var com_data= dragObj.AddComponent<DragCom_Data>();
            com_data.highY=highY;
        }
        if(finalScale!=0){
            var com_data= dragObj.AddComponent<DragCom_Data>();
            com_data.finalScale=finalScale;
        }
    }
}
