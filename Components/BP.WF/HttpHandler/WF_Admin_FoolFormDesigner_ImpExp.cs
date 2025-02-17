﻿using System;
using System.Data;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.CCBill;
using BP.Difference;


namespace BP.WF.HttpHandler
{
    public class WF_Admin_FoolFormDesigner_ImpExp : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_FoolFormDesigner_ImpExp()
        {
        }

        #region 导出.
        /// <summary>
        ///下载
        /// </summary>
        /// <returns></returns>
        public string Exp_DownFormTemplete()
        {
            BP.WF.HttpHandler.WF_Admin_CCBPMDesigner en = new WF_Admin_CCBPMDesigner();
            return en.DownFormTemplete();
        }
        #endregion

        #region 导入
        /// <summary>
        /// 初始化 导入的界面 .
        /// </summary>
        /// <returns></returns>
        public string Imp_Init()
        {
            DataSet ds = new DataSet();

            string sql = "";
            DataTable dt;

            if (this.FlowNo != null)
            {
                //加入节点表单. 如果没有流程参数.

                Paras ps = new Paras();
                ps.SQL = "SELECT NodeID, Name  FROM WF_Node WHERE FK_Flow=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Flow ORDER BY NODEID ";
                ps.Add("FK_Flow", this.FlowNo);
                dt = DBAccess.RunSQLReturnTable(ps);

                dt.TableName = "WF_Node";

                if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                {
                    dt.Columns["NODEID"].ColumnName = "NodeID";
                    dt.Columns["NAME"].ColumnName = "Name";
                }

                if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                {
                    dt.Columns["nodeid"].ColumnName = "NodeID";
                    dt.Columns["name"].ColumnName = "Name";
                }


                ds.Tables.Add(dt);
            }

            #region 加入表单库目录.

            sql = "SELECT No,Name,ParentNo FROM Sys_FormTree ORDER BY  PARENTNO, IDX ";

            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FormTree";

            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            dt.Columns[2].ColumnName = "ParentNo";
            ds.Tables.Add(dt);

            //加入表单
            sql = "SELECT A.No, A.Name, A.FK_FormTree  FROM Sys_MapData A, Sys_FormTree B WHERE A.FK_FormTree=B.No";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";
            ds.Tables.Add(dt);
           
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            dt.Columns[2].ColumnName = "FK_FormTree";
           
            #endregion 加入表单库目录.

            #region 加入流程树目录.
            sql = "SELECT No,Name,ParentNo FROM WF_FlowSort ORDER BY  PARENTNO, IDX ";

            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_FlowSort";
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PARENTNO"].ColumnName = "ParentNo";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["no"].ColumnName = "No";
                dt.Columns["name"].ColumnName = "Name";
                dt.Columns["parentno"].ColumnName = "ParentNo";
            }

            ds.Tables.Add(dt);

