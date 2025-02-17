﻿using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.Port;
using BP.Difference;


namespace BP.WF.Template
{
    /// <summary>
    /// 条件属性
    /// </summary>
    public class CondAttr
    {
        #region 属性.
        /// <summary>
        /// 关联的从表-vue版本的数据格式.
        /// </summary>
        public const string RefPKVal = "RefPKVal";
        /// <summary>
        /// 关联的流程编号
        /// </summary>
        public const string RefFlowNo = "RefFlowNo";
        /// <summary>
        /// 数据来源
        /// </summary>
        public const string DataFrom = "DataFrom";
        public const string DataFromText = "DataFromText";
        /// <summary>
        /// 属性Key
        /// </summary>
        public const string AttrKey = "AttrKey";
        /// <summary>
        /// 名称
        /// </summary>
        public const string AttrName = "AttrName";
        /// <summary>
        /// 属性
        /// </summary>
        public const string FK_Attr = "FK_Attr";
        /// <summary>
        /// 运算符号
        /// </summary>
        public const string FK_Operator = "FK_Operator";
        /// <summary>
        /// 运算的值
        /// </summary>
        public const string OperatorValue = "OperatorValue";
        /// <summary>
        /// 操作值
        /// </summary>
        public const string OperatorValueT = "OperatorValueT";
        /// <summary>
        /// Node
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 条件类型
        /// </summary>
        public const string CondType = "CondType";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 启动的子流程(对子流程有效)
        /// </summary>
        public const string SubFlowNo = "SubFlowNo";
        /// <summary>
        /// 对方向条件有效
        /// </summary>
        public const string ToNodeID = "ToNodeID";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string FK_DBSrc = "FK_DBSrc";
        public const string Tag1 = "Tag1";
        public const string JSFX = "JSFX";
        public const string FrmID = "FrmID";
        public const string FrmName = "FrmName";
        #endregion 属性.

        #region 属性。
        /// <summary>
        /// 指定人员方式
        /// </summary>
        public const string SpecOperWay = "SpecOperWay";
        /// <summary>
        /// 指定人员方式的参数
        /// </summary>
        public const string SpecOperPara = "SpecOperPara";
        #endregion 属性。
    }
    /// <summary>
    /// 条件
    /// </summary>
    public class Cond : EntityMyPK
    {
        #region 身份.
        private WebUserCopy _webUserCopy = null;
        public WebUserCopy WebUser
        {
            get
            {
                if (_webUserCopy == null)
                {
                    _webUserCopy = new WebUserCopy();
                    _webUserCopy.LoadWebUser();
                }
                return _webUserCopy;
            }
            set
            {
                _webUserCopy = value;
            }
        }
        private GuestUserCopy _guestUserCopy = null;
        public GuestUserCopy GuestUser
        {
            get
            {
                if (_guestUserCopy == null)
                {
                    _guestUserCopy = new GuestUserCopy();
                    _guestUserCopy.LoadWebUser();
                }
                return _guestUserCopy;
            }
            set
            {
                _guestUserCopy = value;
            }
        }

        private Node _node = null;
        public  Node HisNode
        {
            get{
                if (_node == null)
                {
                    Node nd = new Node(this.NodeID);
                    _node = nd;
                    return _node;
                }
                return _node;
            }
            set{
                _node = value;
            }
        }

#endregion 身份.

#region 参数属性.
/// <summary>
/// 指定人员方式
/// </summary>
public SpecOperWay SpecOperWay
        {
            get
            {
                return (SpecOperWay)this.GetParaInt(CondAttr.SpecOperWay);
            }
            set
            {
                this.SetPara(CondAttr.SpecOperWay, (int)value);
            }
        }
        /// <summary>
        /// 指定人员参数
        /// </summary>
        public string SpecOperPara
        {
            get
            {
                return this.GetParaString(CondAttr.SpecOperPara);
            }
            set
            {
                this.SetPara(CondAttr.SpecOperPara, value);
            }
        }
        /// <summary>
        /// 求指定的人员.
        /// </summary>
        public string SpecOper
        {
            get
            {
                SpecOperWay way = this.SpecOperWay;
                if (way == SpecOperWay.CurrOper)
                    return BP.Web.WebUser.No;


                if (way == SpecOperWay.SpecNodeOper)
                {
                    string sql = "SELECT FK_Emp FROM WF_GenerWorkerlist WHERE NodeID=" + this.SpecOperPara + " AND WorkID=" + this.WorkID;
                    string fk_emp = DBAccess.RunSQLReturnStringIsNull(sql, null);
                    if (fk_emp == null)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的节点[" + this.SpecOperPara + "]，作为处理人，但是该节点上没有人员。查询SQL:" + sql);
                    return fk_emp;
                }

                if (way == SpecOperWay.SpecSheetField)
                {
                    if (this.en.Row.ContainsKey(this.SpecOperPara.Replace("@", "")) == false)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的字段[" + this.SpecOperPara + "]作为处理人，但是该字段不存在。");

                    return this.en.GetValStringByKey(this.SpecOperPara.Replace("@", ""));
                }

                if (way == SpecOperWay.CurrOper)
                {
                    if (this.en.Row.ContainsKey(this.SpecOperPara.Replace("@", "")) == false)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的字段[" + this.SpecOperPara + "]作为处理人，但是该字段不存在。");

                    return this.en.GetValStringByKey(this.SpecOperPara.Replace("@", ""));
                }

                if (way == SpecOperWay.SpenEmpNo)
                {
                    if (DataType.IsNullOrEmpty(this.SpecOperPara) == false)
                        throw new Exception("@您在配置方向条件时错误，求指定的人员的时候，按照指定的人员[" + this.SpecOperPara + "]作为处理人，但是人员参数没有设置。");
                    return this.SpecOperPara;
                }

                throw new Exception("@配置异常，没有判断的条件类型。");
            }
        }
        #endregion 参数属性.

