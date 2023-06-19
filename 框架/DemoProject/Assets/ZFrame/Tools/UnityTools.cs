using UnityEngine;
namespace ZFrame
{


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
        /// <summary>
        /// 计算旋转缩放后的顶点位置
        /// </summary>
        /// <returns></returns>
        public static Vector3 CalcRotatedVertexPos(Transform ts,Vector3 vertexPos){
              Quaternion r = ts.rotation;
        Vector3 s = ts.lossyScale;
        Matrix4x4 matrix = Matrix4x4.Rotate(r) * Matrix4x4.Scale(s);
        return matrix.MultiplyPoint(vertexPos)+ts.position;
        }
    }
}