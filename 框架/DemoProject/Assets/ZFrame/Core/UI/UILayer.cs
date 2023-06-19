using System;
using System.Collections.Generic;
using UnityEngine;
namespace ZFrame
{
    public partial class UI
    {
        public enum Layer
        {
            bottom,
            game,
            top,
        }

        private static Dictionary<Layer, Transform> _dic;

        private static void layerInit()
        {
            _dic = new Dictionary<Layer, Transform>(){
            {Layer.bottom,createNewLayerNode(Layer.bottom)},
            {Layer.game,createNewLayerNode(Layer.game)},
            {Layer.top,createNewLayerNode(Layer.top)}
        };
        }

        private static Transform createNewLayerNode(Layer layer){
            var obj=new GameObject();
            obj.name=layer.ToString();
            var rts= obj.AddComponent<RectTransform>();
            rts.anchorMin=new Vector2(0,0);
            rts.anchorMax=new Vector2(1,1);
            rts.pivot=new Vector2(0.5f,0.5f);
            obj.transform.SetParent(container.transform,false);
            return obj.transform;
        }

        private static void addToLayer(Layer layer, GameObject uiObj)
        {
            uiObj.transform.SetParent(_dic[layer]);
        }
    }
}