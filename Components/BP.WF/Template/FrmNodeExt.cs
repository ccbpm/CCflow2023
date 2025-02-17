﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.WF.Template.SFlow;


namespace BP.WF.Template
{
    /// <summary>
    /// 节点表单
    /// 节点的工作节点有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class FrmNodeExt : EntityMyPK
    {
        #region 属性.
        public string FK_Frm
        {
            get
            {
                return this.GetValStrByKey(FrmNodeAttr.FK_Frm);
            }
        }
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FK_Node);
            }
        }
        /// <summary>
        /// @李国文 
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Flow);
            }
        }

        /// <summary>
        /// 是否启用节点组件?
        /// </summary>
        public FrmWorkCheckSta IsEnableFWC
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(FrmNodeAttr.IsEnableFWC);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsEnableFWC, (int)value);
            }
        }

        /// <summary>
        /// 签批字段
        /// </summary>
        public string CheckField
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.CheckField);
            }
        }

        public FrmSubFlowSta SFSta
        {
            get
            {
                return (FrmSubFlowSta)this.GetValIntByKey(FrmSubFlowAttr.SFSta);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFSta, (int)value);
            }
        }
        #endregion

        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                //权限控制.
                if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                    uac.OpenForSysAdmin();
                else
                    uac.OpenAll();

                uac.IsInsert = false;

                return uac;
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodeExt() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="mypk"></param>
        public FrmNodeExt(string mypk)
        {
            this.setMyPK(mypk);
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

                Map map = new Map("WF_FrmNode", "节点表单");

                #region 基本信息.
                map.AddGroupAttr("基本信息");
                map.AddMyPK();
                //map.AddTBString(FrmNodeAttr.FK_Frm, null, "表单", true, true, 0, 300, 20);
                map.AddDDLEntities(FrmNodeAttr.FK_Frm, null, "表单", new MapDatas(), false);
                map.AddTBInt(FrmNodeAttr.FK_Node, 0, "节点ID", true, true);

                map.AddBoolean(FrmNodeAttr.IsPrint, false, "是否可以打印", true, true);
                map.AddBoolean(FrmNodeAttr.IsEnableLoadData, false, "是否启用装载填充事件", true, true);

                map.AddBoolean(FrmNodeAttr.IsCloseEtcFrm, false, "打开时是否关闭其它的页面？", true, true, true);
                map.SetHelperAlert(FrmNodeAttr.IsCloseEtcFrm, "默认为不关闭,当该表单以tab标签也打开时,是否关闭其它的tab页?");

                map.AddDDLSysEnum(FrmNodeAttr.WhoIsPK, 0, "谁是主键?", true, true);
                map.SetHelperAlert(FrmNodeAttr.WhoIsPK, "用来控制谁是表单事例的主键的方案，对于父子流程如果子流程需要在看到父流程的表单，就需要设置ParentID是主键。");

                map.AddDDLSysEnum(FrmNodeAttr.FrmSln, 0, "控制方案", true, true, FrmNodeAttr.FrmSln,
                    "@0=默认方案@1=只读方案@2=自定义方案");
                map.SetHelperAlert(FrmNodeAttr.FrmSln, "控制该表单数据元素权限的方案，如果是自定义方案，就要设置每个表单元素的权限.");

                //map.AddBoolean(FrmNodeAttr.IsEnableFWC, false, "是否启用审核组件？", true, true, true);

                //单据编号对应字段
                map.AddDDLSQL(NodeWorkCheckAttr.BillNoField, null, "单据编号对应字段",Glo.SQLOfBillNo, true);


                map.AddTBString(FrmNodeAttr.FrmNameShow, null, "表单显示名字", true, false, 0, 100, 20);
                map.SetHelperAlert(FrmNodeAttr.FrmNameShow, "显示在表单树上的名字,默认为空,表示与表单的实际名字相同.多用于节点表单的名字在表单树上显示.");

                //map.AddDDLSysEnum(BP.WF.Template.FrmWorkCheckAttr.FWCSta, 0, "审核组件(是否启用审核组件？)", true, true);

                //add 2016.3.25.
                map.AddBoolean(FrmNodeAttr.Is1ToN, false, "是否1变N？(分流节点有效)", true, true, true);
                map.AddTBString(FrmNodeAttr.HuiZong, null, "汇总的数据表名", true, false, 0, 300, 20);
                map.SetHelperAlert(FrmNodeAttr.HuiZong, "子线程要汇总的数据表，对当前节点是子线程节点有效。");

                //模版文件，对于office表单有效.
                map.AddTBString(FrmNodeAttr.TempleteFile, null, "模版文件", true, false, 0, 500, 20);

                //是否显示.
                //  map.AddTBString(FrmNodeAttr.GuanJianZiDuan, null, "关键字段", true, false, 0, 20, 20);

                //显示的
                map.AddTBInt(FrmNodeAttr.Idx, 0, "顺序号", true, false);
                map.SetHelperAlert(FrmNodeAttr.Idx, "在表单树上显示的顺序,可以通过列表调整.");

                #endregion 基本信息.

                #region 组件属性.
                map.AddBoolean(FrmNodeAttr.IsEnableFWC, false, "是否启用审核组件?", true, true, true);
                map.SetHelperAlert(FrmNodeAttr.IsEnableFWC, "控制该表单是否启用审核组件？如果启用了就显示在该表单上;");
                map.AddBoolean(FrmNodeAttr.IsEnableSF, false, "是否启用父子流程组件?", true, true, true);
                //签批字段
                map.AddDDLSQL(NodeWorkCheckAttr.CheckField, null, "签批字段",Glo.SQLOfCheckField, true);

                #endregion

                #region 隐藏字段.
                //@0=始终启用@1=有数据时启用@2=有参数时启用@3=按表单的字段表达式@4=按SQL表达式@5=不启用@6=按角色@7=按部门.
                map.AddTBInt(FrmNodeAttr.FrmEnableRole, 0, "启用规则", false, false);
                map.AddTBString(FrmNodeAttr.FrmEnableExp, null, "启用的表达式", true, false, 0, 900, 20);
                map.AddTBString(FrmNodeAttr.FK_Flow, null, "流程编号", false, false, 0, 4, 20);
                #endregion 隐藏字段.

                #region 相关功能..
                map.AddGroupMethod("基本功能");
                RefMethod rm = new RefMethod();
                rm.Title = "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDFrm()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-settings"; //正则表达式
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "启用规则";
                rm.ClassMethodName = this.ToString() + ".DoEnableRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-settings"; //正则表达式
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "表单启用规则";
                //rm.ClassMethodName = this.ToString() + ".DoFrmEnableRole()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "审核组件设置";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmNodeWorkCheck";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // rm.GroupName = "表单组件";
                rm.Icon = "icon-settings"; //正则表达式
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "表单列表";
                //rm.ClassMethodName = this.ToString() + ".DoBindFrms()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批量设置";
                rm.ClassMethodName = this.ToString() + ".DoBatchSetting()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-settings"; //正则表达式
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "改变表单类型";
                rm.ClassMethodName = this.ToString() + ".DoChangeFrmType()";
                rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "修改表单类型", true, true);
                rm.Icon = "icon-settings"; //正则表达式
                map.AddRefMethod(rm);
                #endregion 基本设置.

                #region 表单元素权限.
                map.AddGroupMethod("表单元素权限");
                rm = new RefMethod();
                rm.Title = "字段权限";
                rm.ClassMethodName = this.ToString() + ".DoFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "从表权限";
                rm.ClassMethodName = this.ToString() + ".DoDtls()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "附件权限";
                rm.ClassMethodName = this.ToString() + ".DoAths()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "图片附件权限";
                rm.ClassMethodName = this.ToString() + ".DoImgAths()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "从其他节点Copy权限设置";
                rm.ClassMethodName = this.ToString() + ".DoCopyFromNode()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-settings"; //正则表达式
                map.AddRefMethod(rm);
                #endregion 表单元素权限.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        public string DoBatchSetting()
        {
            //return "../../Admin/Sln/BindFrms.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo;
           
            return "../../Admin/AttrNode/FrmSln/BatchEditSln.htm?NodeID=" + this.NodeID + "&MyPK=" + this.MyPK+"&FrmID="+this.FK_Frm;

        }
        /// <summary>
        /// 设计表单
        /// </summary>
        /// <returns></returns>
        public string DoDFrm()
        {
            return "../../Admin/Sln/BindFrms.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo;
        }
        /// <summary>
        /// 打开绑定表单
        /// </summary>
        /// <returns></returns>
        public string DoBindFrms()
        {
            return "../../Admin/Sln/BindFrms.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo;
        }

        /// <summary>
        /// 审核组件
        /// </summary>
        /// <returns></returns>
        public string DoFrmNodeWorkCheck()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmWorkCheck&PKVal=" + this.NodeID + "&CheckField=" + this.CheckField + "&FK_Frm=" + this.FK_Frm + "&t=" + DataType.CurrentDateTime;
        }

        /// <summary>
        /// 改变表单类型
        /// </summary>
        /// <param name="val">要改变的类型</param>
        /// <returns></returns>
        public string DoChangeFrmType(int val)
        {
            MapData md = new MapData(this.FK_Frm);
            string str = "原来的是:" + md.HisFrmTypeText + "类型，";
            md.HisFrmTypeInt = val;
            str += "现在修改为：" + md.HisFrmTypeText + "类型";
            md.Update();

            return str;
        }

        protected override void afterInsertUpdateAction()
        {
            Node node = new Node();
            node.NodeID = this.NodeID;
            int i = node.RetrieveFromDBSources();
            if (i != 0 && (node.HisFormType == NodeFormType.RefOneFrmTree
            || node.HisFormType == NodeFormType.FoolTruck))
            {
                node.FrmWorkCheckSta = this.IsEnableFWC;
                node.SetValByKey("CheckField", this.CheckField);
                node.Update();
            }
            FrmSubFlow frmSubFlow = new FrmSubFlow(this.NodeID);
            frmSubFlow.SFSta = this.SFSta;
            base.afterInsertUpdateAction();
        }

        #region 表单元素权限.
        public string DoDtls()
        {
            return "../../Admin/Sln/Dtls.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo + "&DoType=Field";
        }
        public string DoFields()
        {
            return "../../Admin/Sln/Fields.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo + "&DoType=Field";
        }

        public string DoComponents()
        {
            return "../../Admin/Sln/Components.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo + "&DoType=Field";
        }
        public string DoAths()
        {
            return "../../Admin/Sln/Aths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo + "&DoType=Field";
        }

        public string DoImgAths()
        {
            return "../../Admin/Sln/ImgAths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo + "&DoType=Field";
        }

        public string DoCopyFromNode()
        {
            return "../../Admin/Sln/Aths.htm?FK_MapData=" + this.FK_Frm + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FlowNo + "&DoType=Field";
        }
        public string DoEnableRole()
        {
            return "../../Admin/AttrNode/BindFrmsNodeEnableRole.htm?MyPK=" + this.MyPK;
        }
        #endregion 表单元素权限.

    }
    /// <summary>
    /// 节点表单s
    /// </summary>
    public class FrmNodeExts : EntitiesMyPK
    {
        #region 构造方法..
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodeExts() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public FrmNodeExts(string fk_flow, int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Flow, fk_flow);
            qo.addAnd();
            qo.AddWhere(FrmNodeAttr.FK_Node, nodeID);

            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();
        }
        #endregion 构造方法..

        #region 公共方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmNodeExt();
            }
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmNodeExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmNodeExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmNodeExt> Tolist()
        {
            System.Collections.Generic.List<FrmNodeExt> list = new System.Collections.Generic.List<FrmNodeExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmNodeExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
