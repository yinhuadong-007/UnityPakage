
using UnityEngine;
namespace ZFrame
{
    namespace BehaviourTree
    {
        public enum ConditionEnum{
            foundOpponent,
            MoveToTest,
        }

        public class BehaviourTreeDefine
        {
            public string bt1 = "bt1";

            public BehaviourTreeSingle createBT1()
            {
                var bt = new BehaviourTreeSingle();
                bt.addRootNode(new RootNode("bt1RootNode")
                .addNode(new SelectNode("FoundOpponentSelectNode").setCondition((int)ZFrame.BehaviourTree.ConditionEnum.foundOpponent)
                                .addNode(new TaskNode("MoveTo").setDuration(2).setCondition((int)ZFrame.BehaviourTree.ConditionEnum.MoveToTest)
                                       , new TaskNode("RunTo").setDuration(2))
                         ,new SelectNode("DefaultNode")
                                .addNode(new TaskNode("Idle").setDuration(2))));

                return bt;
            }
        }
    }
}