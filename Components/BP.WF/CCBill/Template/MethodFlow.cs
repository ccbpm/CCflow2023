﻿using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.CCBill.Template
{
    /// <summary>
    /// 执行流程
    /// </summary>
    public class MethodFlow : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.FrmID);
            }
            set
            {
                this.SetValByKey(MethodAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 方法ID
        /// </summary>
        public string MethodID
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MethodID);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodID, value);
            }
        }
       
        public string MsgErr
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MsgErr);
            }
            set
            {
                this.SetValByKey(MethodAttr.MsgErr, value);
            }
        }
        public string MsgSuccess
        {
            get
            {
                return this.GetValStringByKey(MethodAttr.MsgSuccess);
            }
            set
            {
                this.SetValByKey(MethodAttr.MsgSuccess, value);
            }
        }


        public string MethodDoc_Url
        {
            get
            {
                string s = this.GetValStringByKey(MethodAttr.MethodDoc_Url);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "http://192.168.0.100/MyPath/xxx.xx";
                return s;
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodDoc_Url, value);
            }
        }
        /// <summary>
        /// 获得或者设置sql脚本.
        /// </summary>
        public string MethodDoc_SQL
        {
            get
            {
                string strs = this.GetBigTextFromDB("SQLScript");
                if (DataType.IsNullOrEmpty(strs) == true)
                    return this.MethodDoc_SQL_Demo; //返回默认信息.
                return strs;
            }
            set
            {
                this.SaveBigTxtToDB("SQLScript", value);
            }
        }
        /// <summary>
        /// 获得该实体的demo.
        /// </summary>
        public string MethodDoc_JavaScript_Demo
        {
            get
            {
                string file =  BP.Difference.SystemConfig.CCFlowAppPath + "WF/CCBill/Admin/MethodDoc/MethodDocDemoJS.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                doc = doc.Replace("/#", "+"); //为什么？
                doc = doc.Replace("/$", "-"); //为什么？

                doc = doc.Replace("@FrmID", this.FrmID);

                return doc;
            }
        }
        public string MethodDoc_SQL_Demo
        {
            get
            {
                string file =  BP.Difference.SystemConfig.CCFlowAppPath + "WF/CCBill/Admin/MethodDoc/MethodDocDemoSQL.txt";
                string doc = DataType.ReadTextFile(file); //读取文件.
                doc = doc.Replace("@FrmID", this.FrmID);
                return doc;
            }
        }
        /// <summary>
        /// 获得JS脚本.
        /// </summary>
        /// <returns></returns>
        public string Gener_MethodDoc_JavaScript()
        {
            return this.MethodDoc_JavaScript;
        }

        public string Gener_MethodDoc_JavaScript_function()
        {
            string paras = "";
            MapAttrs attrs = new MapAttrs(this.No);
            foreach (MapAttr item in attrs)
            {
                paras += item.KeyOfEn + ",";
            }
            if (attrs.Count > 1)
                paras = paras.Substring(0, paras.Length - 1);

            string strs = " function " + this.MethodID + "(" + paras + ") {";
            strs += this.MethodDoc_JavaScript;
            strs += "}";
            return strs;
        }
        /// <summary>
        /// 获得SQL脚本
        /// </summary>
        /// <returns></returns>
        public string Gener_MethodDoc_SQL()
        {
            return this.MethodDoc_SQL;
        }
        /// <summary>
        /// 获得或者设置js脚本.
        /// </summary>
        public string MethodDoc_JavaScript
        {
            get
            {
                string strs = this.GetBigTextFromDB("JSScript");
                if (DataType.IsNullOrEmpty(strs) == true)
                    return this.MethodDoc_JavaScript_Demo;

                strs = strs.Replace("/#", "+");
                strs = strs.Replace("/$", "-");
                return strs;
            }
            set
            {

                this.SaveBigTxtToDB("JSScript", value);

            }
        }

        /// <summary>
        /// 方法类型：@0=SQL@1=URL@2=JavaScript@3=业务单元
        /// </summary>
        public int MethodDocTypeOfFunc
        {
            get
            {
                return this.GetValIntByKey(MethodAttr.MethodDocTypeOfFunc);
            }
            set
            {
                this.SetValByKey(MethodAttr.MethodDocTypeOfFunc, value);
            }
        }
        /// <summary>
        /// 方法类型
        /// </summary>
        public RefMethodType RefMethodType
        {
            get
            {
                return (RefMethodType)this.GetValIntByKey(MethodAttr.RefMethodType);
            }
            set
            {
                this.SetValByKey(MethodAttr.RefMethodType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 执行流程
        /// </summary>
        public MethodFlow()
        {
        }
        public MethodFlow(string mypk)
        {
            this.No = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_Method", "功能方法");


                //主键.
                map.AddTBStringPK(MethodAttr.No, null, "编号", true, true, 0, 50, 10);
                map.AddTBString(MethodAttr.Name, null, "方法名", true, false, 0, 300, 10);
                map.AddTBString(MethodAttr.MethodID, null, "方法ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.GroupID, null, "分组ID", true, true, 0, 50, 10);

                //功能标记.
                map.AddTBString(MethodAttr.MethodModel, null, "方法模式", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Tag1, null, "Tag1", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.Mark, null, "Mark", true, true, 0, 300, 10);


                map.AddTBString(MethodAttr.FrmID, null, "表单ID", true, true, 0, 300, 10);
                map.AddTBString(MethodAttr.FlowNo, null, "流程编号", true, true, 0, 10, 10);


                map.AddTBString(MethodAttr.Icon, null, "图标", true, false, 0, 50, 10, true);

                //  map.AddTBString(MethodAttr.WarningMsg, null, "执行流程警告信息", true, false, 0, 300, 10, true);
                map.AddDDLSysEnum(MethodAttr.ShowModel, 0, "显示方式", true, true, MethodAttr.ShowModel,
                  "@0=按钮@1=超链接");

                // map.AddDDLSysEnum(MethodAttr.MethodDocTypeOfFunc, 0, "内容类型", true, false, "MethodDocTypeOfFunc",
                //"@0=SQL@1=URL@2=JavaScript@3=业务单元");
                // map.AddTBString(MethodAttr.MethodDoc_Url, null, "URL执行内容", false, false, 0, 300, 10);
                //   map.AddTBString(MethodAttr.MsgSuccess, null, "成功提示信息", true, false, 0, 300, 10, true);
                //    map.AddTBString(MethodAttr.MsgErr, null, "失败提示信息", true, false, 0, 300, 10, true);

                #region 外观.
                map.AddTBInt(MethodAttr.PopHeight, 100, "弹窗高度", true, false);
                map.AddTBInt(MethodAttr.PopWidth, 260, "弹窗宽度", true, false);
                #endregion 外观.

                #region 显示位置控制.
                map.AddBoolean(MethodAttr.IsMyBillToolBar, true, "是否显示在MyBill.htm工具栏上", true, true, true);
                map.AddBoolean(MethodAttr.IsMyBillToolExt, false, "是否显示在MyBill.htm工具栏右边的更多按钮里", true, true, true);
                map.AddBoolean(MethodAttr.IsSearchBar, false, "是否显示在Search.htm工具栏上(用于批处理)", true, true, true);
                #endregion 显示位置控制.

                #region 相同字段数据同步方式.
                map.AddDDLSysEnum(MethodAttr.DTSDataWay, 0, "同步相同字段数据方式", true, true, MethodAttr.DTSDataWay,
               "@0=不同步@1=同步全部的相同字段的数据@2=同步指定字段的数据");

                map.AddTBString(MethodAttr.DTSSpecFiels, null, "要同步的字段", true, false, 0, 300, 10,true);

                map.AddBoolean(MethodAttr.DTSWhenFlowOver, false, "流程结束后同步？", true, true, true);
                map.AddBoolean(MethodAttr.DTSWhenNodeOver, false, "节点发送成功后同步？", true, true, true);
                #endregion 相同字段数据同步方式.


                RefMethod rm = new RefMethod();
                rm.Title = "设计流程"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAlert";
                rm.Warning = "";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.Func;
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重新导入实体字段"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".ReSetFrm";
                rm.Warning = "现有的表单字段将会被清除，重新导入的字段会被增加上去，数据不会变化，导入需慎重。";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.Func;
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        /// <summary>
        /// 方法参数
        /// </summary>
        /// <returns></returns>
        public string DoAlert()
        {
            return "您需要转入流程设计器去设计流程.";
            // return "../../CCBill/Admin/MethodParas.htm?No=" + this.No;
        }
        /// <summary>
        /// 重新导入实体字段
        /// </summary>
        /// <returns></returns>
        public string ReSetFrm()
        {
            //如果是发起流程的方法，就要表单的字段复制到，流程的表单上去.
            BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp handler = new BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_ImpExp();
            //   handler.AddPara
            handler.Imp_CopyFrm("ND" + int.Parse(this.MethodID + "01"), this.FrmID);
            return "执行成功，您需要转入流程设计器查看表单.";

        }
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }
        #endregion 执行方法.

    }
    /// <summary>
    /// 执行流程
    /// </summary>
    public class MethodFlows : EntitiesNoName
    {
        /// <summary>
        /// 执行流程
        /// </summary>
        public MethodFlows() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MethodFlow();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MethodFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<MethodFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MethodFlow> Tolist()
        {
            System.Collections.Generic.List<MethodFlow> list = new System.Collections.Generic.List<MethodFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MethodFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
