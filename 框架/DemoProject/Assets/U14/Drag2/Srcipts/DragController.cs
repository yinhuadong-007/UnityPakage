using System;
using System.Collections.Generic;
using UnityEngine;
public class DragController
{
    public static List<GameObject> DragObjList=new();
    public static void AddDragObjToList(GameObject dragObj){
        DragObjList.Add(dragObj);
    }

    public static GameObject GetDragObjByName(string name){
        for(int z=0;z<DragObjList.Count;z++){
            if(UnityTools.FindChildName(DragObjList[z].transform,name)!=null){
                return DragObjList[z];
            }
        }
        return null;
    }
     public static GameObject GetDragObjContainName(string name){
        for(int z=0;z<DragObjList.Count;z++){
            if(UnityTools.FindChildContainName(DragObjList[z].transform,name)!=null){
                return DragObjList[z];
            }
        }
        return null;
    }
}