﻿using System;
using BP.En;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Web;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 处理工作退回
    /// </summary>
    public class WorkReturn
    {
        #region 变量
        /// <summary>
        /// 从节点
        /// </summary>
        private Node HisNode = null;
        /// <summary>
        /// 退回到节点
        /// </summary>
        private Node ReturnToNode = null;
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 WorkID = 0;
        /// <summary>
        /// 流程ID
        /// </summary>
        private Int64 FID = 0;
        /// <summary>
        /// 是否按原路返回?
        /// </summary>
        private bool IsBackTrack = false;
        /// <summary>
        /// 退回原因
        /// </summary>
        private string Msg = "退回原因未填写.";
        /// <summary>
        /// 当前节点
        /// </summary>
        private Work HisWork = null;
        /// <summary>
        /// 退回到节点
        /// </summary>
        private Work ReurnToWork = null;
        private string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
        private Paras ps;
        public string ReturnToEmp = null;
        public int ReturnToNodeID = 0;
        /// <summary>
        /// 退回考核规则字段
        /// </summary>
        public string ReturnCHDatas = null;
        #endregion

        /// <summary>
        /// 工作退回
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">WorkID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="currNodeID">从节点</param>
        /// <param name="returnToNodeID">退回到节点, 0表示上一个节点，或者指定的一个节点.</param>
        /// <param name="returnToEmp">退回到人</param>
        /// <param name="isBackTrack">是否需要原路返回？</param>
        /// <param name="returnInfo">退回原因</param>
        public WorkReturn(string fk_flow, Int64 workID, Int64 fid, int currNodeID, int returnToNodeID, string returnToEmp, bool isBackTrack, string returnInfo, string pageData = null)
        {
            this.HisNode = new Node(currNodeID);

            //如果退回的节点为0,就求出可以退回的唯一节点.
            if (returnToNodeID == 0)
            {
                DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(workID);
                if (dt.Rows.Count == 0)
                    throw new Exception("err@当前节点不允许退回，系统根据退回规则没有找到可以退回的到的节点。");

                if (dt.Rows.Count != 1)
                    throw new Exception("err@当前节点可以退回的节点有[" + dt.Rows.Count + "]个，您需要指定要退回的节点才能退回。");

                returnToNodeID = int.Parse(dt.Rows[0][0].ToString());
                if (DataType.IsNullOrEmpty(returnToEmp) == true)
                    returnToEmp = dt.Rows[0][2].ToString();
            }

            this.ReturnToNode = new Node(returnToNodeID);
            this.WorkID = workID;
            this.FID = fid;
            this.IsBackTrack = isBackTrack;
            this.Msg = returnInfo;
            this.ReturnToEmp = returnToEmp;
            this.ReturnToNodeID = returnToNodeID;
            //当前工作.
            this.HisWork = this.HisNode.HisWork;

            this.HisWork.OID = workID;
            this.HisWork.RetrieveFromDBSources();

            //退回工作
            this.ReurnToWork = this.ReturnToNode.HisWork;
            this.ReurnToWork.OID = workID;
            if (this.ReurnToWork.RetrieveFromDBSources() == 0)
            {
                this.ReurnToWork.OID = fid;
                this.ReurnToWork.RetrieveFromDBSources();
            }
            //退回考核规则
            this.ReturnCHDatas = pageData;
        }
        /// <summary>
        /// 删除两个节点之间的业务数据与流程引擎控制数据.
        /// </summary>
        private void DeleteSpanNodesGenerWorkerListData()
        {
            if (this.IsBackTrack == true)
                return;

            Paras ps = new Paras();
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            // 删除FH, 不管是否有这笔数据.
            ps.Clear();

            /*如果不是退回并原路返回，就需要清除 两个节点之间的数据, 包括WF_GenerWorkerlist的数据.*/
            if (this.ReturnToNode.ItIsStartNode == true)
            {
                // 删除其子线程流程.
                ps.Clear();
                ps.SQL = "DELETE FROM WF_GenerWorkFlow WHERE FID=" + dbStr + "FID ";
                ps.Add("FID", this.WorkID);
                DBAccess.RunSQL(ps);

                /*如果退回到了开始的节点，就删除出开始节点以外的数据，不要删除节点表单数据，这样会导致流程轨迹打不开.*/
                ps.Clear();
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE FK_Node!=" + dbStr + "FK_Node AND (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2)";
                ps.Add(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);
                ps.Add("WorkID1", this.WorkID);
                ps.Add("WorkID2", this.WorkID);
                DBAccess.RunSQL(ps);
                return;
            }

            /*找到发送到退回的时间点，把从这个时间点以来的数据都要删除掉.*/
            ps.Clear();

            ps.SQL = "SELECT RDT,ActionType,NDFrom FROM ND" + int.Parse(this.HisNode.FlowNo) + "Track WHERE  NDFrom=" + dbStr + "NDFrom AND WorkID=" + dbStr + "WorkID AND ActionType=" + (int)ActionType.Forward + " ORDER BY RDT desc ";
            ps.Add("NDFrom", this.ReturnToNode.NodeID);
            ps.Add("WorkID", this.WorkID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count >= 1)
            {
                string rdt = dt.Rows[0][0].ToString();

                ps.Clear();
                ps.SQL = "SELECT ActionType,NDFrom FROM ND" + int.Parse(this.HisNode.FlowNo) + "Track WHERE   RDT >=" + dbStr + "RDT AND (WorkID=" + dbStr + "WorkID OR FID=" + dbStr + "FID) AND NDFrom NOT IN(SELECT NDFrom FROM ND" + int.Parse(this.HisNode.FlowNo) + "Track WHERE   RDT <" + dbStr + "RDT And ActionType IN (" + (int)ActionType.Forward + ","
                    + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL + ") AND (WorkID=" + dbStr + "WorkID OR FID=" + dbStr + "FID)) ORDER BY RDT ";
                ps.Add("RDT", rdt);
                ps.Add("WorkID", this.WorkID);
                ps.Add("FID", this.WorkID);
                dt = DBAccess.RunSQLReturnTable(ps);

                foreach (DataRow dr in dt.Rows)
                {
                    ActionType at = (ActionType)int.Parse(dr["ActionType"].ToString());
                    int nodeid = int.Parse(dr["NDFrom"].ToString());
                    if (nodeid == this.ReturnToNode.NodeID)
                        continue;

                    //删除中间的节点.
                    ps.Clear();
                    ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE FK_Node=" + dbStr + "FK_Node AND (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2) ";
                    ps.Add("FK_Node", nodeid);
                    ps.Add("WorkID1", this.WorkID);
                    ps.Add("WorkID2", this.WorkID);
                    DBAccess.RunSQL(ps);

                    //删除审核意见
                    ps.Clear();
                    ps.SQL = "DELETE FROM ND" + int.Parse(this.ReturnToNode.FlowNo) + "Track WHERE NDFrom=" + dbStr + "NDFrom AND  (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2) AND ActionType=22";
                    ps.Add("NDFrom", nodeid);
                    ps.Add("WorkID1", this.WorkID);
                    ps.Add("WorkID2", this.WorkID);
                    DBAccess.RunSQL(ps);
                }
            }


            //删除当前节点的数据.
            ps.Clear();
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE FK_Node=" + dbStr + "FK_Node AND (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2) ";
            ps.Add("FK_Node", this.HisNode.NodeID);
            ps.Add("WorkID1", this.WorkID);
            ps.Add("WorkID2", this.WorkID);
            DBAccess.RunSQL(ps);

            //  string sql = "SELECT * FROM ND" + int.Parse(this.HisNode.FlowNo) + "Track WHERE  NDTo='"+this.ReturnToNode.NodeID+" AND WorkID="+this.WorkID;
            //  ActionType
        }
        /// <summary>
        /// 队列节点上一个人退回另外一个人.
        /// </summary>
        /// <returns></returns>
        public string DoOrderReturn()
        {
            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;

            //如果事件返回的信息不是null，就终止执行。
            string msg = ExecEvent.DoNode(EventListNode.ReturnBefore, this.HisNode, this.HisWork, null, atPara);
            if (msg != null)
                return msg;

            //执行退回的考核.
            Glo.InitCH(this.HisNode.HisFlow, this.HisNode, this.WorkID, this.FID, this.HisNode.Name + ":退回考核.");

            if (DataType.IsNullOrEmpty(this.HisNode.FocusField) == false)
            {
                try
                {
                    string focusField = "";
                    string[] focusFields = this.HisNode.FocusField.Split('@');
                    if (focusFields.Length >= 2)
                        focusField = focusFields[1];
                    else
                        focusField = focusFields[0];



                    // 把数据更新它。
                    this.HisWork.Update(focusField, "");
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError("退回时更新焦点字段错误:" + ex.Message);
                }
            }

            //退回到人.
            Emp returnToEmp = new Emp(this.ReturnToEmp);

            // 退回状态。
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbStr + "WFState,FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName,TodoEmps=" + dbStr + "TodoEmps, TodoEmpsNum=0 WHERE  WorkID=" + dbStr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, (int)WFState.ReturnSta);
            ps.Add(GenerWorkFlowAttr.FK_Node, this.ReturnToNode.NodeID);
            ps.Add(GenerWorkFlowAttr.NodeName, this.ReturnToNode.Name);

            ps.Add(GenerWorkFlowAttr.TodoEmps, returnToEmp.UserID + "," + returnToEmp.Name + ";");

            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);

            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=0,IsRead=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp=" + dbStr + "FK_Emp ";
            ps.Add("FK_Node", this.ReturnToNode.NodeID);
            ps.Add("WorkID", this.WorkID);
            ps.Add("FK_Emp", this.ReturnToEmp);
            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=1000,IsRead=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp=" + dbStr + "FK_Emp ";
            ps.Add("FK_Node", this.HisNode.NodeID);
            ps.Add("WorkID", this.WorkID);
            ps.Add("FK_Emp", WebUser.No);
            DBAccess.RunSQL(ps);

            //更新流程报表数据.
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisNode.HisFlow.PTable + " SET  WFState=" + dbStr + "WFState, FlowEnder=" + dbStr + "FlowEnder, FlowEndNode=" + dbStr + "FlowEndNode WHERE OID=" + dbStr + "OID";
            ps.Add("WFState", (int)WFState.ReturnSta);
            ps.Add("FlowEnder", WebUser.No);
            ps.Add("FlowEndNode", ReturnToNode.NodeID);
            ps.Add("OID", this.WorkID);
            DBAccess.RunSQL(ps);

            ////从工作人员列表里找到被退回人的接受人.
            //GenerWorkerList gwl = new GenerWorkerList();
            //gwl.Retrieve(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);

            /* // 记录退回轨迹。
             ReturnWork rw = new ReturnWork();
             rw.WorkID = this.WorkID;
             rw.ReturnToNode = this.ReturnToNode.NodeID;
             rw.ReturnNodeName = this.HisNode.Name;

             rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
             rw.ReturnToEmp = this.ReturnToEmp; //退回给。
             rw.BeiZhu = Msg;

             rw.setMyPK(DBAccess.GenerOIDByGUID().ToString());
             if (DataType.IsNullOrEmpty(this.ReturnCHDatas) == false)
             {
                 string[] strs = this.ReturnCHDatas.Split('&');
                 foreach (string str in strs)
                 {
                     string[] param = str.Split('=');
                     if (param.Length == 2)
                         rw.SetValByKey(param[0].Replace("TB_", "").Replace("DDL_", "").Replace("CB_", ""), param[1]);
                 }
             }
             rw.Insert();
             */
            // 加入track.
            this.AddToTrack(ActionType.Return, returnToEmp.UserID, returnToEmp.Name,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);

            /*try
            {
                // 记录退回日志.
                ReorderLog(this.HisNode, this.ReturnToNode, rw);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex.Message);
            }*/

            // 以退回到的节点向前数据用递归删除它。
            if (IsBackTrack == false)
            {
                /*如果退回不需要原路返回，就删除中间点的数据。*/
#warning 没有考虑两种流程数据存储模式。
                //DeleteToNodesData(this.ReturnToNode.HisToNodes);
            }
            Work work = this.HisWork;
            work.OID = this.WorkID;
            work.RetrieveFromDBSources();
            // 退回后发送的消息事件
            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, this.ReturnToNode.NodeID, PushMsgAttr.FK_Event, EventListNode.ReturnAfter);
            work.NodeID = this.HisNode.NodeID;
            foreach (PushMsg pm in pms)
            {
                pm.DoSendMessage(this.ReturnToNode, work, null, null, null, this.ReturnToEmp);
            }
            //退回后事件
            atPara += "@SendToEmpIDs=" + this.ReturnToEmp;
            string text = ExecEvent.DoNode(EventListNode.ReturnAfter, this.HisNode, work, null, atPara);
            if (text != null && text.Length > 1000)
                text = "退回事件:无返回信息.";

            // 返回退回信息.
            if (this.ReturnToNode.ItIsGuestNode)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                return "工作已经被您退回到" + this.ReturnToNode.Name + ",<br/>退回给" + gwf.GuestNo + "," + gwf.GuestName + ".\n\r<br/>" + text;
            }
            else
            {
                return "工作已经被您退回到" + this.ReturnToNode.Name + ",<br/>退回给" + returnToEmp.UserID + "," + returnToEmp.Name + ".\n\r<br/>" + text;
            }
        }
        /// <summary>
        /// 要退回到父流程上去
        /// </summary>
        /// <returns></returns>
        private string ReturnToParentFlow()
        {
            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;
            //如果事件返回的信息不是null，就终止执行。
            string msg = ExecEvent.DoNode(EventListNode.ReturnBefore, this.HisNode, this.HisWork, null, atPara);
            if (msg != null)
                return msg;

            //当前 gwf.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            //设置子流程信息.
            GenerWorkFlow gwfP = new GenerWorkFlow(gwf.PWorkID);
            gwfP.WFState = WFState.ReturnSta;
            gwfP.NodeID = this.ReturnToNode.NodeID;
            gwfP.NodeName = this.ReturnToNode.Name;


            //启用待办.
            GenerWorkerList gwl = new GenerWorkerList();
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID, GenerWorkerListAttr.WorkID, gwfP.WorkID);

            string toEmps = "";
            string returnEmps = "";
            foreach (GenerWorkerList item in gwls)
            {
                item.PassInt = 0;
                item.Update();
                gwl = item;

                toEmps += item.EmpNo + "," + item.EmpName + ";";
                returnEmps += item.EmpNo + ",";
            }

            gwfP.TodoEmps = toEmps;
            gwfP.SetPara("IsBackTracking", this.IsBackTrack);
            gwfP.Update();

            #region 写入退回提示.
            // 记录退回轨迹。
            /* ReturnWork rw = new ReturnWork();
             rw.WorkID = gwfP.WorkID;
             rw.ReturnToNode = this.ReturnToNode.NodeID;
             rw.ReturnNodeName = gwfP.NodeName;

             rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
             rw.ReturnToEmp = gwl.EmpNo; //退回给。
             rw.BeiZhu = Msg;

             rw.IsBackTracking = this.IsBackTrack;
             rw.setMyPK(DBAccess.GenerOIDByGUID().ToString());

             if (DataType.IsNullOrEmpty(this.ReturnCHDatas) == false)
             {
                 string[] strs = this.ReturnCHDatas.Split('&');
                 foreach (string str in strs)
                 {
                     string[] param = str.Split('=');
                     if (param.Length == 2)
                         rw.SetValByKey(param[0].Replace("TB_", "").Replace("DDL_", "").Replace("CB_", ""), param[1]);
                 }
             }
             rw.Insert();*/


            // 加入track.
            this.AddToTrack(ActionType.Return, gwl.EmpNo, gwl.EmpName,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);
            #endregion

            //删除当前的流程.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.WorkID, true);

            //设置当前为未读的状态.
            BP.WF.Dev2Interface.Node_SetWorkUnRead(gwfP.WorkID);

            //退回后发送的消息事件 
            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, this.ReturnToNode.NodeID, PushMsgAttr.FK_Event, EventListNode.ReturnAfter);
            Work work = this.HisWork;
            work.OID = gwfP.WorkID;
            work.RetrieveFromDBSources();
            work.NodeID = this.HisNode.NodeID;
            foreach (PushMsg pm in pms)
            {
                pm.DoSendMessage(this.ReturnToNode, work, null, null, null, returnEmps);
            }
            //如果事件返回的信息不是 null，就终止执行。
            atPara += "@SendToEmpIDs=" + returnEmps;
            msg = ExecEvent.DoNode(EventListNode.ReturnAfter, this.HisNode, work, null, atPara);
            if (String.IsNullOrEmpty(msg) == true) //  如果有消息，就返回消息.
                msg = "";
            //返回退回信息.
            return "成功的退回到[" + gwfP.FlowName + " - " + this.ReturnToNode.Name + "],退回给[" + toEmps + "].\n\r" + msg;
        }
        /// <summary>
        /// 执行退回到分流节点，完全退回.
        /// </summary>
        /// <returns></returns>
        public string DoItOfKillEtcThread()
        {
            //干流程的gwf.
            GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
            Node nd = new Node(gwf.NodeID);
            if (nd.ItIsFLHL == false)
                throw new Exception("err@系统已经运行到合流节点，您不能在执行退回.");

            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;
            //如果事件返回的信息不是 null，就终止执行。
            string msg = ExecEvent.DoNode(EventListNode.ReturnBefore, this.HisNode, this.HisWork, null,
                atPara);
            if (String.IsNullOrEmpty(msg) == false) // 2019-08-28 zl 返回空字符串表示执行成功，不应该终止。
                return msg;

            //查询出来所有的子线程,并删除他.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FID, this.FID);

            string delSubThreadInfo = "\t\n";
            foreach (GenerWorkFlow mygwf in gwfs)
            {
                BP.WF.Dev2Interface.Node_FHL_KillSubFlow(mygwf.WorkID);
                delSubThreadInfo += mygwf.Title + "\t\n";
            }

            //更新状态.
            gwf.WFState = WFState.ReturnSta;
            gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
            gwf.NodeID = this.ReturnToNode.NodeID;

            string todoEmps = "";
            //更新他的待办.
            GenerWorkerLists gwls = new GenerWorkerLists(this.FID, this.ReturnToNode.NodeID);
            foreach (GenerWorkerList item in gwls)
            {
                todoEmps += item.EmpNo + "," + item.EmpName + ";";
                item.PassInt = 0;
                item.Update();
            }
            gwf.SetPara("ThreadCount", 0);
            gwf.TodoEmps = todoEmps;
            gwf.TodoEmpsNum = gwls.Count;
            gwf.SetPara("IsBackTracking", this.IsBackTrack);
            gwf.Update();

            //删除子线程的工作.
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + gwf.WorkID);

            //记录退回轨迹. 
            /*ReturnWork rw = new ReturnWork();
            rw.WorkID = gwf.WorkID;

            rw.ReturnNode = this.HisNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.Returner = WebUser.No;
            rw.ReturnerName = WebUser.Name;
            rw.BeiZhu = this.Msg;

            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnToEmp = this.ReturnToEmp; //.TodoEmps;

            //主键.
            rw.setMyPK(BP.DA.DBAccess.GenerGUID());
            rw.Insert();*/

            //设置return记录. 加入track.
            this.AddToTrack(ActionType.Return, WebUser.No, WebUser.Name, this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);

            //如果事件返回的信息不是 null，就终止执行。.
            msg = ExecEvent.DoNode(EventListNode.ReturnAfter, this.HisNode, this.HisWork, null, atPara);
            if (String.IsNullOrEmpty(msg) == false) //  如果有消息，就返回消息.
                return msg;

            return "子线程退回成功, 一共删除了(" + gwfs.Count + ")个其他的子线程:" + delSubThreadInfo;
        }
        /// <summary>
        /// 执行退回.
        /// </summary>
        /// <returns>返回退回信息</returns>
        public string DoIt()
        {

            // 增加要退回到父流程上去. by zhoupeng.
            if (this.ReturnToNode.FlowNo.Equals(this.HisNode.FlowNo) == false)
            {
                /*子流程要退回到父流程的情况.*/
                return ReturnToParentFlow();
            }


            if (this.HisNode.NodeID == this.ReturnToNode.NodeID)
            {
                if (this.HisNode.TodolistModel == TodolistModel.Order)
                {
                    /*一个队列的模式，一个人退回给另外一个人 */
                    return DoOrderReturn();
                }
            }

            if (this.ReturnToNode.TodolistModel == TodolistModel.Order)
            {
                /* 当退回到的节点是 队列模式或者是协作模式时. */
                return DoOrderReturn();
            }

            /* 删除退回选择的信息, forzhuhai: 退回后，删除发送人上次选择的信息.
             * 
             * 场景:
             * 1, a b c d 节点 d节点退回给c 如果d的接收人是c来选择的, 他退回后要把d的选择信息删除掉.
             * 2, a b c d 节点 d节点退回给a 如果 b c d 的任何一个接受人的范围是有上一步发送人来选择的，就要删除选择人的信息.
             * 
             * */

            //是否需要删除中间点. 
            bool isNeedDeleteSpanNodes = true;
            string sql = "";
            foreach (Node nd in this.ReturnToNode.HisToNodes)
            {
                if (nd.NodeID == this.HisNode.NodeID)
                {
                    sql = "DELETE FROM WF_SelectAccper WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID;
                    DBAccess.RunSQL(sql);
                    isNeedDeleteSpanNodes = false;
                }
            }

            //如果有中间步骤.
            if (isNeedDeleteSpanNodes)
            {
                //获得可以退回的节点，这个节点是有顺序的.
                DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.WorkID);
                bool isDelBegin = false;
                foreach (DataRow dr in dt.Rows)
                {
                    int nodeID = int.Parse(dr["No"].ToString());

                    if (nodeID == this.ReturnToNode.NodeID)
                        isDelBegin = true; /*如果等于当前的节点，就开始把他们删除掉.*/

                    if (isDelBegin)
                    {
                        sql = "DELETE FROM WF_SelectAccper WHERE FK_Node=" + nodeID + " AND WorkID=" + this.WorkID;
                        DBAccess.RunSQL(sql);
                    }
                }

                // 删除当前节点信息.
                sql = "DELETE FROM WF_SelectAccper WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID;
                DBAccess.RunSQL(sql);
            }

            //删除.
            Template.NodeWorkCheck fwc = new NodeWorkCheck(this.HisNode.NodeID);
            if (fwc.FWCIsShowReturnMsg == 0)
                BP.WF.Dev2Interface.DeleteCheckInfo(this.HisNode.FlowNo, this.WorkID, this.HisNode.NodeID);

            //删除审核组件设置“协作模式下操作员显示顺序”为“按照接受人员列表先后顺序(官职大小)”，而生成的待审核轨迹信息
            if (fwc.FWCSta == FrmWorkCheckSta.Enable && fwc.FWCOrderModel == FWCOrderModel.SqlAccepter)
            {
                DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.HisNode.FlowNo) + "Track WHERE WorkID = " + this.WorkID +
                                      " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + this.HisNode.NodeID +
                                      " AND NDTo = " + this.HisNode.NodeID + " AND (Msg = '' OR Msg IS NULL)");
            }

            switch (this.HisNode.HisRunModel)
            {
                case RunModel.Ordinary: /* 1： 普通节点向下发送的*/
                    switch (ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary:   /*1-1 普通节to普通节点 */
                            return ExeReturn1_1(); //
                        case RunModel.FL:  /* 1-2 普通节to分流点   */
                            return ExeReturn1_1(); //
                        case RunModel.HL:  /*1-3 普通节to合流点   */
                            return ExeReturn1_1(); //
                        case RunModel.FHL: /*1-4 普通节点to分合流点 */
                            return ExeReturn1_1();
                        case RunModel.SubThreadSameWorkID: /*1-5 普通节to子线程点 */
                        case RunModel.SubThreadUnSameWorkID: /*1-5 普通节to子线程点 */
                        default:
                            throw new Exception("@退回错误:非法的设计模式或退回模式.普通节to子线程点");
                    }
                case RunModel.FL: /* 2: 分流节点向下发送的*/
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary:    /*2.1 分流点to普通节点 */
                            return ExeReturn1_1(); //
                        case RunModel.FL:  /*2.2 分流点to分流点  */
                        case RunModel.HL:  /*2.3 分流点to合流点,分合流点   */
                        case RunModel.FHL:
                            return ExeReturn1_1(); //
                        case RunModel.SubThreadSameWorkID: /* 2.4 分流点to子线程点   */
                        case RunModel.SubThreadUnSameWorkID: /* 2.4 分流点to子线程点   */
                            return ExeReturn2_4(); //
                        default:
                            throw new Exception("@没有判断的节点类型(" + ReturnToNode.Name + ")");
                            break;
                    }
                case RunModel.HL:  /* 3: 合流节点向下退回 */
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*3.1 普通工作节点 */
                            return ExeReturn1_1(); //
                        case RunModel.FL: /*3.2 合流点向分流点退回 */
                            return ExeReturn3_2(); //
                        case RunModel.HL: /*3.3 合流点 */
                        case RunModel.FHL:
                            throw new Exception("@尚未完成.");
                        case RunModel.SubThreadSameWorkID:/*3.4 合流点向子线程退回 */
                        case RunModel.SubThreadUnSameWorkID:/*3.4 合流点向子线程退回 */
                            return ExeReturn3_4();
                        default:
                            throw new Exception("@退回错误:非法的设计模式或退回模式.普通节to子线程点");
                    }
                case RunModel.FHL:  /* 4: 分流节点向下发送的 */
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*4.1 普通工作节点 */
                            return ExeReturn1_1();
                        case RunModel.FL: /*4.2 分流点 */
                        case RunModel.HL: /*4.3 合流点 */
                        case RunModel.FHL:
                            throw new Exception("@尚未完成.");
                        case RunModel.SubThreadSameWorkID:/*4.5 子线程*/
                        case RunModel.SubThreadUnSameWorkID:/*4.5 子线程*/
                            return ExeReturn3_4();
                        default:
                            throw new Exception("@没有判断的节点类型(" + this.ReturnToNode.Name + ")");
                    }
                case RunModel.SubThreadSameWorkID:  /* 5: 子线程节点向下发送的 */
                case RunModel.SubThreadUnSameWorkID:  /* 5: 子线程节点向下发送的 */
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*5.1 普通工作节点 */
                            throw new Exception("@非法的退回模式,,请反馈给管理员.");
                        case RunModel.FL: /*5.2 分流点 */
                            /*子线程退回给分流点.*/
                            return ExeReturn5_2();
                        case RunModel.HL: /*5.3 合流点 */
                            throw new Exception("@非法的退回模式,请反馈给管理员.");
                        case RunModel.FHL: /*5.4 分合流点 */
                            return ExeReturn5_2();
                        //throw new Exception("@目前不支持此场景下的退回,请反馈给管理员.");
                        case RunModel.SubThreadSameWorkID: /*5.5 子线程*/
                        case RunModel.SubThreadUnSameWorkID: /*5.5 子线程*/
                            return ExeReturn1_1();
                        default:
                            throw new Exception("@没有判断的节点类型(" + ReturnToNode.Name + ")");
                    }
                default:
                    throw new Exception("@没有判断的退回类型:" + this.HisNode.HisRunModel);
            }
            throw new Exception("@系统出现未判断的异常.");
        }
        /// <summary>
        /// 分流点退回给子线程
        /// </summary>
        /// <returns></returns>
        private string ExeReturn2_4()
        {
            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;

            //如果事件返回的信息不是null，就终止执行。
            string msg = ExecEvent.DoNode(EventListNode.ReturnBefore, this.HisNode, this.HisWork, null, atPara);
            if (msg != null)
                return msg;

            //更新运动到节点,但是仍然是退回状态.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.NodeID = this.ReturnToNode.NodeID;
            //增加参与的人员
            if (gwf.Emps.Contains("@" + WebUser.No + ",") == false)
                gwf.Emps += WebUser.No + "," + WebUser.Name + "@";
            gwf.SetPara("IsBackTracking", this.IsBackTrack);
            gwf.Update();

            //更新退回到的人员信息可见.
            string returnEmp = "";
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);
            foreach (GenerWorkerList item in gwls)
            {
                item.PassInt = 0;
                item.Update();
                this.ReturnToEmp = item.EmpNo + "," + item.EmpName;
                returnEmp += item.EmpNo + ",";
            }

            // 去掉合流节点工作人员的待办.
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);
            foreach (GenerWorkerList item in gwls)
            {
                item.PassInt = 0;
                item.ItIsRead = false;
                item.Update();
            }

            //把分流节点的待办去掉. 
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FID, this.FID, GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
            foreach (GenerWorkerList item in gwls)
            {
                item.PassInt = -2;
                item.Update();
            }

            // 记录退回轨迹。
            /*ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = returnEmp; //退回给。

            if (DataType.IsNullOrEmpty(this.ReturnCHDatas) == false)
            {
                string[] strs = this.ReturnCHDatas.Split('&');
                foreach (string str in strs)
                {
                    string[] param = str.Split('=');
                    if (param.Length == 2)
                        rw.SetValByKey(param[0].Replace("TB_", "").Replace("DDL_", "").Replace("CB_", ""), param[1]);
                }
            }

            rw.setMyPK(DBAccess.GenerOIDByGUID().ToString());
            rw.BeiZhu = Msg;
            rw.IsBackTracking = this.IsBackTrack;
            rw.Insert();*/

            // 加入track.
            this.AddToTrack(ActionType.Return, returnEmp, ReturnToEmp,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);


            //退回消息事件 
            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, this.HisNode.NodeID, PushMsgAttr.FK_Event, EventListNode.UndoneAfter);
            foreach (PushMsg pm in pms)
            {
                pm.DoSendMessage(this.HisNode, this.HisNode.HisWork, null, null, null, returnEmp);
            }
            //退回后事件
            atPara += "@SendToEmpIDs=" + returnEmp;
            string text = ExecEvent.DoNode(EventListNode.ReturnAfter, this.HisNode, this.HisWork, null, atPara);
            if (text != null && text.Length > 1000)
                text = "退回事件:无返回信息.";
            if (text == null)
                text = "";
            return "成功的把信息退回到：" + this.ReturnToNode.Name + " , 退回给:(" + this.ReturnToEmp + ").\n\r" + text;
        }
        /// <summary>
        /// 子线程退回给分流点
        /// </summary>
        /// <returns></returns>
        private string ExeReturn5_2()
        {
            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;

            //如果事件返回的信息不是null，就终止执行。
            string msg = ExecEvent.DoNode(EventListNode.ReturnBefore, this.HisNode, this.HisWork, null, atPara);
            if (msg != null)
                return msg;
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.NodeID = this.ReturnToNode.NodeID;
            string info = "@工作已经成功的退回到（" + ReturnToNode.Name + "）退回给：";

            //子线程退回应该是单线退回到干流程.
            //GenerWorkerLists gwls = new GenerWorkerLists();
            //gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FID, this.FID, GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);


            //查询退回到的工作人员列表.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FID, this.FID,
                GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);

            string toEmp = "";
            string toEmpName = "";
            GenerWorkerList gwl = null;
            if (gwls.Count == 1)
            {
                gwl = gwls[0] as GenerWorkerList;
                gwl.ItIsPass = false; // 显示待办, 这个是合流节点的工作人员.
                gwl.ItIsRead = false; //
                gwl.Update();
                info += gwl.EmpNo + "," + gwl.EmpName;
                toEmp = gwl.EmpNo;
                toEmpName = gwl.EmpName;
                info += "(" + gwl.EmpNo + "," + gwl.EmpName + ")";
            }
            else
            {
                /*有可能多次退回的情况，表示曾经退回过n次。*/
            }

            // 记录退回轨迹。
            /*ReturnWork rw = new ReturnWork();

            //rw.WorkID = this.FID;
            rw.WorkID = this.WorkID;
            rw.FID = this.FID;

            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = toEmp; //退回给。

            if (DataType.IsNullOrEmpty(this.ReturnCHDatas) == false)
            {
                string[] strs = this.ReturnCHDatas.Split('&');
                foreach (string str in strs)
                {
                    string[] param = str.Split('=');
                    if (param.Length == 2)
                        rw.SetValByKey(param[0].Replace("TB_", "").Replace("DDL_", "").Replace("CB_", ""), param[1]);
                }
            }

            rw.setMyPK(DBAccess.GenerOIDByGUID().ToString());
            rw.BeiZhu = Msg;
            rw.IsBackTracking = this.IsBackTrack;
            rw.Insert();*/

            // 加入track.
            this.AddToTrack(ActionType.Return, toEmp, toEmpName,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);

            gwf.WFState = WFState.ReturnSta;
            gwf.NodeID = this.ReturnToNode.NodeID;
            gwf.NodeName = this.ReturnToNode.Name;
            gwf.Starter = toEmp;
            gwf.StarterName = toEmpName;
            gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
            //增加参与的人员
            string emps = gwf.Emps;
            if (DataType.IsNullOrEmpty(emps) == true)
                emps = "@";
            if (emps.Contains("@" + WebUser.No + ",") == false)
            {
                emps += WebUser.No + "," + WebUser.Name + "@";
            }
            gwf.Emps = emps;
            gwf.SetPara("IsBackTracking", this.IsBackTrack);
            gwf.Update();

            //更新主流程的状态
            GenerWorkFlow mainGwf = new GenerWorkFlow(gwf.FID);
            //mainGwf.WFState = WFState.ReturnSta;
            mainGwf.NodeID = this.ReturnToNode.NodeID;
            mainGwf.Update();

            //找到当前的工作数据.
            GenerWorkerList currWorker = new GenerWorkerList();
            int i = currWorker.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                 GenerWorkerListAttr.WorkID, this.WorkID,
                 GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);

            if (i != 1)
                throw new Exception("@当前的工作人员列表数据丢失了,流程引擎错误.");

            //设置当前的工作数据为退回状态,让其不能看到待办, 这个是约定的值.
            currWorker.PassInt = (int)WFState.ReturnSta;
            currWorker.ItIsRead = false;
            currWorker.Update();

            //退回消息事件
            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, this.HisNode.NodeID, PushMsgAttr.FK_Event, EventListNode.ReturnAfter);
            foreach (PushMsg pm in pms)
            {
                pm.DoSendMessage(this.HisNode, this.HisWork, null, null, null, toEmp);
            }
            //退回后事件
            atPara += "@SendToEmpIDs=" + toEmp;
            string text = ExecEvent.DoNode(EventListNode.ReturnAfter, this.HisNode, this.HisWork, null, atPara);
            if (text != null && text.Length > 1000)
                text = "退回事件:无返回信息.";
            if (text == null)
                text = "";
            // 返回退回信息.
            return info + ".\n\r" + text;
        }
        /// <summary>
        /// 合流点向子线程退回
        /// </summary>
        private string ExeReturn3_4()
        {
            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;

            //如果事件返回的信息不是null，就终止执行。
            string msg = ExecEvent.DoNode(EventListNode.ReturnBefore, this.HisNode, this.HisWork, null, atPara);
            if (msg != null)
                return msg;

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.NodeID = this.ReturnToNode.NodeID;

            string info = "@工作已经成功的退回到（" + ReturnToNode.Name + "）退回给：";
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);

            string toEmp = "";
            string toEmpName = "";
            foreach (GenerWorkerList item in gwls)
            {
                item.ItIsPass = false;
                item.ItIsRead = false;
                item.Update();
                info += item.EmpNo + "," + item.EmpName;
                toEmp = item.EmpNo;
                toEmpName = item.EmpName;
            }

            //删除已经发向合流点的汇总数据.
            MapDtls dtls = new MapDtls("ND" + this.HisNode.NodeID);
            foreach (MapDtl dtl in dtls)
            {
                /*如果是合流数据*/
                if (dtl.ItIsHLDtl)
                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE OID=" + this.WorkID);
            }

            // 记录退回轨迹。
            /*ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = toEmp; //退回给。

            if (DataType.IsNullOrEmpty(this.ReturnCHDatas) == false)
            {
                string[] strs = this.ReturnCHDatas.Split('&');
                foreach (string str in strs)
                {
                    string[] param = str.Split('=');
                    if (param.Length == 2)
                        rw.SetValByKey(param[0].Replace("TB_", "").Replace("DDL_", "").Replace("CB_", ""), param[1]);
                }
            }

            rw.setMyPK(DBAccess.GenerGUID());
            rw.BeiZhu = Msg;
            rw.IsBackTracking = this.IsBackTrack;
            rw.Insert();*/

            // 加入track.
            this.AddToTrack(ActionType.Return, toEmp, toEmpName,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);

            gwf.WFState = WFState.ReturnSta;
            gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
            //增加参与的人员
            string emps = gwf.Emps;
            if (DataType.IsNullOrEmpty(emps) == true)
                emps = "@";
            if (emps.Contains("@" + WebUser.No) == false)
            {
                emps += WebUser.No + "," + WebUser.Name + "@";
            }
            gwf.Emps = emps;
            gwf.SetPara("IsBackTracking", this.IsBackTrack);
            gwf.Update();

            //退回消息事件
            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, this.HisNode.NodeID, PushMsgAttr.FK_Event, EventListNode.ReturnAfter);
            foreach (PushMsg pm in pms)
            {
                pm.DoSendMessage(this.HisNode, this.HisWork, null, null, null, toEmp);
            }
            //退回后事件
            atPara += "@SendToEmpIDs=" + toEmp;
            string text = ExecEvent.DoNode(EventListNode.ReturnAfter, this.HisNode, this.HisWork, null, atPara);
            if (text != null && text.Length > 1000)
                text = "退回事件:无返回信息.";
            if (text == null)
                text = "";
            // 返回退回信息.
            return info + ".\n\r" + text;
        }
        /// <summary>
        /// 合流点向分流点退回
        /// </summary>
        private string ExeReturn3_2()
        {
            //删除分流点与合流点之间的子线程数据。
            //if (this.ReturnToNode.ItIsStartNode == false)
            //    throw new Exception("@没有处理的模式。");

            //求出来退回到的 时间点。
            GenerWorkerList toWL = new GenerWorkerList();
            toWL.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);


            //如果是仅仅退回，就删除子线程数据。
            if (this.IsBackTrack == false)
            {
                //删除子线程节点数据。
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkFlowAttr.FID, this.WorkID);

                foreach (GenerWorkerList item in gwls)
                {
                    if (item.RDT.CompareTo(toWL.RDT) == -1)
                        continue;

                    /* 删除 子线程数据 */
                    if (DBAccess.IsExitsObject("ND" + item.NodeID) == true)
                        DBAccess.RunSQL("DELETE FROM ND" + item.NodeID + " WHERE OID=" + item.WorkID);

                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + this.WorkID + " AND FK_Node=" + item.NodeID);
                }

                //删除流程控制数据。
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + this.WorkID);
            }

            return ExeReturn1_1();
        }
        /// <summary>
        /// 普通节点到普通节点的退回
        /// </summary>
        /// <returns></returns>
        private string ExeReturn1_1()
        {
            //为软通小杨处理rpt变量不能替换的问题.
            GERpt rpt = this.HisNode.HisFlow.HisGERpt;
            rpt.OID = this.WorkID;
            if (rpt.RetrieveFromDBSources() == 0)
            {
                rpt.OID = this.FID;
                rpt.RetrieveFromDBSources();
            }
            rpt.Row.Add("ReturnMsg", Msg);
            //rpt.getRow().put("ReturnMsg", Msg); Java 的语法.

            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;

            //如果事件返回的信息不是 null，就终止执行。
            string msg = ExecEvent.DoNode(EventListNode.ReturnBefore, this.HisNode, this.HisWork, null,
                atPara);
            if (!String.IsNullOrEmpty(msg)) // 2019-08-28 zl。原来是 if(msg!=null)。返回空字符串表示执行成功，不应该终止。
                return msg;

            if (this.HisNode.FocusField != "")
            {
                try
                {
                    string focusField = "";
                    string[] focusFields = this.HisNode.FocusField.Split('@');
                    if (focusFields.Length >= 2)
                        focusField = focusFields[1];
                    else
                        focusField = focusFields[0];

                    // 把数据更新它。
                    this.HisWork.Update(focusField, "");
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError("退回时更新焦点字段错误:" + ex.Message);
                }
            }

            // 计算出来 退回到节点的应完成时间. 
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            //更新节点设置他的状态. 
            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=1 WHERE WorkID =" + gwf.WorkID + "  AND FK_Node=" + gwf.NodeID);
            DateTime dtOfShould;

            //增加天数. 考虑到了节假日.             
            dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, this.ReturnToNode.TimeLimit,
                this.ReturnToNode.TimeLimitHH, this.ReturnToNode.TimeLimitMM, this.ReturnToNode.TWay);

            // 应完成日期.
            string sdt = DataType.SysDateTimeFormat(dtOfShould);

            // 改变当前待办工作节点
            gwf.WFState = WFState.ReturnSta;
            gwf.NodeID = this.ReturnToNode.NodeID;
            gwf.NodeName = this.ReturnToNode.Name;
            gwf.SDTOfNode = sdt;

            gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
            gwf.HuiQianTaskSta = HuiQianTaskSta.None;
            gwf.HuiQianZhuChiRen = "";
            gwf.HuiQianZhuChiRenName = "";
            gwf.Paras_ToNodes = "";

            //获得所有的人员集合，退回到节点的.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID,
               GenerWorkerListAttr.WorkID, this.WorkID);

            if (gwls.Count == 0)
                throw new Exception("err@没有找到要退回到节点的数据,请与管理员联系[WorkID=" + this.WorkID + ",ReturnToNode.NodeID=" + this.ReturnToNode.NodeID + "]");

            //退回到人.
            Emp empReturn = new Emp(this.ReturnToEmp);
            gwf.TodoEmps = empReturn.UserID + "," + empReturn.Name + ";";
            gwf.TodoEmpsNum = 1;
            gwf.SetPara("IsBackTracking", this.IsBackTrack);
            gwf.Update();

            //更新待办状态. 
            bool isHave = false;// 在计算中心项目上，没有找到要更新的gwl. 出现不应该的异常.
            GenerWorkerList mygwl = null;
            foreach (GenerWorkerList item in gwls)
            {
                mygwl = item;
                if (item.EmpNo.Equals(this.ReturnToEmp) == false)
                {
                    item.Delete();
                    continue;
                }

                item.PassInt = 0;
                item.ItIsRead = false;
                item.SDT = sdt;
                item.RDT = DataType.CurrentDateTimess;
                item.Sender = WebUser.No + "," + WebUser.Name;
                item.Update();
                isHave = true;
            }

            //这里做了补偿的措施，否则就会出现异常数据. for:计算中心.
            if (isHave == false)
            {
                mygwl.DeptNo = WebUser.DeptNo;
                mygwl.DeptName = WebUser.DeptName;

                mygwl.EmpNo = WebUser.No;
                mygwl.EmpName = WebUser.Name;

                mygwl.PassInt = 0;
                mygwl.ItIsRead = false;
                mygwl.SDT = sdt;
                mygwl.RDT = DataType.CurrentDateTimess;
                mygwl.Sender = WebUser.No + "," + WebUser.Name;
                mygwl.Insert();
            }

            //更新流程报表数据.
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisNode.HisFlow.PTable + " SET  WFState=" + dbStr + "WFState, FlowEnder=" + dbStr + "FlowEnder, FlowEndNode=" + dbStr + "FlowEndNode WHERE OID=" + dbStr + "OID";
            ps.Add("WFState", (int)WFState.ReturnSta);
            ps.Add("FlowEnder", WebUser.No);
            ps.Add("FlowEndNode", ReturnToNode.NodeID);
            ps.Add("OID", this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录退回轨迹。
            /*ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = this.ReturnToEmp; //退回给。
            rw.BeiZhu = Msg;*/
            //杨玉慧 

            if (this.HisNode.TodolistModel == TodolistModel.Order
                || this.HisNode.TodolistModel == TodolistModel.Sharing
                || this.HisNode.TodolistModel == TodolistModel.TeamupGroupLeader
                || this.HisNode.TodolistModel == TodolistModel.Teamup)
            {
                // 为软通小杨屏蔽， 共享，顺序，协作模式的退回并原路返回的 问题. 
                //rw.IsBackTracking = true; /*如果是共享，顺序，协作模式，都必须是退回并原路返回.*/

                // 需要更新当前人待办的状态, 把1000作为特殊标记，让其发送时可以找到他.
                string sql = "UPDATE WF_GenerWorkerlist SET IsPass=1000 WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID + " AND FK_Emp='" + WebUser.No + "'";
                if (DBAccess.RunSQL(sql) == 0 && 1 == 2)
                    throw new Exception("@退回错误,没有找到要更新的目标数据.技术信息:" + sql);

                //杨玉慧 将流程的  任务池状态设置为  NONE
                sql = "UPDATE WF_GenerWorkFlow SET TaskSta=0 WHERE  WorkID=" + this.WorkID;
                if (DBAccess.RunSQL(sql) == 0 && 1 == 2)
                    throw new Exception("@退回错误，没有找到要更新的目标数据.技术信息:" + sql);
            }

            // 去掉了 else .
            //rw.IsBackTracking = this.IsBackTrack;

            //调用删除GenerWorkerList数据，不然会导致两个节点之间有垃圾数据，特别遇到中间有分合流时候。
            this.DeleteSpanNodesGenerWorkerListData();

            /*if (DataType.IsNullOrEmpty(this.ReturnCHDatas) == false)
            {
                string[] strs = this.ReturnCHDatas.Split('&');
                foreach (string str in strs)
                {
                    string[] param = str.Split('=');
                    if (param.Length == 2)
                        rw.SetValByKey(param[0].Replace("TB_", "").Replace("DDL_", "").Replace("CB_", ""), param[1]);
                }
            }
            rw.setMyPK(DBAccess.GenerGUID());
            rw.Insert();*/

            // 为电建增加一个退回并原路返回的日志类型.
            if (IsBackTrack == true)
            {
                // 加入track.
                this.AddToTrack(ActionType.ReturnAndBackWay, empReturn.UserID, empReturn.Name,
                    this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);
            }
            else
            {
                // 加入track.
                this.AddToTrack(ActionType.Return, empReturn.UserID, empReturn.Name,
                    this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);
            }

            /*try
            {
                // 记录退回日志. this.HisNode, this.ReturnToNode
                ReorderLog(this.ReturnToNode, this.HisNode, rw);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex.Message);
            }*/

            // 退回后发送的消息事件
            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, this.ReturnToNode.NodeID, PushMsgAttr.FK_Event, EventListNode.ReturnAfter);
            foreach (PushMsg pm in pms)
            {
                pm.DoSendMessage(this.ReturnToNode, this.HisWork, null, null, null, this.ReturnToEmp);
            }

            // 把消息
            atPara += "@SendToEmpIDs=" + this.ReturnToEmp;

            string text = ExecEvent.DoNode(EventListNode.ReturnAfter, this.HisNode, this.HisWork, null, atPara);
            if (text == null)
                text = "";

            if (text != null && text.Length > 1000)
                text = "退回事件:无返回信息.";

            // 返回退回信息.
            if (this.ReturnToNode.ItIsGuestNode)
            {
                return "工作已经被您退回到" + this.ReturnToNode.Name + ",<br/>退回给" + gwf.GuestNo + "," + gwf.GuestName + ".\n\r<br/>" + text;
            }
            else
            {
                return "工作已经被您退回到" + this.ReturnToNode.Name + ",<br/>退回给" + empReturn.UserID + "," + empReturn.Name + ".\n\r<br/>" + text;
            }
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at">类型</param>
        /// <param name="toEmp">到人员</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="toNDid">到节点</param>
        /// <param name="toNDName">到节点名称</param>
        /// <param name="msg">消息</param>
        public void AddToTrack(ActionType at, string toEmp, string toEmpName, int toNDid, string toNDName, string msg)
        {
            Track t = new Track();
            t.WorkID = this.WorkID;
            t.FlowNo = this.HisNode.FlowNo;
            t.FID = this.FID;
            t.RDT = DataType.CurrentDateTimess;
            t.HisActionType = at;

            t.NDFrom = this.HisNode.NodeID;
            t.NDFromT = this.HisNode.Name;

            t.EmpFrom = WebUser.No;
            t.EmpFromT = WebUser.Name;
            t.FlowNo = this.HisNode.FlowNo;

            if (toNDid == 0)
            {
                toNDid = this.HisNode.NodeID;
                toNDName = this.HisNode.Name;
            }


            t.NDTo = toNDid;
            t.NDToT = toNDName;

            t.EmpTo = toEmp;
            t.EmpToT = toEmpName;
            t.Msg = msg;
            t.Insert();
        }
        private string infoLog = "";


        /// <summary>
        /// 递归删除两个节点之间的数据
        /// </summary>
        /// <param name="nds">到达的节点集合</param>
        public void DeleteToNodesData(Nodes nds)
        {
            /*开始遍历到达的节点集合*/
            foreach (Node nd in nds)
            {
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                if (wk.Delete() == 0)
                {
                    wk.FID = this.WorkID;
                    if (wk.Delete(WorkAttr.FID, this.WorkID) == 0)
                        continue;
                }

                #region 删除当前节点数据，删除附件信息。
                // 删除明细表信息。
                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    ps = new Paras();
                    ps.SQL = "DELETE FROM " + dtl.PTable + " WHERE RefPK=" + dbStr + "WorkID";
                    ps.Add("WorkID", this.WorkID.ToString());
                    DBAccess.RunSQL(ps);
                }

                // 删除表单附件信息。
                DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                // 删除签名信息。
                DBAccess.RunSQL("DELETE FROM Sys_FrmEleDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                #endregion 删除当前节点数据。


                /*说明:已经删除该节点数据。*/
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2 ) AND FK_Node=" + dbStr + "FK_Node",
                    "WorkID1", this.WorkID, "WorkID2", this.WorkID, "FK_Node", nd.NodeID);

                if (nd.ItIsFL)
                {
                    /* 如果是分流 */
                    GenerWorkerLists wls = new GenerWorkerLists();
                    QueryObject qo = new QueryObject(wls);
                    qo.AddWhere(GenerWorkerListAttr.FID, this.WorkID);
                    qo.addAnd();

                    string[] ndStrs = nd.HisToNDs.Split('@');
                    string inStr = "";
                    foreach (string s in ndStrs)
                    {
                        if (DataType.IsNullOrEmpty(s) == true)
                            continue;
                        inStr += "'" + s + "',";
                    }
                    inStr = inStr.Substring(0, inStr.Length - 1);
                    if (inStr.Contains(",") == true)
                        qo.AddWhere(GenerWorkerListAttr.FK_Node, int.Parse(inStr));
                    else
                        qo.AddWhereIn(GenerWorkerListAttr.FK_Node, "(" + inStr + ")");

                    qo.DoQuery();
                    foreach (GenerWorkerList wl in wls)
                    {
                        Node subNd = new Node(wl.NodeID);
                        Work subWK = subNd.GetWork(wl.WorkID);
                        subWK.Delete();

                        //删除分流下步骤的节点信息.
                        DeleteToNodesData(subNd.HisToNodes);
                    }

                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                }
                DeleteToNodesData(nd.HisToNodes);
            }
        }
        private WorkNode DoReturnSubFlow(int backtoNodeID, string msg, bool isHiden)
        {
            Node nd = new Node(backtoNodeID);
            ps = new Paras();
            ps.SQL = "DELETE  FROM WF_GenerWorkerlist WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID  AND FID=" + dbStr + "FID";
            ps.Add("FK_Node", backtoNodeID);
            ps.Add("WorkID", this.HisWork.OID);
            ps.Add("FID", this.HisWork.FID);
            DBAccess.RunSQL(ps);

            // 找出分合流点处理的人员.
            ps = new Paras();
            ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerlist WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "FID";
            ps.Add("FID", this.HisWork.FID);
            ps.Add("FK_Node", backtoNodeID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count != 1)
                throw new Exception("@ system error , this values must be =1");

            string FK_Emp = dt.Rows[0][0].ToString();
            // 获取当前工作的信息.
            GenerWorkerList wl = new GenerWorkerList(this.HisWork.FID, this.HisNode.NodeID, FK_Emp);
            Emp emp = new Emp(FK_Emp);

            // 改变部分属性让它适应新的数据,并显示一条新的待办工作让用户看到。
            wl.ItIsPass = false;
            wl.WorkID = this.HisWork.OID;
            wl.FID = this.HisWork.FID;
            wl.EmpNo = FK_Emp;
            wl.EmpName = emp.Name;

            wl.NodeID = backtoNodeID;
            wl.NodeName = nd.Name;
            // wl.WarningHour = nd.WarningHour;
            wl.DeptNo = emp.DeptNo;
            wl.DeptName = emp.DeptText;

            DateTime dtNew = DateTime.Now;
            // dtNew = dtNew.AddDays(nd.WarningHour);

            wl.SDT = DataType.SysDateTimeFormat(dtNew); // DataType.CurrentDateTime;
            wl.FlowNo = this.HisNode.FlowNo;
            wl.Insert();

            GenerWorkFlow gwf = new GenerWorkFlow(this.HisWork.OID);
            gwf.NodeID = backtoNodeID;
            gwf.NodeName = nd.Name;
            gwf.DirectUpdate();

            ps = new Paras();
            ps.Add("FK_Node", backtoNodeID);
            ps.Add("WorkID", this.HisWork.OID);
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=3 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
            DBAccess.RunSQL(ps);

            /* 如果是隐性退回。*/
            /*BP.WF.ReturnWork rw = new ReturnWork();
            rw.WorkID = wl.WorkID;
            rw.ReturnToNode = wl.NodeID;
            rw.ReturnNode = this.HisNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;
            rw.ReturnToEmp = FK_Emp;
            rw.BeiZhu = msg;
            try
            {
                rw.setMyPK(rw.ReturnToNode + "_" + rw.WorkID + "_" + DateTime.Now.ToString("yyyyMMddhhmmss"));
                rw.Insert();
            }
            catch
            {
                rw.setMyPK(rw.ReturnToNode + "_" + rw.WorkID + "_" + DBAccess.GenerOID());
                rw.Insert();
            }*/

            // 加入track.
            this.AddToTrack(ActionType.Return, FK_Emp, emp.Name, backtoNodeID, nd.Name, msg);

            WorkNode wn = new WorkNode(this.HisWork.FID, backtoNodeID);
            if (Glo.IsEnableSysMessage)
            {
                //  WF.Port.WFEmp wfemp = new BP.Port.WFEmp(wn.HisWork.Rec);
                string title = string.Format("工作退回：流程:{0}.工作:{1},退回人:{2},需您处理",
                      wn.HisNode.FlowName, wn.HisNode.Name, WebUser.Name);

                BP.WF.Dev2Interface.Port_SendMsg(wn.HisWork.Rec, title, msg,
                    "RESub" + backtoNodeID + "_" + this.WorkID, BP.WF.SMSMsgType.SendSuccess, nd.FlowNo, nd.NodeID, this.WorkID, this.FID);
            }
            return wn;
        }
    }
}
