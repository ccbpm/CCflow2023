﻿using System;
using System.Collections;
using BP.DA;
using System.Reflection;

namespace BP.En
{
    /// <summary>
    /// 显示位置
    /// </summary>
    public enum RMShowWhere
    {
        /// <summary>
        /// 实例左侧
        /// </summary>
        EnLeft,
        /// <summary>
        /// 实例工具栏
        /// </summary>
        EnToolbar,
        /// <summary>
        /// 查询工具栏
        /// </summary>
        SearchToolbar
    }
    /// <summary>
    /// 相关功能类型
    /// </summary>
    public enum RefMethodType
    {
        /// <summary>
        /// 左侧功能
        /// </summary>
        Func,
        /// <summary>
        /// 模态窗口打开
        /// </summary>
        LinkModel,
        /// <summary>
        /// 新窗口打开
        /// </summary>
        LinkeWinOpen,
        /// <summary>
        /// 右侧窗口打开
        /// </summary>
        RightFrameOpen,
        /// <summary>
        /// Tab页签打开
        /// </summary>
        TabOpen
    }
    /// <summary>
    /// RefMethod 的摘要说明。
    /// </summary>
    public class RefMethod
    {
        #region 与窗口有关的方法.
        /// <summary>
        /// 高度
        /// </summary>
        public int Height = 600;
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width = 800;
        /// <summary>
        /// 目标
        /// </summary>
        public string Target = "_B123";
        #endregion

        /// <summary>
        /// 功能
        /// </summary>
        public RefMethodType RefMethodType = RefMethodType.Func;
        /// <summary>
        /// 功能显示位置
        /// </summary>
        public RMShowWhere RMShowWhere = RMShowWhere.EnLeft;
        /// <summary>
        /// 相关字段
        /// </summary>
        public string RefAttrKey = null;
        /// <summary>
        /// 连接标签
        /// </summary>
        public string RefAttrLinkLabel = null;
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName = null;
        /// <summary>
        /// 是否显示在Ens中?
        /// </summary>
        public bool ItIsForEns = false;
        /// <summary>
        /// 显示位置
        /// </summary>
        public string IsShowForEnsCondtion = null;
        /// <summary>
        /// 相关功能
        /// </summary>
        public RefMethod()
        {
        }
        /// <summary>
        /// 参数
        /// </summary>
        private Attrs _HisAttrs = null;
        /// <summary>
        /// 参数
        /// </summary>
        public Attrs HisAttrs
        {
            get
            {
                if (_HisAttrs == null)
                    _HisAttrs = new Attrs();
                return _HisAttrs;
            }
            set
            {
                _HisAttrs = value;
            }
        }
        /// <summary>
        /// 索引位置，用它区分实体.
        /// </summary>
        public int Index = 0;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Visable = true;
        /// <summary>
        /// 是否可以批处理
        /// </summary>
        public bool ItIsCanBatch = false;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title = null;
        /// <summary>
        /// 操作前提示信息
        /// </summary>
        public string Warning = "您确定要执行吗？";
        /// <summary>
        /// 连接
        /// </summary>
        public string ClassMethodName = null;
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon = null;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string ToolTip = null;
        /// <summary>
        /// PKVal
        /// </summary>
        public object PKVal = "PKVal";
        /// <summary>
        /// 
        /// </summary>
        public Entity HisEn = null;
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public object Do(object[] paras)
        {
            string str = this.ClassMethodName.Trim(' ', ';', '.');
            int pos = str.LastIndexOf(".");

            
            string clas = this.HisEn.ToString();
            string meth = str;
            if (pos > 0)
            {
                clas = str.Substring(0, pos);
                meth = str.Substring(pos, str.Length - pos).Trim('.', ' ', '(', ')');
            }

            if (this.HisEn == null)
            {
                this.HisEn = ClassFactory.GetEn(clas);
                Attrs attrs = this.HisEn.EnMap.Attrs;
            }

            //如果当前的en与方法的en不同.
            if (this.HisEn.ToString().Equals(clas) == false)
            {
                this.HisEn = ClassFactory.GetEn(clas);
                this.HisEn.PKVal = this.PKVal;
                this.HisEn.Retrieve();
            }

            Type tp = this.HisEn.GetType();
            MethodInfo mp = tp.GetMethod(meth);
            if (mp == null)
                throw new Exception("@对象实例[" + tp.FullName + "]中没有找到方法[" + meth + "]！");

            try
            {
                return mp.Invoke(this.HisEn, paras); //调用由此 MethodInfo 实例反射的方法或构造函数。
            }
            catch (System.Reflection.TargetException ex)
            {
                string strs = "";
                if (paras == null)
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    foreach (object obj in paras)
                    {
                        strs += "para= " + obj.ToString() + " type=" + obj.GetType().ToString() + "\n<br>";
                    }
                }
                throw new Exception(ex.Message + "  more info:" + strs);
            }
        }
    }
    /// <summary>
    /// 方法集合
    /// </summary>
    [Serializable]
    public class RefMethods : CollectionBase
    {
        /// <summary>
        /// 加入
        /// </summary>
        /// <param name="attr">attr</param>
        public void Add(RefMethod en)
        {
            if (this.ItIsExits(en))
                return;
            en.Index = this.InnerList.Count;
            this.InnerList.Add(en);
        }
        /// <summary>
        /// 是不是存在集合里面
        /// </summary>
        /// <param name="en">要检查的RefMethod</param>
        /// <returns>true/false</returns>
        public bool ItIsExits(RefMethod en)
        {
            foreach (RefMethod dtl in this)
            {
                if (dtl.ClassMethodName == en.ClassMethodName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 根据索引访问集合内的元素Attr。
        /// </summary>
        public RefMethod this[int index]
        {
            get
            {
                return (RefMethod)this.InnerList[index];
            }
        }
    }
}
