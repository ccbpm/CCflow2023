﻿using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.CT;

namespace BP.CT.T0Send
{
    /// <summary>
    /// 线性节点的流程发送
    /// </summary>
    public  class Send024 : TestBase
    {
        #region 变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workID = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        public BP.Port.Emp starterEmp = null;
        #endregion 变量

        /// <summary>
        /// 线性节点的流程发送
        /// </summary>
        public Send024()
        {
            this.Title = "线性节点的流程发送";
            this.DescIt = "流程:024简单3节点(合并模式).";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 过程说明：
        /// 1，以流程 024最简单的3节点(轨迹模式)， 为测试用例。
        /// 2，仅仅测试发送功能，与检查发送后的数据是否完整.
        /// 3, 此测试有三个节点发起点、中间点、结束点，对应三个测试方法。
        /// </summary>
        public override void Do()
        {
            #region 定义变量.
            fk_flow = "024";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);
            #endregion 定义变量.

            //执行第1步骤. 让 zhanghaicheng 发起流程.
            this.Step1();

            //执行第2步骤. 让zhoupeng 处理.
            this.Step2();

            //执行第3步骤. 让zhanghaicheng 结束.
            this.Step3();
        }
        /// <summary>
        /// 步骤1 让zhanghaicheng 发起流程.
        /// </summary>
        public void Step1()
        {
            //给发起人赋值.
            starterEmp = new Port.Emp(userNo);

            //让 userNo 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作, 发起开始节点.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            #region 检查创建工作是否符合预期.
            //检查流程表的数据.
            sql = "SELECT * FROM " + fl.PTable + " WHERE OID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@流程报表数据被删除了.");
            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.FID:
                        if (val != "0")
                            throw new Exception("@应当是0");
                        break;
                    case GERptAttr.FK_Dept:
                        if (val != WebUser.FK_Dept)
                            throw new Exception("@应当是" + WebUser.FK_Dept + ", 现在是:" + val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@应当是" + DataType.CurrentYearMonth + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@应当是 0 , 现在是:" + val);
                        break;
                    case GERptAttr.FlowEmps:
                        if (BP.WF.Glo.UserInfoShowModel != UserInfoShowModel.UserNameOnly)
                        {
                            if (val.Contains(WebUser.No) == false)
                                throw new Exception("@应当是包含当前人员, 现在是:" + val);
                        }
                        break;
                    case GERptAttr.FlowEnder:
                        if (val != WebUser.No)
                            throw new Exception("@应当是 当前人员, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEnderRDT:
                        if (val.Contains(DataType.CurrentData) == false)
                            throw new Exception("@应当是 当前日期, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEndNode:
                        if (val != "2401")
                            throw new Exception("@应当是 2401, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStarter:
                        if (val != WebUser.No)
                            throw new Exception("@应当是  WebUser.No, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStartRDT:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@应当不能为空,现在是:" + val);
                        break;
                    case GERptAttr.Title:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@不能为空title" + val);
                        break;
                    case GERptAttr.WFState:
                        WFState sta = (WFState)int.Parse(val);
                        if (sta != WFState.Blank)
                            throw new Exception("@应当是  WFState.Blank 现在是" + sta.ToString());
                        break;
                    default:
                        break;
                }
            }
            #endregion 检查创建工作是否符合预期

            //执行发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, null, null, 0, "zhoupeng");

            #region 第1步: 检查发送对象.
            //从获取的发送对象里获取到下一个工作者. zhangyifan(张一帆)、zhoushengyu(周升雨).
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@下一步的接受人不正确, 应当是: zhoupeng.现在是:" + objs.VarAcceptersID);

            if (objs.VarToNodeID != 2402)
                throw new Exception("@应该是 2401 节点. 现在是:" + objs.VarToNodeID);

            if (objs.VarWorkID != workID)
                throw new Exception("@主线程的workid不应该变化:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 2401)
                throw new Exception("@当前节点的编号不能变化:" + objs.VarCurrNodeID);

            if (objs.VarTreadWorkIDs != null)
                throw new Exception("@不应当获得子线程WorkID.");
            #endregion 第1步: 检查发送对象.

            #region 第2步: 检查流程引擎表.
            //检查创建这个空白是否有数据完整?
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + workID + " AND FK_Emp='" + objs.VarAcceptersID + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@不应该找不到当前人员的待办.");

            gwf = new GenerWorkFlow(workID);
            if (gwf.Starter != WebUser.No || gwf.StarterName != WebUser.Name)
                throw new Exception("没有写入发起人的信息.");

            if (gwf.FK_Dept != starterEmp.FK_Dept)
                throw new Exception("@发起人的部门有变化，应当是" + starterEmp.FK_Dept + ",现在是:" + gwf.FK_Dept);

            if (gwf.Starter != starterEmp.No)
                throw new Exception("@发起人的 No 有变化，应当是" + starterEmp.No + ",现在是:" + gwf.Starter);

            //判断当前点.
            if (gwf.FK_Node != 2402)
                throw new Exception("@当前点应该是 2402 现在是:" + gwf.FK_Node);

            //判断当前点.
            if (gwf.FID != 0)
                throw new Exception("@当前点应该是 FID=0  现在是:" + gwf.FID);

            //判断PWorkID，没有谁调用它，应当是 0. 
            if (gwf.PWorkID != 0)
                throw new Exception("@没有谁调用它, 当前点应该是 PWorkID=0  现在是:" + gwf.PWorkID);

            //判断 WFState . 
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是 WFState=Runing 现在是:" + gwf.WFState.ToString());

            //检查开始节点 发送人的WF_GenerWorkerList 的.
            gwl = new GenerWorkerList();
            gwl.FK_Emp = Web.WebUser.No;
            gwl.FK_Node = 2401;
            gwl.WorkID = workID;
            gwl.Retrieve();

            // 没有分合流应当是 0 .
            if (gwl.FID != 0)
                throw new Exception("@没有分合流应当是 0.");

            if (gwl.IsEnable == false)
                throw new Exception("@应该是启用的状态 ");

            if (gwl.IsPass == false)
                throw new Exception("@应该是通过的状态 ");

            //if (gwl.Sender.Contains(WebUser.No) == false)
            //    throw new Exception("@应该是 包含当前状态 . ");


            //检查接受人的 WF_GenerWorkerList 的.
            gwl = new GenerWorkerList();
            gwl.FK_Emp = objs.VarAcceptersID;
            gwl.FK_Node = 2402;
            gwl.WorkID = workID;
            gwl.Retrieve();

            // 没有分合流应当是 0 .
            if (gwl.FID != 0)
                throw new Exception("@没有分合流应当是 0.");

            if (gwl.IsEnable == false)
                throw new Exception("@应该是启用的状态 ");

            if (gwl.IsPass == true)
                throw new Exception("@应该是未通过的状态 ");

            //if (gwl.Sender.Contains(WebUser.No) == false)
            //    throw new Exception("@应该是 当前人发送的，现在是: " + gwl.Sender);
            #endregion 第2步: 检查流程引擎表.

            #region 第3步: 检查节点数据表.
            //检查流程表的数据.
            sql = "SELECT * FROM " + fl.PTable + " WHERE OID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@流程报表数据被删除了.");
            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.PWorkID:
                        if (val != "0")
                            throw new Exception("@PWorkID应当是0, 现在是:" + val);
                        break;
                    case GERptAttr.PFlowNo:
                        if (val != "")
                            throw new Exception("@PFlowNo应当是 '' 现在是:" + val);
                        break;
                    case GERptAttr.FID:
                        if (val != "0")
                            throw new Exception("@应当是0");
                        break;
                    case GERptAttr.FK_Dept:
                        if (val != starterEmp.FK_Dept)
                            throw new Exception("@应当是" + starterEmp.FK_Dept + ", 现在是:" + val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@应当是" + DataType.CurrentYearMonth + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@应当是 0 , 现在是:" + val);
                        break;
                    case GERptAttr.FlowEmps:
                        if (BP.WF.Glo.UserInfoShowModel != UserInfoShowModel.UserNameOnly)
                        {
                            if (val.Contains(WebUser.No) == false)
                                throw new Exception("@应当是包含当前人员, 现在是:" + val);
                        }
                        break;
                    case GERptAttr.FlowEnder:
                        if (val != WebUser.No)
                            throw new Exception("@应当是 当前人员, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEnderRDT:
                        if (val.Contains(DataType.CurrentData) == false)
                            throw new Exception("@应当是 当前日期, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEndNode:
                        if (val != "2402")
                            throw new Exception("@应当是 2402, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStarter:
                        if (val != starterEmp.No)
                            throw new Exception("@应当是  " + starterEmp.No + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowStartRDT:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@应当不能为空,现在是:" + val);
                        break;
                    case GERptAttr.Title:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@不能为空title" + val);
                        break;
                    case GERptAttr.WFState:
                        WFState sta = (WFState)int.Parse(val);
                        if (sta != WFState.Runing)
                            throw new Exception("@应当是  WFState.Runing 现在是" + sta.ToString());
                        break;
                    default:
                        break;
                }
            }
            #endregion 第3步: 检查节点数据表.
        }

        /// <summary>
        /// 步骤1 让zhoupeng 登录去处理.
        /// </summary>
        public void Step2()
        {
            //让 zhouepng 登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //让他向下发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, null, null, 0, "zhanghaicheng");

            #region 第1步: 检查发送对象.
            //从获取的发送对象里获取到下一个工作者. zhangyifan(张一帆)、zhoushengyu(周升雨).
            if (objs.VarAcceptersID != "zhanghaicheng")
                throw new Exception("@下一步的接受人不正确, 应当是: zhanghaicheng.现在是:" + objs.VarAcceptersID);

            if (objs.VarToNodeID != 2499)
                throw new Exception("@应该是 2499 节点. 现在是:" + objs.VarToNodeID);

            if (objs.VarWorkID != workID)
                throw new Exception("@主线程的workid不应该变化:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 2402)
                throw new Exception("@当前节点的编号不能变化,现在是:" + objs.VarCurrNodeID);

            if (objs.VarTreadWorkIDs != null)
                throw new Exception("@不应当获得子线程WorkID.");

            #endregion 第1步: 检查发送对象.

            #region 第2步: 检查流程引擎表.
            //检查待办是否存在。
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + workID + " AND FK_Emp='" + objs.VarAcceptersID + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@不应该找不到当前人员的待办.");

            gwf = new GenerWorkFlow(workID);
            if (gwf.Starter != "zhanghaicheng")
                throw new Exception("发起人信息有变化.");

            if (gwf.FK_Dept != starterEmp.FK_Dept)
                throw new Exception("@发起人的部门有变化，应当是" + starterEmp.FK_Dept + ",现在是:" + gwf.FK_Dept);

            if (gwf.Starter != starterEmp.No)
                throw new Exception("@发起人的 No 有变化，应当是" + starterEmp.No + ",现在是:" + gwf.Starter);


            //判断当前点.
            if (gwf.FK_Node != 2499)
                throw new Exception("@当前点应该是 2499 现在是:" + gwf.FK_Node);

            //判断当前点.
            if (gwf.FID != 0)
                throw new Exception("@当前点应该是 FID=0  现在是:" + gwf.FID);

            //判断PWorkID，没有谁调用它，应当是 0. 
            if (gwf.PWorkID != 0)
                throw new Exception("@没有谁调用它, 当前点应该是 PWorkID=0  现在是:" + gwf.PWorkID);

            //判断 WFState . 
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是 WFState=Runing 现在是:" + gwf.WFState.ToString());

            //检查开始节点 发送人的WF_GenerWorkerList 的.
            gwl = new GenerWorkerList();
            gwl.FK_Emp = Web.WebUser.No;
            gwl.FK_Node = 2402;
            gwl.WorkID = workID;
            gwl.Retrieve();

            // 没有分合流应当是 0 .
            if (gwl.FID != 0)
                throw new Exception("@没有分合流应当是 0.");

            if (gwl.IsEnable == false)
                throw new Exception("@应该是启用的状态 ");

            if (gwl.IsPass == false)
                throw new Exception("@应该是通过的状态 ");

            //if (gwl.Sender.Contains("zhanghaicheng") == false)
            //    throw new Exception("@应该是 包含当前状态 . ");

            //检查接受人的 WF_GenerWorkerList 的.
            gwl = new GenerWorkerList();
            gwl.FK_Emp = objs.VarAcceptersID;
            gwl.FK_Node = 2499;
            gwl.WorkID = workID;
            gwl.Retrieve();

            // 没有分合流应当是 0 .
            if (gwl.FID != 0)
                throw new Exception("@没有分合流应当是 0.");

            if (gwl.IsEnable == false)
                throw new Exception("@应该是启用的状态 ");

            if (gwl.IsPass == true)
                throw new Exception("@应该是未通过的状态 ");

            //if (gwl.Sender.Contains(WebUser.No) == false)
            //    throw new Exception("@应该是 当前人发送的，现在是: " + gwl.Sender);
            #endregion 第2步: 检查流程引擎表.

            #region 第3步: 检查节点数据表.
            //检查流程表的数据.
            sql = "SELECT * FROM " + fl.PTable + " WHERE OID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@流程报表数据被删除了.");
            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.FID:
                        if (val != "0")
                            throw new Exception("@应当是0");
                        break;
                    case GERptAttr.FK_Dept:
                        BP.Port.Emp emp = new Port.Emp("zhanghaicheng");
                        if (val != emp.FK_Dept)
                            throw new Exception("@应当是" + emp.FK_Dept + ", 现在是:" + val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@应当是" + DataType.CurrentYearMonth + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@应当是 0 , 现在是:" + val);
                        break;
                    case GERptAttr.FlowEmps:
                        if (BP.WF.Glo.UserInfoShowModel != UserInfoShowModel.UserNameOnly)
                        {
                            if (val.Contains(WebUser.No) == false)
                                throw new Exception("@应当是包含当前人员, 现在是:" + val);
                        }
                        break;
                    case GERptAttr.FlowEnder:
                        if (val != WebUser.No)
                            throw new Exception("@应当是 当前人员, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEnderRDT:
                        if (val.Contains(DataType.CurrentData) == false)
                            throw new Exception("@应当是 当前日期, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEndNode:
                        if (val != "2499")
                            throw new Exception("@应当是 2499, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStarter:
                        if (val == WebUser.No)
                            throw new Exception("@应当是  WebUser.No, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStartRDT:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@应当不能为空,现在是:" + val);
                        break;
                    case GERptAttr.Title:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@不能为空title" + val);
                        break;
                    case GERptAttr.WFState:
                        if (int.Parse(val) != (int)WFState.Runing)
                            throw new Exception("@应当是  WFState.Runing 现在是" + val);
                        break;
                    default:
                        break;
                }
            }
            #endregion 第3步: 检查节点数据表.
        }

        /// <summary>
        /// 步骤3 让zhanghaicheng 结束流程.
        /// </summary>
        public void Step3()
        {
            //让 zhanghaicheng 登录.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");

            //让他向下发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID);

            #region 第1步: 检查发送对象.
            //从获取的发送对象里获取到下一个工作者. zhangyifan(张一帆)、zhoushengyu(周升雨).
            if (objs.VarAcceptersID != null)
                throw new Exception("@接受人员应当为空." + objs.VarAcceptersID);

            if (objs.VarToNodeID != 0)
                throw new Exception("@应当是 0  现在是:" + objs.VarToNodeID);

            if (objs.VarWorkID != workID)
                throw new Exception("@主线程的workid不应该变化:" + objs.VarWorkID);

            if (objs.VarCurrNodeID != 2499)
                throw new Exception("@当前节点的编号不能变化,现在是:" + objs.VarCurrNodeID);

            if (objs.VarTreadWorkIDs != null)
                throw new Exception("@不应当获得子线程WorkID.");

            #endregion 第1步: 检查发送对象.

            #region 第2步: 检查流程引擎表.
            //检查待办是否存在。
            sql = "SELECT * FROM WF_EmpWorks WHERE WorkID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@不应该有待办.");


            sql = "SELECT * FROM WF_GenerWorkFlow WHERE WorkID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@流程信息未删除.");

            sql = "SELECT * FROM WF_GenerWorkerList  WHERE WorkID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@工作人员信息未删除..");
            #endregion 第2步: 检查流程引擎表.

            #region 第3步: 检查节点数据表.
            BP.Port.Emp emp=new Port.Emp("zhanghaicheng");

            //检查流程表的数据.
            sql = "SELECT * FROM " + fl.PTable + " WHERE OID=" + workID;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@流程报表数据被删除了.");
            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.Title:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@流程走完后标题丢失了");
                        break;
                    case GERptAttr.FID:
                        if (val != "0")
                            throw new Exception("@应当是0");
                        break;
                    case GERptAttr.FK_Dept:
                        if (val != emp.FK_Dept)
                            throw new Exception("@发起人的部门发生了变化，应当是("+emp.FK_Dept+"),现在是:" + val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@应当是" + DataType.CurrentYearMonth + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@应当是 0 , 现在是:" + val);
                        break;
                    case GERptAttr.FlowEmps:
                        if (BP.WF.Glo.UserInfoShowModel != UserInfoShowModel.UserNameOnly)
                        {
                            if (val.Contains("zhanghaicheng") == false || val.Contains("zhoupeng") == false)
                                throw new Exception("@应当包含的人员，现在不存在, 现在是:" + val);
                        }
                        break;
                    case GERptAttr.FlowEnder:
                        if (val != "zhanghaicheng")
                            throw new Exception("@应当是 zhanghaicheng , 现在是:" + val);
                        break;
                    case GERptAttr.FlowEnderRDT:
                        if (val.Contains(DataType.CurrentData) == false)
                            throw new Exception("@应当是 当前日期, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEndNode:
                        if (val != "2499")
                            throw new Exception("@应当是 2499, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStarter:
                        if (val != "zhanghaicheng")
                            throw new Exception("@应当是 zhanghaicheng, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStartRDT:
                        if (string.IsNullOrEmpty(val))
                            throw new Exception("@应当不能为空,现在是:" + val);
                        break;
                    case GERptAttr.WFState:
                        if (int.Parse(val) != (int)WFState.Complete)
                            throw new Exception("@应当是  WFState.Complete 现在是" + val);
                        break;
                    default:
                        break;
                }
            }
            #endregion 第3步: 检查节点数据表.
        }
    }
}
