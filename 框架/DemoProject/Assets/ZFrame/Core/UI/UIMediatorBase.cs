using System.Collections.Generic;
using System;
using UnityEngine;
namespace ZFrame
{
    abstract public class UIMediatorBase
    {
        public static Dictionary<Type, int> dic = new();
        protected GameObject viewObj;
        private string _viewName;
        private dynamic _viewData;
        public void init(dynamic viewData)
        {
            this._viewData=viewData;
            if (this.checkHasView(this.GetType()))
            {
                this.checkViewObj();
            }
        }

        protected bool checkHasView(Type type)
        {
            if (UIMediatorBase.dic.ContainsKey(type))
            {
                return false;
            }
            UIMediatorBase.dic.Add(type, 1);
            return true;
        }

        protected void checkViewObj()
        {
            string viewName = "";
            //通过特性拿到视图字符串
            foreach (var attr in Attribute.GetCustomAttributes(this.GetType()))
            {
                if (attr is UIViewNameAttribute)
                {
                    viewName = (attr as UIViewNameAttribute).viewName;
                }
            }
            this._viewName=viewName;
            this.loadRes(this.createViewObj);
        } 

        protected virtual void loadRes(Action afterLoadFunc){
            afterLoadFunc();
        }

        protected void createViewObj(){
            //创建视图实体
            this.viewObj = GameObject.Instantiate(Res.getRes<GameObject>(this._viewName), ZFrame.UI.container.transform);
            this.beginLogic();
        }

        protected void dispose()
        {
            GameObject.Destroy(viewObj);
            UIMediatorBase.dic.Remove(this.GetType());
        }

        protected abstract void beginLogic();

        public T getData<T>() where T:class{
            return this._viewData as T;
        }
    }

    public class UIViewNameAttribute : Attribute
    {
        public string viewName { get; set; }
        public UIViewNameAttribute(string name)
        {
            this.viewName = name;
        }
    }

    public class UIMediatorAttribute : Attribute
    {
        public Type mediator { get; set; }
        public UIMediatorAttribute(Type type)
        {
            this.mediator = type;
        }
    }
}