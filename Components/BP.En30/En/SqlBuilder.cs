﻿using System;
using BP.DA;
using BP.Difference;
using BP.Sys;


namespace BP.En
{
    public class SqlBuilder
    {
        #region 关于IEntitiy的操作
        //		public static String GetKeyCondition(IEntity en)
        //		{ 
        //			return null;
        //		}
        //		public static String RetrieveAll(IEntity en)
        //		{ 
        //			return null;
        //		}
        //		public static String Retrieve(IEntity en)
        //		{ 
        //			return null;
        //		}
        //		public static String Insert(IEntity en)
        //		{ 
        //			return null;
        //		}
        //		public static String Delete(IEntity en)
        //		{ 
        //			return null;
        //		}
        //		public static String Update(IEntity en)
        //		{ 
        //			return null;
        //		}

        #endregion

        #region GetKeyCondition

        /// <summary>
        /// 得到主健
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static String GetKeyConditionOfMS(Entity en)
        {
            // 判断特殊情况。
            switch (en.PKField)
            {
                case "OID":
                    return " OID=@OID";
                case "No":
                    return " No=@No";
                case "MyPK":
                    return " MyPK=@MyPK";
                    break;
                default:
                    break;
            }

            if (en.EnMap.Attrs.Contains("OID"))
            {
                int key = en.GetValIntByKey("OID");
                if (key == 0)
                    throw new Exception("@在执行[" + en.EnMap.EnDesc + " " + en.EnMap.PhysicsTable + "]，没有给PK OID 赋值,不能进行查询操作。");
                //   if (en.PKs.Length == 1)
                return " OID=@OID" + key;
            }

            string sql = " (1=1) ";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.PKFK || attr.MyFieldType == FieldType.PKEnum)
                {
                    if (attr.MyDataType == DataType.AppString)
                    {
                        string val = en.GetValByKey(attr.Key).ToString();
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@在执行[" + en.EnMap.EnDesc + " " + en.EnMap.PhysicsTable + "]没有给PK " + attr.Key + " 赋值,不能进行查询操作。");
                        sql = sql + " AND " + attr.Field + "=@" + attr.Key;
                        continue;
                    }
                    if (attr.MyDataType == DataType.AppInt)
                    {
                        if (en.GetValIntByKey(attr.Key) == 0 && attr.Key == "OID")
                            throw new Exception("@在执行[" + en.EnMap.EnDesc + " " + en.EnMap.PhysicsTable + "]，没有给PK " + attr.Key + " 赋值,不能进行查询操作。");
                        sql = sql + " AND " + attr.Field + "=@" + attr.Key;
                        continue;
                    }
                }
            }
            return sql;
        }
        /// <summary>
        /// 得到主健
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static String GetKeyConditionOfOLE(Entity en)
        {
            // 判断特殊情况。
            if (en.EnMap.Attrs.Contains("OID"))
            {
                int key = en.GetValIntByKey("OID");
                if (key == 0)
                    throw new Exception("@在执行[" + en.EnMap.EnDesc + " " + en.EnMap.PhysicsTable + "]，没有给PK OID 赋值,不能进行查询操作。");

                if (en.PKs.Length == 1)
                    return en.EnMap.PhysicsTable + ".OID=" + key;
            }
            //			if (en.EnMap.Attrs.Contains("MID"))
            //			{
            //				int key=en.GetValIntByKey("MID");
            //				if (key==0)
            //					throw new Exception("@在执行["+en.EnMap.EnDesc+ " " +en.EnMap.PhysicsTable +"]，没有给PK MID 赋值,不能进行查询操作。");
            //				return en.EnMap.PhysicsTable+".MID="+key ;
            //			}

            string sql = " (1=1) ";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.PKFK || attr.MyFieldType == FieldType.PKEnum)
                {
                    if (attr.MyDataType == DataType.AppString)
                    {
                        string val = en.GetValByKey(attr.Key).ToString();
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@在执行[" + en.EnMap.EnDesc + " " + en.EnMap.PhysicsTable + "]没有给PK " + attr.Key + " 赋值,不能进行查询操作。");
                        sql = sql + " AND " + en.EnMap.PhysicsTable + ".[" + attr.Field + "]='" + en.GetValByKey(attr.Key).ToString() + "'";
                        continue;
                    }

                    if (attr.MyDataType == DataType.AppInt)
                    {
                        if (en.GetValIntByKey(attr.Key) == 0 && attr.Key == "OID")
                            throw new Exception("@在执行[" + en.EnMap.EnDesc + " " + en.EnMap.PhysicsTable + "]，没有给PK " + attr.Key + " 赋值,不能进行查询操作。");
                        sql = sql + " AND " + en.EnMap.PhysicsTable + ".[" + attr.Field + "]=" + en.GetValStringByKey(attr.Key);
                        continue;
                    }
                }
            }
            return sql;
        }
        /// <summary>
        /// 得到主健
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static String GenerWhereByPK(Entity en, string dbStr)
        {
            if (en.PKCount == 1)
            {
                if (dbStr == "?")
                    return en.EnMap.PhysicsTable + "." + en.PKField + "=" + dbStr;
                else
                    return en.EnMap.PhysicsTable + "." + en.PKField + "=" + dbStr + en.PK;
            }

            string sql = " (1=1) ";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.PKFK || attr.MyFieldType == FieldType.PKEnum)
                {
                    if (dbStr == "?")
                        sql = sql + " AND " + en.EnMap.PhysicsTable + "." + attr.Field + "=" + dbStr;
                    else
                        sql = sql + " AND " + en.EnMap.PhysicsTable + "." + attr.Field + "=" + dbStr + attr.Field;
                }
            }
            return sql;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static Paras GenerParasPK(Entity en)
        {
            Paras paras = new Paras();
            string pk = en.PK;
            Attrs attrs = en.EnMap.Attrs;
            Attr attr1 = null;
            switch (pk)
            {
                case "OID":
                    attr1 = attrs.GetAttrByKey("OID");
                    if (attr1.ItIsNum)
                        paras.Add("OID", en.GetValIntByKey("OID"));
                    else
                        paras.Add("OID", en.GetValStrByKey("OID"));
                    return paras;
                case "No":
                    paras.Add("No", en.GetValStrByKey("No"));
                    return paras;
                case "MyPK":
                    paras.Add("MyPK", en.GetValStrByKey("MyPK"));
                    return paras;
                case "NodeID":
                    paras.Add("NodeID", en.GetValIntByKey("NodeID"));
                    return paras;
                case "WorkID":
                    attr1 = attrs.GetAttrByKey("WorkID");
                    if (attr1.ItIsNum)
                        paras.Add("WorkID", en.GetValIntByKey("WorkID"));
                    else
                        paras.Add("WorkID", en.GetValStrByKey("WorkID"));
                    return paras;
                default:
                    break;
            }


            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.PK
                    || attr.MyFieldType == FieldType.PKFK
                    || attr.MyFieldType == FieldType.PKEnum)
                {
                    if (attr.MyDataType == DataType.AppString)
                    {
                        paras.Add(attr.Key, en.GetValByKey(attr.Key).ToString());
                        continue;
                    }

                    if (attr.MyDataType == DataType.AppInt)
                    {
                        paras.Add(attr.Key, en.GetValIntByKey(attr.Key));
                        continue;
                    }
                }
            }
            return paras;
        }
        public static String GetKeyConditionOfOraForPara(Entity en)
        {
            // 不能删除物理表名称，会引起未定义列。

            // 判断特殊情况, 
            switch (en.PK)
            {
                case "OID":
                    return en.EnMap.PhysicsTable + ".OID=" + en.HisDBVarStr + "OID";
                case "ID":
                    return en.EnMap.PhysicsTable + ".ID=" + en.HisDBVarStr + "ID";
                case "No":
                    return en.EnMap.PhysicsTable + ".No=" + en.HisDBVarStr + "No";
                case "MyPK":
                    return en.EnMap.PhysicsTable + ".MyPK=" + en.HisDBVarStr + "MyPK";
                case "NodeID":
                    return en.EnMap.PhysicsTable + ".NodeID=" + en.HisDBVarStr + "NodeID";
                case "WorkID":
                    return en.EnMap.PhysicsTable + ".WorkID=" + en.HisDBVarStr + "WorkID";
                default:
                    break;
            }

            string sql = " (1=1) ";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.PKFK || attr.MyFieldType == FieldType.PKEnum)
                {
                    if (attr.MyDataType == DataType.AppString)
                    {
                        sql = sql + " AND " + en.EnMap.PhysicsTable + "." + attr.Field + "=" + en.HisDBVarStr + attr.Key;
                        continue;
                    }
                    if (attr.MyDataType == DataType.AppInt)
                    {
                        sql = sql + " AND " + en.EnMap.PhysicsTable + "." + attr.Field + "=" + en.HisDBVarStr + attr.Key;
                        continue;
                    }
                }
            }
            return sql.Substring(" (1=1)  AND ".Length);
        }
        public static String GetKeyConditionOfInformixForPara(Entity en)
        {
            // 不能删除物理表名称，会引起未定义列。

            // 判断特殊情况, 
            switch (en.PK)
            {
                case "OID":
                    return en.EnMap.PhysicsTable + ".OID=?";
                case "No":
                    return en.EnMap.PhysicsTable + ".No=?";
                case "MyPK":
                    return en.EnMap.PhysicsTable + ".MyPK=?";
                case "ID":
                    return en.EnMap.PhysicsTable + ".ID=?";
                default:
                    break;
            }

            string sql = " (1=1) ";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.PKFK || attr.MyFieldType == FieldType.PKEnum)
                {
                    if (attr.MyDataType == DataType.AppString)
                    {
                        sql = sql + " AND " + en.EnMap.PhysicsTable + "." + attr.Field + "=?";
                        continue;
                    }
                    if (attr.MyDataType == DataType.AppInt)
                    {
                        sql = sql + " AND " + en.EnMap.PhysicsTable + "." + attr.Field + "=?";
                        continue;
                    }
                }
            }
            return sql.Substring(" (1=1)  AND ".Length);
        }
        #endregion

        /// <summary>
        /// 查询全部信息
        /// </summary>
        /// <param name="en">实体</param>
        /// <returns>sql</returns>
        public static string RetrieveAll(Entity en)
        {
            return SqlBuilder.SelectSQL(en, BP.Difference.SystemConfig.TopNum);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="en">实体</param>
        /// <returns>string</returns>
        public static string Retrieve(Entity en)
        {
            string sql = "";
            switch (en.EnMap.EnDBUrl.DBType)
            {
                case DBType.MSSQL:
                case DBType.MySQL:
                    sql = SqlBuilder.SelectSQLOfMS(en, 1) + "   AND ( " + SqlBuilder.GenerWhereByPK(en, "@") + " )";
                    break;
                case DBType.Access:
                    // sql = SqlBuilder.SelectSQLOfOLE(en, 1) + "  AND ( " + SqlBuilder.GetKeyConditionOfOLE(en,"@") + " )";
                    sql = SqlBuilder.SelectSQLOfOLE(en, 1) + "  AND ( " + SqlBuilder.GenerWhereByPK(en, "@") + " )";
                    break;
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                case DBType.Informix:
                    sql = SqlBuilder.SelectSQLOfOra(en, 1) + "  AND ( " + SqlBuilder.GenerWhereByPK(en, ":") + " )";
                    break;
                case DBType.DB2:
                    throw new Exception("还没有实现。");
                default:
                    throw new Exception("还没有实现。");
            }
            sql = sql.Replace("WHERE   AND", " WHERE ");
            sql = sql.Replace("WHERE  AND", " WHERE ");
            sql = sql.Replace("WHERE AND", " WHERE ");
            return sql;
        }
        public static string RetrieveForPara(Entity en)
        {
            string sql = null;
            switch (en.EnMap.EnDBUrl.DBType)
            {
                case DBType.MSSQL:
                    sql = SqlBuilder.SelectSQLOfMS(en, 1) + " AND " + SqlBuilder.GenerWhereByPK(en, "@");
                    break;
                case DBType.MySQL:
                    sql = SqlBuilder.SelectSQLOfMySQL(en, 1) + " AND " + SqlBuilder.GenerWhereByPK(en, "@");
                    break;
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    sql = SqlBuilder.SelectSQLOfPostgreSQL(en, 1) + " AND " + SqlBuilder.GenerWhereByPK(en, "@");
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = SqlBuilder.SelectSQLOfOra(en, 1) + "AND (" + SqlBuilder.GenerWhereByPK(en, ":") + " )";
                    break;
                case DBType.Informix:
                    sql = SqlBuilder.SelectSQLOfInformix(en, 1) + " WHERE (" + SqlBuilder.GenerWhereByPK(en, "?") + " )";
                    break;
                case DBType.Access:
                    sql = SqlBuilder.SelectSQLOfOLE(en, 1) + " AND " + SqlBuilder.GenerWhereByPK(en, "@");
                    break;
                case DBType.DB2:
                default:
                    throw new Exception("还没有实现。");
            }
            sql = sql.Replace("WHERE  AND", "WHERE");
            sql = sql.Replace("WHERE AND", "WHERE");
            return sql;
        }
        public static string RetrieveForPara_bak(Entity en)
        {
            switch (en.EnMap.EnDBUrl.DBType)
            {
                case DBType.MSSQL:
                case DBType.MySQL:
                case DBType.Access:
                    if (en.EnMap.HisFKAttrs.Count == 0)
                        return SqlBuilder.SelectSQLOfMS(en, 1) + SqlBuilder.GetKeyConditionOfOraForPara(en);
                    else
                        return SqlBuilder.SelectSQLOfMS(en, 1) + "  AND ( " + SqlBuilder.GetKeyConditionOfOraForPara(en) + " )";
                    return SqlBuilder.SelectSQLOfMS(en, 1) + "  AND ( " + SqlBuilder.GetKeyConditionOfMS(en) + " )";
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                case DBType.Informix:
                    if (en.EnMap.HisFKAttrs.Count == 0)
                        return SqlBuilder.SelectSQLOfOra(en, 1) + SqlBuilder.GetKeyConditionOfOraForPara(en);
                    else
                        return SqlBuilder.SelectSQLOfOra(en, 1) + "  AND ( " + SqlBuilder.GetKeyConditionOfOraForPara(en) + " )";
                case DBType.DB2:
                default:
                    throw new Exception("还没有实现。");
            }
        }
       
        public static string GenerFormWhereOfOra(Entity en)
        {
            string from = " FROM " + en.EnMap.PhysicsTable;

            if (en.EnMap.HisFKAttrs.Count == 0)
                return from + " WHERE (1=1) ";

            string mytable = en.EnMap.PhysicsTable;
            from += ",";
            // 产生外键表列表。
            Attrs fkAttrs = en.EnMap.HisFKAttrs;
            foreach (Attr attr in fkAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.UIBindKey.Contains(","))
                {
                    string[] strs = attr.UIBindKey.Split(',');
                    string ptable = strs[0];
                    string noCol = strs[1];
                    string nameCol = strs[2];
                    ptable = ptable + " T" + attr.Key;
                    from += ptable + " ,";
                }
                else
                {

                    string fktable = attr.HisFKEn.EnMap.PhysicsTable;

                    fktable = fktable + " T" + attr.Key;
                    from += fktable + " ,";
                }
            }

            from = from.Substring(0, from.Length - 1);

            string where = " WHERE ";
            bool isAddAnd = true;

            // 开始形成 外键 where.
            foreach (Attr attr in fkAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.UIBindKey.Contains(","))
                {
                    string[] strs = attr.UIBindKey.Split(',');
                    string ptable = strs[0];
                    string noCol = strs[1];
                    string nameCol = strs[2];

                    string fktable = "T" + attr.Key;
                    if (isAddAnd == false)
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";
                        else
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";

                        isAddAnd = true;
                    }
                    else
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";
                        else
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";
                    }

                }
                else
                {

                    Entity en1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                    string fktable = "T" + attr.Key;

                    if (isAddAnd == false)
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";
                        else
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";

                        isAddAnd = true;
                    }
                    else
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";
                        else
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";
                    }
                }
            }

            where = where.Replace("WHERE  AND", "WHERE");
            where = where.Replace("WHERE AND", "WHERE");
            return from + where;
        }
        public static string GenerFormWhereOfInformix(Entity en)
        {
            string from = " FROM " + en.EnMap.PhysicsTable;
            string mytable = en.EnMap.PhysicsTable;
            Attrs fkAttrs = en.EnMap.HisFKAttrs;
            string where = "";
            bool isAddAnd = true;

            // 开始形成 外键 where.
            foreach (Attr attr in fkAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.UIBindKey.Contains(","))
                {
                    string[] strs = attr.UIBindKey.Split(',');
                    string fktable = strs[0];
                    string noCol = strs[1];
                    string nameCol = strs[2];
                    if (isAddAnd == true)
                    {
                        isAddAnd = false;
                        where += " LEFT JOIN " + fktable + "  " + fktable + "_" + attr.Key + "  ON " + mytable + "." + attr.Key + "=" + fktable + "_" + attr.Key + "." + nameCol;
                    }
                    else
                    {
                        where += " LEFT JOIN " + fktable + "  " + fktable + "_" + attr.Key + "  ON " + mytable + "." + attr.Key + "=" + fktable + "_" + attr.Key + "." + nameCol;
                    }
                }
                else
                {

                    Entity en1 = attr.HisFKEn;
                    string fktable = en1.EnMap.PhysicsTable;
                    if (isAddAnd == true)
                    {
                        isAddAnd = false;
                        where += " LEFT JOIN " + fktable + "  " + fktable + "_" + attr.Key + "  ON " + mytable + "." + attr.Key + "=" + fktable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue);
                    }
                    else
                    {
                        where += " LEFT JOIN " + fktable + "  " + fktable + "_" + attr.Key + "  ON " + mytable + "." + attr.Key + "=" + fktable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue);
                    }
                }
            }

            where = where.Replace("WHERE  AND", "WHERE");
            where = where.Replace("WHERE AND", "WHERE");
            return from + where;
        }
        /// <summary>
        /// 生成sql.
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GenerCreateTableSQLOfMS(Entity en)
        {
            if (DataType.IsNullOrEmpty(en.EnMap.PhysicsTable))
                return "DELETE FROM " + BP.Sys.Base.Glo.SysEnum() + " where enumkey='sdsf44a23'";

            //    throw new Exception(en.ToString() +" map error "+en.GetType() );

            string sql = "CREATE TABLE  " + en.EnMap.PhysicsTable + " ( ";
            Attrs attrs = en.EnMap.Attrs;
            if (attrs.Count == 0)
                throw new Exception("@" + en.EnDesc + " , [" + en.EnMap.PhysicsTable + "]没有属性/字段集合 attrs.Count = 0 ,能执行数据表的创建.");

            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                int len = attr.MaxLength;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.ItIsPK)
                            sql += "[" + attr.Field + "]  NVARCHAR (" + attr.MaxLength + ") NOT NULL,";
                        else
                        {
                            if (attr.MaxLength >= 4000)
                                sql += "[" + attr.Field + "]  NVARCHAR (MAX) NULL,";
                            else
                                sql += "[" + attr.Field + "]  NVARCHAR (" + attr.MaxLength + ") NULL,";
                        }
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:

                        if (attr.MaxLength >= 4000)
                            sql += "[" + attr.Field + "]  NVARCHAR (MAX) NULL,";
                        else
                            sql += "[" + attr.Field + "]  NVARCHAR (" + attr.MaxLength + ") NULL,";

                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                        sql += "[" + attr.Field + "] FLOAT NULL,";
                        break;
                    case DataType.AppBoolean:
                    case DataType.AppInt:
                        if (attr.ItIsPK)
                        {
                            //说明这个是自动增长的列.
                            if (attr.UIBindKey == "1")
                                sql += "[" + attr.Field + "] INT  primary key identity(1,1),";
                            else
                                sql += "[" + attr.Field + "] INT NOT NULL,";
                        }
                        else
                        {
                            sql += "[" + attr.Field + "] INT  NULL,";
                        }
                        break;
                    case DataType.AppDouble:
                        sql += "[" + attr.Field + "]  FLOAT  NULL,";
                        break;
                    default:
                        break;
                }
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")";
            return sql;
        }
        /// <summary>
        /// 执行PSQL.
        /// </summary>
        /// <param name="en">实体</param>
        /// <returns>生成的SQL</returns>
        public static string GenerCreateTableSQLOfPostgreSQL(Entity en)
        {
            if (DataType.IsNullOrEmpty(en.EnMap.PhysicsTable))
                return "DELETE FROM " + BP.Sys.Base.Glo.SysEnum() + " where enumkey='sdsf44a23'";

            //    throw new Exception(en.ToString() +" map error "+en.GetType() );

            string sql = "CREATE TABLE  " + en.EnMap.PhysicsTable + " ( ";
            Attrs attrs = en.EnMap.Attrs;
            if (attrs.Count == 0)
                throw new Exception("@" + en.EnDesc + " , [" + en.EnMap.PhysicsTable + "]没有属性/字段集合 attrs.Count = 0 ,能执行数据表的创建.");

            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                int len = attr.MaxLength;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.ItIsPK)
                            sql += attr.Field + "  VARCHAR (" + attr.MaxLength + ") NOT NULL,";
                        else
                        {
                            if (attr.MaxLength >= 4000)
                                sql += attr.Field + " VARCHAR (" + attr.MaxLength + ") NULL,";
                            else
                                sql += attr.Field + " VARCHAR (" + attr.MaxLength + ") NULL,";
                        }
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        sql += attr.Field + " VARCHAR (" + attr.MaxLength + ") NULL,";
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                        sql += attr.Field + " FLOAT NULL,";
                        break;
                    case DataType.AppBoolean:
                    case DataType.AppInt:
                        if (attr.ItIsPK)
                        {
                            //说明这个是自动增长的列.
                            if (attr.UIBindKey == "1")
                                sql += attr.Field + " INT  primary key identity(1,1),";
                            else
                                sql += attr.Field + " INT NOT NULL,";
                        }
                        else
                        {
                            sql += attr.Field + "  INT  NULL,";
                        }
                        break;
                    case DataType.AppDouble:
                        sql += attr.Field + " FLOAT  NULL,";
                        break;
                    default:
                        break;
                }
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")";
            return sql;
        }









        public static string GenerCreateTableSqlOfKingBase(Entity en)
        {
            string from = " FROM " + en.EnMap.PhysicsTable;

            if (en.EnMap.HisFKAttrs.Count == 0)
                return from + " WHERE (1=1) ";

            string mytable = en.EnMap.PhysicsTable;
            from += ",";
            // 产生外键表列表。
            Attrs fkAttrs = en.EnMap.HisFKAttrs;
            foreach (Attr attr in fkAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.UIBindKey.Contains(","))
                {
                    string[] strs = attr.UIBindKey.Split(',');
                    string ptable = strs[0];
                    string noCol = strs[1];
                    string nameCol = strs[2];

                    string fktable = ptable;
                    fktable = fktable + " T" + attr.Key;
                    from += fktable + " ,";

                }
                else
                {
                    string fktable = attr.HisFKEn.EnMap.PhysicsTable;
                    fktable = fktable + " T" + attr.Key;
                    from += fktable + " ,";
                }
            }

            from = from.Substring(0, from.Length - 1);

            string where = " WHERE ";
            bool isAddAnd = true;

            // 开始形成 外键 where.
            foreach (Attr attr in fkAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.UIBindKey.Contains(","))
                {
                    string[] strs = attr.UIBindKey.Split(',');
                    string ptable = strs[0];
                    string noCol = strs[1];
                    string nameCol = strs[2];

                    string fktable = "T" + attr.Key;

                    if (isAddAnd == false)
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";
                        else
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";

                        isAddAnd = true;
                    }
                    else
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";
                        else
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + nameCol + "  (+) )";
                    }

                }
                else
                {

                    Entity en1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                    string fktable = "T" + attr.Key;

                    if (isAddAnd == false)
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";
                        else
                            where += "(  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";

                        isAddAnd = true;
                    }
                    else
                    {
                        if (attr.MyDataType == DataType.AppString)
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";
                        else
                            where += " AND (  " + mytable + "." + attr.Key + "=" + fktable + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue) + "  (+) )";
                    }
                }
            }

            where = where.Replace("WHERE  AND", "WHERE");
            where = where.Replace("WHERE AND", "WHERE");
            return from + where;
        }
        public static string GenerCreateTableSQLOf_OLE(Entity en)
        {
            string sql = "CREATE TABLE  " + en.EnMap.PhysicsTable + " (";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                int len = attr.MaxLength;
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.MaxLength <= 254)
                        {
                            if (attr.ItIsPK)
                                sql += "[" + attr.Field + "]  varchar (" + attr.MaxLength + ") NOT NULL,";
                            else
                                sql += "[" + attr.Field + "]  varchar (" + attr.MaxLength + ") NULL,";
                        }
                        else
                        {
                            if (attr.ItIsPK)
                                sql += "[" + attr.Field + "]  text  NOT NULL,";
                            else
                                sql += "[" + attr.Field + "] text,";
                        }
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        sql += "[" + attr.Field + "]  varchar (" + attr.MaxLength + ") NULL,";
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                        sql += "[" + attr.Field + "] float  NULL,";
                        break;
                    case DataType.AppBoolean:
                    case DataType.AppInt:
                        if (attr.ItIsPK)
                            sql += "[" + attr.Field + "] int NOT NULL,";
                        else
                            sql += "[" + attr.Field + "] int  NULL,";
                        break;
                    case DataType.AppDouble:
                        sql += "[" + attr.Field + "]  float  NULL,";
                        break;
                    default:
                        break;
                }
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")";
            return sql;
        }
        public static string GenerCreateTableSQL(Entity en)
        {
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    return GenerCreateTableSQLOfOra(en);
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    return GenerCreateTableSQLOfPostgreSQL(en);
                case DBType.Informix:
                    return GenerCreateTableSQLOfInfoMix(en);
                case DBType.MSSQL:
                case DBType.Access:
                    return GenerCreateTableSQLOfMS(en);
                default:
                    break;
            }
            return null;
        }
        /// <summary>
        /// 生成sql.
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GenerCreateTableSQLOfOra(Entity en)
        {
            if (en.EnMap.PhysicsTable == null)
                throw new Exception("您没有为[" + en.EnDesc + "],设置物理表。");

            if (en.EnMap.PhysicsTable.Trim().Length == 0)
                throw new Exception("您没有为[" + en.EnDesc + "],设置物理表。");

            string sql = "CREATE TABLE  " + en.EnMap.PhysicsTable + " (";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.ItIsPK)
                            sql += attr.Field + " varchar (" + attr.MaxLength + ") NOT NULL,";
                        else
                        {
                            if (attr.MaxLength >= 4000)
                                sql += attr.Field + " varchar (" + attr.MaxLength + ") NULL,";
                            else
                                sql += attr.Field + " varchar (" + attr.MaxLength + ") NULL,";
                        }
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        sql += attr.Field + " varchar (" + attr.MaxLength + ") NULL,";
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                    case DataType.AppDouble:
                        sql += attr.Field + " float NULL,";
                        break;
                    case DataType.AppBoolean:
                    case DataType.AppInt:

                        if (attr.ItIsPK)
                        {
                            if (attr.UIBindKey == "1")
                                if(SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                                    sql += attr.Field + " INT GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1) PRIMARY KEY,";
                                else
                                    sql += attr.Field + " int  primary key identity(1,1),";
                            else
                                sql += attr.Field + " int NOT NULL,";
                        }
                        else
                        {
                            sql += attr.Field + " int ,";
                        }
                        break;
                    default:
                        break;
                }
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")";

            return sql;
        }
        public static string GenerCreateTableSQLOfInfoMix(Entity en)
        {
            if (en.EnMap.PhysicsTable == null)
                throw new Exception("您没有为[" + en.EnDesc + "],设置物理表。");

            if (en.EnMap.PhysicsTable.Trim().Length == 0)
                throw new Exception("您没有为[" + en.EnDesc + "],设置物理表。");

            string sql = "CREATE TABLE  " + en.EnMap.PhysicsTable + " (";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.MaxLength >= 255)
                        {
                            if (attr.ItIsPK)
                                sql += attr.Field + " lvarchar (" + attr.MaxLength + "),";
                            else
                                sql += attr.Field + " lvarchar (" + attr.MaxLength + "),";
                        }
                        else
                        {
                            if (attr.ItIsPK)
                                sql += attr.Field + " varchar (" + attr.MaxLength + ") NOT NULL,";
                            else
                                sql += attr.Field + " varchar (" + attr.MaxLength + "),";
                        }
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        sql += attr.Field + " varchar (" + attr.MaxLength + "),";
                        break;
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                    case DataType.AppDouble:
                        sql += attr.Field + " float,";
                        break;
                    case DataType.AppBoolean:
                    case DataType.AppInt:
                        //说明这个是自动增长的列.
                        if (attr.ItIsPK)
                        {
                            if (attr.UIBindKey == "1")
                                sql += attr.Field + "  Serial not null,";
                            else
                                sql += attr.Field + " int8 NOT NULL,";
                        }
                        else
                        {
                            sql += attr.Field + " int8,";
                        }
                        break;
                    default:
                        break;
                }
            }

            sql = sql.Substring(0, sql.Length - 1);
            sql += ")";

            return sql;
        }
        /// <summary>
        /// 生成sql.
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GenerCreateTableSQLOfMySQL(Entity en)
        {
            if (en.EnMap.PhysicsTable == null)
                throw new Exception("您没有为[" + en.EnDesc + "],设置物理表。");

            if (en.EnMap.PhysicsTable.Trim().Length == 0)
                throw new Exception("您没有为[" + en.EnDesc + "],设置物理表。");

            string sql = "CREATE TABLE  " + en.EnMap.PhysicsTable + " (";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.ItIsPK)
                            sql += attr.Field + " NVARCHAR (" + attr.MaxLength + ") NOT NULL COMMENT '" + attr.Desc + "',";
                        else
                        {
                            if (attr.MaxLength > 3000)
                                sql += attr.Field + " TEXT NULL COMMENT '" + attr.Desc + "',";
                            else
                                sql += attr.Field + " NVARCHAR (" + attr.MaxLength + ") NULL COMMENT '" + attr.Desc + "',";
                        }
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        sql += attr.Field + " NVARCHAR (" + attr.MaxLength + ") NULL COMMENT '" + attr.Desc + "',";
                        break;
                    case DataType.AppFloat:
                        sql += attr.Field + " float  NULL COMMENT '" + attr.Desc + "',";
                        break;
                    case DataType.AppMoney:
                        sql += attr.Field + " decimal(20,4)  NULL COMMENT '" + attr.Desc + "',";
                        break;
                    case DataType.AppDouble:
                        sql += attr.Field + " double  NULL COMMENT '" + attr.Desc + "',";
                        break;
                    case DataType.AppBoolean:
                    case DataType.AppInt:
                        if (attr.ItIsPK && attr.Key == "OID")
                        {
                            if (attr.UIBindKey == "1")
                                sql += attr.Field + " INT(4) primary key not null auto_increment COMMENT '" + attr.Desc + "',";
                            else
                                sql += attr.Field + " INT NOT NULL COMMENT '" + attr.Desc + "',";
                        }
                        else
                        {
                            if (attr.DefValType == 0 && attr.DefaultVal.ToString().Equals(MapAttrAttr.DefaultVal) == true)
                                sql += attr.Field + " INT  NULL COMMENT '" + attr.Desc + "',";
                            else
                                sql += attr.Field + " INT DEFAULT " + attr.DefaultVal + " COMMENT '" + attr.Desc + "',";
                        }
                        break;
                    default:
                        break;
                }
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ") default charset=utf8 ";

            return sql;
        }
        public static string GenerFormWhereOfOra(Entity en, Map map)
        {
            string from = " FROM " + map.PhysicsTable;
            //	string where="  ";
            string table = "";
            string tableAttr = "";
            string enTable = map.PhysicsTable;

            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.Normal || attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                {
                    //Entity en1= ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                    if (attr.UIBindKey.Contains(",") == true)
                    {
                        string[] strs = attr.UIBindKey.Split(',');

                        table = strs[0]; // en1.EnMap.PhysicsTable;
                        string noCol = strs[1]; // en1.EnMap.PhysicsTable;
                        string nameCol = strs[2]; // en1.EnMap.PhysicsTable;

                        tableAttr = "T" + attr.Key + "";
                        from = from + " LEFT OUTER JOIN " + table + "   " + tableAttr + " ON " + enTable + "." + attr.Field + "=" + tableAttr + "." + noCol;
                        //where=where+" AND "+" ("+en.EnMap.PhysicsTable+"."+attr.Field+"="+en1.EnMap.PhysicsTable+"_"+attr.Key+"."+en1.EnMap.Attrs.GetFieldByKey(attr.UIRefKeyValue )+" ) "  ;
                        continue;

                    }
                    else
                    {
                        Entity en1 = attr.HisFKEn;

                        table = en1.EnMap.PhysicsTable;
                        tableAttr = "T" + attr.Key + "";
                        from = from + " LEFT OUTER JOIN " + table + "   " + tableAttr + " ON " + enTable + "." + attr.Field + "=" + tableAttr + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue);
                        //where=where+" AND "+" ("+en.EnMap.PhysicsTable+"."+attr.Field+"="+en1.EnMap.PhysicsTable+"_"+attr.Key+"."+en1.EnMap.Attrs.GetFieldByKey(attr.UIRefKeyValue )+" ) "  ;
                        continue;
                    }
                }
                if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                {
                    //from= from+ " LEFT OUTER JOIN "+table+" AS "+tableAttr+ " ON "+enTable+"."+attr.Field+"="+tableAttr+"."+en1.EnMap.Attrs.GetFieldByKey( attr.UIRefKeyValue );
                    tableAttr = "Enum_" + attr.Key;
                    from = from + " LEFT OUTER JOIN ( SELECT Lab, IntKey FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + attr.UIBindKey + "' )  Enum_" + attr.Key + " ON " + enTable + "." + attr.Field + "=" + tableAttr + ".IntKey ";
                    //	where=where+" AND  ( "+en.EnMap.PhysicsTable+"."+attr.Field+"= Enum_"+attr.Key+".IntKey ) ";
                }
            }
            //from=from+", "+en.EnMap.PhysicsTable;
            //where="("+where+")";			
            return from + "  WHERE (1=1) ";
        }

        public static string GenerFormWhereOfMS(Entity en)
        {
            string from = " FROM " + en.EnMap.PhysicsTable;

            //去除原因有时获取外键不正确
            //if (en.EnMap.HisFKAttrs.Count == 0)
            //    return from + " WHERE (1=1)";

            string mytable = en.EnMap.PhysicsTable;
            Attrs fkAttrs = en.EnMap.Attrs;
            MapAttr mapAttr = null;
            foreach (Attr attr in fkAttrs)
            {
                if (attr.ItIsFK == false)
                    continue;

                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                mapAttr = attr.ToMapAttr;

                //去除webservice填充DDL数据的类型，2015.9.25，added by liuxc
                if (mapAttr.LGType == FieldTypeS.Normal && attr.UIContralType == UIContralType.DDL)
                    continue;


                if (attr.UIBindKey.Contains(",") == true)
                {
                    string[] strs = attr.UIBindKey.Split(',');

                    string fktable = strs[0];
                    string noCol = strs[1];
                    string nameCol = strs[2];

                    if (fktable.Equals("Port_Emp") == true && mytable.Equals("WF_NodeEmp") == false && BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                        from += " LEFT JOIN " + fktable + " AS " + fktable + "_" + attr.Key + " ON " + mytable + "." + attr.Field + "=" + fktable + "_" + attr.Field + ".UserID  AND " + fktable + "_" + attr.Field + ".OrgNo = '" + BP.Web.WebUser.OrgNo + "' ";
                    else
                        from += " LEFT JOIN " + fktable + " AS " + fktable + "_" + attr.Key + " ON " + mytable + "." + attr.Field + "=" + fktable + "_" + attr.Field + "." + noCol;
                }
                else
                {
                    string fktable = attr.HisFKEn.EnMap.PhysicsTable;
                    Attr refAttr = attr.HisFKEn.EnMap.GetAttrByKey(attr.UIRefKeyValue);
                    //added by liuxc,2017-9-11，此处增加是否存在实体表，因新增的字典表类型“动态SQL查询”，此类型没有具体的实体表，完全由SQL动态生成的数据集合，此处不判断会使生成的SQL报错
                    //if (DBAccess.IsExitsObject(fktable))
                    if (fktable.Equals("Port_Emp") == true && mytable.Equals("WF_NodeEmp") == false && BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                        from += " LEFT JOIN " + fktable + " AS " + fktable + "_" + attr.Key + " ON " + mytable + "." + attr.Field + "=" + fktable + "_" + attr.Field + ".UserID  AND " + fktable + "_" + attr.Field + ".OrgNo = '" + BP.Web.WebUser.OrgNo + "' ";
                    else
                        from += " LEFT JOIN " + fktable + " AS " + fktable + "_" + attr.Key + " ON " + mytable + "." + attr.Field + "=" + fktable + "_" + attr.Field + "." + refAttr.Field;
                }

            }
            return from + " WHERE (1=1) ";
        }
        /// <summary>
        /// GenerFormWhere
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GenerFormWhereOfMS(Entity en, Map map)
        {
            string from = " FROM " + map.PhysicsTable;
            //	string where="  ";
            string table = "";
            string tableAttr = "";
            string enTable = map.PhysicsTable;
            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.Normal || attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                {
                    if (attr.UIBindKey.Contains(",") == true)
                    {
                        string[] strs = attr.UIBindKey.Split(',');
                        string ptable = strs[0];
                        string noCol = strs[1];
                        string nameCol = strs[2];

                        tableAttr = ptable + "_" + attr.Key;
                        if (attr.MyDataType == DataType.AppInt)
                        {
                            from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON ISNULL( " + enTable + "." + attr.Field + ", " + en.GetValIntByKey(attr.Key) + ")=" + tableAttr + "." + noCol;
                        }
                        else
                        {
                            if (table.Equals("Port_Emp") == true && BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                                from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON ISNULL( " + enTable + "." + attr.Field + ", '" + en.GetValByKey(attr.Key) + "')=" + tableAttr + ".UserID AND " + tableAttr + ".OrgNo = '" + BP.Web.WebUser.OrgNo + "' ";
                            else
                                from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON ISNULL( " + enTable + "." + attr.Field + ", '" + en.GetValByKey(attr.Key) + "')=" + tableAttr + "." + noCol;
                        }
                    }
                    else
                    {
                        //Entity en1= ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        Entity en1 = attr.HisFKEn;

                        table = en1.EnMap.PhysicsTable;
                        tableAttr = en1.EnMap.PhysicsTable + "_" + attr.Key;
                        if (attr.MyDataType == DataType.AppInt)
                        {
                            from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON ISNULL( " + enTable + "." + attr.Field + ", " + en.GetValIntByKey(attr.Key) + ")=" + tableAttr + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue);
                        }
                        else
                        {
                            if (table.Equals("Port_Emp") == true && BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                                from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON ISNULL( " + enTable + "." + attr.Field + ", '" + en.GetValByKey(attr.Key) + "')=" + tableAttr + ".UserID AND " + tableAttr + ".OrgNo = '" + BP.Web.WebUser.OrgNo + "' ";
                            else
                                from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON ISNULL( " + enTable + "." + attr.Field + ", '" + en.GetValByKey(attr.Key) + "')=" + tableAttr + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue);

                        }
                    }
                    //where=where+" AND "+" ("+en.EnMap.PhysicsTable+"."+attr.Field+"="+en1.EnMap.PhysicsTable+"_"+attr.Key+"."+en1.EnMap.Attrs.GetFieldByKey(attr.UIRefKeyValue )+" ) "  ;
                    continue;
                }

                if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                {
                    //from= from+ " LEFT OUTER JOIN "+table+" AS "+tableAttr+ " ON "+enTable+"."+attr.Field+"="+tableAttr+"."+en1.EnMap.Attrs.GetFieldByKey( attr.UIRefKeyValue );
                    tableAttr = "Enum_" + attr.Key;
                    from = from + " LEFT OUTER JOIN ( SELECT Lab, IntKey FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + attr.UIBindKey + "' )  Enum_" + attr.Key + " ON ISNULL( " + enTable + "." + attr.Field + ", " + en.GetValIntByKey(attr.Key) + ")=" + tableAttr + ".IntKey ";
                    //	where=where+" AND  ( "+en.EnMap.PhysicsTable+"."+attr.Field+"= Enum_"+attr.Key+".IntKey ) ";
                }
            }
            //from=from+", "+en.EnMap.PhysicsTable;
            //where="("+where+")";			
            return from + "  WHERE (1=1) ";
        }
        /// <summary>
        /// GenerFormWhere
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        protected static string GenerFormWhereOfMSOLE(Entity en)
        {
            string fromTop = en.EnMap.PhysicsTable;

            string from = "";
            //	string where="  ";
            string table = "";
            string tableAttr = "";
            string enTable = en.EnMap.PhysicsTable;
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Normal || attr.MyFieldType == FieldType.PK || attr.MyFieldType == FieldType.RefText)
                    continue;
                fromTop = "(" + fromTop;

                if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                {
                    if (attr.UIBindKey.Contains(","))
                    {
                        string[] strs = attr.UIBindKey.Split(',');
                        string ptable = strs[0];
                        string noCol = strs[1];
                        string nameCol = strs[2];

                        table = ptable;
                        tableAttr = ptable + "_" + attr.Key;

                        if (attr.MyDataType == DataType.AppInt)
                        {
                            from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON IIf( ISNULL( " + enTable + "." + attr.Field + "), " + en.GetValIntByKey(attr.Key) + " , " + enTable + "." + attr.Field + " )=" + tableAttr + "." + nameCol;
                        }
                        else
                        {
                            from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON IIf( ISNULL( " + enTable + "." + attr.Field + "), '" + en.GetValStringByKey(attr.Key) + "', " + enTable + "." + attr.Field + " )=" + tableAttr + "." + nameCol;
                        }
                    }
                    else
                    {
                        Entity en1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        table = en1.EnMap.PhysicsTable;
                        tableAttr = en1.EnMap.PhysicsTable + "_" + attr.Key;

                        if (attr.MyDataType == DataType.AppInt)
                        {
                            from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON IIf( ISNULL( " + enTable + "." + attr.Field + "), " + en.GetValIntByKey(attr.Key) + " , " + enTable + "." + attr.Field + " )=" + tableAttr + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue);
                        }
                        else
                        {
                            from = from + " LEFT OUTER JOIN " + table + " AS " + tableAttr + " ON IIf( ISNULL( " + enTable + "." + attr.Field + "), '" + en.GetValStringByKey(attr.Key) + "', " + enTable + "." + attr.Field + " )=" + tableAttr + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyValue);
                        }
                    }
                }
                if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                {
                    //from= from+ " LEFT OUTER JOIN "+table+" AS "+tableAttr+ " ON "+enTable+"."+attr.Field+"="+tableAttr+"."+en1.EnMap.Attrs.GetFieldByKey( attr.UIRefKeyValue );
                    tableAttr = "Enum_" + attr.Key;
                    from = from + " LEFT OUTER JOIN ( SELECT Lab, IntKey FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + attr.UIBindKey + "' )  Enum_" + attr.Key + " ON IIf( ISNULL( " + enTable + "." + attr.Field + "), " + en.GetValIntByKey(attr.Key) + ", " + enTable + "." + attr.Field + ")=" + tableAttr + ".IntKey ";
                    //	where=where+" AND  ( "+en.EnMap.PhysicsTable+"."+attr.Field+"= Enum_"+attr.Key+".IntKey ) ";
                }

                from = from + ")";
            }
            fromTop = " FROM " + fromTop;
            //from=from+", "+en.EnMap.PhysicsTable;
            //where="("+where+")";			
            return fromTop + from + "  WHERE (1=1) ";
        }

        protected static string SelectSQLOfOra(Entity en, int topNum)
        {
            string val = ""; // key = null;
            string mainTable = "";
            Map map = en.EnMap;
            if (en.EnMap.HisFKAttrs.Count != 0)
                mainTable = en.EnMap.PhysicsTable + ".";
            Attrs attrs = map.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                        {
                            if (attr.Field == "Domain")
                            {
                                if (en.EnMap.EnDBUrl.DBType == DBType.DM)
                                    val = val + ", " + mainTable + "\"" + attr.Field + "\"";
                            }
                            else
                            {
                                if (attr.ItIsKeyEqualField)
                                    val = val + ", " + mainTable + attr.Field;
                                else
                                    val = val + "," + mainTable + attr.Field + " " + attr.Key;
                            }
                        }
                        else
                        {
                            val = val + ",NVL(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') " + attr.Key;
                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {

                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", T" + attr.Key + "." + noCol + " AS " + attr.Key + "Text";
                            }
                            else
                            {

                                Map mapFK = attr.HisFKEn.EnMap;
                                val = val + ", T" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                            }
                        }
                        if ((attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) && attr.UIContralType != UIContralType.CheckBok)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey))
                                throw new Exception("@" + en.ToString() + " key=" + attr.Key + " UITag=" + attr.UITag);

                            SysEnums ses = new SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(en.ToString(), mainTable, attr.Key, attr.Field, attr.UIBindKey,
                                attr.DefaultVal.ToString());
                        }
                        break;
                    case DataType.AppInt:
                        if (attr.DefValType == 0)
                            val = val + ", " + mainTable + attr.Key;
                        else
                            val = val + ",NVL(" + mainTable + attr.Field + "," + attr.DefaultVal + ")   " + attr.Key + "";

                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey))
                                throw new Exception("@" + en.ToString() + " key=" + attr.Key + " UITag=" + attr.UITag);

                            SysEnums ses = new SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(en.ToString(), mainTable, attr.Key, attr.Field, attr.UIBindKey,
                                attr.DefaultVal.ToString());
                        }
                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {

                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];

                                val = val + ", T" + attr.Key + "." + nameCol + "  AS " + attr.Key + "Text";

                            }
                            else
                            {

                                if (attr.HisFKEns == null)
                                    throw new Exception("@生成SQL错误 Entity=" + en.ToString() + " 外键字段｛" + attr.Key + "." + attr.Desc + ", UIBindKey=" + attr.UIBindKey + "｝已经无效, 也许该类或者外键字段被移除，请通知管理员解决。");

                                Map mapFK = attr.HisFKEn.EnMap;
                                val = val + ", T" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + "  AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppFloat:
                        val = val + ", NVL( " + mainTable + attr.Field + " ," +
                            attr.DefaultVal.ToString() + ") AS  " + attr.Key;
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", NVL( " + mainTable + attr.Field + ",0) " + attr.Key;
                        else
                            val = val + ", NVL(" + mainTable + attr.Field + ",1) " + attr.Key;
                        break;
                    case DataType.AppDouble:
                        val = val + ", NVL( " + mainTable + attr.Field + " ," +
                            attr.DefaultVal.ToString() + ") " + attr.Key;
                        break;
                    case DataType.AppMoney:
                        val = val + ", NVL( " + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") " + attr.Key;
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                            val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        else
                        {
                            val = val + ",NVL(" + mainTable + attr.Field + ",'" +
                                                         attr.DefaultVal.ToString() + "') " + attr.Key;
                        }
                        break;
                    default:
                        throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }

            return " SELECT  " + val.Substring(1) + SqlBuilder.GenerFormWhereOfOra(en);
        }
        /// <summary>
        /// SelectSQLOfInformix
        /// </summary>
        /// <param name="en"></param>
        /// <param name="topNum"></param>
        /// <returns></returns>
        protected static string SelectSQLOfInformix(Entity en, int topNum)
        {
            string val = "";
            string mainTable = "";

            if (en.EnMap.HisFKAttrs.Count != 0)
                mainTable = en.EnMap.PhysicsTable + ".";

            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                        {
                            if (attr.ItIsKeyEqualField)
                                val = val + ", " + mainTable + attr.Field;
                            else
                                val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        }
                        else
                        {
                            val = val + ",NVL(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') " + attr.Key;
                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", " + ptable + "_" + attr.Key + "." + nameCol + " AS " + attr.Key + "Text";
                            }
                            else
                            {
                                Map map = attr.HisFKEn.EnMap;
                                val = val + ", " + map.PhysicsTable + "_" + attr.Key + "." + map.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppInt:

                        val = val + ",NVL(" + mainTable + attr.Field + "," +
                        attr.DefaultVal + ")   " + attr.Key + "";

                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey))
                                throw new Exception("@" + en.ToString() + " key=" + attr.Key + " UITag=" + attr.UITag);

                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(en.ToString(), mainTable, attr.Key, attr.Field, attr.UIBindKey,
                                attr.DefaultVal.ToString());
                        }
                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            Map map = attr.HisFKEn.EnMap;
                            val = val + ", " + map.PhysicsTable + "_" + attr.Key + "." + map.GetFieldByKey(attr.UIRefKeyText) + "  AS " + attr.Key + "Text";
                        }
                        break;
                    case DataType.AppFloat:
                        val = val + ", NVL( round(" + mainTable + attr.Field + ",4) ," +
                            attr.DefaultVal.ToString() + ") AS  " + attr.Key;
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", NVL( " + mainTable + attr.Field + ",0) " + attr.Key;
                        else
                            val = val + ", NVL(" + mainTable + attr.Field + ",1) " + attr.Key;
                        break;
                    case DataType.AppDouble:
                        val = val + ", NVL( round(" + mainTable + attr.Field + " ,4) ," +
                            attr.DefaultVal.ToString() + ") " + attr.Key;
                        break;
                    case DataType.AppMoney:
                        val = val + ", NVL( round(" + mainTable + attr.Field + ",4)," +
                            attr.DefaultVal.ToString() + ") " + attr.Key;
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                            val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        else
                        {
                            val = val + ",NVL(" + mainTable + attr.Field + ",'" +
                                                         attr.DefaultVal.ToString() + "') " + attr.Key;
                        }
                        break;
                    default:
                        throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }
            return " SELECT  " + val.Substring(1) + SqlBuilder.GenerFormWhereOfInformix(en);
        }

        public static string SelectSQL(Entity en, int topNum)
        {
            switch (en.EnMap.EnDBUrl.DBType)
            {
                case DBType.MSSQL:
                    return SqlBuilder.SelectSQLOfMS(en, topNum);
                case DBType.MySQL:
                    return SqlBuilder.SelectSQLOfMySQL(en, topNum);
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    return SqlBuilder.SelectSQLOfPostgreSQL(en, topNum);
                case DBType.Access:
                    return SqlBuilder.SelectSQLOfOLE(en, topNum);
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    return SqlBuilder.SelectSQLOfOra(en, topNum);
                case DBType.Informix:
                    return SqlBuilder.SelectSQLOfInformix(en, topNum);
                default:
                    throw new Exception("没有判断的情况");
            }
        }

        /// <summary>
        /// 得到sql of select
        /// </summary>
        /// <param name="en">实体</param>
        /// <param name="top">top</param>
        /// <returns>sql</returns>
        public static string SelectCountSQL(Entity en)
        {
            switch (en.EnMap.EnDBUrl.DBType)
            {
                case DBType.MSSQL:
                    return SqlBuilder.SelectCountSQLOfMS(en);
                case DBType.Access:
                    return SqlBuilder.SelectSQLOfOLE(en, 0);
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                case DBType.Informix:
                    return SqlBuilder.SelectSQLOfOra(en, 0);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 建立SelectSQLOfOLE 
        /// </summary>
        /// <param name="en">要执行的en</param>
        /// <param name="topNum">最高查询个数</param>
        /// <returns>返回查询sql</returns>
        public static string SelectSQLOfOLE(Entity en, int topNum)
        {
            string val = ""; // key = null;
            string mainTable = en.EnMap.PhysicsTable + ".";
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        val = val + ", IIf( ISNULL(" + mainTable + attr.Field + "), '" +
                            attr.DefaultVal.ToString() + "', " + mainTable + attr.Field + " ) AS [" + attr.Key + "]";
                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", IIf( ISNULL(" + ptable + "_" + attr.Key + "." + nameCol + ") ,''," + ptable + "_" + attr.Key + "." + nameCol + ") AS " + attr.Key + "Text";
                            }
                            else
                            {
                                Entity en1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                                val = val + ", IIf( ISNULL(" + en1.EnMap.PhysicsTable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + ") ,''," + en1.EnMap.PhysicsTable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + ") AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppInt:
                        val = val + ",IIf( ISNULL(" + mainTable + attr.Field + ")," +
                            attr.DefaultVal.ToString() + "," + mainTable + attr.Field + ") AS [" + attr.Key + "]";
                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                        {
                            val = val + ",IIf( ISNULL( Enum_" + attr.Key + ".Lab),'',Enum_" + attr.Key + ".Lab ) AS " + attr.Key + "Text";
                        }
                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            Entity en1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                            val = val + ", IIf( ISNULL(" + en1.EnMap.PhysicsTable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + "),0 ," + en1.EnMap.PhysicsTable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + ") AS " + attr.Key + "Text";
                        }
                        break;
                    case DataType.AppFloat:
                        val = val + ",IIf( ISNULL( Round(" + mainTable + attr.Field + ",2) )," +
                            attr.DefaultVal.ToString() + "," + mainTable + attr.Field + ") AS [" + attr.Key + "]";
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", IIf( ISNULL(" + mainTable + attr.Field + "),0 ," + mainTable + attr.Field + ") AS [" +
                                attr.Key + "]";
                        else
                            val = val + ",IIf( ISNULL(" + mainTable + attr.Field + "),1," + mainTable + attr.Field + ") AS [" +
                                attr.Key + "]";
                        break;
                    case DataType.AppDouble:
                        val = val + ", IIf(ISNULL( Round(" + mainTable + attr.Field + ",4) )," +
                            attr.DefaultVal.ToString() + "," + mainTable + attr.Field + ") AS [" + attr.Key + "]";
                        break;
                    case DataType.AppMoney:
                        val = val + ",IIf( ISNULL(  Round(" + mainTable + attr.Field + ",2) )," +
                            attr.DefaultVal.ToString() + "," + mainTable + attr.Field + ") AS [" + attr.Key + "]";
                        break;
                    case DataType.AppDate:
                        val = val + ", IIf(ISNULL( " + mainTable + attr.Field + "), '" +
                            attr.DefaultVal.ToString() + "'," + mainTable + attr.Field + ") AS [" + attr.Key + "]";
                        break;
                    case DataType.AppDateTime:
                        val = val + ", IIf(ISNULL(" + mainTable + attr.Field + "), '" +
                            attr.DefaultVal.ToString() + "'," + mainTable + attr.Field + ") AS [" + attr.Key + "]";
                        break;
                    default:
                        throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }
            if (topNum == -1 || topNum == 0)
                topNum = 99999;

            //return  " SELECT TOP " +topNum.ToString()+" " +val.Substring(1) + " FROM "+en.EnMap.PhysicsTable;
            return " SELECT TOP " + topNum.ToString() + " " + val.Substring(1) + SqlBuilder.GenerFormWhereOfMSOLE(en);
        }
        /// <summary>
        /// 生成sqlserver标准的sql
        /// </summary>
        /// <param name="en">实体类</param>
        /// <param name="topNum">前几行</param>
        /// <returns>生成的SQL</returns>
        public static string SelectSQLOfMS(Entity en, int topNum)
        {
            string val = ""; // key = null;
            string mainTable = "";

            Map map = en.EnMap;

            // if (en.EnMap.HisFKAttrs.Count != 0)
            mainTable = map.PhysicsTable + ".";

            Attrs attrs = map.Attrs;
            if (attrs.Count == 0)
            {
                en.DTSMapToSys_MapData();
                throw new Exception("err@错误:" + en.ToString() + "没有attrs属性，无法生成SQL, 如果是动态实体请关闭后，重新打开一次。");
            }

            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                        {
                            if (attr.ItIsKeyEqualField)
                            {
                                if (attr.Key == "Domain")
                                {
                                    if (BP.Difference.SystemConfig.AppCenterDBType == DBType.DM)
                                        val = val + ", " + mainTable + " \"" + attr.Field + "\"";
                                    else
                                        val = val + ", " + mainTable + attr.Field;
                                }
                                else
                                    val = val + ", " + mainTable + attr.Field;

                            }
                            else
                            {
                                if (attr.Key == "Domain")
                                {
                                    if (BP.Difference.SystemConfig.AppCenterDBType == DBType.DM)
                                        val = val + ", " + mainTable + " \"" + attr.Field + "\" " + attr.Key;
                                    else
                                        val = val + "," + mainTable + attr.Field + " " + attr.Key;
                                }
                                else
                                    val = val + "," + mainTable + attr.Field + " " + attr.Key;
                            }
                        }
                        else
                        {
                            val = val + ",ISNULL(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') " + attr.Key;
                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            string ptable = "", noCol = "", nameCol = "";


                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');

                                ptable = strs[0];
                                noCol = strs[1];
                                nameCol = strs[2];

                                if (DBAccess.IsExitsObject(ptable))
                                    val = val + ", " + ptable + "_" + attr.Key + "." + noCol + " AS " + attr.Key + "Text";
                                else
                                    val = val + ", '' AS " + attr.Key + "Text";

                            }
                            else
                            {
                                if (attr.HisFKEns == null)
                                    throw new Exception("@生成SQL错误 Entity=" + en.ToString() + " 外键字段｛" + attr.Key + "." + attr.Desc + ", UIBindKey=" + attr.UIBindKey + "｝已经无效, 也许该类或者外键字段被移除，请通知管理员解决。");

                                Map mapFK = attr.HisFKEn.EnMap;

                                if (DBAccess.IsExitsObject(mapFK.PhysicsTable))
                                    val = val + ", " + mapFK.PhysicsTable + "_" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                                else
                                    val = val + ", '' AS " + attr.Key + "Text";
                            }


                        }
                        if ((attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) && attr.UIContralType != UIContralType.CheckBok)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey) == true)
                                throw new Exception("@系统严重异常:" + en.ToString() + " Key=" + attr.Key + " UITag=" + attr.UITag + ", UIBindKey 无数据，请描述该字段的设计过程，反馈给开发人员。");