        #region 基本属性.
        public GERpt en = null;
        /// <summary>
        /// 数据来源
        /// </summary>
        public ConnDataFrom HisDataFrom
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(CondAttr.DataFrom);
            }
            set
            {
                switch (value)
                {
                    case ConnDataFrom.NodeForm:
                        this.DataFromText = "节点表单";
                        break;
                    case ConnDataFrom.StandAloneFrm:
                        this.DataFromText = "独立表单";
                        break;
                    case ConnDataFrom.CondOperator:
                        this.DataFromText = "操作符";
                        break;
                    case ConnDataFrom.Depts:
                        this.DataFromText = "部门条件";
                        break;
                    case ConnDataFrom.Stas:
                        this.DataFromText = "角色条件";
                        break;
                    case ConnDataFrom.SQL:
                        this.DataFromText = "SQL条件";
                        break;
                    case ConnDataFrom.SQLTemplate:
                        this.DataFromText = "SQL模板条件";
                        break;
                    case ConnDataFrom.Paras:
                        this.DataFromText = "参数条件";
                        break;
                    case ConnDataFrom.Url:
                        this.DataFromText = "URL条件";
                        break;
                    case ConnDataFrom.WebApi:
                        this.DataFromText = "WebAPI条件";
                        break;
                    case ConnDataFrom.WorkCheck:
                        this.DataFromText = "审核组件立场";
                        break;
                    default:
                        break;
                }

                this.SetValByKey(CondAttr.DataFrom, (int)value);
            }
        }
        public string RefPKVal
        {
            get
            {
                return this.GetValStringByKey(CondAttr.RefPKVal);
            }
            set
            {
                this.SetValByKey(CondAttr.RefPKVal, value);
            }
        }
        public string DataFromText
        {
            get
            {
                return this.GetValStringByKey(CondAttr.DataFromText);
            }
            set
            {
                this.SetValByKey(CondAttr.DataFromText, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(CondAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(CondAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 隶属流程编号，用于备份删除.
        /// </summary>
        public string RefFlowNo
        {
            get
            {
                return this.GetValStringByKey(CondAttr.RefFlowNo);
            }
            set
            {
                this.SetValByKey(CondAttr.RefFlowNo, value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DBSrcNo
        {
            get
            {
                return this.GetValStringByKey(CondAttr.FK_DBSrc);
            }
            set
            {
                this.SetValByKey(CondAttr.FK_DBSrc, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStringByKey(CondAttr.Note);
            }
            set
            {
                this.SetValByKey(CondAttr.Note, value);
            }
        }
        /// <summary>
        /// 条件类型(表单条件，角色条件，部门条件，开发者参数)
        /// </summary>
        public CondType CondType
        {
            get
            {
                return (CondType)this.GetValIntByKey(CondAttr.CondType);
            }
            set
            {
                this.SetValByKey(CondAttr.CondType, (int)value);
            }
        }
        public int CondTypeInt
        {
            get
            {
                return this.GetValIntByKey(CondAttr.CondType);
            }
            set
            {
                this.SetValByKey(CondAttr.CondType, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(CondAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(CondAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 对方向条件有效
        /// </summary>
        public int ToNodeID
        {
            get
            {
                return this.GetValIntByKey(CondAttr.ToNodeID);
            }
            set
            {
                this.SetValByKey(CondAttr.ToNodeID, value);
            }
        }
        #endregion

        #region 重写.
        protected override bool beforeUpdateInsertAction()
        {
            if (DataType.IsNullOrEmpty(this.RefFlowNo) == true)
            {
                if (this.CondType == CondType.Dir
                    || this.CondType == CondType.Node
                    || this.CondType == CondType.SubFlow)
                {
                    Node nd = new Node(this.NodeID);
                    this.RefFlowNo = nd.FlowNo;
                }

                if (this.CondType == CondType.Flow)
                {
                    this.RefFlowNo = this.FlowNo;
                    if (DataType.IsNullOrEmpty(this.RefFlowNo) == true)
                        throw new Exception("err@流程完成条件设置错误，没有给FlowNo赋值。");
                }

                //for vue版本数据格式.增加一个主从表的标记字段.
                if (this.CondType == CondType.Dir)
                    this.RefPKVal = this.FlowNo + '_' + this.NodeID + '_' + this.ToNodeID;
            }
            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeInsert()
        {
            //设置他的主键。
            this.setMyPK(DBAccess.GenerGUID());
            return base.beforeInsert();
        }
        /// <summary>
        /// 清除数据缓存
        /// </summary>
        protected override void afterInsertUpdateAction()
        {
            Flow flow = new Flow(this.FlowNo);
            flow.ClearAutoNumCache(true);
            base.afterInsertUpdateAction();
        }
        protected override void afterDelete()
        {
            Flow flow = new Flow(this.FlowNo);
            flow.ClearAutoNumCache(true);
            base.afterDelete();
        }
        #endregion 重写.

        #region 实现基本的方方法
        /// <summary>
        /// 属性
        /// </summary>
        public string AttrNo
        {
            get
            {
                return this.GetValStringByKey(CondAttr.FK_Attr);
            }
            set
            {
                if (value == null)
                    throw new Exception("FK_Attr不能设置为null");

                value = value.Trim();

                this.SetValByKey(CondAttr.FK_Attr, value);

                BP.Sys.MapAttr attr = new BP.Sys.MapAttr(value);

                if (attr.LGType == FieldTypeS.Enum)
                {
                    /*是一个枚举类型的*/
                    SysEnum se = new SysEnum(attr.UIBindKey, this.OperatorValueInt);
                    this.OperatorValueT = se.Lab;
                }

                this.SetValByKey(CondAttr.AttrKey, attr.KeyOfEn);
                this.SetValByKey(CondAttr.AttrName, attr.Name);
            }
        }
        /// <summary>
        /// 要运算的实体属性
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(CondAttr.AttrKey);
            }
            set
            {
                this.SetValByKey(CondAttr.AttrKey, value);
            }
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttrName
        {
            get
            {
                return this.GetValStringByKey(CondAttr.AttrName);
            }
            set
            {
                this.SetValByKey(CondAttr.AttrName, value);
            }
        }
        /// <summary>
        /// Idx
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(CondAttr.Idx);
            }
            set
            {
                this.SetValByKey(CondAttr.Idx, value);
            }
        }
        /// <summary>
        /// 计算方向
        /// </summary>
        public bool JSFX
        {
            get
            {
                return this.GetValBooleanByKey(CondAttr.JSFX);
            }
        }
        /// <summary>
        /// 操作的值
        /// </summary>
        public string OperatorValueT
        {
            get
            {
                return this.GetValStringByKey(CondAttr.OperatorValueT);
            }
            set
            {
                this.SetValByKey(CondAttr.OperatorValueT, value);
            }
        }
        /// <summary>
        /// 运算符号
        /// </summary>
        public string OperatorNo
        {
            get
            {
                string s = this.GetValStringByKey(CondAttr.FK_Operator);
                if (s == null || s == "")
                    return "=";
                return s;
            }
            set
            {
                string val = "";

                switch (value)
                {
                    case "dengyu":
                        val = "=";
                        break;
                    case "dayu":
                        val = ">";
                        break;
                    case "dayudengyu":
                        val = ">=";
                        break;
                    case "xiaoyu":
                        val = "<";
                        break;
                    case "xiaoyudengyu":
                        val = "<=";
                        break;
                    case "budengyu":
                        val = "!=";
                        break;
                    case "like":
                        val = " LIKE ";
                        break;
                    default:
                        break;
                }

                this.SetValByKey(CondAttr.FK_Operator, val);
            }
        }
        /// <summary>
        /// 运算值
        /// </summary>
        public object OperatorValue
        {
            get
            {
                string s = this.GetValStringByKey(CondAttr.OperatorValue);
                s = s.Replace("~", "'");

                if (s.Contains("@") == true)
                {
                    if (s.Equals("@WebUser.No") == true)
                        return WebUser.No;
                    if (s.Equals("@WebUser.Name") == true)
                        return WebUser.Name;
                    if (s.Equals("@WebUser.FK_Dept") == true)
                        return WebUser.DeptNo;
                    if (s.Equals("@WebUser.FK_DeptName") == true)
                        return WebUser.DeptName;
                }
                return s;
            }
            set
            {
                this.SetValByKey(CondAttr.OperatorValue, value as string);
            }
        }
        /// <summary>
        /// 操作值Str
        /// </summary>
        public string OperatorValueStr
        {
            get
            {
                string sql = this.GetValStringByKey(CondAttr.OperatorValue);
                sql = sql.Replace("~", "'");
                return sql;
            }
        }
        /// <summary>
        /// 操作值int
        /// </summary>
        public int OperatorValueInt
        {
            get
            {
                return this.GetValIntByKey(CondAttr.OperatorValue);
            }
        }
        /// <summary>
        /// 操作值boolen
        /// </summary>
        public bool OperatorValueBool
        {
            get
            {
                return this.GetValBooleanByKey(CondAttr.OperatorValue);
            }
        }
        private Int64 _FID = 0;
        public Int64 FID
        {
            get
            {
                return _FID;
            }
            set
            {
                _FID = value;
            }
        }

        /// <summary>
        /// workid
        /// </summary>
        private Int64 _WorkID = 0;
        public Int64 WorkID
        {
            get
            {
                return _WorkID;
            }
            set
            {
                _WorkID = value;
            }
        }
        /// <summary>
        /// 条件消息
        /// </summary>
        public string MsgOfCond = "";
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public void DoUp(int fk_node)
        {
            int condtypeInt = (int)this.CondType;
            this.DoOrderUp(CondAttr.FK_Node, fk_node.ToString(), CondAttr.CondType, condtypeInt.ToString(), CondAttr.Idx);
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public void DoDown(int fk_node)
        {
            int condtypeInt = (int)this.CondType;
            this.DoOrderDown(CondAttr.FK_Node, fk_node.ToString(), CondAttr.CondType, condtypeInt.ToString(), CondAttr.Idx);
        }
        /// <summary>
        /// 方向条件-下移
        /// </summary>
        public void DoDown2020Cond()
        {
            if (this.CondType == CondType.Dir)
                this.DoOrderDown(CondAttr.FK_Node, this.NodeID, CondAttr.ToNodeID,
                    this.ToNodeID, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);

            if (this.CondType == CondType.Flow)
                this.DoOrderDown(CondAttr.FK_Node, this.NodeID, CondAttr.CondType, (int)CondType.Flow, CondAttr.Idx);

            if (this.CondType == CondType.SubFlow)
                this.DoOrderDown(CondAttr.FK_Node, this.NodeID, CondAttr.CondType, (int)CondType.SubFlow, CondAttr.Idx);

            if (this.CondType == CondType.Node)
                this.DoOrderDown(CondAttr.FK_Node, this.NodeID, CondAttr.CondType, (int)CondType.Node, CondAttr.Idx);
        }
        /// <summary>
        /// 方向条件-上移
        /// </summary>
        public void DoUp2020Cond()
        {
            if (this.CondType == CondType.Dir)
                this.DoOrderUp(CondAttr.FK_Node, this.NodeID, CondAttr.ToNodeID,
                    this.ToNodeID, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);

            if (this.CondType == CondType.Flow)
                this.DoOrderUp(CondAttr.FK_Node, this.NodeID, CondAttr.CondType, (int)CondType.Flow, CondAttr.Idx);

            if (this.CondType == CondType.SubFlow)
                this.DoOrderUp(CondAttr.FK_Node, this.NodeID, CondAttr.CondType, (int)CondType.SubFlow, CondAttr.Idx);

            if (this.CondType == CondType.Node)
                this.DoOrderUp(CondAttr.FK_Node, this.NodeID, CondAttr.CondType, (int)CondType.Node, CondAttr.Idx);
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 条件
        /// </summary>
        public Cond() { }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="mypk"></param>
        public Cond(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 这个条件能不能通过
        /// </summary>
        public virtual bool IsPassed
        {
            get
            {
                Node nd = new Node(this.NodeID);
                if (this.en == null)
                {
                    #region 实体不存在则进行重新初始化
                    GERpt en = nd.HisFlow.HisGERpt;
                    try
                    {
                        en.SetValByKey("OID", this.WorkID);
                        en.Retrieve();
                        en.ResetDefaultVal();
                        this.en = en;
                    }
                    catch (Exception ex)
                    {
                        //this.Delete();
                        return false;
                        //throw new Exception("@在取得判断条件实体[" + nd.EnDesc + "], 出现错误:" + ex.Message + "@错误原因是定义流程的判断条件出现错误,可能是你选择的判断条件工作类是当前工作节点的下一步工作造成,取不到该实体的实例.");
                    }
                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.Stas)
                {
                    #region 按角色控制
                    string strs = "," + this.OperatorValue.ToString() + ",";
                    strs += "," + this.OperatorValueT.ToString() + ",";
                    strs = strs.Replace("@", ",");

                    string strs1 = "";

                    BP.Port.DeptEmpStations sts = GetCondShengFenStations();
                    foreach (BP.Port.DeptEmpStation st in sts)
                    {
                        if (strs.Contains("," + st.StationNo + ",") == true)
                        {
                            this.MsgOfCond = "@以角色判断方向，条件为true：角色集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")角色:" + st.StationNo + st.StationT;

                            //处理计算方向.
                            if (this.JSFX == false)
                                return true;
                            else
                                return false;
                        }
                        strs1 += st.StationNo + "-" + st.StationT;
                    }

                    this.MsgOfCond = "@以角色判断方向，条件为false：角色集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")角色:" + strs1;

                    //处理计算方向.
                    if (this.JSFX == false)
                        return false;
                    else
                        return true;
                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.Depts)
                {
                    #region 按部门控制
                    string strs = "," + this.OperatorValue.ToString() + ",";
                    strs = strs.Replace("@", ",");

                    // 需要递归计算.
                    string subDeptStr = "";
                    if (this.ItIsSubDept)
                    {
                        foreach (string str in strs.Split(','))
                        {
                            subDeptStr = GenerDeptNosString(str, subDeptStr);
                        }
                    }
                    strs += subDeptStr + ",";

                    //计算出来当前人员的所有部门.
                    BP.Port.DeptEmps sts = GetCondShengFenDepts();
                    string strs1 = "";
                    foreach (BP.Port.DeptEmp st in sts)
                    {
                        if (strs.Contains("," + st.DeptNo + ",") == true)
                        {
                            this.MsgOfCond = "@以角色判断方向，条件为true：部门集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")部门:" + st.DeptNo;

                            //处理计算方向.
                            if (this.JSFX == false)
                                return true;
                            else
                                return false;
                        }

                        strs1 += st.DeptNo;
                    }

                    this.MsgOfCond = "@以部门判断方向，条件为false：部门集合" + strs + "，操作员(" + BP.Web.WebUser.No + ")部门:" + strs1;

                    //处理计算方向.
                    if (this.JSFX == false)
                        return false;
                    else
                        return true;
                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.SQL)
                {
                    #region 按SQL 计算
                    //this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.OperatorNo + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                    string sql = this.OperatorValueStr;
                    sql = sql.Replace("~", "'");
                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.DeptName);
                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

                    sql = sql.Replace("@WebUser.OrgNo", BP.Web.WebUser.OrgNo);
                    //获取参数值
                    //System.Collections.Specialized.NameValueCollection urlParams = HttpContextHelper.RequestParams; // System.Web.HttpContext.Current.Request.Form;
                    foreach (string key in HttpContextHelper.RequestParamKeys)
                    {
                        //循环使用数组
                        if (DataType.IsNullOrEmpty(key) == false && sql.Contains(key) == true)
                            sql = sql.Replace("@" + key, HttpContextHelper.RequestParams(key));
                        //sql = sql.Replace("@" + key, urlParams[key]);
                    }

                    if (en.ItIsOIDEntity == true)
                    {
                        sql = sql.Replace("@WorkID", en.GetValStrByKey("OID"));
                        sql = sql.Replace("@OID", en.GetValStrByKey("OID"));
                    }

                    if (sql.Contains("@") == true)
                    {
                        /* 如果包含 @ */
                        foreach (Attr attr in this.en.EnMap.Attrs)
                        {
                            sql = sql.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                        }
                    }

                    int result = 0;
                    //如果是本地数据源
                    if (DataType.IsNullOrEmpty(this.DBSrcNo) == true || this.DBSrcNo.Equals("local"))
                        result = DBAccess.RunSQLReturnValInt(sql, -1);
                    else
                    {
                        SFDBSrc dbSrc = new SFDBSrc(this.DBSrcNo);
                        result = dbSrc.RunSQLReturnInt(sql, 0);
                    }
                    if (result <= 0)
                        return false;

                    return true;
                    #endregion 按SQL 计算
                }

                if (this.HisDataFrom == ConnDataFrom.SQLTemplate)
                {
                    #region 按SQLTemplate 计算
                    //this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.OperatorNo + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                    string fk_sqlTemplate = this.OperatorValueStr;
                    SQLTemplate sqltemplate = new SQLTemplate();
                    sqltemplate.No = fk_sqlTemplate;
                    if (sqltemplate.RetrieveFromDBSources() == 0)
                        throw new Exception("@配置的SQLTemplate编号为[" + sqltemplate + "]被删除了,判断条件丢失.");

                    string sql = sqltemplate.Docs;
                    sql = sql.Replace("~", "'");
                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

                    if (en.ItIsOIDEntity == true)
                    {
                        sql = sql.Replace("@WorkID", en.GetValStrByKey("OID"));
                        sql = sql.Replace("@OID", en.GetValStrByKey("OID"));
                    }

                    if (sql.Contains("@") == true)
                    {
                        /* 如果包含 @ */
                        foreach (Attr attr in this.en.EnMap.Attrs)
                        {
                            sql = sql.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                        }
                    }

                    int result = 0;
                    //如果是本地数据源
                    if (DataType.IsNullOrEmpty(this.DBSrcNo) == true || this.DBSrcNo == "local")
                        result = DBAccess.RunSQLReturnValInt(sql, -1);
                    else
                    {
                        SFDBSrc dbSrc = new SFDBSrc(this.DBSrcNo);
                        result = dbSrc.RunSQLReturnInt(sql, 0);
                    }
                    if (result <= 0)
                        return false;

                    if (result >= 1)
                        return true;

                    throw new Exception("@您设置的sql返回值，不符合农芯BPM的要求，必须是0或大于等于1。");
                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.Url)
                {
                    #region URL 参数计算
                    string url = this.OperatorValueStr;
                    if (url.Contains("?") == false)
                        url = url + "?1=2";

                    url = url.Replace("@SDKFromServHost", BP.Difference.SystemConfig.AppSettings["SDKFromServHost"]);
                    url = BP.WF.Glo.DealExp(url, this.en, "");

                    #region 加入必要的参数.
                    if (url.Contains("&FlowNo") == false)
                        url += "&FlowNo=" + this.FlowNo;
                    if (url.Contains("&NodeID") == false)
                        url += "&NodeID=" + this.NodeID;

                    if (url.Contains("&WorkID") == false)
                        url += "&WorkID=" + this.WorkID;

                    if (url.Contains("&FID") == false)
                        url += "&FID=" + this.FID;

                    if (url.Contains("&Token") == false)
                        url += "&Token=" + BP.Web.WebUser.Token;

                    if (url.Contains("&UserNo") == false)
                        url += "&UserNo=" + BP.Web.WebUser.No;


                    #endregion 加入必要的参数.

                    #region 对url进行处理.
                    if (BP.Difference.SystemConfig.isBSsystem)
                    {
                        /*是bs系统，并且是url参数执行类型.*/
                        string myurl = HttpContextHelper.RequestRawUrl;// BP.Sys.Base.Glo.Request.RawUrl;
                        if (myurl.IndexOf('?') != -1)
                            myurl = myurl.Substring(myurl.IndexOf('?'));

                        myurl = myurl.Replace("?", "&");
                        string[] paras = myurl.Split('&');
                        foreach (string s in paras)
                        {
                            string[] strs = s.Split('=');

                            //如果已经有了这个参数.
                            if (url.Contains(strs[0] + "=") == true)
                                continue;

                            if (url.Contains(s))
                                continue;
                            url += "&" + s;
                        }
                        url = url.Replace("&?", "&");
                    }

                    //替换特殊的变量.
                    url = url.Replace("&?", "&");

                    if (BP.Difference.SystemConfig.isBSsystem == false)
                    {
                        /*非bs模式下调用,比如在cs模式下调用它,它就取不到参数. */
                    }


                    if (url.Contains("http") == false)
                    {
                        /*如果没有绝对路径 */
                        if (BP.Difference.SystemConfig.isBSsystem)
                        {
                            /*在cs模式下自动获取*/
                            string host = HttpContextHelper.RequestUrlHost;//BP.Sys.Base.Glo.Request.Url.Host;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + host + HttpContextHelper.RequestApplicationPath);//BP.Sys.Base.Glo.Request.ApplicationPath
                            else//BP.Sys.Base.Glo.Request.Url.Authority
                                url = "http://" + HttpContextHelper.RequestUrlAuthority + url;
                        }

                        if (BP.Difference.SystemConfig.isBSsystem == false)
                        {
                            /*在cs模式下它的baseurl 从web.config中获取.*/
                            string cfgBaseUrl = BP.Difference.SystemConfig.AppSettings["HostURL"];
                            if (DataType.IsNullOrEmpty(cfgBaseUrl))
                            {
                                string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                                BP.DA.Log.DebugWriteError(err);
                                throw new Exception(err);
                            }
                            url = cfgBaseUrl + url;
                        }
                    }
                    #endregion 对url进行处理.

                    #region 求url的值
                    try
                    {
                        url = url.Replace("'", "");
                        // url = url.Replace("//", "/");
                        // url = url.Replace("//", "/");
                        System.Text.Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                        string text = DataType.ReadURLContext(url, 8000, encode);
                        if (text == null)
                            //throw new Exception("@流程设计的方向条件错误，执行的URL错误:" + url + ", 返回为null, 请检查设置是否正确。");
                            return false;

                        if (DataType.IsNullOrEmpty(text) == true)
                            // throw new Exception("@错误，没有接收到返回值.");
                            return false;

                        if (DataType.IsNumStr(text) == false)
                            //throw new Exception("@错误，不符合约定的格式，必须是数字类型。");
                            return false;
                        try
                        {
                            float f = float.Parse(text);
                            if (f > 0)
                                return true;
                            else
                                return false;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@判断url方向出现错误:" + ex.Message + ",执行url错误:" + url);
                    }
                    #endregion 对url进行处理.

                    #endregion
                }

                if (this.HisDataFrom == ConnDataFrom.WebApi)
                {
                    #region WebApi接口
                    //返回值
                    string postData = "";
                    string apiUrl = this.OperatorValueStr;
                    if (apiUrl.Contains("@WebApiHost"))//可以替换配置文件中配置的webapi地址
                        apiUrl = apiUrl.Replace("@WebApiHost", BP.Difference.SystemConfig.AppSettings["WebApiHost"]);

                    //如果有参数
                    if (apiUrl.Contains("?"))
                    {
                        //api接口地址
                        string apiHost = apiUrl.Split('?')[0];
                        //api参数
                        string apiParams = apiUrl.Split('?')[1];
                        //参数替换
                        apiParams = BP.WF.Glo.DealExp(apiParams, this.en);
                        //执行POST
                        postData = BP.Tools.PubGlo.HttpPostConnect(apiHost, apiParams,"POST",true);

                        if (postData == "true")
                            return true;
                        else
                            return false;
                    }
                    else
                    {//如果没有参数，执行GET
                        postData = BP.Tools.PubGlo.HttpGet(apiUrl);
                        if (postData == "true")
                            return true;
                        else
                            return false;
                    }
                    #endregion WebApi接口
                }

                #region 审核组件的立场
                if (this.HisDataFrom == ConnDataFrom.WorkCheck)
                {
                    //获取当前节点的审核组件信息
                    string tag = BP.WF.Dev2Interface.GetCheckTag(this.FlowNo, this.WorkID, this.NodeID, WebUser.No);
                    if (tag.Contains("@FWCView=" + this.OperatorValue) == true)
                        return true;
                    return false;
                }
                #endregion 审核组件的立场

                if (this.HisDataFrom == ConnDataFrom.Paras)
                {
                    Hashtable ht = en.Row;
                    return BP.WF.Glo.CondExpPara(this.OperatorValueStr, ht, en.OID);
                }

                //从节点表单里判断.
                if (this.HisDataFrom == ConnDataFrom.NodeForm)
                {
                    if (en.EnMap.Attrs.Contains(this.AttrKey) == false)
                        throw new Exception("err@判断条件方向出现错误：实体：" + nd.EnDesc + " 属性" + this.AttrKey + "已经被删除方向条件判断失败.");

                    this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.OperatorNo + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                    return CheckIsPass(en);
                }

                //从独立表单里判断.
                if (this.HisDataFrom == ConnDataFrom.StandAloneFrm)
                {
                    MapAttr attr = new MapAttr(this.AttrNo);
                    attr.setMyPK(this.AttrNo);
                    if (attr.RetrieveFromDBSources() == 0)
                        throw new Exception("err@到达【" + this.ToNodeID + "】方向条件设置错误,原来做方向条件的字段:" + this.AttrNo + ",已经不存在了.");

                    GEEntity myen = new GEEntity(attr.FrmID, en.OID);
                    return CheckIsPass(myen);
                }
                return false;
            }
        }
        private DeptEmps GetCondShengFenDepts()
        {

            int specOperWay = this.GetValIntByKey(CondAttr.SpecOperWay);
            String specOperPara = this.SpecOperPara;
            DeptEmps des = new DeptEmps();
            String userNo = this.SpecOper;
		    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS && this.SpecOper.StartsWith(WebUser.OrgNo) == false)
			    userNo = WebUser.OrgNo + "_" + this.SpecOper;
            GenerWorkerList gwl = new GenerWorkerList();
            int i = 0;
		    switch(specOperWay){
			    case 0://发送人登录部门
				    des.Retrieve(DeptEmpAttr.FK_Dept, WebUser.DeptNo,DeptEmpAttr.FK_Emp,userNo);
				    break;
			    case 1: //发送人的所有部门
				    des.Retrieve("FK_Emp", userNo);
				    break;
			    case 2: //发送人使用的部门
			    case 3: //发送人使用部门的父级
				    int nodeID = this.HisNode.NodeID;
                    i = gwl.Retrieve(GenerWorkerListAttr.WorkID,this.WorkID, GenerWorkerListAttr.FK_Node, nodeID, GenerWorkerListAttr.FK_Emp, this.SpecOper);
				    if(i == 0)
					    throw new Exception("err@不可能出现的错误");
				    if(specOperWay == 2)
					    des.Retrieve(DeptEmpAttr.FK_Emp, userNo, DeptEmpAttr.FK_Dept, gwl.DeptNo);
				    if(specOperWay == 3){
					    Dept dept = new Dept();
                        dept.setNo(gwl.DeptNo);
					    if(dept.Retrieve() ==1 )
						    des.Retrieve(DeptEmpAttr.FK_Emp, userNo, DeptEmpAttr.FK_Dept, dept.No);
				    }

				    break;

			    case 10://指定节点提交人的使用部门
			    case 11: //指定节点提交人的所有部门
			    case 12: //指定节点提交人的主部门
				    if(DataType.IsNullOrEmpty(specOperPara) == true)
					    throw new Exception("err@方向条件，人员身份按指定节点提交人计算时请选择节点");
                    GenerWorkerLists gwls = new GenerWorkerLists();
                    i = gwls.Retrieve(GenerWorkerListAttr.WorkID,this.WorkID, GenerWorkerListAttr.FK_Node, specOperPara);
				    if(i == 0)
					    throw new Exception("err@不可能出现的错误");
                    gwl = (GenerWorkerList) gwls[0];
				    if(specOperWay == 10)
					    des.Retrieve("FK_Emp", gwl.EmpNo,"FK_Dept",gwl.DeptNo);
				    if(specOperWay == 11)
					    des.Retrieve("FK_Emp", gwl.EmpNo);
				    if(specOperWay == 13){
					    Emp emp = new Emp(gwl.EmpNo);
                        des.Retrieve("FK_Emp", gwl.EmpNo,"FK_Dept",emp.DeptNo);
				    }

                    break;
			    case 20: //字段(参数)人员的主部门
			    case 21: //字段(参数)人员的所有部门
			    case 22: //字段(参数)就是部门编号
				    if (DataType.IsNullOrEmpty(specOperPara) == true)
                        throw new Exception("err@方向条件，人员身份按表单字段人员计算时请选择表单字段");
                    string val = this.en.GetValStringByKey(specOperPara);
                    if (DataType.IsNullOrEmpty(val))
                        throw new Exception("err@方向条件，人员身份按表单字段人员计算时字段[" + specOperPara + "]的值为空");
                    if (specOperWay == 20)
                    {
                        Emp emp = new Emp();
                        emp.UserID=val;
                        if (emp.RetrieveFromDBSources() == 0)
                            throw new Exception("err@方向条件，人员身份按表单字段人员计算时未找到人员编号=[" + val + "]");
                        des.Retrieve("FK_Emp", val, "FK_Dept", emp.DeptNo);
                    }
                    if (specOperWay == 21)
                    {
                        des.Retrieve("FK_Emp", val);
                    }
                    if (specOperWay == 22)
                        des.Retrieve("FK_Dept", val);
                    break;
			    case 23: //系统(参数)就是部门编号
				    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    string val1 = gwf.GetParaString(specOperPara);
                    if (DataType.IsNullOrEmpty(val1))
                        throw new Exception("err@方向条件，人员身份按表单字段人员计算时字段[" + specOperPara + "]的值为空");
                    des.Retrieve("FK_Dept", val1);
                    break;
                 default:
				     throw new Exception("err@部门人员身份[" + specOperWay + "]未解析");
		    }
		    return des;
	    }
        /*
	     * 岗位人员身份
	     * @return
	     * @throws Exception
	     */
        private DeptEmpStations GetCondShengFenStations()
        {
            int specOperWay = this.GetValIntByKey(CondAttr.SpecOperWay);
            String specOperPara = this.SpecOperPara;
            DeptEmpStations sts = new DeptEmpStations();
            String userNo = this.SpecOper;
		    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS && this.SpecOper.StartsWith(WebUser.OrgNo) == false)
			    userNo = WebUser.OrgNo + "_" + this.SpecOper;
            GenerWorkerList gwl = new GenerWorkerList();
            int i = 0;
		    switch(specOperWay){
			    case 0://发送人登录部门下所有岗位
			    case 2: //发送人选择部门+岗位
				    int nodeID = this.HisNode.NodeID;
                    i = gwl.Retrieve(GenerWorkerListAttr.WorkID,this.WorkID, GenerWorkerListAttr.FK_Node, nodeID, GenerWorkerListAttr.FK_Emp, this.SpecOper);
				    if(i == 0)
					    throw new Exception("err@不可能出现的错误");
				    if(specOperWay == 0)
					    sts.Retrieve("FK_Emp", userNo,"FK_Dept",gwl.DeptNo);
				    if(specOperWay == 2)
					    sts.Retrieve("FK_Emp", userNo,"FK_Station",gwl.GetValStrByKey("StaNo"));
				    break;
			    case 1: //发送人的所有部门的岗位
				    sts.Retrieve("FK_Emp", userNo);
				    break;
			    case 10://指定节点提交人的使用部门下所有岗位
			    case 11: //指定节点提交人的所有部门的岗位
				    if(DataType.IsNullOrEmpty(specOperPara) == true)
					    throw new Exception("err@方向条件，人员身份按指定节点提交人计算时请选择节点");
                    GenerWorkerLists gwls = new GenerWorkerLists();
                    i = gwls.Retrieve(GenerWorkerListAttr.WorkID,this.WorkID, GenerWorkerListAttr.FK_Node, specOperPara);
				    if(i == 0)
					    throw new Exception("err@不可能出现的错误");
                    gwl = (GenerWorkerList) gwls[0];
				    if(specOperWay == 10)
					    sts.Retrieve("FK_Emp", gwl.EmpNo,"FK_Dept",gwl.DeptNo);
				    if(specOperWay == 11)
					    sts.Retrieve("FK_Emp", gwl.EmpNo);
				    break;
			    case 20: //字段(参数)人员的主部门下所有的岗位
			    case 21: //字段(参数)人员的所有部门的岗位
			    case 22: //字段(参数)就是岗位编号
				    if(DataType.IsNullOrEmpty(specOperPara) == true)
					    throw new Exception("err@方向条件，人员身份按表单字段人员计算时请选择表单字段");
                    string val = this.en.GetValStringByKey(specOperPara);
				    if(DataType.IsNullOrEmpty(val))
					    throw new Exception("err@方向条件，人员身份按表单字段人员计算时字段["+specOperPara+"]的值为空");
				    if(specOperWay == 20){
					    Emp emp = new Emp();
                        emp.UserID=val;
					    if(emp.RetrieveFromDBSources()==0)
						    throw new Exception("err@方向条件，人员身份按表单字段人员计算时未找到人员编号=["+val+"]");
                        sts.Retrieve("FK_Emp", val,"FK_Dept",emp.DeptNo);
				    }
				    if(specOperWay == 21){
					    sts.Retrieve("FK_Emp", val);
				    }
                    if (specOperWay == 22)
                        sts.Retrieve("FK_Station", val);
                    break;
                default:
				    throw new Exception("err@岗位人员身份[" + specOperWay + "]未解析");
		    }
		    return sts;
	    }
        private bool CheckIsPass(Entity en)
        {
            MapAttr attr = new MapAttr(this.AttrNo);
            attr.setMyPK(this.AttrNo);
            if (attr.RetrieveFromDBSources() == 0)
                throw new Exception("err@到达【" + this.ToNodeID + "】方向条件设置错误,原来做方向条件的字段:" + this.AttrNo + ",已经不存在了.");

            try
            {
                switch (this.OperatorNo.Trim().ToLower())
                {
                    case "<>":
                    case "!=":
                    case "budingyu":
                    case "budengyu": //不等于.
                        if (attr.ItIsNum == true)
                        {
                            if (en.GetValDoubleByKey(this.AttrKey) != Double.Parse(this.OperatorValue.ToString()))
                                return true;
                            else
                                return false;
                        }
                        if (en.GetValStringByKey(this.AttrKey).Equals(this.OperatorValue.ToString()) == false)
                            return true;
                        else
                            return false;
                    case "=":  // 如果是 = 
                    case "dengyu":
                        if (attr.ItIsNum == true)
                        {
                            if (en.GetValDoubleByKey(this.AttrKey) == Double.Parse(this.OperatorValue.ToString()))
                                return true;
                            else
                                return false;
                        }
                        if (en.GetValStringByKey(this.AttrKey).Equals(this.OperatorValue.ToString().Replace("\"", "")) == true)
                            return true;
                        else
                            return false;
                    case ">":
                    case "dayu":
                        if (en.GetValDoubleByKey(this.AttrKey) > Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;

                    //if (en.GetValDoubleByKey(this.AttrKey) > Double.Parse(this.OperatorValue.ToString()))
                    //    return true;
                    //else
                    //    return false;
                    case ">=":
                    case "dayudengyu":
                        if (en.GetValDoubleByKey(this.AttrKey) >= Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    case "<":
                    case "xiaoyu":
                        if (en.GetValDoubleByKey(this.AttrKey) < Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    //if (en.GetValDoubleByKey(this.AttrKey) < Double.Parse(this.OperatorValue.ToString()))
                    //    return true;
                    //else
                    //    return false;
                    case "<=":
                    case "xiaoyudengyu":
                        if (en.GetValDoubleByKey(this.AttrKey) <= Double.Parse(this.OperatorValue.ToString()))
                            return true;
                        else
                            return false;
                    case "like":
                    case "baohan":
                        if (en.GetValStringByKey(this.AttrKey).IndexOf(this.OperatorValue.ToString()) == -1)
                            return false;
                        else
                            return true;
                    case "notlike":
                        if (en.GetValStringByKey(this.AttrKey).IndexOf(this.OperatorValue.ToString()) == -1)
                            return true;
                        else
                            return false;
                    case "in":
                        string value = "," + this.OperatorValue + ",";
                        if (value.Contains("," + en.GetValStringByKey(this.AttrKey) + ","))
                            return true;
                        return false;
                    case "notin":
                        string val = "," + this.OperatorValue + ",";
                        if (val.Contains("," + en.GetValStringByKey(this.AttrKey) + ",")==false)
                            return true;
                        return false;
                    default:
                        throw new Exception("@没有找到操作符号(" + this.OperatorNo.Trim().ToLower() + ").");
                }
            }
            catch (Exception ex)
            {
                Node nd23 = new Node(this.NodeID);
                throw new Exception("@判断条件:Node=[" + this.NodeID + "," + nd23.EnDesc + "], 出现错误。@" + ex.Message + "。有可能您设置了非法的条件判断方式。");
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_Cond", "条件");

                map.AddMyPK();

                //for vue版本数据主从格式. 2022.9.28.
                map.AddTBString(CondAttr.RefPKVal, null, "关联主键", true, true, 0, 40, 20);

                //为流程的模板导出.
                map.AddTBString(CondAttr.RefFlowNo, null, "流程编号", true, true, 0, 5, 20);

                //用于整体流程的删除，导入，导出.
                map.AddTBString(CondAttr.RefFlowNo, null, "流程编号", true, true, 0, 5, 20);
                map.AddTBInt(CondAttr.FK_Node, 0, "节点ID", true, true);

                //@0=节点完成条件@1=流程条件@2=方向条件@3=启动子流程.
                map.AddTBInt(CondAttr.CondType, 0, "条件类型", true, true);
                map.AddTBString(CondAttr.FK_Flow, null, "流程", true, true, 0, 4, 20); 

                //对于启动子流程规则有效. *************************************
                map.AddTBString(CondAttr.SubFlowNo, null, "子流程编号", true, true, 0, 5, 20);

                //对方向条件有效. *************************************
                map.AddTBInt(CondAttr.ToNodeID, 0, "ToNodeID（对方向条件有效）", true, true);

                //条件字段. *************************************
                //@0=NodeForm表单数据,1=StandAloneFrm独立表单,2=Stas角色数据,3=Depts,4=按sql计算.
                //5,按sql模版计算.6,按参数,7=按Url @=100条件表达式.
                map.AddTBInt(CondAttr.DataFrom, 0, "条件数据来源", true, true);
                map.AddTBString(CondAttr.DataFromText, null, "条件数据来源T", true, true, 0, 4, 20);
                map.AddTBString(CondAttr.FK_Attr, null, "属性", true, true, 0, 80, 20);
                map.AddTBString(CondAttr.AttrKey, null, "属性键", true, true, 0, 60, 20);
                map.AddTBString(CondAttr.AttrName, null, "中文名称", true, true, 0, 500, 20);
                map.AddTBString(CondAttr.FK_Operator, "=", "运算符号", true, true, 0, 60, 20);
                map.AddTBString(CondAttr.OperatorValue, "", "要运算的值", true, true, 0, 4000, 20);
                map.AddTBString(CondAttr.OperatorValueT, "", "要运算的值T", true, true, 0, 4000, 20);
                map.AddTBString(CondAttr.Note, null, "备注", true, true, 0, 500, 20);
                map.AddTBString(CondAttr.FK_DBSrc, "local", "SQL的数据来源", false, true, 0, 50, 20);
                //参数 for wangrui add 2015.10.6. 条件为station,depts模式的时候，需要指定人员。
                map.AddTBAtParas(2000);
                map.AddTBInt(CondAttr.Idx, 0, "优先级", true, true);
                map.AddTBInt(CondAttr.JSFX, 0, "计算方向", true, true);


                map.AddTBString(CondAttr.FrmID, "", "表单ID", true, true, 0, 100, 20);
                map.AddTBString(CondAttr.FrmName, "", "表单名称", true, true, 0, 100, 20);

                map.AddTBInt(CondAttr.SpecOperWay, 0, "指定人员方式", false, true);
               // map.AddTBString(CondAttr.SpecOperWay, null, "指定人员方式", true, true, 0, 10, 20);
                map.AddTBString(CondAttr.SpecOperPara, null, " 指定人员方式的参数", false, true, 0, 50, 20);

                //用到了UIBindKey的存储.
                map.AddTBString(CondAttr.Tag1, null, "Tag1", true, true, 0, 100, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        private string GenerDeptNosString(string deptNo, string deptNos)
        {
            BP.Port.Depts ens = new BP.Port.Depts();
            ens.Retrieve(EntityTreeAttr.ParentNo, deptNo);

            foreach (BP.Port.Dept en in ens)
            {
                deptNos += "," + en.No;
                GenerDeptNosString(en.No, deptNos);
            }
            return deptNos;
        }

        /// <summary>
        /// 是否递归子部门 - 对部门条件计算有效.
        /// </summary>
        public bool ItIsSubDept
        {
            get
            {
                string val = this.GetValStringByKey("Tag1");
                if (DataType.IsNullOrEmpty(val) == true || val.Equals("0"))
                    return false;
                return true;
            }
        }


    }
    /// <summary>
    /// 条件s
    /// </summary>
    public class Conds : Entities
    {
        #region 属性
        public string ConditionDesc
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// 获得Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get { return new Cond(); }
        }
        public bool GenerResult(GERpt en, WebUserCopy webUser)
        {
            return GenerResult(en, webUser,null);
        }
        /// <summary>
        /// 执行计算
        /// </summary>
        /// <param name="runModel">模式</param>
        /// <returns></returns>
        public bool GenerResult(GERpt en,WebUserCopy webUser,Node nd)
        {

            try
            {
                if (this.Count == 0)
                    throw new Exception("err@没有要计算的条件，无法计算.");

                //给条件赋值.
                if (en != null)
                {
                    foreach (Cond cd in this)
                    {
                        cd.WorkID = en.OID;
                        cd.en = en;
                        cd.WebUser= webUser;
                        cd.HisNode = nd;
                    }
                }

                #region 首先计算简单的.
                //如果只有一个条件,就直接范围该条件的执行结果.
                if (this.Count == 1)
                {
                    Cond cond = this[0] as Cond;
                    return cond.IsPassed;
                }
                #endregion 首先计算简单的.

                #region 处理混合计算。
                string exp = "";
                foreach (Cond item in this)
                {
                    if (item.HisDataFrom == ConnDataFrom.CondOperator)
                    {
                        exp += " " + item.OperatorValue;
                        continue;
                    }

                    if (item.IsPassed)
                        exp += " 1=1 ";
                    else
                        exp += " 1=2 ";
                }

                //如果是混合计算.
                string sql = "";
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        sql = " SELECT TOP 1 No FROM WF_Emp WHERE " + exp;
                        break;
                    case DBType.MySQL:
                    case DBType.PostgreSQL:
                    case DBType.UX:
                    case DBType.HGDB:
                        sql = " SELECT No FROM WF_Emp WHERE " + exp + "    limit 1 ";
                        break;
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                    case DBType.DM:
                        sql = " SELECT No FROM WF_Emp WHERE " + exp + "  AND  rownum <=1 ";
                        break;
                    default:
                        throw new Exception("err@没有做的数据库类型判断.");
                }

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    return false;
                return true;
                #endregion 处理混合计算。
            }
            catch (Exception ex)
            {
                throw new Exception("err@计算条件出现错误:" + this.NodeID + " - " + ex.Message);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string MsgOfDesc
        {
            get
            {
                string msg = "";
                foreach (Cond c in this)
                {
                    msg += "@" + c.MsgOfCond;
                }
                return msg;
            }
        }
        public int NodeID = 0;
        #endregion

        #region 构造
        /// <summary>
        /// 条件
        /// </summary>
        public Conds()
        {
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        public Conds(string fk_flow)
        {
            this.Retrieve(CondAttr.FK_Flow, fk_flow);
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="ct">类型</param>
        /// <param name="nodeID">节点</param>
        public Conds(CondType ct, int nodeID, Int64 workid, GERpt enData)
        {
            this.NodeID = nodeID;
            this.Retrieve(CondAttr.FK_Node, nodeID, CondAttr.CondType, (int)ct, CondAttr.Idx);
            foreach (Cond en in this)
            {
                en.WorkID = workid;
                en.en = enData;
            }
        }
        /// <summary>
        /// 条件 - 配置信息
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="nodeID"></param>
        public Conds(CondType ct, int nodeID)
        {
            this.Retrieve(CondAttr.FK_Node, nodeID, CondAttr.CondType, (int)ct, CondAttr.Idx);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Cond> ToJavaList()
        {
            return (System.Collections.Generic.IList<Cond>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Cond> Tolist()
        {
            System.Collections.Generic.List<Cond> list = new System.Collections.Generic.List<Cond>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Cond)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
