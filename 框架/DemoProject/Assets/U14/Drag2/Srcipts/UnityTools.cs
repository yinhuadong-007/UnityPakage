using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityTools
{

    
        /// <summary>
        /// 寻找节点树上的某个节点
        /// </summary>
        /// <param name="name">节点名</param>
        public static Transform FindChildName(Transform ts, string name)
        {
            for (int z = 0; z < ts.childCount; z++)
            {
                Transform childTs = ts.GetChild(z);
                if (childTs.name == name)
                {
                    return childTs;
                }
                else
                {
                    var grandChildTs = FindChildName(childTs, name);
                    if (grandChildTs != null)
                    {
                        return grandChildTs;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 包含名字的物体
        /// </summary>
        /// <param name="name">节点名</param>
        public static Transform FindChildContainName(Transform ts, string name)
        {
            for (int z = 0; z < ts.childCount; z++)
            {
                Transform childTs = ts.GetChild(z);
                if (childTs.name.Contains(name))
                {
                    return childTs;
                }
                else
                {
                    var grandChildTs = FindChildName(childTs, name);
                    if (grandChildTs != null)
                    {
                        return grandChildTs;
                    }
                }
            }
            return null;
        }
    
}