#warning 20111-12-03 不应出现异常。
                            if (attr.UIBindKey.Contains(".") == true)
                                throw new Exception("@系统异常:" + en.ToString() + " &UIBindKey=" + attr.UIBindKey + " @Key=" + attr.Key);

                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(mainTable, attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString());
                        }
                        break;
                    case DataType.AppInt:
                        if (attr.ItIsNull || attr.DefValType == 0)
                            val = val + "," + mainTable + attr.Field + " " + attr.Key + "";
                        else
                            val = val + ",ISNULL(" + mainTable + attr.Field + "," + attr.DefaultVal + ")   " + attr.Key + "";

                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey) == true)
                                throw new Exception("@系统严重异常:" + en.ToString() + " Key=" + attr.Key + " UITag=" + attr.UITag + ", UIBindKey 无数据，请描述该字段的设计过程，反馈给开发人员。");

#warning 20111-12-03 不应出现异常。
                            if (attr.UIBindKey.Contains(".") == true)
                                throw new Exception("@系统异常:" + en.ToString() + " &UIBindKey=" + attr.UIBindKey + " @Key=" + attr.Key);

                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(mainTable, attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString());
                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", " + ptable + "_" + attr.Key + "." + nameCol + "  AS " + attr.Key + "Text";
                            }
                            else
                            {
                                Map mapFK = attr.HisFKEn.EnMap;
                                val = val + ", " + mapFK.PhysicsTable + "_" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + "  AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppFloat:
                    case DataType.AppDouble:
                    case DataType.AppMoney:
                        if (attr.ItIsNull)
                            val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        else //不处理
                            val = val + ", ISNULL( " + mainTable + attr.Field + " ," + attr.DefaultVal.ToString() + ") AS  " + attr.Key;
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", ISNULL( " + mainTable + attr.Field + ",0) " + attr.Key;
                        else
                            val = val + ", ISNULL(" + mainTable + attr.Field + ",1) " + attr.Key;
                        break;
                    case DataType.AppDate:
                        if (BP.Difference.SystemConfig.DateType.Equals("datetime") == true)
                        {
                            if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                                val = val + ",ISNULL(CONVERT(varchar(100), " + mainTable + attr.Field + ", 23),'" +
                                                       "CONVERT(varchar(100),  getdate(), 23)') " + attr.Key;
                            else
                                val = val + ",ISNULL(CONVERT(varchar(100), " + mainTable + attr.Field + ", 23),'" +
                                                        "CONVERT(varchar(100), " + attr.DefaultVal.ToString() + ", 23)') " + attr.Key;
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(attr.DefaultVal as string))

                                val = val + "," + mainTable + attr.Field + " " + attr.Key;
                            else
                                val = val + ",ISNULL(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') " + attr.Key;
                        }

                        break;
                    case DataType.AppDateTime:
                        if (BP.Difference.SystemConfig.DateType.Equals("datetime") == true)
                        {
                            if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                                val = val + ",ISNULL(CONVERT(varchar(100), " + mainTable + attr.Field + ", 20),'" +
                                                         "CONVERT(varchar(100), getdate(), 20)') " + attr.Key;
                            else
                                val = val + ",ISNULL(CONVERT(varchar(100), " + mainTable + attr.Field + ", 20),'" +
                                                         "CONVERT(varchar(100), " + attr.DefaultVal.ToString() + ", 20)') " + attr.Key;
                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                                val = val + "," + mainTable + attr.Field + " " + attr.Key;
                            else
                                val = val + ",ISNULL(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') " + attr.Key;
                        }

                        break;
                    default:
                        throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }

            //return  " SELECT TOP " +topNum.ToString()+" " +val.Substring(1) + " FROM "+en.EnMap.PhysicsTable;
            if (topNum == -1 || topNum == 0)
                topNum = 99999;
            return " SELECT  TOP " + topNum.ToString() + " " + val.Substring(1) + SqlBuilder.GenerFormWhereOfMS(en);
        }
        /// <summary>
        /// 生成postgresql标准的sql
        /// </summary>
        /// <param name="en">实体类</param>
        /// <param name="topNum">前几行</param>
        /// <returns>生成的SQL</returns>
        public static string SelectSQLOfPostgreSQL(Entity en, int topNum)
        {
            string val = ""; // key = null;
            string mainTable = "";

            Map map = en.EnMap;
            Attrs attrs = map.Attrs;

            if (map.HisFKAttrs.Count != 0)
                mainTable = map.PhysicsTable + ".";

            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                        {
                            if (attr.ItIsKeyEqualField)
                                val = val + ", " + mainTable + attr.Field;
                            else
                                val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        }
                        else
                        {
                            //   val = val + ",COALESCE(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') AS " + attr.Key;
                            val = val + ",COALESCE(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') AS " + attr.Key;

                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {

                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];

                                if (DBAccess.IsExitsObject(ptable))
                                    val = val + ", " + ptable + "_" + attr.Key + "." + nameCol + " AS " + attr.Key + "Text";
                                else
                                    val = val + ", '' AS " + attr.Key + "Text";
                            }
                            else
                            {

                                if (attr.HisFKEns == null)
                                    throw new Exception("@生成SQL错误 Entity=" + en.ToString() + " 外键字段｛" + attr.Key + "." + attr.Desc + ", UIBindKey=" + attr.UIBindKey + "｝已经无效, 也许该类或者外键字段被移除，请通知管理员解决。");

                                Map mapFK = attr.HisFKEn.EnMap;

                                if (DBAccess.IsExitsObject(mapFK.PhysicsTable))
                                    val = val + ", " + mapFK.PhysicsTable + "_" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                                else
                                    val = val + ", '' AS " + attr.Key + "Text";
                            }
                        }
                        if ((attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) && attr.UIContralType != UIContralType.CheckBok)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey))
                                throw new Exception("@" + en.ToString() + " key=" + attr.Key + " UITag=" + attr.UITag + "");

