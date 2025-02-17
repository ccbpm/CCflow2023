using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.BPFramework
{
    /// <summary>
    /// 学生 属性
    /// </summary>
    public class StudentAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// 地址
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// 登录系统密码
        /// </summary>
        public const string PWD = "PWD";
        /// <summary>
        /// 班级
        /// </summary>
        public const string FK_BanJi = "FK_BanJi";
        /// <summary>
        /// 年龄
        /// </summary>
        public const string Age = "Age";
        /// <summary>
        /// 邮件
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 注册时间
        /// </summary>
        public const string RegDate = "RegDate";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 是否特困生？
        /// </summary>
        public const string IsTeKunSheng = "IsTeKunSheng";
        /// <summary>
        /// 是否有重大疾病史？
        /// </summary>
        public const string IsJiBing = "IsJiBing";
        /// <summary>
        /// 是否偏远山区？
        /// </summary>
        public const string IsPianYuanShanQu = "IsPianYuanShanQu";
        /// <summary>
        /// 是否独生子
        /// </summary>
        public const string IsDuShengZi = "IsDuShengZi";
        /// <summary>
        /// 整治面貌
        /// </summary>
        public const string ZZMM = "ZZMM";
        #endregion

        /// <summary>
        /// 片区
        /// </summary>
        public const string FK_PQ = "FK_PQ";
        /// <summary>
        /// 省份
        /// </summary>
        public const string FK_SF = "FK_SF";
        /// <summary>
        /// 城市
        /// </summary>
        public const string FK_City = "FK_City";
    }
    /// <summary>
    /// 学生
    /// </summary>
    public class Student : BP.En.EntityNoName
    {
        #region 属性
        /// <summary>
        /// 登录系统密码
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.PWD);
            }
            set
            {
                this.SetValByKey(StudentAttr.PWD, value);
            }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age
        {
            get
            {
                return this.GetValIntByKey(StudentAttr.Age);
            }
            set
            {
                this.SetValByKey(StudentAttr.Age, value);
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Addr);
            }
            set
            {
                this.SetValByKey(StudentAttr.Addr, value);
            }
        }
        /// <summary>
        /// 性别
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(StudentAttr.XB);
            }
            set
            {
                this.SetValByKey(StudentAttr.XB, value);
            }
        }
        /// <summary>
        /// 性别名称
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.XB);
            }
        }
        /// <summary>
        /// 班级编号
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(StudentAttr.FK_BanJi, value);
            }
        }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Email);
            }
            set
            {
                this.SetValByKey(StudentAttr.Email, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Tel);
            }
            set
            {
                this.SetValByKey(StudentAttr.Tel, value);
            }
        }
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.RegDate);
            }
            set
            {
                this.SetValByKey(StudentAttr.RegDate, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 实体的权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {

                UAC uac = new UAC();
                //  uac.LoadRightFromCCGPM(this); //从GPM里面装载.
                // return uac;
                if (BP.Web.WebUser.No.Equals("admin")==true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                    uac.IsView = true;
                }
                else
                {
                    uac.IsView = true;
                }
                uac.IsImp = true;
                return uac;
            }
        }
        /// <summary>
        /// 学生
        /// </summary>
        public Student()
        {
        }
        /// <summary>
        /// 学生
        /// </summary>
        /// <param name="no"></param>
        public Student(string no)
            : base(no)
        {
        }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_Student", "学生");

                //基础信息.
                map.IsAllowRepeatName = true; //是否允许名称重复.
                map.IsAutoGenerNo = true; //是否自动生成编号.
                map.CodeStruct = "4"; // 4位数的编号，从 0001 开始，到 9999.

                #region 字段映射 - 普通字段.
                map.AddTBStringPK(StudentAttr.No, null, "学号", true, true, 4, 4, 90); // 如果设置自动编号字段必须是只读的.
                map.AddTBString(StudentAttr.Name, null, "名称", true, false, 0, 200, 70);
                //map.AddTBStringDoc(StudentAttr.PWD, null, "密码", true, false);
                map.AddTBString(StudentAttr.PWD, null, "密码", true, false, 0, 200, 50);

                map.AddTBString(StudentAttr.Addr, null, "地址", true, false, 0, 200, 100, false);
                map.AddTBInt(StudentAttr.Age, 18, "年龄", true, false);

                map.AddTBString(StudentAttr.Tel, null, "电话", true, false, 0, 200, 100);
                map.AddTBString(StudentAttr.Email, null, "邮件", true, false, 0, 200, 100);
                map.AddTBDateTime(StudentAttr.RegDate, null, "注册日期", true, true);

                //map.AddDDLEntities(StudentAttr.FK_PQ, null, "片区",new BP.CN.PQs(),true);
                //map.AddDDLEntities(StudentAttr.FK_SF, null, "省份",new BP.CN.SFs(),true);
                //map.AddDDLEntities(StudentAttr.FK_City, null, "城市",new BP.CN.Citys(),true);

                //枚举字段
                map.AddDDLSysEnum(StudentAttr.XB, 0, "性别", true, true, StudentAttr.XB, "@0=女@1=男");

                //外键字段.
                map.AddDDLEntities(StudentAttr.FK_BanJi, null, "班级", new BP.Demo.BPFramework.BanJis(), true);
               // map.AddTBString(StudentAttr.FK_BanJi, null, "班级", true, false, 0, 200, 100, true);

                //增加checkbox属性.
                map.AddBoolean(StudentAttr.IsDuShengZi, false, "是否是独生子？", true, true, true);
                map.AddBoolean(StudentAttr.IsJiBing, false, "是否有重大疾病？", true, true, true);
                map.AddBoolean(StudentAttr.IsPianYuanShanQu, false, "是否偏远山区？", true, true);
                map.AddBoolean(StudentAttr.IsTeKunSheng, false, "是否是特困生？", true, true);

                map.AddTBStringDoc(ResumeAttr.BeiZhu, null, "备注", true, false);

                //枚举字段 - 整治面貌.
                map.AddDDLSysEnum(StudentAttr.ZZMM, 0, "整治面貌", true, true, StudentAttr.ZZMM,
                    "@0=少先队员@1=团员@2=党员");

                map.AddTBAtParas(2000);

                #endregion 字段映射 - 普通字段.

                map.AddMyFile("简历");//上传单附件

                #region 设置查询条件。

                //String字段类型的模糊查询：定义方式map.SearchFields,其赋值格式是@名称=字段英文名
                //如果不设置该字段则进行关键字查询即所有string字段的模糊查询
               // map.SearchFields = "@名称=Name@地址=Addr@电话=Tel";
                //数值型字段查询：定义方式map.SearchFieldsOfNum，其赋值格式是@名称=字段英文名
                //查询方式是从Age1到Age2阶段查询：
                //①如果Age1有值，Age2无值，则查询大于等于Age1的结果集
                //②如果Age1无值，Age2有值，则查询小于等于Age2的结果集
                //③如果Age1有值，Age2有值，则查询大于等于Age1小于等于Age2的结果集
              
                //数值范围查询
                map.SearchFieldsOfNum = "@年龄=Age";

                //日期查询.
                map.DTSearchKey = "RegDate";
                map.DTSearchLable = "注册日期";
                map.DTSearchWay = Sys.DTSearchWay.ByDate; 


                //设置Search.htm页面查询条件换行的规则是增加的查询字段的宽度超过4000，则换行
                map.AddSearchAttr(StudentAttr.XB,4000);
                map.AddSearchAttr(StudentAttr.ZZMM);

                // map.AddSearchAttr(StudentAttr.FK_BanJi);
                //隐藏条件的查询
                // map.AddHidden(StudentAttr.ZZMM, " = ", "1");


                #endregion 设置查询条件

                //多对多的映射.
                map.AttrsOfOneVSM.Add(new StudentKeMus(), new KeMus(), StudentKeMuAttr.FK_Student,
                  StudentKeMuAttr.FK_KeMu, KeMuAttr.Name, KeMuAttr.No, "选修的科目");

                //明细表映射.
                map.AddDtl(new Resumes(), ResumeAttr.RefPK);

                //带有参数的方法.
                RefMethod rm = new RefMethod();
                rm.Title = "缴纳班费";
                rm.HisAttrs.AddTBDecimal("JinE", 100, "缴纳金额", true, false);
                rm.HisAttrs.AddTBString("Note", null, "备注", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoJiaoNaBanFei";
                rm.GroupName = "功能执行测试";
                //  rm.IsCanBatch = false; //是否可以批处理？
                map.AddRefMethod(rm);

                //不带有参数的方法.
                rm = new RefMethod();
                rm.Title = "注销学籍";
                rm.Warning = "您确定要注销吗？";
                rm.ClassMethodName = this.ToString() + ".DoZhuXiao";
                rm.IsForEns = true;
                rm.IsCanBatch = true; //是否可以批处理？
                map.AddRefMethod(rm);

                //不带有参数的方法.
                rm = new RefMethod();
                rm.Title = "发起劝退流程";
                rm.ClassMethodName = this.ToString() + ".DoStartFlow";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.IsCanBatch = false; //是否可以批处理？
                map.AddRefMethod(rm);


                //不带有参数的方法.
                rm = new RefMethod();
                rm.Title = "打印学生证";
                rm.ClassMethodName = this.ToString() + ".DoPrintStuLicence";
                rm.IsCanBatch = true; //是否可以批处理？
                map.AddRefMethod(rm);

                //不带有参数的方法.
                rm = new RefMethod();
                rm.Title = "单独打开页面演示";
                rm.ClassMethodName = this.ToString() + ".DoOpenit";
                rm.IsCanBatch = true; //是否可以批处理？
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                ////不带有参数的方法.
                //rm = new RefMethod();
                //rm.Title = "批量打印学生证";
                //rm.ClassMethodName = this.ToString() + ".EnsMothed";
                ////rm.IsForEns = true; //是否可以批处理？
                //rm.RefMethodType = RefMethodType.FuncBacthEntities; //是否可以批处理？
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoOpenit()
        {
            return "/WebForm1.aspx?No="+this.No;

        }
        /// <summary>
        /// 重写基类的方法.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            //在插入之前设置注册时间.
            this.RegDate = DataType.CurrentDataTime;
            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (this.Email.Length == 0)
                throw new Exception("@email 不能为空.");

            return base.beforeUpdateInsertAction();
        }

        #endregion 重写基类方法

        #region 方法
        public string DoPrintStuLicence()
        {

            BP.Pub.RTFEngine en = new Pub.RTFEngine();

            Student stu = new Student(this.No);

            en.HisGEEntity = stu; //当前的实体.

            //增加从表.
            BP.Demo.Resumes dtls = new Resumes();
            dtls.Retrieve(ResumeAttr.RefPK, stu.No);
            en.AddDtlEns(dtls);

            string saveTo = BP.Sys.SystemConfig.PathOfTemp; // \\DataUser\\Temp\\
            string billFileName = this.No + "StuTest.doc";

            //要生成的数据.
            en.MakeDoc(BP.Sys.SystemConfig.PathOfDataUser + "\\CyclostyleFile\\StudentDemo.rtf", saveTo, billFileName);

            string url = "/DataUser/Temp/" + billFileName;

            string info = "单据生成成功:<a href='" + url + "' >打印</a>，<a href='/SDKFlowDemo/App/PrintJoin.aspx'>拼接打印</a>";
            return info;
        }
        public string DoStartFlow()
        {
            return "/WF/MyFlow.htm?FK_Flow=045&XH=" + this.No + "&XM=" + this.Name;
        }
        /// <summary>
        /// 带有参数的方法:缴纳班费
        /// 说明：都要返回string类型.
        /// </summary>
        /// <returns></returns>
        public string DoJiaoNaBanFei(decimal jine, string note)
        {
            return "学号:" + this.No + ",姓名:" + this.Name + ",缴纳了:" + jine + "元,说明:" + note;
        }
        /// <summary>
        /// 无参数的方法:注销学籍
        /// 说明：都要返回string类型.
        /// </summary>
        /// <returns></returns>
        public string DoZhuXiao()
        {
            //    DBAccess.RunSQL("DELETE RR");
            //   DataTable DT=    DBAccess.RunSQLReturnTable("elect * from ");
            return "学号:" + this.No + ",姓名:" + this.Name + ",已经注销.";
        }
        /// <summary>
        /// 校验密码
        /// </summary>
        /// <param name="pass">原始密码</param>
        /// <returns>是否成功</returns>
        public bool CheckPass(string pass)
        {
            return this.PWD.Equals(pass);
        }
        #endregion

        protected override bool beforeDelete()
        {
            return base.beforeDelete();
        }

    }
    /// <summary>
    /// 学生s
    /// </summary>
    public class Students : BP.En.EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 学生s
        /// </summary>
        public Students() { }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Student();
            }
        }
        #endregion 重写基类方法

        #region 测试方法.
        public string EnsMothed()
        {
            return "EnsMothed@执行成功.";
        }
        public string EnsMothedParas(string para1, string para2)
        {
            return "EnsMothedParas@执行成功." + para1 + " - " + para2;
        }
        #endregion

    }
}