            //加入表单
            sql = "SELECT No, Name, FK_FlowSort  FROM WF_Flow ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Flow";
            ds.Tables.Add(dt);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["no"].ColumnName = "No";
                dt.Columns["name"].ColumnName = "Name";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
            }
            #endregion 加入流程树目录.

            #region 数据源
            BP.Sys.SFDBSrcs ens = new BP.Sys.SFDBSrcs();
            ens.RetrieveAll();
            ds.Tables.Add(ens.ToDataTableField("SFDBSrcs"));
            #endregion

            //加入系统表.
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 如果是单据.




        /// <summary>
        /// 从本机装载表单模版
        /// </summary>
        /// <param name="fileByte">文件流</param>
        /// <param name="fk_mapData">表单模版ID</param>
        /// <param name="isClear">是否清空？</param>
        /// <returns>执行结果</returns>
        public string Imp_LoadFrmTempleteFromLocalFile()
        {
            try
            {
                if (HttpContextHelper.RequestFilesCount == 0)
                    return "err@请上传导入的模板文件.";

                //创建临时文件.
                string temp =  BP.Difference.SystemConfig.PathOfTemp + Guid.NewGuid() + ".xml";
                HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), temp);
                string fk_mapData = this.FrmID;
                MapData mapData = new MapData(fk_mapData);
                DataSet ds = new DataSet();
                ds.ReadXml(temp);

                //执行装载.
                MapData.ImpMapData(fk_mapData, ds);
                if (this.NodeID != 0)
                {
                    Node nd = new Node(this.NodeID);
                    nd.RepareMap(nd.HisFlow);
                }
                //清空缓存
                MapData mymd = new MapData(fk_mapData);
                mymd.RepairMap();
                if (mymd.HisEntityType == (int)EntityType.FrmBill)
                {
                    BP.CCBill.FrmBill bill = new FrmBill(mymd.No);
                    bill.EntityType = EntityType.FrmBill;
                    bill.BillNoFormat = "ccbpm{yyyy}-{MM}-{dd}-{LSH4}";

                    //设置默认的查询条件.
                    bill.SetPara("IsSearchKey", 1);
                    bill.SetPara("DTSearchWay", 0);

                    bill.Update();
                    bill.CheckEnityTypeAttrsFor_Bill();
                }

                #region 如果是实体 EnityNoName .
                if (mymd.HisEntityType == (int)EntityType.FrmDict)
                {
                    BP.CCBill.FrmDict entityDict = new FrmDict(mymd.No);
                    entityDict.BillNoFormat = "3"; //编码格式.001,002,003.
                    entityDict.BtnNewModel = 0;

                    //设置默认的查询条件.
                    entityDict.SetPara("IsSearchKey", 1);
                    entityDict.SetPara("DTSearchWay", 0);

                    entityDict.EntityType = EntityType.FrmDict;

                    entityDict.Update();
                    entityDict.CheckEnityTypeAttrsFor_EntityNoName();
                }
                SystemConfig.DoClearCache();
                return "执行成功.";
            }
            catch (Exception ex)
            {
                //第一次导入，可能因为没有字段，导致报错，系统会刷新一次，并修复字段
                //所以再执行一次导入
                try
                {
                    string fk_mapData = this.FrmID;

                    //读取上传的XML 文件.
                    DataSet ds = new DataSet();
                    //ds.ReadXml(path);
                    ds.ReadXml(HttpContextHelper.RequestFileStream(0));//this.context.Request.Files[0].InputStream

                    //执行装载.
                    MapData.ImpMapData(fk_mapData, ds);

                    if (this.NodeID != 0)
                    {
                        Node nd = new Node(this.NodeID);
                        nd.RepareMap(nd.HisFlow);
                    }
                    //清空缓存
                    MapData mymd = new MapData(fk_mapData);
                    mymd.RepairMap();
                    if (mymd.HisEntityType == (int)EntityType.FrmBill)
                    {
                        BP.CCBill.FrmBill bill = new FrmBill(mymd.No);
                        bill.EntityType = EntityType.FrmBill;
                        bill.BillNoFormat = "ccbpm{yyyy}-{MM}-{dd}-{LSH4}";

                        //设置默认的查询条件.
                        bill.SetPara("IsSearchKey", 1);
                        bill.SetPara("DTSearchWay", 0);

                        bill.Update();
                        bill.CheckEnityTypeAttrsFor_Bill();
                    }
                    #endregion 如果是单据.

                    #region 如果是实体 EnityNoName .
                    if (mymd.HisEntityType == (int)EntityType.FrmDict)
                    {
                        BP.CCBill.FrmDict entityDict = new FrmDict(mymd.No);
                        entityDict.BillNoFormat = "3"; //编码格式.001,002,003.
                        entityDict.BtnNewModel = 0;

                        //设置默认的查询条件.
                        entityDict.SetPara("IsSearchKey", 1);
                        entityDict.SetPara("DTSearchWay", 0);

                        entityDict.EntityType = EntityType.FrmDict;

                        entityDict.Update();
                        entityDict.CheckEnityTypeAttrsFor_EntityNoName();
                    }
                    SystemConfig.DoClearCache();
                    return "执行成功.";
                }
                catch (Exception newex)
                {
                    return "err@导入失败:" + newex.Message;
                }
            }
        }

        /// <summary>
        /// 从流程上copy表单
        /// @徐彪来调用.
        /// </summary>
        /// <returns></returns>
        public string Imp_CopyFromFlow()
        {
            string ndfrm = "ND" + int.Parse(this.FlowNo) + "01";
            return Imp_CopyFrm(null, ndfrm);
        }
        public string Imp_FrmEnsName()
        {

            MapData md = new MapData(this.FrmID);

            Entity en = BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity;

            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr item in attrs)
            {
                if (item.ItIsPK == true)
                    continue;

                MapAttr mapAttr = item.ToMapAttr;
                mapAttr.setMyPK(this.FrmID + "_" + item.Key);
                mapAttr.FrmID =this.FrmID;
                mapAttr.Save();
            }

            return this.EnsName+"的属性导入["+this.FrmID+"]成功.";
        }

        /// <summary>
        /// 从表单库导入
        /// 从节点导入
        /// </summary>
        /// <returns></returns>
        public string Imp_FromsCopyFrm()
        {
            return Imp_CopyFrm();
        }
        /// <summary>
        /// 从节点上Copy
        /// </summary>
        /// <param name="fromMapData">从表单ID</param>
        /// <param name="fk_mapdata">到表单ID</param>
        /// <param name="isClear">是否清楚现有的元素？</param>
        /// <param name="isSetReadonly">是否设置为只读？</param>
        /// <returns>执行结果</returns>
        public string Imp_CopyFrm(string toFrmID = null, string fromFrmID = null)
        {
            try
            {
                if (toFrmID == null)
                    toFrmID = this.FrmID;


                string fromMapData = fromFrmID;
                if (fromMapData == null)
                    fromMapData = this.GetRequestVal("FromFrmID");

                bool isClear = this.GetRequestValBoolen("IsClear");
                bool isSetReadonly = this.GetRequestValBoolen("IsSetReadonly");

                //首先初始化本部门的.
                MapData mymd = new MapData(toFrmID);
                string frmSort = mymd.FormTreeNo; //表单类别,防止表单类别冲掉,导致表单树看不到他.


                MapData mdFrom = new MapData(fromMapData);
                DataSet ds = BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(mdFrom.No);

                MapData.ImpMapData(toFrmID, ds);

                //设置为只读模式.
                if (isSetReadonly == true)
                    MapData.SetFrmIsReadonly(toFrmID);

                //清空缓存
                mymd = new MapData(toFrmID);

                // 如果是节点表单，就要执行一次修复，以免漏掉应该有的系统字段。
                if (toFrmID.Contains("ND") == true)
                {
                    string fk_node = toFrmID.Replace("ND", "");
                    Node nd = new Node(int.Parse(fk_node));
                    nd.RepareMap(nd.HisFlow);

                    //设置节点ID.
                    mymd.Name = nd.Name;
                    mymd.FormTreeNo = "";
                    mymd.Update();

                    //如果包含ND，就保持附件的从表一致.
                    if (fromMapData.IndexOf("ND") == 0)
                    {
                        MapDtls dtls = new MapDtls(fromMapData);

                    }


                }
                else
                {
                    mymd.FormTreeNo = frmSort;
                    mymd.Update();

                }

                mymd.RepairMap();
                if (mymd.HisEntityType == (int)EntityType.FrmBill)
                {
                    BP.CCBill.FrmBill bill = new FrmBill(mymd.No);
                    bill.EntityType = EntityType.FrmBill;
                    bill.BillNoFormat = "ccbpm{yyyy}-{MM}-{dd}-{LSH4}";

                    //设置默认的查询条件.
                    bill.SetPara("IsSearchKey", 1);
                    bill.SetPara("DTSearchWay", 0);

                    bill.Update();
                    bill.CheckEnityTypeAttrsFor_Bill();
                }
                #endregion 如果是单据.

                #region 如果是实体 EnityNoName .
                if (mymd.HisEntityType == (int)EntityType.FrmDict)
                {
                    BP.CCBill.FrmDict entityDict = new FrmDict(mymd.No);
                    entityDict.BillNoFormat = "3"; //编码格式.001,002,003.
                    entityDict.BtnNewModel = 0;

                    //设置默认的查询条件.
                    entityDict.SetPara("IsSearchKey", 1);
                    entityDict.SetPara("DTSearchWay", 0);

                    entityDict.EntityType = EntityType.FrmDict;

                    entityDict.Update();
                    entityDict.CheckEnityTypeAttrsFor_EntityNoName();
                }
                SystemConfig.DoClearCache();
                return "执行成功.";
                #endregion

            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        #region 04.从外部数据源导入
        /// <summary>
        /// 选择一个数据源，进入步骤2
        /// </summary>
        /// <returns></returns>
        public string Imp_Src_Step2_Init()
        {
            SFDBSrc src = new SFDBSrc(this.GetRequestVal("FK_SFDBSrc"));

            //获取所有的表/视图
            DataTable dtTables = src.GetTables();

            return BP.Tools.FormatToJson.ToJson(dtTables);
        }

        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <returns></returns>
        public string Imp_Src_Step2_GetColumns()
        {
            DataSet ds = new DataSet();

            //01.当前节点表单已经存在的列
            MapAttrs attrs = new MapAttrs(this.FrmID);
            ds.Tables.Add(attrs.ToDataTableField("MapAttrs"));

            //02.数据源表中的列
            SFDBSrc src = new SFDBSrc(this.GetRequestVal("FK_SFDBSrc"));
            DataTable tableColumns = src.GetColumns(this.GetRequestVal("STable"));
            tableColumns.TableName = "TableColumns";
            ds.Tables.Add(tableColumns);

            return BP.Tools.Json.ToJson(ds);
        }

        public string Imp_Src_Step3_Init()
        {
            DataSet ds = new DataSet();

            string SColumns = this.GetRequestVal("SColumns");
            SFDBSrc src = new SFDBSrc(this.GetRequestVal("FK_SFDBSrc"));
            DataTable tableColumns = src.GetColumns(this.GetRequestVal("STable"));

            //01.添加列
            DataTable dt = tableColumns.Clone();
            foreach (DataRow dr in tableColumns.Rows)
            {
                if (SColumns.Contains(dr["no"].ToString()))
                    dt.ImportRow(dr);
            }
            dt.TableName = "Columns";
            ds.Tables.Add(dt);

            //02.添加枚举
            SysEnums ens = new SysEnums(MapAttrAttr.MyDataType);
            ds.Tables.Add(ens.ToDataTableField("EnumsDataType"));
            ens = new SysEnums(MapAttrAttr.LGType);
            ds.Tables.Add(ens.ToDataTableField("EnumsLGType"));

            return BP.Tools.Json.ToJson(ds);

        }

        public string Imp_Src_Step3_Save()
        {

            string hidImpFields = this.GetRequestVal("hidImpFields");
            string[] fields = hidImpFields.TrimEnd(',').Split(',');

            MapData md = new MapData();
            md.No = this.FrmID;
            md.RetrieveFromDBSources();


            string msg = "导入字段信息:";
            bool isLeft = true;
            for (int i = 0; i < fields.Length; i++)
            {
                string colname = fields[i];

                MapAttr ma = new MapAttr();
                ma.setKeyOfEn(colname);
                ma.Name = this.GetRequestVal("TB_Desc_" + colname);
                ma.FrmID =this.FrmID;
                ma.MyDataType = int.Parse(this.GetRequestVal("DDL_DBType_" + colname));
                ma.setMaxLen(int.Parse(this.GetRequestVal("TB_Len_" + colname)));
                ma.UIBindKey = this.GetRequestVal("TB_BindKey_" + colname);
                ma.setMyPK(this.FrmID + "_" + ma.KeyOfEn);
                ma.LGType = BP.En.FieldTypeS.Normal;

                if (ma.UIBindKey != "")
                {
                    SysEnums se = new SysEnums();
                    se.Retrieve(SysEnumAttr.EnumKey, ma.UIBindKey);
                    if (se.Count > 0)
                    {
                        ma.setMyDataType(DataType.AppInt);
                        ma.LGType = BP.En.FieldTypeS.Enum;
                        ma.setUIContralType(BP.En.UIContralType.DDL);
                    }

                    SFTable tb = new SFTable();
                    tb.No = ma.UIBindKey;
                    if (tb.IsExits == true)
                    {
                        ma.setMyDataType(DataType.AppString);
                        ma.LGType = BP.En.FieldTypeS.FK;
                        ma.setUIContralType(BP.En.UIContralType.DDL);
                    }
                }

                if (ma.MyDataType == DataType.AppBoolean)
                    ma.UIContralType = BP.En.UIContralType.CheckBok;
                if (ma.IsExits)
                    continue;
                ma.Insert();

                msg += "\t\n字段:" + ma.KeyOfEn + "" + ma.Name + "加入成功.";

                isLeft = !isLeft;
            }


            return msg;

        }
        #endregion


    }
}