#warning 20111-12-03 不应出现异常。
                            if (attr.UIBindKey.Contains("."))
                                throw new Exception("@" + en.ToString() + " &UIBindKey=" + attr.UIBindKey + " @Key=" + attr.Key);

                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(mainTable, attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString());
                        }
                        break;
                    case DataType.AppInt:
                        if (attr.ItIsNull || attr.DefValType == 0)
                            val = val + "," + mainTable + attr.Field + " " + attr.Key + "";
                        else
                            val = val + ",COALESCE(" + mainTable + attr.Field + "," + attr.DefaultVal + ")  AS " + attr.Key + "";

                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey))
                                throw new Exception("@" + en.ToString() + " key=" + attr.Key + " UITag=" + attr.UITag + "");

#warning 20111-12-03 不应出现异常。
                            if (attr.UIBindKey.Contains("."))
                                throw new Exception("@" + en.ToString() + " &UIBindKey=" + attr.UIBindKey + " @Key=" + attr.Key);

                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(mainTable, attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString());
                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", " + ptable + "_" + attr.Key + "." + nameCol + "  AS " + attr.Key + "Text";
                            }
                            else
                            {
                                Map mapFK = attr.HisFKEn.EnMap;
                                val = val + ", " + mapFK.PhysicsTable + "_" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + "  AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppFloat:
                    case DataType.AppDouble:
                    case DataType.AppMoney:
                        if (attr.ItIsNull)
                            val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        else //不处理
                            val = val + ", COALESCE( " + mainTable + attr.Field + " ," + attr.DefaultVal.ToString() + ") AS  " + attr.Key;
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", COALESCE( " + mainTable + attr.Field + ",0) AS " + attr.Key;
                        else
                            val = val + ", COALESCE(" + mainTable + attr.Field + ",1) AS " + attr.Key;
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                            val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        else
                        {
                            val = val + ",COALESCE(" + mainTable + attr.Field + ",'" +
                                                         attr.DefaultVal.ToString() + "') AS " + attr.Key;
                        }
                        break;
                    default:
                        throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }

            //return  " SELECT TOP " +topNum.ToString()+" " +val.Substring(1) + " FROM "+en.EnMap.PhysicsTable;
            if (topNum == -1 || topNum == 0)
                topNum = 99999;
            return " SELECT " + val.Substring(1) + SqlBuilder.GenerFormWhereOfMS(en);
        }

        public static string SelectSQLOfMySQL(Entity en, int topNum)
        {
            string val = ""; // key = null;
            string mainTable = "";

            Map map = en.EnMap;
            Attrs attrs = map.Attrs;
            if (map.HisFKAttrs.Count != 0)
                mainTable = en.EnMap.PhysicsTable + ".";

            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                        {
                            if (attr.ItIsKeyEqualField)
                                val = val + ", " + mainTable + attr.Field;
                            else
                                val = val + "," + mainTable + attr.Field + " " + attr.Key;
                        }
                        else
                        {
                            val = val + ",IFNULL(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') " + attr.Key;
                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", " + ptable + "_" + attr.Key + "." + nameCol + " AS " + attr.Key + "Text";
                            }
                            else
                            {
                                Map mapFK = attr.HisFKEn.EnMap;
                                val = val + ", " + mapFK.PhysicsTable + "_" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                            }
                        }
                        if ((attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) && attr.UIContralType != UIContralType.CheckBok)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey))
                                throw new Exception("@" + en.ToString() + " key=" + attr.Key + "绑定的枚举 UIBindKey为空");

