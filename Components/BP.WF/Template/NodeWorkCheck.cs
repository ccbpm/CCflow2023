﻿using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF;
namespace BP.WF.Template
{
    /// <summary>
    /// 附件类型
    /// </summary>
    public enum FWCAth
    {
        /// <summary>
        /// 使用附件
        /// </summary>
        None,
        /// <summary>
        /// 多附件
        /// </summary>
        MinAth,
        /// <summary>
        /// 单附件
        /// </summary>
        SingerAth,
        /// <summary>
        /// 图片附件
        /// </summary>
        ImgAth
    }
    /// <summary>
    /// 类型
    /// </summary>
    public enum FWCType
    {
        /// <summary>
        /// 审核组件
        /// </summary>
        Check,
        /// <summary>
        /// 日志组件
        /// </summary>
        DailyLog,
        /// <summary>
        /// 周报
        /// </summary>
        WeekLog,
        /// <summary>
        /// 月报
        /// </summary>
        MonthLog
    }
    /// <summary>
    /// 显示格式
    /// </summary>
    public enum FrmWorkShowModel
    {
        /// <summary>
        /// 表格
        /// </summary>
        Table,
        /// <summary>
        /// 自由显示
        /// </summary>
        Free
    }
    /// <summary>
    /// 显示控制方式
    /// </summary>
    public enum SFShowCtrl
    {
        /// <summary>
        /// 所有的子线程都可以看到
        /// </summary>
        All,
        /// <summary>
        /// 仅仅查看我自己的
        /// </summary>
        MySelf
    }
  
    /// <summary>
    /// 审核组件状态
    /// </summary>
    public enum FrmWorkCheckSta
    {
        /// <summary>
        /// 不可用
        /// </summary>
        Disable,
        /// <summary>
        /// 可用
        /// </summary>
        Enable,
        /// <summary>
        /// 只读
        /// </summary>
        Readonly
    }
    /// <summary>
    /// 协作模式下操作员显示顺序
    /// </summary>
    public enum FWCOrderModel
    {
        /// <summary>
        /// 按审批时间先后排序
        /// </summary>
        RDT = 0,
        /// <summary>
        /// 按照接受人员列表先后顺序(官职大小)
        /// </summary>
        SqlAccepter = 1
    }
    /// <summary>
    /// 审核组件
    /// </summary>
    public class NodeWorkCheckAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 傻瓜表单审核标签
        /// </summary>
        public const string FWCLab = "FWCLab";
        /// <summary>
        /// 是否可以审批
        /// </summary>
        public const string FWCSta = "FWCSta";
       
        /// <summary>
        /// H
        /// </summary>
        public const string FWC_H = "FWC_H";
       
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string FWCType = "FWCType";
        /// <summary>
        /// 附件
        /// </summary>
        public const string FWCAth = "FWCAth";
        /// <summary>
        /// 显示方式.
        /// </summary>
        public const string FWCShowModel = "FWCShowModel";
        /// <summary>
        /// 轨迹图是否显示?
        /// </summary>
        public const string FWCTrackEnable = "FWCTrackEnable";
        /// <summary>
        /// 历史审核信息是否显示?
        /// </summary>
        public const string FWCListEnable = "FWCListEnable";
        /// <summary>
        /// 是否显示所有的步骤？
        /// </summary>
        public const string FWCIsShowAllStep = "FWCIsShowAllStep";
        /// <summary>
        /// 默认审核信息
        /// </summary>
        public const string FWCDefInfo = "FWCDefInfo";
        /// <summary>
        /// 节点意见名称
        /// </summary>
        public const string FWCNodeName = "FWCNodeName";

