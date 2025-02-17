﻿using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.WF.DTS
{
    /// <summary>
    /// 装载已经完成的流程数据到WF_GenerWorkflow
    /// </summary>
    public class LoadNDxxRpt2GernerWorkFlow : Method
    {
        /// <summary>
        /// 装载已经完成的流程数据到WF_GenerWorkflow
        /// </summary>
        public LoadNDxxRpt2GernerWorkFlow()
        {
            this.Title = "装载已经完成的流程数据到WF_GenerWorkflow（升级扩展流程数据完成模式下的旧数据查询不到的问题）";
            this.Help = "升级扩展流程数据完成模式下的旧数据查询不到的问题。";
            this.GroupName = "流程维护";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {

        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No.Equals("admin") == true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            BP.WF.Flows ens = new Flows();
            foreach (BP.WF.Flow en in ens)
            {
                string sql = "SELECT * FROM " + en.PTable + " WHERE OID NOT IN (SELECT WorkID FROM WF_GenerWorkFlow WHERE FK_Flow='" + en.No + "')";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    gwf.WorkID = Int64.Parse(dr[GERptAttr.OID].ToString());
                    gwf.FID = Int64.Parse(dr[GERptAttr.FID].ToString());

                    gwf.FlowSortNo = en.FlowSortNo;
                    gwf.SysType = en.SysType;

                    gwf.FlowNo = en.No;
                    gwf.FlowName = en.Name;
                    gwf.Title = dr[GERptAttr.Title].ToString();
                    gwf.WFState = (WFState)int.Parse(dr[GERptAttr.WFState].ToString());
                    //    gwf.WFSta = WFSta.Complete;

                    gwf.Starter = dr[GERptAttr.FlowStarter].ToString();
                    gwf.StarterName = dr[GERptAttr.FlowStarter].ToString();
                    gwf.RDT = dr[GERptAttr.FlowStartRDT].ToString();
                    gwf.NodeID = int.Parse(dr[GERptAttr.FlowEndNode].ToString());
                    gwf.DeptNo = dr[GERptAttr.FK_Dept].ToString();

                    BP.Port.Dept dept = null;
                    try
                    {
                        dept = new BP.Port.Dept(gwf.DeptNo);
                        gwf.DeptName = dept.Name;
                    }
                    catch
                    {
                        gwf.DeptName = gwf.DeptName;
                    }

                    try
                    {
                        gwf.PRI = int.Parse(dr[GERptAttr.PRI].ToString());
                    }
                    catch
                    {

                    }

                    //  gwf.SDTOfNode = dr[NDXRptBaseAttr.FK_Dept].ToString();
                    // gwf.SDTOfFlow = dr[NDXRptBaseAttr.FK_Dept].ToString();

                    gwf.PFlowNo = dr[GERptAttr.PFlowNo].ToString();
                    gwf.PWorkID = Int64.Parse(dr[GERptAttr.PWorkID].ToString());
                    gwf.PNodeID = int.Parse(dr[GERptAttr.PNodeID].ToString());
                    gwf.PEmp = dr[GERptAttr.PEmp].ToString();

                    //gwf.CFlowNo = dr[NDXRptBaseAttr.CFlowNo].ToString();
                    //gwf.CWorkID = Int64.Parse(dr[NDXRptBaseAttr.CWorkID].ToString());

                    gwf.GuestNo = dr[GERptAttr.GuestNo].ToString();
                    gwf.GuestName = dr[GERptAttr.GuestName].ToString();
                    gwf.BillNo = dr[GERptAttr.BillNo].ToString();
                    //gwf.FlowNote = dr[NDXRptBaseAttr.flowno].ToString();

                    gwf.SetValByKey("Emps", dr[GERptAttr.FlowEmps].ToString());
                    //   gwf.AtPara = dr[NDXRptBaseAttr.FK_Dept].ToString();
                    //  gwf.GUID = dr[NDXRptBaseAttr.gu].ToString();
                    gwf.Insert();
                }

            }
            return "执行成功...";
        }
    }
}
