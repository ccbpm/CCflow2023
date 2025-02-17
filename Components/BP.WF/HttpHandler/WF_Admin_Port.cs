﻿using System;
using System.Data;
using BP.DA;
using BP.Difference;


namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Port : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_Port()
        {
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" +HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        #region OrderOfDept 部门顺序调整 .
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string OrderOfDept_Init()
        {
            string sql = "SELECT No,Name,ParentNo,Idx FROM Port_Dept";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if(SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
                dt.Columns[2].ColumnName = "ParentNo";
                dt.Columns[3].ColumnName = "Idx";
            }
            return "";
        }
        #endregion xxx 界面方法.

       

    }
}
