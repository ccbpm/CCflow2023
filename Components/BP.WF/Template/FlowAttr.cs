﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程属性
    /// </summary>
    public class FlowAttr
    {
        #region 基本属性
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// CCType
        /// </summary>
        public const string CCType = "CCType";
        /// <summary>
        /// 抄送方式
        /// </summary>
        public const string CCWay = "CCWay";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
        /// <summary>
        /// 建立的日期。
        /// </summary>
        public const string CreateDate = "CreateDate";
        /// <summary>
        /// 创建人
        /// </summary>
        public const string Creater = "Creater";
        /// <summary>
        /// BillTable
        /// </summary>
        public const string BillTable = "BillTable";
        /// <summary>
        /// 开始工作节点类型
        /// </summary>
        public const string StartNodeType = "StartNodeType";
        /// <summary>
        /// StartNodeID
        /// </summary>
        public const string StartNodeID = "StartNodeID";
        /// <summary>
        /// 能不能流程Num考核。
        /// </summary>
        public const string IsCanNumCheck = "IsCanNumCheck";
        /// <summary>
        /// 是否显示附件
        /// </summary>
        public const string IsFJ = "IsFJ";
        /// <summary>
        /// 标题生成规则
        /// </summary>
        public const string TitleRole = "TitleRole";
        /// <summary>
        /// 生成标题的节点
        /// </summary>
        public const string TitleRoleNodes = "TitleRoleNodes";
        /// <summary>
        /// 流程类型
        /// </summary>
        public const string FlowType = "FlowType";
        /// <summary>
        /// 流程运行类型
        /// </summary>
        public const string FlowRunWay = "FlowRunWay";
        /// <summary>
        /// 工作模式
        /// </summary>
        public const string WorkModel = "WorkModel";
        /// <summary>
        /// 运行的设置
        /// </summary>
        public const string RunObj = "RunObj";
        /// <summary>
        /// 是否有Bill
        /// </summary>
        public const string NumOfBill = "NumOfBill";
        /// <summary>
        /// 明细表数量
        /// </summary>
        public const string NumOfDtl = "NumOfDtl";
        /// <summary>
        /// 是否可以启动？
        /// </summary>
        public const string IsCanStart = "IsCanStart";
        /// <summary>
        /// 是否可以在手机里启用?
        /// </summary>
        public const string IsStartInMobile = "IsStartInMobile";

        /// <summary>
        /// 是否自动计算未来的处理人？
        /// </summary>
        public const string IsFullSA = "IsFullSA";
        /// <summary>
        /// 类型
        /// </summary>
        public const string FlowAppType = "FlowAppType";
        /// <summary>
        /// 图像类型
        /// </summary>
        public const string ChartType = "ChartType";
        /// <summary>
        /// 流程计划完成日期设置规则
        /// </summary>
        public const string SDTOfFlowRole = "SDTOfFlowRole";
        public const string SDTOfFlowRoleSQL = "SDTOfFlowRoleSQL";
        /// <summary>
        /// 草稿
        /// </summary>
        public const string Draft = "Draft";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 参数
        /// </summary>
        public const string Paras = "Paras";
        /// <summary>
        /// 业务主表
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// 表单连接
        /// </summary>
        public const string FrmUrl = "FrmUrl";
        /// <summary>
        /// 表单URL
        /// </summary>
        public const string FlowDevModel = "FlowDevModel";
        /// <summary>
        /// 流程标记
        /// </summary>
        public const string FlowMark = "FlowMark";
        /// <summary>
        /// 流程事件实体
        /// </summary>
        public const string FlowEventEntity = "FlowEventEntity";
        /// <summary>
        /// 外部客户参与流程规则
        /// </summary>
        public const string GuestFlowRole = "GuestFlowRole";
        /// <summary>
        /// 单据编号格式
        /// </summary>
        public const string BillNoFormat = "BillNoFormat";
        /// <summary>
        /// 待办字段s
        /// </summary>
        public const string BuessFields = "BuessFields";
        /// <summary>
        /// 高级查询人员字段
        /// </summary>
        public const string AdvEmps = "AdvEmps";
        /// <summary>
        /// 数据权限控制方式
        /// </summary>
        public const string DRCtrlType = "DRCtrlType";
        /// <summary>
        /// 是否可以批量发起?
        /// </summary>
        public const string IsBatchStart = "IsBatchStart";
        /// <summary>
        /// 批量发起填写的字段.
        /// </summary>
        public const string BatchStartFields = "BatchStartFields";
        /// <summary>
        /// 是否是MD5
        /// </summary>
        public const string IsMD5 = "IsMD5";
        /// <summary>
        /// 是否是数据加密
        /// </summary>
        public const string IsJM = "IsJM";
        /// <summary>
        /// 是否启用数据版本控制
        /// </summary>
        public const string IsEnableDBVer = "IsEnableDBVer";

        public const string CCStas = "CCStas";
        public const string Note = "Note";
        /// <summary>
        /// 运行的SQL
        /// </summary>
        public const string RunSQL = "RunSQL";

        /// <summary>
        /// 流程轨迹中显示的Tab标签页的控制
        /// </summary>
        //public const string IsFrmEnable = "IsFrmEnable";
        public const string IsTruckEnable = "IsTruckEnable";
        public const string IsTimeBaseEnable = "IsTimeBaseEnable";
        public const string IsTableEnable = "IsTableEnable";
        //public const string IsOPEnable = "IsOPEnable";
        /// <summary>
        /// 排序方式
        /// </summary>
        public const string TrackOrderBy = "TrackOrderBy";
        /// <summary>
        /// 子流程轨迹图中的显示
        /// </summary>
        public const string SubFlowShowType = "SubFlowShowType";
        #endregion 基本属性

        #region 发起限制规则.
        /// <summary>
        /// 发起限制规则
        /// </summary>
        public const string StartLimitRole = "StartLimitRole";
        /// <summary>
        /// 规则内容
        /// </summary>
        public const string StartLimitPara = "StartLimitPara";
        /// <summary>
        /// 规则提示
        /// </summary>
        public const string StartLimitAlert = "StartLimitAlert";
        /// <summary>
        /// 提示时间
        /// </summary>
        public const string StartLimitWhen = "StartLimitWhen";
        #endregion 发起限制规则.

        #region 开始节点数据导入规则.
        /// <summary>
        /// 发起前置规则
        /// </summary>
        public const string StartGuideWay = "StartGuideWay";
        /// <summary>
        /// 超链接
        /// </summary>
        public const string StartGuideLink = "StartGuideLink";
        /// <summary>
        /// 标签
        /// </summary>
        public const string StartGuideLab = "StartGuideLab";
        /// <summary>
        /// 发起前置参数1
        /// </summary>
        public const string StartGuidePara1 = "StartGuidePara1";
        /// <summary>
        /// 发起前置参数2
        /// </summary>
        public const string StartGuidePara2 = "StartGuidePara2";
        /// <summary>
        /// StartGuidePara3
        /// </summary>
        public const string StartGuidePara3 = "StartGuidePara3";
        /// <summary>
        /// 是否启用开始节点的数据重置？
        /// </summary>
        public const string IsResetData = "IsResetData";
        /// <summary>
        /// 是否启用导入历史数据按钮?
        /// </summary>
        public const string IsImpHistory = "IsImpHistory";
        /// <summary>
        /// 是否自动装载上一笔数据？
        /// </summary>
        public const string IsLoadPriData = "IsLoadPriData";
        /// <summary>
        /// 是否启用数据模版？
        /// </summary>
        public const string IsDBTemplate = "IsDBTemplate";
        /// <summary>
        /// 系统类别（第2级流程树节点编号）
        /// </summary>
        public const string SysType = "SysType";
        #endregion 开始节点数据导入规则.

        #region 父子流程
        /// <summary>
        /// 版本号
        /// </summary>
        public const string Ver = "Ver";
        /// <summary>
        /// 子流程结束时，让父流程自动运行到下一步
        /// </summary>
        public const string IsToParentNextNode = "IsToParentNextNode";
        /// <summary>
        /// OrgNo
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 创建日期
        /// </summary>
        public const string RDT = "RDT";
        #endregion 父子流程

        #region 数据同步方式.
        /// <summary>
        /// 数据同步方式.
        /// </summary>
        public const string DataDTSWay = "DataDTSWay";
        /// <summary>
        /// 业务表主键
        /// </summary>
        public const string DTSBTablePK = "DTSBTablePK";
        /// <summary>
        /// 执行同步时间点
        /// </summary>
        public const string DTSTime = "DTSTime";
        /// <summary>
        /// 同步格式配置.
        /// </summary>
        public const string DTSSpecNodes = "DTSSpecNodes";
        public const string DTSField = "DTSField";
        public const string DTSFields = "DTSFields";
        /// <summary>
        /// 业务表
        /// </summary>
        public const string DTSBTable = "DTSBTable";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DTSDBSrc = "DTSDBSrc";
        /// <summary>
        /// webapi
        /// </summary>
        public const string DTSWebAPI = "DTSWebAPI";
        #endregion

        #region 权限组.
        /// <summary>
        /// 发起人可看
        /// </summary>
        public const string PStarter = "PStarter";
        /// <summary>
        /// 参与人可看
        /// </summary>
        public const string PWorker = "PWorker";
        /// <summary>
        /// 被抄送人可看
        /// </summary>
        public const string PCCer = "PCCer";
        /// <summary>
        /// 本部门人可看
        /// </summary>
        public const string PMyDept = "PMyDept";
        /// <summary>
        /// 直属上级部门可看
        /// </summary>
        public const string PPMyDept = "PPMyDept";
        /// <summary>
        /// 上级部门可看
        /// </summary>
        public const string PPDept = "PPDept";
        /// <summary>
        /// 平级部门可看
        /// </summary>
        public const string PSameDept = "PSameDept";
        /// <summary>
        /// 指定部门可看
        /// </summary>
        public const string PSpecDept = "PSpecDept";
        /// <summary>
        /// 指定的角色可看
        /// </summary>
        public const string PSpecSta = "PSpecSta";
        /// <summary>
        /// 指定的权限组可看
        /// </summary>
        public const string PSpecGroup = "PSpecGroup";
        /// <summary>
        /// 指定的人员可看
        /// </summary>
        public const string PSpecEmp = "PSpecEmp";
        #endregion 权限组.
        /// <summary>
        /// 流程发起测试人
        /// </summary>
        public const string Tester = "Tester";

        public const string DevModelType = "DevModelType";
        public const string DevModelPara = "DevModelPara";
    }
    /// <summary>
    /// 草稿规则
    /// </summary>
    public enum DraftRole
    {
        /// <summary>
        /// 不设置草稿
        /// </summary>
        None,
        /// <summary>
        /// 保存到待办
        /// </summary>
        SaveToTodolist,
        /// <summary>
        /// 保存到草稿箱
        /// </summary>
        SaveToDraftList

    }
    /// <summary>
    /// 流程计划完成日期计算规则
    /// </summary>
    public enum SDTOfFlowRole
    {
        /// <summary>
        /// 不计算
        /// </summary>
        None,
        /// <summary>
        /// 按照指定的字段计算
        /// </summary>
        BySpecDateField,
        /// <summary>
        /// 按照sql
        /// </summary>
        BySQL,
        /// <summary>
        /// 所有的节点之和.
        /// </summary>
        ByAllNodes,
        /// <summary>
        /// 按照设置的天数
        /// </summary>
        ByDays,
        /// <summary>
        /// 按照时间规则计算
        /// </summary>
        TimeDT,
        /// <summary>
        /// 为子流程时的规则
        /// </summary>
        ChildFlowDT,
        /// <summary>
        /// 按照发起字段不能重复规则
        /// </summary>
        AttrNonredundant
    }
    //方向条件控制
    public enum CondModel
    {
        /// <summary>
        /// 主观选择：下拉框模式
        /// </summary>
         DropDown = 0,
        /// <summary>
        /// 主观选择：按钮模式
        /// </summary>
         Radio = 1,
        /// <summary>
        /// 由连接线控制
        /// </summary>
         Line = 2,
        /// <summary>
        /// 发送后手工选择到达节点与接受人
        /// </summary>
         ManuallySelect = 3
    }
    ///自动发起
    public enum AutoStart
    {
        /// <summary>
        /// 手工启动（默认）
        /// </summary>
        None = 0,
        /// <summary>
        /// 按照指定的人员
        /// </summary>
        ByDesignee = 1,
        /// <summary>
        /// 数据集按时启动
        /// </summary>
        ByTineData = 2,
        /// <summary>
        /// 触发试启动
        /// </summary>
        ByTrigger = 3
    }
    /// <summary>
    /// 流程发起导航方式
    /// </summary>
    public enum StartGuideWay
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// SQL单条模式
        /// </summary>
        BySQLOne = 1,
        /// <summary>
        /// 按系统的URL-(子父流程)多条模式.
        /// </summary>
        SubFlowGuide = 2,
        /// <summary>
        /// 按系统的URL-(实体记录)单条模式
        /// </summary>
        BySystemUrlOneEntity = 3,
        /// <summary>
        /// 按系统的URL-(实体记录)多条模式
        /// </summary>
        SubFlowGuideEntity = 4,
        /// <summary>
        /// 历史数据
        /// </summary>
        ByHistoryUrl = 5,
        /// <summary>
        /// SQL多条模式
        /// </summary>
        BySQLMulti = 6,
        /// <summary>
        /// 按自定义的Url
        /// </summary>
        BySelfUrl = 10,
        /// <summary>
        /// 按照用户选择的表单.
        /// </summary>
        ByFrms = 8,
        /// <summary>
        /// 父子流程模式
        /// </summary>
        ByParentFlowModel = 9
    }
    /// <summary>
    /// 数据同步方案
    /// </summary>
    public enum DataDTSWay
    {
        /// <summary>
        /// 不同步
        /// </summary>
        None,
        /// <summary>
        /// 按数据源同步
        /// </summary>
        Syn,
        /// <summary>
        /// 按WebAPI方式同步
        /// </summary>
        WebAPI
    }
    /// <summary>
    /// 数据同步的时间
    /// </summary>
    public enum FlowDTSTime
    {
        /// <summary>
        /// 所有的节点发送后
        /// </summary>
        AllNodeSend,
        /// <summary>
        /// 指定的节点发送后
        /// </summary>
        SpecNodeSend,
        /// <summary>
        /// 当流程结束时
        /// </summary>
        WhenFlowOver
    }
    /// <summary>
    /// 流程数据存储模式
    /// </summary>
    public enum DataStoreModel
    {
        /// <summary>
        /// 轨迹模式
        /// </summary>
        ByCCFlow,
        /// <summary>
        /// 数据合并模式
        /// </summary>
        SpecTable
    }
    /// <summary>
    /// 流程部门权限控制类型(与报表查询相关)
    /// </summary>
    public enum FlowDeptDataRightCtrlType
    {
        /// <summary>
        /// 只能查看本部门
        /// </summary>
        MyDeptOnly,
        /// <summary>
        /// 查看本部门与下级部门
        /// </summary>
        MyDeptAndBeloneToMyDeptOnly,
        /// <summary>
        /// 按指定该流程的部门人员权限控制
        /// </summary>
        BySpecFlowDept,
        /// <summary>
        /// 不控制，任何人都可以查看任何部门的数据.
        /// </summary>
        AnyoneAndAnydept
    }
    /// <summary>
    /// 图像类型
    /// </summary>
    public enum FlowChartType
    {
        /// <summary>
        /// 几何图形
        /// </summary>
        Geometrical,
        /// <summary>
        /// 头像图形
        /// </summary>
        Icon
    }
}