#warning 2011-12-03 不应出现异常。
                            if (attr.UIBindKey.Contains("."))
                                throw new Exception("@" + en.ToString() + " &UIBindKey=" + attr.UIBindKey + " @Key=" + attr.Key);

                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(mainTable, attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString());
                        }
                        break;
                    case DataType.AppInt:
                        if (attr.DefValType == 0)
                            val = val + ", " + mainTable + attr.Key;
                        else
                            val = val + ",IFNULL(" + mainTable + attr.Field + "," +
                                attr.DefaultVal + ")   " + attr.Key + "";

                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                        {
                            if (DataType.IsNullOrEmpty(attr.UIBindKey))
                                throw new Exception("@" + en.ToString() + " key=" + attr.Key + "绑定的枚举 UIBindKey为空");

#warning 2011-12-03 不应出现异常。
                            if (attr.UIBindKey.Contains("."))
                                throw new Exception("@" + en.ToString() + " &UIBindKey=" + attr.UIBindKey + " @Key=" + attr.Key);

                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(mainTable, attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString());
                        }

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {

                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", " + ptable + "_" + attr.Key + "." + nameCol + "  AS " + attr.Key + "Text";
                            }
                            else
                            {
                                if (attr.HisFKEns == null)
                                    throw new Exception("@生成SQL错误 Entity=" + en.ToString() + " 外键字段｛" + attr.Key + "." + attr.Desc + ", UIBindKey=" + attr.UIBindKey + "｝已经无效, 也许该类或者外键字段被移除，请通知管理员解决。");

                                Map mapFK = attr.HisFKEn.EnMap;
                                val = val + ", " + mapFK.PhysicsTable + "_" + attr.Key + "." + mapFK.GetFieldByKey(attr.UIRefKeyText) + "  AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppFloat:
                        val = val + ", IFNULL( " + mainTable + attr.Field + " ," +
                            attr.DefaultVal.ToString() + ") AS  " + attr.Key;
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", IFNULL( " + mainTable + attr.Field + ",0) " + attr.Key;
                        else
                            val = val + ", IFNULL(" + mainTable + attr.Field + ",1) " +
                                attr.Key;
                        break;
                    case DataType.AppDouble:
                        val = val + ", IFNULL( " + mainTable + attr.Field + "  ," +
                            attr.DefaultVal.ToString() + ") " + attr.Key;
                        break;
                    case DataType.AppMoney:
                        val = val + ", IFNULL( " + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") " + attr.Key;
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        if (BP.Difference.SystemConfig.DateType.Equals("datetime") == true)
                        {
                            string format = GetDataTypeFormt(attr.ItIsSupperText);

                            if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                                val = val + ",IFNULL(DATE_FORMAT(" + mainTable + attr.Field + ",'" + format + "'),DATE_FORMAT(now(),'" + format + "')) " + attr.Key;
                            else
                                val = val + ",IFNULL(DATE_FORMAT(" + mainTable + attr.Field + ",'" + format + "'),DATE_FORMAT('" + attr.DefaultVal.ToString() + "','" + format + "')) " + attr.Key;

                        }
                        else
                        {
                            if (DataType.IsNullOrEmpty(attr.DefaultVal as string))
                                val = val + "," + mainTable + attr.Field + " " + attr.Key;
                            else
                                val = val + ",IFNULL(" + mainTable + attr.Field + ",'" +
                                                             attr.DefaultVal.ToString() + "') " + attr.Key;
                        }

                        break;
                    default:
                        throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }

            return " SELECT   " + val.Substring(1) + SqlBuilder.GenerFormWhereOfMS(en);
        }

        /// <summary>
        /// 建立selectSQL 
        /// </summary>
        /// <param name="en">要执行的en</param>
        /// <param name="topNum">最高查询个数</param>
        /// <returns>返回查询sql</returns>
        public static string SelectCountSQLOfMS(Entity en)
        {
            // 判断内存里面是否有 此sql.
            string sql = "SELECT COUNT(*) ";
            return sql + SqlBuilder.GenerFormWhereOfMS(en, en.EnMap);
        }
        public static string SelectSQLOfOra(string enName, Map map)
        {
            string val = ""; // key = null;
            string mainTable = map.PhysicsTable + ".";

            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.DefaultVal == null)
                            val = val + ", " + mainTable + attr.Field + " AS " + attr.Key;
                        else
                            val = val + ", NVL(" + mainTable + attr.Field + ", '" + attr.DefaultVal + "') AS " + attr.Key;

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {

                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                val = val + ", T" + attr.Key + "." + nameCol + " AS " + attr.Key + "Text";
                            }
                            else
                            {
                                Entity en1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                                val = val + ", T" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppInt:
                        val = val + ", NVL(" + mainTable + attr.Field + "," +
                            attr.DefaultVal + ") AS  " + attr.Key + "";
                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                        {
                            SysEnums ses = new BP.Sys.SysEnums(attr.UIBindKey, attr.UITag);
                            val = val + "," + ses.GenerCaseWhenForOracle(enName, mainTable, attr.Key, attr.Field, attr.UIBindKey, attr.DefaultVal.ToString());
                        }
                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            Entity en1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                            val = val + ", T" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + "  AS " + attr.Key + "Text";
                        }
                        break;
                    case DataType.AppFloat:
                        val = val + ", NVL( " + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") AS  " + attr.Key;
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", NVL( " + mainTable + attr.Field + " , 0)  AS " + attr.Key;
                        else
                            val = val + ", NVL(  " + mainTable + attr.Field + ", 1)  AS " +
                                attr.Key;
                        break;
                    case DataType.AppDouble:
                        val = val + ", NVL( " + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") AS " + attr.Key;
                        break;
                    case DataType.AppMoney:
                        val = val + ", NVL( " + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") AS " + attr.Key;
                        break;
                    case DataType.AppDate:
                        val = val + ", NVL(  " + mainTable + attr.Field + ", '" +
                            attr.DefaultVal.ToString() + "')  AS " + attr.Key;
                        break;
                    case DataType.AppDateTime:
                        val = val + ", NVL(" + mainTable + attr.Field + ", '" +
                            attr.DefaultVal.ToString() + "') AS " + attr.Key;
                        break;
                    default:
                        throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }
            return "SELECT " + val.Substring(1);
        }
        /// <summary>
        /// 产生sql
        /// </summary>
        /// <returns></returns>
        public static string SelectSQLOfMS(Map map)
        {
            string val = ""; // key = null;
            string mainTable = map.PhysicsTable + ".";
            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.DefaultVal == null)
                            val = val + " " + mainTable + attr.Field + " AS [" + attr.Key + "]";
                        else
                            val = val + ",ISNULL(" + mainTable + attr.Field + ", '" + attr.DefaultVal.ToString() + "') AS [" + attr.Key + "]";

                        if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {

                            if (attr.UIBindKey.Contains(","))
                            {
                                string[] strs = attr.UIBindKey.Split(',');
                                string ptable = strs[0];
                                string noCol = strs[1];
                                string nameCol = strs[2];
                                //  val = val + ", T" + attr.Key + "." + nameCol + " AS " + attr.Key + "Text";
                                val = val + "," + ptable + "_" + attr.Key + "." + nameCol + " AS " + attr.Key + "Text";
                            }
                            else
                            {
                                Entity en1 = attr.HisFKEn;
                                val = val + "," + en1.EnMap.PhysicsTable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppInt:
                        if (attr.ItIsNull)
                        {
                            /*如果允许为空*/
                            val = val + ", " + mainTable + attr.Field + " AS [" + attr.Key + "]";
                        }
                        else
                        {
                            val = val + ", ISNULL(" + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") AS [" + attr.Key + "]";
                            if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
                            {
                                val = val + ", Enum_" + attr.Key + ".Lab  AS " + attr.Key + "Text";
                            }
                            if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                            {
                                //Entity en1= ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                                Entity en1 = attr.HisFKEn;
                                val = val + ", " + en1.EnMap.PhysicsTable + "_" + attr.Key + "." + en1.EnMap.GetFieldByKey(attr.UIRefKeyText) + " AS " + attr.Key + "Text";
                            }
                        }
                        break;
                    case DataType.AppFloat:
                        val = val + ", ISNULL(" + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") AS [" + attr.Key + "]";
                        break;
                    case DataType.AppBoolean:
                        if (attr.DefaultVal.ToString() == "0")
                            val = val + ", ISNULL(" + mainTable + attr.Field + ",0) AS [" +
                                attr.Key + "]";
                        else
                            val = val + ", ISNULL(" + mainTable + attr.Field + ",1) AS [" +
                                attr.Key + "]";
                        break;
                    case DataType.AppDouble:
                        val = val + ", ISNULL(" + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") AS [" + attr.Key + "]";
                        break;
                    case DataType.AppMoney:
                        val = val + ", ISNULL(" + mainTable + attr.Field + "," +
                            attr.DefaultVal.ToString() + ") AS [" + attr.Key + "]";
                        break;
                    case DataType.AppDate:
                        val = val + ", ISNULL(" + mainTable + attr.Field + ", '" +
                            attr.DefaultVal.ToString() + "') AS [" + attr.Key + "]";
                        break;
                    case DataType.AppDateTime:
                        val = val + ", ISNULL(" + mainTable + attr.Field + ", '" +
                            attr.DefaultVal.ToString() + "') AS [" + attr.Key + "]";
                        break;
                    default:
                        if (attr.Key == "MyNum")
                        {
                            val = val + ", COUNT(*)  AS MyNum ";
                            break;
                        }
                        else
                            throw new Exception("@没有定义的数据类型! attr=" + attr.Key + " MyDataType =" + attr.MyDataType);
                }
            }
            return "SELECT TOP @TopNum " + val.Substring(1);
        }
        public static Paras GenerParas(Entity en, string[] keys)
        {
            bool IsEnableNull = BP.Difference.SystemConfig.isEnableNull;
            string mykeys = "@";
            if (keys != null)
                foreach (string key in keys)
                    mykeys += key + "@";

            Map map = en.EnMap;
            Paras ps = new Paras();
            string errKey = "";
            try
            {
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                        continue;

                    if (keys != null)
                    {
                        if (mykeys.Contains("@" + attr.Key + "@") == false)
                        {
                            if (attr.ItIsPK == false)
                                continue;
                        }
                    }

                    errKey = attr.Key;

                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
#warning 替换掉',是因为在直接使用SQL中查询条件是该值的时候会出现SQL错误
                            string val = en.GetValStrByKey(attr.Key).Replace('\'', '~');
                            //对存储的数据进行加密.
                            if (en.EnMap.ItIsJM && attr.ItIsPK == false && DataType.IsNullOrEmpty(val) == false)
                            {

                            }

                            if (attr.MaxLength >= 4000)
                                ps.Add(attr.Key, val, true);
                            else
                                ps.Add(attr.Key, val);

                            break;
                        case DataType.AppBoolean:
                            ps.Add(attr.Key, en.GetValIntByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            if (attr.Key.Equals("MyPK")) //特殊判断解决 truck 是64位的int类型的数值问题.
                            {
                                ps.Add(attr.Key, en.GetValInt64ByKey(attr.Key));
                            }
                            else
                            {
                                if (en.Row[attr.Key] == DBNull.Value)
                                {
                                    if (DataType.IsNullOrEmpty(attr.DefaultValOfReal))
                                        ps.Add(attr.Key, 0);
                                    else
                                        ps.Add(attr.Key, int.Parse(attr.DefaultValOfReal));
                                    continue;
                                }

                                //判断是否为空.
                                Object strInt = en.Row[attr.Key];
                                if (strInt == null || DataType.IsNullOrEmpty(strInt.ToString()))
                                {
                                    if (DataType.IsNullOrEmpty(attr.DefaultValOfReal) == true)
                                        ps.Add(attr.Key, 0);
                                    else
                                        ps.Add(attr.Key, int.Parse(attr.DefaultValOfReal));
                                }
                                else
                                    ps.Add(attr.Key, int.Parse(strInt.ToString()));
                            }
                            break;

                        case DataType.AppFloat:
                        case DataType.AppDouble:
                            //ps.Add(attr.Key, en.GetValFloatByKey(attr.Key, 0));
                            string str = en.GetValStrByKey(attr.Key) as string;
                            if (DataType.IsNullOrEmpty(str))
                            {
                                if (IsEnableNull) //2023-06-06修改，类型不匹配执行SQL报错
                                    ps.Add(attr.Key, null);
                                else
                                    ps.Add(attr.Key, 0);
                            }
                            else
                            {
                                if (attr.DefValType == 0
                                       && attr.DefaultVal.Equals(MapAttrAttr.DefaultVal) == true
                                       && en.GetValIntByKey(attr.Key) == Int64.Parse(MapAttrAttr.DefaultVal))
                                    ps.Add(attr.Key, null);
                                else
                                    ps.Add(attr.Key, decimal.Parse(str));
                            }
                            break;
                        case DataType.AppMoney:
                            object val1 = en.Row.GetValByKey(attr.Key);
                            if (val1 == null || val1 == DBNull.Value)
                            {
                                str = "0";
                            }
                            else
                            {
                                str = val1.ToString();
                                str = str.Replace("￥", "");
                                str = str.Replace(",", "");
                            }
                            if (attr.DefValType == 0
                                       && attr.DefaultVal.Equals(MapAttrAttr.DefaultVal) == true
                                       && str.Equals(MapAttrAttr.DefaultVal))
                                ps.Add(attr.Key, null);
                            else
                                ps.Add(attr.Key, double.Parse(str, System.Globalization.NumberStyles.Any));

                            break;
                        case DataType.AppDate: // 如果是日期类型。
                            string da = en.GetValStrByKey(attr.Key);
                            if (BP.Difference.SystemConfig.DateType.Equals("datetime") && DataType.IsNullOrEmpty(da) == true)
                            {
                                da = GetDefValByDataType(attr.ItIsSupperText);
                            }
                            ps.Add(attr.Key, da);

                            break;
                        case DataType.AppDateTime:
                            string datime = en.GetValStrByKey(attr.Key);
                            if (BP.Difference.SystemConfig.DateType.Equals("datetime") && DataType.IsNullOrEmpty(datime) == true)
                            {
                                datime = GetDefValByDataType(attr.ItIsSupperText);
                            }
                            ps.Add(attr.Key, datime);
                            break;
                        default:
                            throw new Exception("@SqlBulider.update, 没有这个数据类型");
                    }
                }
            }
            catch (Exception ex)
            {
                Attr attr = en.EnMap.GetAttrByKey(errKey);
                errKey = "@attrKey=" + attr.Key + ",AttrVal=" + en.Row[attr.Key] + ",DataType=" + attr.MyDataTypeStr;
                throw new Exception("生成参数期间错误:" + en.ToString() + "," + en.EnMap.PhysicsTable + ";" + errKey + "@错误信息:" + ex.Message);
            }

            if (keys != null)
            {
                switch (en.PK)
                {
                    case "OID":
                    case "WorkID":
                        ps.Add(en.PK, en.GetValIntByKey(en.PK));
                        break;
                    default:
                        ps.Add(en.PK, en.GetValStrByKey(en.PK));
                        break;
                }
            }
            return ps;
        }
        public static string UpdateForPara(Entity en, string[] keys)
        {
            string mykey = "";
            if (keys != null)
                foreach (string s in keys)
                    mykey += "@" + s + ",";

            string dbVarStr = en.HisDBVarStr;
            //  string dbstr = en.HisDBVarStr;
            Map map = en.EnMap;
            string val = "";
            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText || attr.ItIsPK)
                    continue;
                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                if (keys != null)
                    if (mykey.Contains("@" + attr.Key + ",") == false)
                        continue;

                if (dbVarStr == "?")
                {
                    if (attr.Key == "Domain")
                    {
                        if (BP.Difference.SystemConfig.AppCenterDBType == DBType.DM)
                            val = val + ",\"" + attr.Field + "\"=" + dbVarStr;
                        else
                            val = val + "," + attr.Field + "=" + dbVarStr;
                    }
                    else
                        val = val + "," + attr.Field + "=" + dbVarStr;
                }

                else
                {
                    if (attr.Key == "Domain")
                    {
                        if (BP.Difference.SystemConfig.AppCenterDBType == DBType.DM)
                            val = val + ",\"" + attr.Field + "\"=" + dbVarStr + attr.Key;
                        else
                            val = val + "," + attr.Field + "=" + dbVarStr + attr.Key;
                    }
                    else
                        val = val + "," + attr.Field + "=" + dbVarStr + attr.Key;
                }
            }
            if (DataType.IsNullOrEmpty(val))
            {
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (keys != null)
                        if (mykey.Contains("@" + attr.Key + ",") == false)
                            continue;

                    if (attr.Key == "Domain")
                    {
                        if (BP.Difference.SystemConfig.AppCenterDBType == DBType.DM)
                            val = val + ",\"" + attr.Field + "\"=" + attr.Field;
                        else
                            val = val + "," + attr.Field + "=" + attr.Field;
                    }
                    else
                        val = val + "," + attr.Field + "=" + attr.Field;
                }
                //   throw new Exception("@生成SQL出现错误:" + map.EnDesc + "，" + en.ToString() + "，要更新的字段为空。");
            }
            if (!DataType.IsNullOrEmpty(val))
                val = val.Substring(1);
            string sql = "";
            switch (en.EnMap.EnDBUrl.DBType)
            {
                case DBType.MSSQL:
                case DBType.Access:
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    sql = "UPDATE " + en.EnMap.PhysicsTable + " SET " + val +
                        " WHERE " + SqlBuilder.GenerWhereByPK(en, "@");
                    break;
                case DBType.Informix:
                    sql = "UPDATE " + en.EnMap.PhysicsTable + " SET " + val +
                        " WHERE " + SqlBuilder.GenerWhereByPK(en, "?");
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = "UPDATE " + en.EnMap.PhysicsTable + " SET " + val +
                        " WHERE " + SqlBuilder.GenerWhereByPK(en, ":");
                    break;
                default:
                    throw new Exception("no this case db type . ");
            }
            return sql.Replace(",=''", "");
        }
        /// <summary>
        /// Delete sql
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string DeleteForPara(Entity en)
        {
            string dbstr = en.HisDBVarStr;
            string sql = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " +
                SqlBuilder.GenerWhereByPK(en, dbstr);
            return sql;
        }
        public static string InsertForPara(Entity en)
        {
            string dbstr = en.HisDBVarStr;

            bool isInnkey = false;
            if (en.ItIsOIDEntity)
            {
                EntityOID myen = en as EntityOID;
                isInnkey = false; // myen.IsInnKey;
            }

            Map map = en.EnMap;
            Attrs attrs = map.Attrs;

            string key = "", field = "", val = "";
            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                if (isInnkey)
                {
                    if (attr.Key == "OID")
                        continue;
                }


                if (attr.Key == "Domain")
                {
                    if (BP.Difference.SystemConfig.AppCenterDBType == DBType.DM)
                    {
                        key = attr.Key;
                        field = field + ",[\"" + attr.Field + "\"]";
                        val = val + "," + dbstr + attr.Key;
                    }
                    else
                    {
                        key = attr.Key;
                        field = field + ",[" + attr.Field + "]";
                        val = val + "," + dbstr + attr.Key;
                    }
                }
                else
                {
                    key = attr.Key;
                    field = field + ",[" + attr.Field + "]";
                    val = val + "," + dbstr + attr.Key;
                }
            }
            string sql = "INSERT INTO " + en.EnMap.PhysicsTable + " (" +
                field.Substring(1) + " ) VALUES ( " + val.Substring(1) + ")";
            return sql;
        }
        /// <summary>
        /// Insert 
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string Insert(Entity en)
        {
            string key = "", field = "", val = "";
            Map map = en.EnMap;
            Attrs attrs = map.Attrs;
            if (attrs.Count == 0)
                throw new Exception("@实体：" + en.ToString() + " ,Attrs属性集合信息丢失，导致无法生成SQL。");

            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (map.ParaFields != null && map.ParaFields.Contains(attr.Key + ",") == true)
                    continue;

                key = attr.Key;
                field = field + "," + attr.Field;
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        string str = en.GetValStringByKey(key);
                        if (DataType.IsNullOrEmpty(str))
                            str = "";
                        else
                            str = str.Replace('\'', '~');

                        if (attr.UIIsDoc && attr.Key == "Doc")
                        {
                            if (attrs.Contains("DocLength"))
                                en.SetValByKey("DocLength", str.Length);

                            if (str.Length >= 2000)
                            {
                                SysDocFile.SetValV2(en.ToString(), en.PKVal.ToString(), str);
                                if (attrs.Contains("DocLength"))
                                    en.SetValByKey("DocLength", str.Length);
                                val = val + ",''";
                            }
                            else
                            {
                                val = val + ",'" + str + "'";
                            }
                        }
                        else
                        {
                            val = val + ",'" + en.GetValStringByKey(key).Replace('\'', '~') + "'";
                        }
                        break;
                    case DataType.AppInt:
                    case DataType.AppBoolean:
                        val = val + "," + en.GetValIntByKey(key);
                        break;
                    case DataType.AppFloat:
                    case DataType.AppDouble:
                    case DataType.AppMoney:
                        string strNum = en.GetValStringByKey(key).ToString();
                        strNum = strNum.Replace("￥", "");
                        strNum = strNum.Replace(",", "");
                        if (DataType.IsNullOrEmpty(strNum))
                            strNum = "0";
                        val = val + "," + strNum;
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                        string da = en.GetValStringByKey(attr.Key);
                        if (BP.Difference.SystemConfig.DateType.Equals("datetime") == true && DataType.IsNullOrEmpty(da) == true)
                        {
                            da = GetDefValByDataType(attr.ItIsSupperText);
                        }

                        if (da.IndexOf("_DATE") == -1)
                            val = val + ",'" + da + "'";
                        else
                            val = val + "," + da;

                        break;
                    default:
                        throw new Exception("@bulider insert sql error: 没有这个数据类型");
                }
            }
            string sql = "INSERT INTO " + en.EnMap.PhysicsTable + " (" +
                field.Substring(1) + " ) VALUES ( " + val.Substring(1) + ")";
            return sql;
        }
        /// <summary>
        /// 获取判断指定表达式如果为空，则返回指定值的SQL表达式
        /// <para>注：目前只对MSSQL/ORACLE/MYSQL三种数据库做兼容</para>
        /// <para>added by liuxc,2017-03-07</para>
        /// </summary>
        /// <param name="expression">要判断的表达式，在SQL中的写法</param>
        /// <param name="isNullBack">判断的表达式为NULL，返回值的表达式，在SQL中的写法</param>
        /// <returns></returns>
        public static string GetIsNullInSQL(string expression, string isNullBack)
        {
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.MSSQL:
                    return " ISNULL(" + expression + "," + isNullBack + ")";
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    return " NVL(" + expression + "," + isNullBack + ")";
                case DBType.MySQL:
                    return " IFNULL(" + expression + "," + isNullBack + ")";
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    return " COALESCE(" + expression + "," + isNullBack + ")";
                default:
                    throw new Exception("GetIsNullInSQL未涉及的数据库类型");
            }
        }

        private static string GetDataTypeFormt(int dataType)
        {
            string format = "%Y-%m-%d";
            switch (dataType)
            {
                case 0:
                    format = "%Y-%m-%d";
                    break;
                case 1:
                    format = "%Y-%m-%d %H:%i";
                    break;
                case 2:
                    format = "%Y-%m-%d %T";
                    break;
                case 3:
                    format = "%Y-%m";
                    break;
                case 4:
                    format = "%H:%i";
                    break;
                case 5:
                    format = "%H:%i:%s";
                    break;
                case 6:
                    format = "%m-%d";
                    break;
                default:
                    format = "%Y-%m-%d";
                    break;
            }
            return format;
        }
        private static string GetDefValByDataType(int dataType)
        {
            string format = "yyyy-MM-dd";
            switch (dataType)
            {
                case 0:
                    format = "yyyy-MM-dd";
                    break;
                case 1:
                    format = "yyyy-MM-dd HH:mm";
                    break;
                case 2:
                    format = "yyyy-MM-dd HH:mm:ss";
                    break;
                case 3:
                    format = "yyyy-MM";
                    break;
                case 4:
                    format = "HH:mm";
                    break;
                case 5:
                    format = "HH:mm:ss";
                    break;
                case 6:
                    format = "MM-dd";
                    break;
                case 7:
                    format = "yyyy";
                    break;
                default:
                    format = "yyyy-MM-dd";
                    break;
            }
            return DataType.CurrentDateByFormart(format);
        }
    }

}
