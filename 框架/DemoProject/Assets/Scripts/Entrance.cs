using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        new ZFrame.Entrance();

        ZFrame.Res.loadAddressableRes("default",loadedCompleted);

       
    }

    void loadedCompleted(){   
        // ZFrame.EventSystem.GetRegisteEventFunc();

        ZFrame.EventSystem.AddEvent(this);
       

         ZFrame.Res.createObj(ResName.prefab_Prefab_Cube_aaa);     

         ZFrame.EventSystem.goEvent(EventName.u14testEvent2,1,"abc");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ZFrame.EventReceiveAttribute("u14testEvent")]
    public void testEvent(int a,string b){
        Debug.Log("testEvent"+a+b);
    }

     [ZFrame.EventReceiveAttribute("u14testEvent2")]
    public void testEvent2(int a,string b){
        Debug.Log("testEvent2"+a+b);
    }
}
