using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZFrame
{
    namespace BehaviourTree
    {
        public class BTDebug{
            public static void Debug(object message){
                UnityEngine.Debug.Log(message);
            }
        }


        public delegate bool ConditionEventHandler();

        public enum ConditionType
        {
            noCondition,
            hasCondition,
        }


        public enum NodeState
        {
            next,
            back,
            running,

        }

        public struct NodeData
        {
            public NodeState nodeState;
            public BehaviourTreeBaseNode node;
        }

        public class BehaviourTreeSingle
        {


            private BehaviourTreeBaseNode currNode;
            private BehaviourTreeBaseNode rootNode;
            private Dictionary<int, ConditionEventHandler> _dic_conditionFunc = new();

            private float loopInterval = 0;


            public void addRootNode(BehaviourTreeBaseNode rootNode)
            {
                this.rootNode = rootNode;
                this.currNode = rootNode;
            }

            public NodeState getCurrState()
            {
                return currNode.nodeState;
            }
            public void setConditionFunc(ConditionEventHandler func, int conditionEnum)
            {
                _dic_conditionFunc.Add(conditionEnum, func);
            }
            public void conditionRegisterToNode(BehaviourTreeBaseNode node = null)
            {
                if (node == null)
                {
                    node = rootNode;
                }
                if (node.childrenNodes == null)
                {
                    return;
                }
                foreach (var child in node.childrenNodes)
                {
                    if (_dic_conditionFunc.ContainsKey(child.conditionEnum))
                    {
                        child.setConditionFunc(_dic_conditionFunc[child.conditionEnum]);
                    }
                    conditionRegisterToNode(child);
                }
            }

            public BehaviourTreeBaseNode doLoop()
            {
                currNode = currNode.check();
                //treeEnd
                if (currNode == null)
                {
                   BTDebug.Debug("tree loop end");
                    this.currNode = this.rootNode;
                    this.rootNode.loopIdx++;
                     BTDebug.Debug("this.rootNode.loopIdx:" + this.rootNode.loopIdx);
                }
                //tree task running;
                else if (currNode.nodeState == NodeState.running)
                {
                     BTDebug.Debug(currNode.nodeName + " running");
                    MonoSubstitute.instance.StartCoroutine(timeElapsed());
                    return currNode;
                }

                return doLoop();
            }



            private Coroutine _beginCoro;
            public void begin(float loopInterval)
            {
                this.loopInterval = loopInterval;
                if (_beginCoro != null)
                {
                    MonoSubstitute.instance.StopCoroutine(_beginCoro);
                }
                _beginCoro = MonoSubstitute.instance.StartCoroutine(timeIntervalLoop());
            }
            IEnumerator timeIntervalLoop()
            {
                doLoop();
                yield return new WaitForSeconds(loopInterval);
                MonoSubstitute.instance.StartCoroutine(timeIntervalLoop());

            }
            IEnumerator timeElapsed()
            {
                yield return new WaitForSeconds((currNode as TaskNode).duration);
                currNode.nodeState = NodeState.back;
                //任务完成后立马刷新
                this.begin(loopInterval);
            }
        }


        abstract public class BehaviourTreeBaseNode
        {
            public string nodeName = "noNameNode";
            public int loopIdx = 0;

            public BehaviourTreeBaseNode parentNode;
            public BehaviourTreeBaseNode[] childrenNodes;
            public NodeState nodeState = NodeState.next;
            public ConditionType conditionType = ConditionType.noCondition;

            public int conditionEnum = -999;

            private ConditionEventHandler _conditionFunc;


            protected int currIdx;
            public BehaviourTreeBaseNode(string nodeName)
            {
                this.nodeName = nodeName;
            }

            public BehaviourTreeBaseNode addNode(params BehaviourTreeBaseNode[] nodesList)
            {
                childrenNodes = nodesList;
                foreach (var node in nodesList)
                {
                    node.parentNode = this;
                }
                return this;
            }

            // public void checkLoopIdx(int currIdx){
            //     if(this.loopIdx)
            // }

            public BehaviourTreeBaseNode check()
            {
                //根节点了,无父节点
                if (this.parentNode == null)
                {
                    // nodeState = NodeState.wait;
                    nodeState = NodeState.next;
                }
                //重新循环，重设初始节点状态，默认为next
                else if (this.loopIdx != this.parentNode.loopIdx)
                {
                    this.loopIdx = this.parentNode.loopIdx;
                    // nodeState = NodeState.wait;
                    nodeState = NodeState.next;
                }
                //有条件的节点进行判断
                // if (nodeState == NodeState.wait && conditionType != ConditionType.noCondition)
                if (conditionType != ConditionType.noCondition)
                {
                    bool conditionPass = _conditionFunc();
                    nodeState = conditionPass == true ? NodeState.next : NodeState.back;
                }
                if (nodeState == NodeState.back)
                {
                     BTDebug.Debug(parentNode.nodeName + "<====" + nodeName);
                    currIdx = 0;
                    return parentNode;
                }
                 BTDebug.Debug("====>" + nodeName);
                return this.BehaviourCheck();
            }

            public BehaviourTreeBaseNode nextChild()
            {
                if (currIdx > childrenNodes.Length - 1)
                {
                    currIdx = 0;
                    this.nodeState = NodeState.back;
                    if (parentNode != null)
                    {
                        BTDebug.Debug(parentNode.nodeName + "<====" + nodeName);
                    }
                    return this.parentNode;
                }

                BehaviourTreeBaseNode childNode = childrenNodes[currIdx].check();
                currIdx++;

                if (childNode.nodeState == NodeState.back)
                {
                    return nextChild();
                }

                return childNode;
            }


            public BehaviourTreeBaseNode setCondition(int conditionEnum) 
            {
                this.conditionType = ConditionType.hasCondition;
                this.conditionEnum = conditionEnum;
                return this;
            }

            public void setConditionFunc(ConditionEventHandler func)
            {
                _conditionFunc = func;
            }

            abstract public BehaviourTreeBaseNode BehaviourCheck();
        }
        public class RootNode : BehaviourTreeBaseNode
        {
            public RootNode(string nodeName) : base(nodeName)
            {
                // this.nodeState = NodeState.next;
                this.conditionType = ConditionType.noCondition;
            }

            public override BehaviourTreeBaseNode BehaviourCheck()
            {
                return nextChild();
            }
        }

        public class CompositeNode : BehaviourTreeBaseNode
        {
            public CompositeNode(string nodeName) : base(nodeName)
            {

            }
            public override BehaviourTreeBaseNode BehaviourCheck()
            {
                return this;
            }
        }

        public class SelectNode : CompositeNode
        {
            public SelectNode(string nodeName) : base(nodeName)
            {
            }
            public override BehaviourTreeBaseNode BehaviourCheck()
            {
                return nextChild();
            }
        }

        public class TaskNode : BehaviourTreeBaseNode
        {
            public TaskNode(string nodeName) : base(nodeName)
            {

            }
            public override BehaviourTreeBaseNode BehaviourCheck()
            {
                nodeState = NodeState.running;
                // MonoSubstitute.instance.StartCoroutine(timeElapsed());
                return this;
            }
            //设定执行时间
            public float duration;
            public BehaviourTreeBaseNode setDuration(float duration)
            {
                this.duration = duration;
                return this;
            }

            // IEnumerator timeElapsed(){
            //     yield return new WaitForSeconds(duration);
            //     nodeState=NodeState.back;
            // }
        }
    }
}