﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.Template
{
    /// <summary>
    /// Btn属性
    /// </summary>
    public class BtnAttr
    {
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 发送标签
        /// </summary>
        public const string SendLab = "SendLab";
        /// <summary>
        /// 子线程按钮是否启用
        /// </summary>
        public const string ThreadEnable = "ThreadEnable";
        /// <summary>
        /// 是否可以删除（已经发出去的）子线程.
        /// </summary>
        public const string ThreadIsCanDel = "ThreadIsCanDel";
        /// <summary>
        /// 是否可以增加子线程.
        /// </summary>
        public const string ThreadIsCanAdd = "ThreadIsCanAdd";
        /// <summary>
        /// 是否可以移交
        /// </summary>
        public const string ThreadIsCanShift = "ThreadIsCanShift";
        /// <summary>
        /// 子线程按钮标签
        /// </summary>
        public const string ThreadLab = "ThreadLab";
        /// <summary>
        /// 子流程标签
        /// </summary>
        public const string SubFlowLab = "SubFlowLab";
        /// <summary>
        /// 子流程删除规则.
        /// </summary>
        public const string SubFlowCtrlRole = "SubFlowCtrlRole";
        /// <summary>
        /// 可否启用
        /// </summary>
        public const string SubFlowEnable = "SubFlowEnable";
        /// <summary>
        /// 保存是否启用
        /// </summary>
        public const string SaveEnable = "SaveEnable";
        /// <summary>
        /// 跳转规则
        /// </summary>
        public const string JumpWayLab = "JumpWayLab";
        /// <summary>
        /// 保存标签
        /// </summary>
        public const string SaveLab = "SaveLab";
        /// <summary>
        /// 退回是否启用
        /// </summary>
        public const string ReturnRole = "ReturnRole";
        /// <summary>
        /// 退回标签
        /// </summary>
        public const string ReturnLab = "ReturnLab";
        /// <summary>
        /// 退回的信息填写字段
        /// </summary>
        public const string ReturnField = "ReturnField";
        /// <summary>
        /// 打印单据标签
        /// </summary>
        public const string PrintDocLab = "PrintDocLab";
        /// <summary>
        /// 打印单据是否启用
        /// </summary>
        public const string PrintDocEnable = "PrintDocEnable";
        public const string PrintDocMyView = "PrintDocMyView";
        public const string PrintDocMyCC = "PrintDocMyCC";
        /// <summary>
        /// 移交是否启用
        /// </summary>
        public const string ShiftEnable = "ShiftEnable";
        /// <summary>
        /// 移交标签
        /// </summary>
        public const string ShiftLab = "ShiftLab";
        /// <summary>
        /// 查询标签
        /// </summary>
        public const string SearchLab = "SearchLab";
        /// <summary>
        /// 查询是否可用
        /// </summary>
        public const string SearchEnable = "SearchEnable";
        /// <summary>
        /// 轨迹
        /// </summary>
        public const string TrackLab = "TrackLab";
        /// <summary>
        /// 轨迹是否启用
        /// </summary>
        public const string TrackEnable = "TrackEnable";
        public const string TrackEnableMyView = "TrackEnableMyView";
        public const string TrackEnableMyCC = "TrackEnableMyCC";
        /// <summary>
        /// 抄送
        /// </summary>
        public const string CCLab = "CCLab";
        /// <summary>
        /// 抄送规则
        /// </summary>
        public const string CCRole = "CCRole";
        /// <summary>
        /// 二维码
        /// </summary>
        public const string QRCodeLab = "QRCodeLab";
        /// <summary>
        /// 二维码规则
        /// </summary>
        public const string QRCodeRole = "QRCodeRole";
        /// <summary>
        /// 删除
        /// </summary>
        public const string DelLab = "DelLab";
        /// <summary>
        /// 删除是否启用
        /// </summary>
        public const string DelEnable = "DelEnable";
        /// <summary>
        /// 结束流程
        /// </summary>
        public const string EndFlowLab = "EndFlowLab";
        /// <summary>
        /// 结束流程
        /// </summary>
        public const string EndFlowEnable = "EndFlowEnable";
        /// <summary>
        /// 发送按钮
        /// </summary>
        public const string SendJS = "SendJS";
        /// <summary>
        /// 挂起
        /// </summary>
        public const string HungLab = "HungLab";
        /// <summary>
        /// 是否启用挂起
        /// </summary>
        public const string HungEnable = "HungEnable";
        /// <summary>
        /// 查看父流程
        /// </summary>
        public const string ShowParentFormLab = "ShowParentFormLab";
        /// <summary>
        /// 是否启用查看父流程
        /// </summary>
        public const string ShowParentFormEnable = "ShowParentFormEnable";
        public const string ShowParentFormEnableMyView = "ShowParentFormEnableMyView";
        public const string ShowParentFormEnableMyCC = "ShowParentFormEnableMyCC";
        /// <summary>
        /// 数据批阅标签
        /// </summary>
        public const string FrmDBRemarkLab = "FrmDBRemarkLab";
        /// <summary>
        /// 数据批阅
        /// </summary>
        public const string FrmDBRemarkEnable = "FrmDBRemarkEnable";
        public const string FrmDBRemarkEnableMyView = "FrmDBRemarkEnableMyView";
        /// <summary>
        /// 审核
        /// </summary>
        public const string WorkCheckLab = "WorkCheckLab";
        /// <summary>
        /// 审核是否可用
        /// </summary>
        public const string WorkCheckEnable = "WorkCheckEnable";
        /// <summary>
        /// 批处理
        /// </summary>
        public const string BatchLab = "BatchLab";
        /// <summary>
        /// 批处理是否可用
        /// </summary>
        public const string BatchEnable = "BatchEnable";
        /// <summary>
        /// 加签
        /// </summary>
        public const string AskforLab = "AskforLab";
        /// <summary>
        /// 加签标签
        /// </summary>
        public const string AskforEnable = "AskforEnable";
        /// <summary>
        /// 会签标签
        /// </summary>
        public const string HuiQianLab = "HuiQianLab";
        /// <summary>
        /// 会签规则
        /// </summary>
        public const string HuiQianRole = "HuiQianRole";
        /// <summary>
        /// 协作模式被加签的人处理规则
        /// </summary>
       // public const string IsCanAddHuiQianer = "IsCanAddHuiQianer";
        /// <summary>
        /// 会签组长模式
        /// </summary>
        public const string HuiQianLeaderRole = "HuiQianLeaderRole";
        /// <summary>
        /// 加组长标签
        /// </summary>
        public const string AddLeaderLab = "AddLeaderLab";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string AddLeaderEnable = "AddLeaderEnable";
        /// <summary>
        /// 流转自定义 TransferCustomLab
        /// </summary>
        public const string TCLab = "TCLab";
        /// <summary>
        /// 是否启用-流转自定义
        /// </summary>
        public const string TCEnable = "TCEnable";
        /// <summary>
        /// 公文
        /// </summary>
        public const string WebOfficeLab = "WebOffice";
        /// <summary>
        /// 公文按钮标签
        /// </summary>
        public const string WebOfficeEnable = "WebOfficeEnable";
        /// <summary>
        /// 节点时限规则
        /// </summary>
        public const string CHRole = "CHRole";
        /// <summary>
        /// 节点时限lab
        /// </summary>
        public const string CHLab = "CHLab";
        /// <summary>
        /// 重要性 
        /// </summary>
        public const string PRILab = "PRILab";
        /// <summary>
        /// 是否启用-重要性
        /// </summary>
        public const string PRIEnable = "PRIEnable";
        /// <summary>
        /// 关注 
        /// </summary>
        public const string FocusLab = "FocusLab";
        /// <summary>
        /// 是否启用-关注
        /// </summary>
        public const string FocusEnable = "FocusEnable";
        /// <summary>
        /// 确认
        /// </summary>
        public const string ConfirmLab = "ConfirmLab";
        /// <summary>
        /// 是否启用确认
        /// </summary>
        public const string ConfirmEnable = "ConfirmEnable";
        /// <summary>
        /// 打印html
        /// </summary>
        public const string PrintHtmlLab = "PrintHtmlLab";
        /// <summary>
        /// 打印html
        /// </summary>
        public const string PrintHtmlEnable = "PrintHtmlEnable";
        /// <summary>
        /// 显示在抄送?
        /// </summary>
        public const string PrintHtmlMyView = "PrintHtmlMyView";
        /// <summary>
        /// 显示在查看器?
        /// </summary>
        public const string PrintHtmlMyCC = "PrintHtmlMyCC";
        /// <summary>
        /// 打印pdf
        /// </summary>
        public const string PrintPDFLab = "PrintPDFLab";
        /// <summary>
        /// 打印pdf
        /// </summary>
        public const string PrintPDFEnable = "PrintPDFEnable";
        public const string PrintPDFMyView = "PrintPDFMyView";
        public const string PrintPDFMyCC = "PrintPDFMyCC";
        /// <summary>
        /// 打印pdf规则
        /// </summary>
        public const string PrintPDFModle = "PrintPDFModle";
        /// <summary>
        /// 水印设置规则
        /// </summary>
        public const string ShuiYinModle = "ShuiYinModle";
        /// <summary>
        /// 打包下载
        /// </summary>
        public const string PrintZipLab = "PrintZipLab";
        /// <summary>
        /// 是否启用打包下载
        /// </summary>
        public const string PrintZipEnable = "PrintZipEnable";
        public const string PrintZipMyView = "PrintZipMyView";
        public const string PrintZipMyCC = "PrintZipMyCC";
        /// <summary>
        /// 分配
        /// </summary>
        public const string AllotLab = "AllotLab";
        /// <summary>
        /// 分配启用
        /// </summary>
        public const string AllotEnable = "AllotEnable";
        /// <summary>
        /// 选择接受人
        /// </summary>
        public const string SelectAccepterLab = "SelectAccepterLab";
        /// <summary>
        /// 是否启用选择接受人
        /// </summary>
        public const string SelectAccepterEnable = "SelectAccepterEnable";
        /// <summary>
        /// 备注
        /// </summary>
        public const string NoteLab = "NoteLab";
        /// <summary>
        //备注是否可用
        /// </summary>
        public const string NoteEnable = "NoteEnable";
        /// <summary>
        /// 帮助按钮
        /// </summary>
        public const string HelpLab = "HelpLab";
        /// <summary>
        /// 提示方式.
        /// </summary>
        public const string HelpRole = "HelpRole";
        /// <summary>
        /// 下一条
        /// </summary>
        public const string NextLab = "NextLab";
        /// <summary>
        /// 获得规则
        /// </summary>
        public const string NextRole = "NextRole";
        /// <summary>
        /// 公文标签
        /// </summary>
        public const string OfficeBtnLab = "OfficeBtnLab";
        /// <summary>
        /// 公文标签接受人
        /// </summary>
        public const string OfficeBtnEnable = "OfficeBtnEnable";
        /// <summary>
        /// 文件类型.
        /// </summary>
        public const string OfficeFileType = "OfficeFileType";
        /// <summary>
        /// 显示位置
        /// </summary>
        public const string OfficeBtnLocal = "OfficeBtnLocal";
        public const string DocLeftWord = "DocLeftWord";
        public const string DocRightWord = "DocRightWord";
        /// <summary>
        /// 工作方式
        /// </summary>
        public const string WebOfficeFrmModel = "WebOfficeFrmModel";
        /// <summary>
        /// 列表
        /// </summary>
        public const string ListLab = "ListLab";
        /// <summary>
        /// 是否启用-列表
        /// </summary>
        public const string ListEnable = "ListEnable";
        /// <summary>
        /// 数据版本的控制
        /// </summary>
        public const string FrmDBVerLab = "FrmDBVerLab";
        public const string FrmDBVerEnable = "FrmDBVerEnable";
        public const string FrmDBVerMyView = "FrmDBVerMyView";
        public const string FrmDBVerMyCC = "FrmDBVerMyCC";

        /// <summary>
        /// 小纸条
        /// </summary>
        public const string ScripLab = "ScripLab";
        public const string ScripRole = "ScripRole";
        /// <summary>
        /// 评论
        /// </summary>
        public const string FlowBBSLab = "FlowBBSLab";
        public const string FlowBBSRole = "FlowBBSRole";
        /// <summary>
        /// 即时通讯
        /// </summary>
        public const string IMLab = "IMLab";
        public const string IMEnable = "IMEnable";

        /// <summary>
        /// 是否启用催办
        /// </summary>
        public const string PressLab = "PressLab";
        public const string PressEnable = "PressEnable";
        /// <summary>
        /// 回滚
        /// </summary>
        public const string RollbackLab = "RollbackLab";
        public const string RollbackEnable = "RollbackEnable";

        /// <summary>
        /// 切换组织标签
        /// </summary>
        public const string ChangeDeptLab = "ChangeDeptLab";
        /// <summary>
        /// 是否启用切换组织
        /// </summary>
        public const string ChangeDeptEnable = "ChangeDeptEnable";
        /// <summary>
        /// 延期发送
        /// </summary>
        public const string DelayedSendLab = "DelayedSendLab";
        public const string DelayedSendEnable = "DelayedSendEnable";
    }
}
