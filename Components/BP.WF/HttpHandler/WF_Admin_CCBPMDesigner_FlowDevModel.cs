﻿using System;
using BP.DA;
using BP.Web;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_CCBPMDesigner_FlowDevModel : BP.WF.HttpHandler.DirectoryPageBase
    {
        #region 变量.
        /// <summary>
        /// 类别
        /// </summary>
        public string SortNo
        {
            get
            {
                return this.GetRequestVal("SortNo");
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetRequestVal("FlowName");
            }
        }
        public FlowDevModel FlowDevModel
        {
            get
            {
                return (FlowDevModel)this.GetRequestValInt("FlowDevModel");
            }
        }
        #endregion 变量.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_CCBPMDesigner_FlowDevModel()
        {
        }
        /// <summary>
        /// 获取默认的开发模式.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            string sql = "SELECT val FROM Sys_GloVar WHERE No='FlowDevModel_" + BP.Web.WebUser.No + "'";
            int val = DBAccess.RunSQLReturnValInt(sql, 1);
            return val.ToString();
        }
        public string FlowDevModel_Save()
        {
            string SortNo = GetRequestVal("SortNo");
            string FlowName = GetRequestVal("FlowName");
            string url = GetRequestVal("Url");
            string frmURL = GetRequestVal("FrmUrl");
            string frmID = GetRequestVal("FrmID");
            string frmPK = GetRequestVal("FrmPK"); //自定义表单主键.

            if (DataType.IsNullOrEmpty(frmURL) == true)
                frmURL = frmID;
            //执行创建流程模版.
            string flowNo = BP.WF.Template.TemplateGlo.NewFlowTemplate(SortNo, FlowName, DataStoreModel.ByCCFlow, null, null);
            Flow fl = new Flow(flowNo);
            fl.FlowDevModel = this.FlowDevModel; //流程开发模式.
            if (this.FlowDevModel == FlowDevModel.JiJian)
                frmURL = "ND" + int.Parse(flowNo + "01");

            fl.SetPara("FrmPK", frmPK); //自定义主键模式的表单.
            fl.FrmUrl = frmURL;
            fl.Update();
            //发起测试人为当前登录人No
            DBAccess.RunSQL("UPDATE WF_Flow SET Tester = '" + BP.Web.WebUser.No + "' WHERE No='" + flowNo + "'");

            //设置极简类型的表单信息.
            if (this.FlowDevModel == FlowDevModel.JiJian)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.NodeFrmID = "ND" + int.Parse(fl.No) + "01";
                    if (nd.ItIsStartNode == false)
                    {
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;

                        FrmNode fn = new FrmNode();
                        fn.FK_Frm = nd.NodeFrmID;
                        fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                        fn.NodeID = nd.NodeID;
                        fn.FlowNo = flowNo;
                        fn.FrmSln = FrmSln.Readonly;
                        fn.setMyPK(fn.FK_Frm + "_" + fn.NodeID + "_" + fn.FlowNo);
                        //执行保存.
                        fn.Save();
                    }
                    nd.DirectUpdate();
                }
            }

            //设置累加类型的表单信息.
            if (this.FlowDevModel == FlowDevModel.FoolTruck)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    //表单方案的保存
                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = nd.NodeFrmID;
                    //fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                    fn.NodeID = nd.NodeID;
                    fn.FlowNo = flowNo;
                    fn.FrmSln = FrmSln.Readonly;
                    fn.setMyPK(fn.FK_Frm + "_" + fn.NodeID + "_" + fn.FlowNo);
                    //执行保存.
                    fn.Save();
                    nd.HisFormType = NodeFormType.FoolTruck;
                    nd.DirectUpdate();

                }
            }
            //设置绑定表单库的表单信息.
            if (this.FlowDevModel == FlowDevModel.RefOneFrmTree)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.NodeFrmID = fl.FrmUrl;
                    if (nd.ItIsStartNode == true)
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Disable;
                    else
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;
                    nd.HisFormType = NodeFormType.RefOneFrmTree;
                    nd.DirectUpdate();

                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = nd.NodeFrmID;
                    if (nd.ItIsStartNode == true)
                    {
                        fn.IsEnableFWC = FrmWorkCheckSta.Disable;
                        fn.FrmSln = FrmSln.Default;
                    }
                    else
                    {
                        fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                        fn.FrmSln = FrmSln.Readonly;
                    }

                    fn.NodeID = nd.NodeID;
                    fn.FlowNo = flowNo;
                    fn.FrmSln = FrmSln.Readonly;
                    fn.setMyPK(fn.FK_Frm + "_" + fn.NodeID + "_" + fn.FlowNo);
                    //执行保存.
                    fn.Save();
                }
            }

            //绑定表单库的表单,现在绑定了一个表单
            if (this.FlowDevModel == FlowDevModel.FrmTree)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No, null);
                foreach (Node nd in nds)
                {
                    //nd.setNodeFrmID(fl.getFrmUrl());
                    if (nd.ItIsStartNode == true)
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Disable;
                    else
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;
                    nd.HisFormType = NodeFormType.SheetTree;
                    nd.DirectUpdate();
                    FrmNode fn = new FrmNode();
                    string[] frmIDs = fl.FrmUrl.Split(',');

                    foreach (string str in frmIDs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        fn.FK_Frm = str;
                        fn.FrmNameShow = DBAccess.RunSQLReturnString("SELECT Name FROM Sys_MapData WHERE No='" + str + "'");
                        if (nd.ItIsStartNode == true)
                        {
                            fn.IsEnableFWC = FrmWorkCheckSta.Disable;
                            fn.FrmSln = FrmSln.Default;
                        }
                        else
                        {
                            fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                            fn.FrmSln = FrmSln.Readonly;
                        }

                        fn.NodeID = nd.NodeID;
                        fn.FlowNo = flowNo;
                        fn.FrmSln = FrmSln.Readonly;
                        fn.setMyPK(fn.FK_Frm + "_" + fn.NodeID + "_" + fn.FlowNo);
                        fn.Save(); //执行保存.
                    }
                }
            }
            if (this.FlowDevModel == FlowDevModel.SDKFrmSelfPK || this.FlowDevModel == FlowDevModel.SDKFrmWorkID)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.HisFormType = NodeFormType.SDKForm;
                    nd.FormUrl = fl.FrmUrl;
                    nd.DirectUpdate();
                }
            }
            if (this.FlowDevModel == FlowDevModel.SelfFrm)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.HisFormType = NodeFormType.SDKForm;
                    nd.FormUrl = fl.FrmUrl;
                    nd.DirectUpdate();
                }
            }

            ///保存模式.
            SaveModel(this.FlowDevModel);

            //返回流程编号
            return flowNo;
        }
        /// <summary>
        /// 保存模式
        /// </summary>
        /// <param name="val"></param>
        public void SaveModel(FlowDevModel val)
        {
            string pk = "FlowDevModel_" + WebUser.No;

            string sql = "SELECT Val FROM Sys_GloVar WHERE No='" + pk + "'";
            int valInt = DBAccess.RunSQLReturnValInt(sql, 1);
            if (valInt == (int)val)
                return;

            sql = "UPDATE Sys_GloVar SET Val=" + (int)val + " WHERE No='" + pk + "'";
            int myval = DBAccess.RunSQL(sql);
            if (myval == 1)
                return;

            sql = "INSERT INTO Sys_GloVar (No,Name,Val) VALUES('" + pk + "','FlowDevModel','" + (int)val + "')";
            DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 创建流程-早期版本模式
        /// </summary>
        /// <returns></returns>
        public string Default_NewFlowMode_0()
        {
            try
            {
                int runModel = this.GetRequestValInt("RunModel");
                string FlowName = this.GetRequestVal("FlowName");
                string FlowSort = this.GetRequestVal("FlowSort").Trim();
                FlowSort = FlowSort.Trim();

                int dataStoreModel = this.GetRequestValInt("DataStoreModel");
                string PTable = this.GetRequestVal("PTable");
                string FlowMark = this.GetRequestVal("FlowMark");
                int FlowFrmModel = this.GetRequestValInt("FlowFrmModel");
                string FrmUrl = this.GetRequestVal("FrmUrl");
                string FlowVersion = this.GetRequestVal("FlowVersion");

                string flowNo = BP.WF.Template.TemplateGlo.NewFlowTemplate(FlowSort, FlowName,
                        Template.DataStoreModel.SpecTable, PTable, FlowMark);

                Flow fl = new Flow(flowNo);


                //清空WF_Emp 的StartFlows ,让其重新计算.
                // DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");
                return flowNo;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

    }
}