        /// <summary>
        /// 如果用户未审核是否按照默认意见填充？
        /// </summary>
        public const string FWCIsFullInfo = "FWCIsFullInfo";
        /// <summary>
        /// 操作名词(审核，审定，审阅，批示)
        /// </summary>
        public const string FWCOpLabel = "FWCOpLabel";
        /// <summary>
        /// 操作人是否显示数字签名
        /// </summary>
        public const string SigantureEnabel = "SigantureEnabel";
        /// <summary>
        /// 操作字段
        /// </summary>
        public const string FWCFields = "FWCFields";
        /// <summary>
        /// 自定短语
        /// </summary>
        public const string FWCNewDuanYu = "FWCNewDuanYu";
        /// <summary>
        /// 是否显示未审核的轨迹
        /// </summary>
        public const string FWCIsShowTruck = "FWCIsShowTruck";
        /// <summary>
        /// 是否显示退回信息
        /// </summary>
        public const string FWCIsShowReturnMsg = "FWCIsShowReturnMsg";
        /// <summary>
        /// 协作模式下操作员显示顺序
        /// </summary>
        public const string FWCOrderModel = "FWCOrderModel";
        /// <summary>
        /// 审核意见显示模式()
        /// </summary>
        public const string FWCMsgShow = "FWCMsgShow";
        /// <summary>
        /// 审核意见版本号控制
        /// </summary>
        public const string FWCVer = "FWCVer";
        /// <summary>
        /// 签批字段
        /// </summary>
        public const string CheckField = "CheckField";
        /// <summary>
        /// 编号对应的字段
        /// </summary>
        public const string BillNoField = "BillNoField";
        /// <summary>
        /// 审核意见立场 不同意、不通过、同意、赞成
        /// </summary>
        public const string FWCView = "FWCView";
    }
    /// <summary>
    /// 节点表单的审核组件
    /// </summary>
    public class NodeWorkCheck : Entity
    {
        #region 属性
        /// <summary>
        /// 节点编号
        /// </summary>
        public string No
        {
            get
            {
                return "ND" + this.NodeID;
            }
            set
            {
                string nodeID = value.Replace("ND", "");
                this.NodeID = int.Parse(nodeID);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public FrmWorkCheckSta HisFrmWorkCheckSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeWorkCheckAttr.FWCSta);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCSta, (int)value);
            }
        }
        /// <summary>
        /// 显示格式(0=表格,1=自由.)
        /// </summary>
        public FrmWorkShowModel HisFrmWorkShowModel
        {
            get
            {
                return (FrmWorkShowModel)this.GetValIntByKey(NodeWorkCheckAttr.FWCShowModel);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCShowModel, (int)value);
            }
        }
        /// <summary>
        /// 附件类型
        /// </summary>
        public FWCAth FWCAth
        {
            get
            {
                return (FWCAth)this.GetValIntByKey(NodeWorkCheckAttr.FWCAth);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCAth, (int)value);
            }
        }
        /// <summary>
        /// 组件类型
        /// </summary>
        public FWCType HisFrmWorkCheckType
        {
            get
            {
                return (FWCType)this.GetValIntByKey(NodeWorkCheckAttr.FWCType);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCType, (int)value);
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string FWCLab
        {
            get
            {
                return this.GetValStrByKey(NodeWorkCheckAttr.FWCLab);
            }
        }
        /// <summary>
        /// 组件类型名称
        /// </summary>
        public string FWCTypeT
        {
            get
            {
                return this.GetValRefTextByKey(NodeWorkCheckAttr.FWCType);
            }
        }
         
        /// <summary>
        /// H
        /// </summary>
        public float FWC_H
        {
            get
            {
                return this.GetValFloatByKey(NodeWorkCheckAttr.FWC_H);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWC_H, value);
            }
        }
        public string FWC_Hstr
        {
            get
            {
                if (this.FWC_H == 0)
                    return "100%";
                return this.FWC_H + "px";
            }
        }
        /// <summary>
        /// 轨迹图是否显示?
        /// </summary>
        public bool FWCTrackEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCTrackEnable);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCTrackEnable, value);
            }
        }
        /// <summary>
        /// 历史审核信息是否显示?
        /// </summary>
        public bool FWCListEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCListEnable);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCListEnable, value);
            }
        }
        /// <summary>
        /// 在轨迹表里是否显示所有的步骤？
        /// </summary>
        public bool FWCIsShowAllStep
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCIsShowAllStep);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsShowAllStep, value);
            }
        }
        /// <summary>
        /// 是否显示轨迹在没有走到的节点
        /// </summary>
        public bool FWCIsShowTruck
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCIsShowTruck);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsShowTruck, value);
            }
        }
        /// <summary>
        /// 是否显示退回信息？
        /// </summary>
        public int FWCIsShowReturnMsg
        {
            get
            {
                return this.GetValIntByKey(NodeWorkCheckAttr.FWCIsShowReturnMsg);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsShowReturnMsg, value);
            }
        }
        /// <summary>
        /// 如果用户未审核是否按照默认意见填充?
        /// </summary>
        public bool FWCIsFullInfo
        {
            get
            {
                return this.GetValBooleanByKey(NodeWorkCheckAttr.FWCIsFullInfo);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCIsFullInfo, value);
            }
        }
        /// <summary>
        /// 默认审核信息
        /// </summary>
        public string FWCDefInfo
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCDefInfo);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCDefInfo, value);
            }
        }
        /// <summary>
        /// 节点名称.
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
        }
        /// <summary>
        /// 节点意见名称，如果为空则取节点名称.
        /// </summary>
        public string FWCNodeName
        {
            get
            {
                string str = this.GetValStringByKey(NodeWorkCheckAttr.FWCNodeName);
                if (DataType.IsNullOrEmpty(str))
                    return this.Name;
                return str;
            }
        }
        /// <summary>
        /// 操作名词(审核，审定，审阅，批示)
        /// </summary>
        public string FWCOpLabel
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCOpLabel);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCOpLabel, value);
            }
        }
        /// <summary>
        /// 操作字段
        /// </summary>
        public string FWCFields
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCFields);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCFields, value);
            }
        }
        /// <summary>
        /// 自定义常用短语
        /// </summary>
        public string FWCNewDuanYu
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCNewDuanYu);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCNewDuanYu, value);
            }
        }
        /// <summary>
        /// 是否显示数字签名？
        /// </summary>
        public int SigantureEnabel
        {
            get
            {
                return this.GetValIntByKey(NodeWorkCheckAttr.SigantureEnabel);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.SigantureEnabel, value);
            }
        }

        /// <summary>
        /// 协作模式下操作员显示顺序
        /// </summary>
        public FWCOrderModel FWCOrderModel
        {
            get
            {
                return (FWCOrderModel)this.GetValIntByKey(NodeWorkCheckAttr.FWCOrderModel, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCOrderModel, (int)value);
            }
        }
        /// <summary>
        /// 审核组件状态
        /// </summary>
        public FrmWorkCheckSta FWCSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeWorkCheckAttr.FWCSta, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCSta, (int)value);
            }
        }

        public int FWCVer
        {
            get
            {
                return this.GetValIntByKey(NodeWorkCheckAttr.FWCVer, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCVer,value);
            }
        }
        public string FWCView
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.FWCView);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCView, value);
            }
        }
        public string CheckField
        {
            get
            {
                return this.GetValStringByKey(NodeWorkCheckAttr.CheckField);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.CheckField, value);
            }
        }

        public int FWCMsgShow
        {
            get
            {
                return this.GetValIntByKey(NodeWorkCheckAttr.FWCMsgShow);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 重写主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        /// <summary>
        /// 审核组件
        /// </summary>
        public NodeWorkCheck()
        {
        }
        /// <summary>
        /// 审核组件
        /// </summary>
        /// <param name="no"></param>
        public NodeWorkCheck(string mapData)
        {
            if (mapData.Contains("ND") == false)
            {
                this.HisFrmWorkCheckSta = FrmWorkCheckSta.Disable;
                return;
            }

            string mapdata = mapData.Replace("ND", "");
            if (DataType.IsNumStr(mapdata) == false)
            {
                this.HisFrmWorkCheckSta = FrmWorkCheckSta.Disable;
                return;
            }

            try
            {
                this.NodeID = int.Parse(mapdata);
            }
            catch
            {
                return;
            }
            this.Retrieve();
        }
        /// <summary>
        /// 审核组件
        /// </summary>
        /// <param name="no"></param>
        public NodeWorkCheck(int nodeID)
        {
            this.NodeID = nodeID;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node", "审核组件");

                #region 基本设置
                map.AddGroupAttr("基本设置");
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10);
                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCSta, (int)FrmWorkCheckSta.Disable, "审核组件状态",
                   true, true, NodeWorkCheckAttr.FWCSta, "@0=禁用@1=启用@2=只读");

                map.AddTBString(NodeWorkCheckAttr.FWCLab, "审核信息", "显示标签", true, false, 0, 100, 10, true);


                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCType, (int)FWCType.Check, "审核组件", true, true,
                    NodeWorkCheckAttr.FWCType, "@0=审核组件@1=日志组件@2=周报组件@3=月报组件");

                map.AddTBString(NodeWorkCheckAttr.FWCNodeName, null, "节点意见名称", true, false, 0, 100, 10);

                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCAth, (int)FWCAth.None, "附件上传", true, true,
                   NodeWorkCheckAttr.FWCAth, "@0=不启用@1=多附件@2=单附件(暂不支持)@3=图片附件(暂不支持)");
                map.SetHelperAlert(NodeWorkCheckAttr.FWCAth,
                    "在审核期间，是否启用上传附件？启用什么样的附件？注意：附件的属性在节点表单里配置。"); //使用alert的方式显示帮助信息.

                map.AddTBString(NodeWorkCheckAttr.FWCOpLabel, "审核", "操作名词(审核/审阅/批示)", true, false, 0, 50, 10);
                map.AddTBString(NodeWorkCheckAttr.FWCDefInfo, "", "默认审核信息", true, false, 0, 50, 10);

                map.AddDDLSysEnum(NodeWorkCheckAttr.SigantureEnabel, 0, "签名方式", true, true, NodeWorkCheckAttr.SigantureEnabel, "@0=不签名@1=图片签名@2=写字板@3=电子签名@4=电子盖章@5=电子签名+盖章");
                map.SetHelperUrl(NodeWorkCheckAttr.SigantureEnabel, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=5415110&doc_id=31094");

                map.AddBoolean(NodeWorkCheckAttr.FWCIsFullInfo, true, "如果用户未审核是否按照默认意见填充？", true, true, true);
                //map.AddBoolean("WhetherStamp ", false, "是否启用盖章？", true, true, false);

                map.AddTBString(NodeWorkCheckAttr.FWCFields, null, "审批格式字段", true, false, 0, 50, 10, true);
                //map.AddTBString(NodeWorkCheckAttr.FWCNewDuanYu, null, "自定义常用短语(使用@分隔)", true, false, 0, 100, 10, true);


                //增加如下字段是为了查询与排序的需要.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 0, 5, 10);
                map.AddTBInt(NodeAttr.Step, 0, "步骤", false, false);


                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCVer, 1, "审核意见保存规则", true, true, NodeWorkCheckAttr.FWCVer,
                "@0=1个节点1个人保留1个意见@1=保留节点历史意见(默认)");

                //签批字段解决关联字段或者极简模式查询不到结果的修改
                map.AddTBString(NodeAttr.NodeFrmID, null, "节点表单ID", false, true, 0, 50, 10);
                String sql = "SELECT KeyOfEn AS No,Name From Sys_MapAttr Where UIContralType=14 AND (FK_MapData='ND@NodeID' OR FK_MapData='@NodeFrmID' )";
                map.AddDDLSQL(NodeWorkCheckAttr.CheckField, null, "签批字段", sql, true);

               // map.AddTBString(NodeWorkCheckAttr.CheckField, null, "签批字段", true, false, 0, 50, 10, false);

                map.AddTBString(NodeWorkCheckAttr.FWCView, null, "审核意见立场", true, false, 0, 200, 200,true);
                map.SetHelperAlert(NodeWorkCheckAttr.FWCView, "比如:同意,不同意,酌情处理, 多个立场用逗号分开,此立场可以作为方向条件.");
                #endregion 基本设置

                #region 外观设置.
                map.AddGroupAttr("外观设置");
                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCShowModel, (int)FrmWorkShowModel.Free, "显示方式",
                    true, true, NodeWorkCheckAttr.FWCShowModel, "@0=表格方式@1=自由模式"); //此属性暂时没有用.
                //协作模式下审核人显示顺序. add for yantai zongheng.
                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCOrderModel, 0, "协作模式下操作员显示顺序", true, true,
                  NodeWorkCheckAttr.FWCOrderModel, "@0=按审批时间先后排序@1=按照接受人员列表先后顺序(官职大小)");
                //for tianye , 多人审核的时候，不让其看到其他人的意见.
                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCMsgShow, 0, "审核意见显示方式", true, true,
                  NodeWorkCheckAttr.FWCMsgShow, "@0=都显示@1=仅显示自己的意见");
                map.AddTBFloat(NodeWorkCheckAttr.FWC_H, 300, "高度(0=100%)", true, false);

                map.AddDDLSysEnum(NodeWorkCheckAttr.FWCIsShowReturnMsg, 0, "退回信息显示规则", true, true,
                  NodeWorkCheckAttr.FWCIsShowReturnMsg, "@0=不显示@1=退回到的节点显示@2=显示全部退回信息");

                map.AddBoolean(NodeWorkCheckAttr.FWCTrackEnable, true, "轨迹图是否显示？", true, true, false);
                map.AddBoolean(NodeWorkCheckAttr.FWCListEnable, true, "历史审核信息是否显示？(否,仅出现意见框)", true, true, true);
                map.AddBoolean(NodeWorkCheckAttr.FWCIsShowAllStep, false, "在轨迹表里是否显示所有的步骤？", true, true);

                map.AddBoolean(NodeWorkCheckAttr.FWCIsShowTruck, false, "是否显示未审核的轨迹？", true, true, true);
                //map.AddBoolean(NodeWorkCheckAttr.FWCIsShowReturnMsg, false, "是否显示退回信息？", true, true, true);
                #endregion 外观

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            if (this.FWCAth == FWCAth.MinAth && this.FWCSta != FrmWorkCheckSta.Disable)
            {
                FrmAttachment workCheckAth = new FrmAttachment();
                bool isHave = workCheckAth.RetrieveByAttr(FrmAttachmentAttr.MyPK, "ND"+this.NodeID + "_FrmWorkCheck");
                //不包含审核组件
                if (isHave == false)
                {
                    workCheckAth = new FrmAttachment();
                    /*如果没有查询到它,就有可能是没有创建.*/
                    workCheckAth.setMyPK("ND" + this.NodeID + "_FrmWorkCheck");
                    workCheckAth.FrmID ="ND" + this.NodeID.ToString();
                    workCheckAth.NoOfObj = "FrmWorkCheck";
                    workCheckAth.Exts = "*.*";

                    //存储路径.
                   // workCheckAth.SaveTo = "/DataUser/UploadFile/";
                    workCheckAth.ItIsNote = false; //不显示note字段.
                    workCheckAth.ItIsVisable = false; // 让其在form 上不可见.

                    //位置.
           
                    workCheckAth.H = (float)150;

                    //多附件.
                    workCheckAth.UploadType = AttachmentUploadType.Multi;
                    workCheckAth.Name = "审核组件";
                    workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                    workCheckAth.Insert();
                }
            }

           

            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            fl.Update();
            GroupField gf = new GroupField();
            if (this.HisFrmWorkCheckSta == FrmWorkCheckSta.Disable)
            {
                gf.Delete(GroupFieldAttr.FrmID, this.No, GroupFieldAttr.CtrlType, GroupCtrlType.FWC);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.FrmID, this.No, GroupFieldAttr.CtrlType, GroupCtrlType.FWC) == false)
                {
                    gf = new GroupField();
                    gf.FrmID = "ND" + this.NodeID;
                    gf.CtrlType = GroupCtrlType.FWC;
                    gf.Lab = "审核组件";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 审核组件s
    /// </summary>
    public class NodeWorkChecks : Entities
    {
        #region 构造
        /// <summary>
        /// 审核组件s
        /// </summary>
        public NodeWorkChecks()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeWorkCheck();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeWorkCheck> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeWorkCheck>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeWorkCheck> Tolist()
        {
            System.Collections.Generic.List<NodeWorkCheck> list = new System.Collections.Generic.List<NodeWorkCheck>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeWorkCheck)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
