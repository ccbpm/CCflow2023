﻿using System;
using System.Data;
using BP.DA;
using BP.WF.Port;
using BP.Web;
using System.Linq;
using System.Collections;
using BP.Sys;
using BP.En;
using BP.Difference;
using BP.Port;
using System.Collections.Generic;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class GPMPage : DirectoryPageBase
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public GPMPage()
        {
        }
        #endregion 构造函数

        #region 签名.
        /// <summary>
        /// 图片签名初始化
        /// </summary>
        /// <returns></returns>
        public string Siganture_Init()
        {
            if (BP.Web.WebUser.NoOfRel == null)
                return "err@登录信息丢失";
            Hashtable ht = new Hashtable();
            ht.Add("No", BP.Web.WebUser.No);
            ht.Add("Name", BP.Web.WebUser.Name);
            ht.Add("FK_Dept", BP.Web.WebUser.DeptNo);
            ht.Add("FK_DeptName", BP.Web.WebUser.DeptName);
            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 签名保存
        /// </summary>
        /// <returns></returns>
        public string Siganture_Save()
        {
            var f = HttpContextHelper.RequestFiles(0);

            //判断文件类型.
            string fileExt = ",bpm,jpg,jpeg,png,gif,";
            string ext = f.FileName.Substring(f.FileName.LastIndexOf('.') + 1).ToLower();
            if (fileExt.IndexOf(ext + ",") == -1)
            {
                return "err@上传的文件必须是以图片格式:" + fileExt + "类型, 现在类型是:" + ext;
            }

            try
            {
                string tempFile = BP.Difference.SystemConfig.PathOfWebApp + "DataUser/Siganture/" + this.EmpNo + ".jpg";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                //f.SaveAs(tempFile);
                HttpContextHelper.UploadFile(f, tempFile);

                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                return "err@";
            }

            HttpContextHelper.UploadFile(f, BP.Difference.SystemConfig.PathOfWebApp + "DataUser/Siganture/" + this.EmpNo + ".jpg");
            return "上传成功！";
        }
        #endregion

        #region 组织结构维护.
        /// <summary>
        /// 初始化组织结构部门表维护.
        /// </summary>
        /// <returns></returns>
        public string Organization_Init()
        {

            BP.Port.Depts depts = new BP.Port.Depts();
            string parentNo = this.GetRequestVal("ParentNo");
            if (DataType.IsNullOrEmpty(parentNo) == true)
                parentNo = "0";

            if (DataType.IsNullOrEmpty(parentNo) == true)
            {
                if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    parentNo = BP.Web.WebUser.OrgNo;
                else
                    parentNo = "0";
            }

            QueryObject qo = new QueryObject(depts);
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                if (parentNo.Equals("0") == true)
                {
                    qo.AddWhere(BP.Port.DeptAttr.ParentNo, parentNo);
                    qo.addOr();
                    qo.AddWhereInSQL(BP.Port.DeptAttr.ParentNo, "SELECT No From Port_Dept Where ParentNo='0'");
                }
                else
                    qo.AddWhere(BP.Port.DeptAttr.ParentNo, parentNo);
            }
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc || SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                qo.AddWhere(BP.Port.DeptAttr.No, WebUser.OrgNo);
            }
            qo.addOrderBy(BP.Port.DeptAttr.Idx);
            qo.DoQuery();

            return depts.ToJson();

        }

        public string Organization_GetDeptsByParentNo()
        {

            BP.Port.Depts depts = new BP.Port.Depts();
            QueryObject qo = new QueryObject(depts);
            String parentNo = GetRequestVal("ParentNo");
            qo.AddWhere(BP.Port.DeptAttr.ParentNo, parentNo);
            qo.addOrderBy(BP.Port.DeptAttr.Idx);
            qo.DoQuery();
            return depts.ToJson("dt");
        }

        /// <summary>
        /// 获取本部门及人员信息
        /// </summary>
        /// <returns></returns>
        public string DeptEmp_Init()
        {

            BP.Port.Depts depts = new BP.Port.Depts();
            BP.Port.Emps emps = new BP.Port.Emps();
            string parentNo = this.GetRequestVal("ParentNo");
            QueryObject qo = new QueryObject(depts);
            if (DataType.IsNullOrEmpty(parentNo) == false)
            {
                if (parentNo.Equals("0") == true)
                {
                    emps.RetrieveIn(BP.Port.EmpAttr.FK_Dept, "SELECT No From Port_Dept Where ParentNo='0'");
                    qo.AddWhere(BP.Port.DeptAttr.ParentNo, parentNo);
                    qo.addOr();
                    qo.AddWhereInSQL(BP.Port.DeptAttr.ParentNo, "SELECT No From Port_Dept Where ParentNo='0'");

                }
                else
                {
                    emps.Retrieve(BP.Port.EmpAttr.FK_Dept, parentNo);
                    qo.AddWhere(BP.Port.DeptAttr.ParentNo, parentNo);
                    qo.addOr();
                    qo.AddWhere(BP.Port.DeptAttr.No, parentNo);
                }


            }
            qo.addOrderBy(BP.Port.DeptAttr.Idx);
            qo.DoQuery();
            DataSet ds = new DataSet();
            ds.Tables.Add(depts.ToDataTableField("Depts"));
            ds.Tables.Add(emps.ToDataTableField("Emps"));
            return BP.Tools.Json.ToJson(ds);

        }

        ///// <summary>
        ///// 获取该部门的所有人员
        ///// </summary>
        ///// <returns></returns>        
        //public string LoadDatagridDeptEmp_Init()
        //{
        //    string deptNo = this.GetRequestVal("deptNo");
        //    if (string.IsNullOrEmpty(deptNo))
        //    {
        //        return "{ total: 0, rows: [] }";
        //    }
        //    string orderBy = this.GetRequestVal("orderBy");


        //    string searchText = this.GetRequestVal("searchText");
        //    if (!DataType.IsNullOrEmpty(searchText))
        //    {
        //        searchText.Trim();
        //    }
        //    string addQue = "";
        //    if (!string.IsNullOrEmpty(searchText))
        //    {
        //        addQue = "  AND (pe.No like '%" + searchText + "%' or pe.Name like '%" + searchText + "%') ";
        //    }

        //    string pageNumber = this.GetRequestVal("pageNumber");
        //    int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
        //    //每页多少行
        //    string pageSize = this.GetRequestVal("pageSize");
        //    int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

        //    string sql = "(select pe.*,pd.name FK_DutyText from Port_Emp pe left join port_duty pd on pd.no = pe.fk_duty where pe.no in (select fk_emp from Port_DeptEmp where fk_dept='" + deptNo + "') "
        //        + addQue + " ) dbSo ";


        //    return DBPaging(sql, iPageNumber, iPageSize, "No", orderBy);

        //}

        ///// <summary>
        ///// 以下算法只包含 oracle mysql sqlserver 三种类型的数据库 qin
        ///// </summary>
        ///// <param name="dataSource">表名</param>
        ///// <param name="pageNumber">当前页</param>
        ///// <param name="pageSize">当前页数据条数</param>
        ///// <param name="key">计算总行数需要</param>
        ///// <param name="orderKey">排序字段</param>
        ///// <returns></returns>
        //public string DBPaging(string dataSource, int pageNumber, int pageSize, string key, string orderKey)
        //{
        //    string sql = "";
        //    string orderByStr = "";

        //    if (!string.IsNullOrEmpty(orderKey))
        //        orderByStr = " ORDER BY " + orderKey;

        //    switch (DBAccess.AppCenterDBType)
        //    {
        //        case DBType.Oracle:
        //        case DBType.KingBaseR3:
        //        case DBType.KingBaseR6:
        //            int beginIndex = (pageNumber - 1) * pageSize + 1;
        //            int endIndex = pageNumber * pageSize;

        //            sql = "SELECT * FROM ( SELECT A.*, ROWNUM RN " +
        //                "FROM (SELECT * FROM  " + dataSource + orderByStr + ") A WHERE ROWNUM <= " + endIndex + " ) WHERE RN >=" + beginIndex;
        //            break;
        //        case DBType.MSSQL:
        //            sql = "SELECT TOP " + pageSize + " * FROM " + dataSource + " WHERE " + key + " NOT IN  ("
        //            + "SELECT TOP (" + pageSize + "*(" + pageNumber + "-1)) " + key + " FROM " + dataSource + " )" + orderByStr;
        //            break;
        //        case DBType.MySQL:
        //            pageNumber -= 1;
        //            sql = "select * from  " + dataSource + orderByStr + " limit " + pageNumber + "," + pageSize;
        //            break;
        //        default:
        //            throw new Exception("暂不支持您的数据库类型.");
        //    }

        //    DataTable DTable = DBAccess.RunSQLReturnTable(sql);

        //    int totalCount = DBAccess.RunSQLReturnCOUNT("select " + key + " from " + dataSource);

        //    return DataTableConvertJson.DataTable2Json(DTable, totalCount);
        //}
        #endregion

        #region 获取菜单权限.
        /// <summary>
        /// 获得菜单数据.
        /// </summary>
        /// <returns></returns>
        public string GPM_DB_Menus()
        {
            string appNo = this.GetRequestVal("AppNo");

            var sql1 = "SELECT No,Name,FK_Menu,ParentNo,UrlExt,Icon,Idx ";
            sql1 += " FROM V_GPM_EmpMenu ";
            sql1 += " WHERE FK_Emp = '" + WebUser.No + "' ";
            sql1 += " AND MenuType = '3' ";
            sql1 += " AND FK_App = '" + appNo + "' ";
            sql1 += " UNION ";  //加入不需要权限控制的菜单.
            sql1 += "SELECT No,Name, No as FK_Menu,ParentNo,UrlExt,Icon,Idx";
            sql1 += " FROM GPM_Menu ";
            sql1 += " WHERE MenuCtrlWay=1 ";
            sql1 += " AND MenuType = '3' ";
            sql1 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";
            var dirs = DBAccess.RunSQLReturnTable(sql1);
            dirs.TableName = "Dirs"; //获得目录.

            string sql2 = "SELECT No,Name,FK_Menu,ParentNo,UrlExt,Icon,Idx ";
            sql2 += " FROM V_GPM_EmpMenu ";
            sql2 += " WHERE FK_Emp = '" + WebUser.No + "'";
            sql2 += " AND MenuType = '4' ";
            sql2 += " AND FK_App = '" + appNo + "' ";
            sql2 += " UNION ";  //加入不需要权限控制的菜单.
            sql2 += "SELECT No,Name, No as FK_Menu,ParentNo,UrlExt,Icon,Idx ";
            sql2 += " FROM GPM_Menu "; //加入不需要权限控制的菜单.
            sql2 += " WHERE MenuCtrlWay=1 ";
            sql2 += " AND MenuType = '4' ";
            sql2 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";

            DataTable menus = DBAccess.RunSQLReturnTable(sql2);
            menus.TableName = "Menus"; //获得菜单.
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                menus.Columns[0].ColumnName = "No";
                menus.Columns[1].ColumnName = "Name";
                menus.Columns[2].ColumnName = "FK_Menu";
                menus.Columns[3].ColumnName = "ParentNo";
                menus.Columns[4].ColumnName = "UrlExt";
                menus.Columns[5].ColumnName = "Icon";
                menus.Columns[6].ColumnName = "Idx";
            }
            //组装数据.
            DataSet ds = new DataSet();
            ds.Tables.Add(dirs);
            ds.Tables.Add(menus);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 是否可以执行当前工作
        /// </summary>
        /// <returns></returns>
        public string GPM_IsCanExecuteFunction()
        {
            DataTable dt = GPM_GenerFlagDB(); //获得所有的标记.
            string funcNo = this.GetRequestVal("FuncFlag");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString().Equals(funcNo) == true)
                    return "1";
            }
            return "0";
        }
        /// <summary>
        /// 获得所有的权限标记.
        /// </summary>
        /// <returns></returns>
        public DataTable GPM_GenerFlagDB()
        {
            string appNo = this.GetRequestVal("AppNo");
            string sql2 = "SELECT Flag,Idx";
            sql2 += " FROM V_GPM_EmpMenu ";
            sql2 += " WHERE FK_Emp = '" + WebUser.No + "'";
            sql2 += " AND MenuType = '5' ";
            sql2 += " AND FK_App = '" + appNo + "' ";
            sql2 += " UNION ";  //加入不需要权限控制的菜单.
            sql2 += "SELECT Flag,Idx ";
            sql2 += " FROM GPM_Menu "; //加入不需要权限控制的菜单.
            sql2 += " WHERE MenuCtrlWay=1 ";
            sql2 += " AND MenuType = '5' ";
            sql2 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql2);
            return dt;
        }
        /// <summary>
        /// 获得所有权限的标记
        /// </summary>
        /// <returns></returns>
        public string GPM_AutoHidShowPageElement()
        {
            var dt = GPM_GenerFlagDB(); //获得所有的标记.
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 组织结构查询
        /// </summary>
        /// <returns></returns>
        public string GPM_Search()
        {
            string searchKey = this.GetRequestVal("searchKey");
            string cloum = "";
            if(SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                cloum = "e.userid AS UserID,";
            }
            string sql = "SELECT e.no AS \"No\"," + cloum + "e.name AS \"Name\",d.No AS FK_Dept,d.Name AS deptName,e.Email AS Email,e.Tel AS Tel from Port_Dept d,Port_Emp e " +
                "where d.No=e.FK_Dept AND (e.No LIKE '%" + searchKey + "%' or e.NAME LIKE '%" + searchKey + "%' or d.Name LIKE '%" + searchKey + "%' or e.Tel LIKE '%" + searchKey + "%')";
            if (DataType.IsNullOrEmpty(WebUser.OrgNo) == false)
                sql += " AND e.OrgNo='" + WebUser.OrgNo + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

        public string Template_Save()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                throw new Exception("err@仅仅导入单组织版.");
            //var files = HttpContextHelper.RequestFiles();
            //string ext = ".xls";
            //string fileName = System.IO.Path.GetFileName(files[0].FileName);
            //if (fileName.Contains(".xlsx"))
            //    ext = ".xlsx";

            ////设置文件名
            //string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + ext;

            ////文件存放路径
            //string filePath =  BP.Difference.SystemConfig.PathOfTemp +  fileNewName;
            //HttpContextHelper.UploadFile(files[0], filePath);

            string filePath = @"D:\ccflow组织结构批量导入模板.xls";

            #region 获得数据源.
            List<string> sheetNameList = BP.DA.DBLoad.GenerTableNames(filePath).ToList();
            if (sheetNameList.Count < 3 || sheetNameList.Contains("部门$") == false || sheetNameList.Contains("岗位$") == false || sheetNameList.Contains("人员$") == false)
                throw new Exception("excel不符合要求，需要包含Sheet(部门、岗位、人员)");

            //获得部门数据.
            DataTable dtDept = BP.DA.DBLoad.ReadExcelFileToDataTable(filePath, sheetNameList.IndexOf("部门$"));
            for (int i = 0; i < dtDept.Columns.Count; i++)
            {
                string name = dtDept.Columns[i].ColumnName;
                name = name.Replace(" ", "");
                name = name.Replace("*", "");
                dtDept.Columns[i].ColumnName = name;
            }

            //获得角色数据.
            DataTable dtStation = BP.DA.DBLoad.ReadExcelFileToDataTable(filePath, sheetNameList.IndexOf("岗位$"));
            for (int i = 0; i < dtStation.Columns.Count; i++)
            {
                string name = dtStation.Columns[i].ColumnName;
                name = name.Replace(" ", "");
                name = name.Replace("*", "");
                dtStation.Columns[i].ColumnName = name;
            }

            //获得人员数据.
            DataTable dtEmp = BP.DA.DBLoad.ReadExcelFileToDataTable(filePath, sheetNameList.IndexOf("人员$"));
            for (int i = 0; i < dtEmp.Columns.Count; i++)
            {
                string name = dtEmp.Columns[i].ColumnName;
                name = name.Replace(" ", "");
                name = name.Replace("*", "");
                dtEmp.Columns[i].ColumnName = name;
            }


            #endregion 获得数据源.

            #region 检查是否有根目录为 0 的数据?
            //检查数据的完整性.
            //1.检查是否有根目录为0的数据?
            int num = 0;
            bool isHave = false;
            foreach (DataRow dr in dtDept.Rows)
            {
                string str1 = dr[0] as string;
                if (DataType.IsNullOrEmpty(str1) == true)
                    continue;

                num++;
                string str = dr[1] as string;
                if (str == null || str.Equals(DBNull.Value))
                    return "err@导入出现数据错误:" + str1 + "的.上级部门名称-不能用空行的数据， 第[" + num + "]行数据.";

                if (str.Equals("0") == true || str.Equals("root") == true)
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                return "err@导入数据没有找到部门根目录节点.";
            #endregion 检查是否有根目录为0的数据

            #region 检查部门名称是否重复?
            string deptStrs = "";
            foreach (DataRow dr in dtDept.Rows)
            {
                string deptName = dr[0] as string;
                if (DataType.IsNullOrEmpty(deptName) == true)
                    continue;

                if (deptStrs.Contains("," + deptName + ",") == true)
                    return "err@部门名称:" + deptName + "重复.";

                //加起来..
                deptStrs += "," + deptName + ",";
            }
            #endregion 检查部门名称是否重复?

            #region 检查人员帐号是否重复?
            string emps = "";
            foreach (DataRow dr in dtEmp.Rows)
            {
                string empNo = dr[0] as string;
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                if (emps.Contains("," + empNo + ",") == true)
                    return "err@人员帐号:" + empNo + "重复.";

                //加起来..
                emps += "," + empNo + ",";
            }
            #endregion 检查人员帐号是否重复?

            #region 检查角色名称是否重复?
            string staStrs = "";
            foreach (DataRow dr in dtStation.Rows)
            {
                string staName = dr[0] as string;
                if (DataType.IsNullOrEmpty(staName) == true)
                    continue;

                if (staStrs.Contains("," + staName + ",") == true)
                    return "err@角色名称:" + staName + "重复.";

                //加起来..
                staStrs += "," + staName + ",";
            }
            #endregion 检查角色名称是否重复?

            #region 检查人员的部门名称是否存在于部门数据里?
            int idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string emp = dr[0] as string;
                if (DataType.IsNullOrEmpty(emp) == true)
                    continue;

                idx++;
                //去的部门编号.
                string strs = dr["部门名称"] as string;
                if (DataType.IsNullOrEmpty(strs) == true)
                    return "err@第[" + idx + "]行,人员[" + emp + "]部门不能为空:" + strs + ".";

                string[] mystrs = strs.Split(',');
                foreach (string str in mystrs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    if (str.Equals("0") || str.Equals("root") == true)
                        continue;

                    //先看看数据是否有?
                    BP.Port.Dept dept = new BP.Port.Dept();
                    if (dept.Retrieve("Name", str) == 1)
                        continue;

                    //从xls里面判断.
                    isHave = false;
                    foreach (DataRow drDept in dtDept.Rows)
                    {
                        if (str.Equals(drDept[0].ToString()) == true)
                        {
                            isHave = true;
                            break;
                        }
                    }
                    if (isHave == false)
                        return "err@第[" + idx + "]行,人员[" + emp + "]部门名[" + str + "]，不存在模版里。";
                }
            }
            #endregion 检查人员的部门名称是否存在于部门数据里

            #region 检查人员的角色名称是否存在于角色数据里?
            idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string emp = dr[0] as string;
                if (DataType.IsNullOrEmpty(emp) == true)
                    continue;

                idx++;

                //角色名称..
                string strs = dr["角色名称"] as string;
                if (DataType.IsNullOrEmpty(strs) == true)
                    continue;

                //判断角色.
                string[] mystrs = strs.Split(',');
                foreach (string str in mystrs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //先看看数据是否有?
                    BP.Port.Station stationEn = new BP.Port.Station();
                    if (stationEn.Retrieve("Name", str) == 1)
                        continue;

                    //从 xls 判断.
                    isHave = false;
                    foreach (DataRow drSta in dtStation.Rows)
                    {
                        if (str.Equals(drSta[0].ToString()) == true)
                        {
                            isHave = true;
                            break;
                        }
                    }
                    if (isHave == false)
                        return "err@第[" + idx + "]行,人员[" + emp + "]角色名称[" + str + "]，不存在模版里。";
                }
            }
            #endregion 检查人员的部门名称是否存在于部门数据里

            #region 检查部门负责人是否存在于人员列表里?
            string empStrs = ",";
            foreach (DataRow item in dtEmp.Rows)
            {
                empStrs += item[0].ToString() + ",";
            }
            idx = 0;
            foreach (DataRow dr in dtDept.Rows)
            {
                string empNo = dr[2] as string;
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                idx++;
                if (empStrs.Contains("," + empNo + ",") == false)
                    return "err@部门负责人[" + empNo + "]不存在与人员表里，第[" + idx + "]行.";
            }
            #endregion 检查部门负责人是否存在于人员列表里

            #region 检查直属领导帐号是否存在于人员列表里?
            idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string empNo = dr[6] as string;
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                idx++;
                if (empStrs.Contains("," + empNo + ",") == false)
                    return "err@部门负责人[" + empNo + "]不存在与人员表里，第[" + idx + "]行.";
            }
            #endregion 检查部门负责人是否存在于人员列表里

            #region 插入数据到 Port_StationType. 
            idx = -1;
            foreach (DataRow dr in dtStation.Rows)
            {
                idx++;
                string str = dr[1] as string;

                //判断是否是空.
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                if (str.Equals("角色类型") == true)
                    continue;

                str = str.Trim();

                //看看数据库是否存在.
                BP.Port.StationType st = new BP.Port.StationType();
                if (st.IsExit("Name", str) == false)
                {
                    st.Name = str;
                    st.OrgNo = BP.Web.WebUser.OrgNo;
                    st.No = DBAccess.GenerGUID();
                    st.Insert();
                }
            }
            #endregion 插入数据到 Port_StationType. 

            #region 插入数据到 Port_Station. 
            idx = -1;
            foreach (DataRow dr in dtStation.Rows)
            {
                idx++;
                string str = dr[0] as string;

                //判断是否是空.
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                if (str.Equals("角色名称") == true)
                    continue;


                //获得类型的外键的编号.
                string stationTypeName = dr[1].ToString().Trim();
                BP.Port.StationType st = new BP.Port.StationType();
                if (st.Retrieve("Name", stationTypeName) == 0)
                    return "err@系统出现错误,没有找到角色类型[" + stationTypeName + "]的数据.";

                //看看数据库是否存在.
                BP.Port.Station sta = new BP.Port.Station();
                sta.Name = str;
                sta.Idx = idx;

                //不存在就插入.
                if (sta.IsExit("Name", str) == false)
                {
                    sta.OrgNo = BP.Web.WebUser.OrgNo;
                    sta.FK_StationType = st.No;
                    sta.No = DBAccess.GenerGUID();
                    sta.Insert();
                }
                else
                {
                    //存在就更新.
                    sta.FK_StationType = st.No;
                    sta.Update();
                }
            }
            #endregion 插入数据到 Port_Station. 

            #region 插入数据到 Port_Dept.
            idx = -1;
            foreach (DataRow dr in dtDept.Rows)
            {
                //获得部门名称.
                string deptName = dr[0] as string;
                if (deptName.Equals("部门名称") == true)
                    continue;

                string parentDeptName = dr[1] as string;
                string leader = dr[2] as string;

                //说明是根目录.
                if (parentDeptName.Equals("0") == true || parentDeptName.Equals("root") == true)
                {
                    BP.Port.Dept root = new BP.Port.Dept();
                    root.No = BP.Web.WebUser.DeptNo;
                    if (root.RetrieveFromDBSources() == 0)
                        return "err@没有找到根目录节点，请联系管理员。";

                    root.Name = deptName;
                    root.Update();
                    continue;
                }


                //先求出来父节点.
                BP.Port.Dept parentDept = new BP.Port.Dept();
                int i = parentDept.Retrieve("Name", parentDeptName);
                if (i == 0)
                    return "err@没有找到当前部门[" + deptName + "]的上一级部门[" + parentDeptName + "]";

                BP.Port.Dept myDept = new BP.Port.Dept();

                //如果数据库存在.
                i = parentDept.Retrieve("Name", deptName);
                if (i >= 1)
                    continue;

                //插入部门.
                myDept.Name = deptName;
                //   myDept.OrgNo = BP.Web.WebUser.OrgNo;
                myDept.No = DBAccess.GenerGUID();
                myDept.ParentNo = parentDept.No;
                myDept.Leader = leader; //领导.
                myDept.Idx = idx;
                myDept.Insert();
            }
            #endregion 插入数据到 Port_Dept.

            #region 插入到 Port_Emp.
            idx = 0;
            foreach (DataRow dr in dtEmp.Rows)
            {
                string empNo = dr["人员帐号"].ToString();
                string empName = dr["人员姓名"].ToString();
                string deptNames = dr["部门名称"].ToString();
                string deptPaths = dr["部门路径"].ToString();

                string stationNames = dr["角色名称"].ToString();
                string tel = dr["电话"].ToString();
                string email = dr["邮箱"].ToString();
                string leader = dr["直属领导"].ToString(); //部门领导.

                BP.Port.Emp emp = new BP.Port.Emp();
                int i = emp.Retrieve("No", empNo);
                if (i >= 1)
                {
                    emp.Tel = tel;
                    emp.Name = empName;
                    emp.Update();
                    continue;
                }

                //找到人员的部门.
                string[] myDeptStrs = deptNames.Split(',');
                BP.Port.Dept dept = new BP.Port.Dept();
                foreach (string deptName in myDeptStrs)
                {
                    if (DataType.IsNullOrEmpty(deptName) == true)
                        continue;

                    i = dept.Retrieve("Name", deptName);
                    if (i <= 0)
                        return "err@部门名称不存在." + deptName;

                    BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                    de.DeptNo = dept.No;
                    de.EmpNo = empNo;
                    de.OrgNo = WebUser.OrgNo;
                    de.setMyPK(de.DeptNo + "_" + de.EmpNo);
                    de.Delete();
                    de.Insert();
                }

                //插入角色.
                string[] staNames = stationNames.Split(',');
                BP.Port.Station sta = new BP.Port.Station();
                foreach (string staName in staNames)
                {
                    if (DataType.IsNullOrEmpty(staName) == true)
                        continue;

                    i = sta.Retrieve("Name", staName);
                    if (i == 0)
                        return "err@角色名称不存在." + staName;

                    BP.Port.DeptEmpStation des = new BP.Port.DeptEmpStation();
                    des.DeptNo = dept.No;
                    des.EmpNo = empNo;
                    des.StationNo = sta.No;
                    //   des.OrgNo = WebUser.OrgNo;
                    des.setMyPK(des.DeptNo + "_" + des.EmpNo + "_" + des.StationNo);
                    des.Delete();
                    des.Insert();
                }

                //插入到数据库.
                emp.No = empNo;
                //   emp.UserID = empNo;
                emp.Name = empName;
                emp.DeptNo = dept.No;
                // emp.OrgNo = WebUser.OrgNo;
                emp.Tel = tel;
                //emp.Email = email;
                //emp.Leader = leader;
                //emp.Idx = idx;

                emp.Insert();
            }
            #endregion 插入到 Port_Emp.


            //删除临时文件
            //  System.IO.File.Delete(filePath);

            return "执行完成.";
        }

        public string EmpDepts_Init()
        {
            string empNo = this.EmpNo;
            if (DataType.IsNullOrEmpty(empNo) == true)
                return "err@参数FK_Emp不能为空";
            //if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            //    empNo = BP.Web.WebUser.OrgNo + "_" + empNo;

            Emp emp = new Emp();
            emp.No = empNo;
            emp.Retrieve();

            DataSet ds = new DataSet();
            string dbstr = SystemConfig.AppCenterDBVarStr;

            string sql = "";

            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                //获取当前人员所在的部门及兼职部门
                sql = "SELECT B.No AS FK_Dept,B.Name AS  FK_DeptText,A.MyPK  From Port_DeptEmp A,Port_Dept B WHERE A.FK_Dept=B.No AND A.FK_Emp='" + empNo + "'";
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    sql += " AND B.OrgNo='" + emp.OrgNo + "'";
            }
            else
            {
                //获取当前人员所在的部门及兼职部门
                sql = "SELECT B.No AS FK_Dept,B.Name AS  FK_DeptText,A.MyPK  From Port_DeptEmp A,Port_Dept B WHERE A.FK_Dept=B.No AND A.FK_Emp='" + empNo + "'";
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    sql += " AND B.OrgNo='" + emp.OrgNo + "'";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "FK_Dept";
                dt.Columns[1].ColumnName = "FK_DeptText";
                dt.Columns[2].ColumnName = "MyPK";
            }

            if (dt.Rows.Count == 0)
            {
                DeptEmp deptEmp = new DeptEmp();
                deptEmp.DeptNo = emp.DeptNo;
                deptEmp.EmpNo = emp.No;
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    deptEmp.MyPK = emp.DeptNo + "_" + emp.UserID;
                else
                    deptEmp.MyPK = emp.DeptNo + "_" + emp.No;

                deptEmp.OrgNo = emp.OrgNo;
                deptEmp.Insert();
                DataRow dr = dt.NewRow();
                dr[0] = emp.DeptNo;
                dr[1] = emp.DeptText;
                dr[2] = deptEmp.MyPK;
                dt.Rows.Add(dr);
            }
            dt.TableName = "Port_DeptEmp";
            ds.Tables.Add(dt);
            //获取岗位
            sql = "SELECT B.No AS FK_Station,B.Name AS FK_StationText ,A.FK_Dept AS FK_Dept From Port_DeptEmpStation A,Port_Station B WHERE A.FK_Station=B.No AND A.FK_Emp='" + empNo + "'";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                sql += " AND B.OrgNo='" + WebUser.OrgNo + "'";

            dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "FK_Station";
                dt.Columns[1].ColumnName = "FK_StationText";
                dt.Columns[2].ColumnName = "FK_Dept";
            }

            dt.TableName = "Port_DeptEmpStation";
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        /**
        取消人员部门岗位管理关系
        @return
        */
        public string DeptEmpStation_Dele()
        {
            string sql = "delete from Port_DeptEmpStation where FK_Emp='" + this.GetRequestVal("FK_Emp") + "' and FK_Dept='" + this.GetRequestVal("FK_Dept") + "'";
            DBAccess.RunSQL(sql);
            return "执行成功 ";
        }
        /// <summary>
        /// 绑定人员
        /// </summary>
        /// <returns></returns>
        public string BindEmp()
        {
            string deptNo = this.GetRequestVal("DeptNo");

            Emps emps = new Emps();
            emps.Retrieve("FK_Dept", deptNo, "Idx");
            DataTable dt = emps.ToDataTableField();
            dt.Columns.Add("State");

            string sql = "SELECT No,Name,Tel,Email FROM Port_Emp A, Port_DeptEmp B WHERE A.No=B.FK_Emp AND B.FK_Dept='" + deptNo + "' AND A.FK_Dept!='" + deptNo + "'";
            DataTable mydt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow mydr in mydt.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["No"] = mydr[0];
                dr["Name"] = mydr[1];
                dr["Tel"] = mydr[2];
                dr["Email"] = mydr[3];
                dr["State"] = 1; //兼职人员.
                //加入里面.
                dt.Rows.Add(dr);
            }
            return BP.Tools.Json.ToJson(dt);
        }
    }
}
