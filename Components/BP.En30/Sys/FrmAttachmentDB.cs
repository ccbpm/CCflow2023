﻿using System;
using System.Collections;
using System.IO;
using System.Security.AccessControl;
using Aliyun.OSS;
using BP.DA;
using BP.Difference;
using BP.En;
using BP.Tools;

namespace BP.Sys
{
    /// <summary>
    /// 附件数据存储 - 属性
    /// </summary>
    public class FrmAttachmentDBAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 附件
        /// </summary>
        public const string FK_FrmAttachment = "FK_FrmAttachment";
        /// <summary>
        /// 附件标识
        /// </summary>
        public const string NoOfObj = "NoOfObj";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// RefPKVal
        /// </summary>
        public const string RefPKVal = "RefPKVal";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 文件名称
        /// </summary>
        public const string FileName = "FileName";
        /// <summary>
        /// 文件扩展
        /// </summary>
        public const string FileExts = "FileExts";
        /// <summary>
        /// 文件大小
        /// </summary>
        public const string FileSize = "FileSize";
        /// <summary>
        /// 保存到
        /// </summary>
        public const string FileFullName = "FileFullName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名字
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 所在部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 所在部门名称
        /// </summary>
        public const string FK_DeptName = "FK_DeptName";
        /// <summary>
        /// 类别
        /// </summary>
        public const string Sort = "Sort";
        /// <summary>
        /// 备注
        /// </summary>
        public const string MyNote = "MyNote";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 是否锁定行
        /// </summary>
        public const string IsRowLock = "IsRowLock";
        /// <summary>
        /// 上传的GUID
        /// </summary>
        public const string UploadGUID = "UploadGUID";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 保存方式
        /// </summary>
        public const string AthSaveWay = "AthSaveWay";
    }
    /// <summary>
    /// 附件数据存储
    /// </summary>
    public class FrmAttachmentDB : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 类别
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.Sort);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.Sort, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                string str = this.GetValStringByKey(FrmAttachmentDBAttr.RDT);
                return str.Substring(5, 11);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.RDT, value);
            }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public string FileFullName
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FileFullName);
            }
            set
            {
                string str = value;
                str = str.Replace("~", "-");
                str = str.Replace("'", "-");
                str = str.Replace("*", "-");
                this.SetValByKey(FrmAttachmentDBAttr.FileFullName, str);
            }
        }
        /// <summary>
        /// 上传GUID
        /// </summary>
        public string UploadGUID
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.UploadGUID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.UploadGUID, value);
            }
        }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string FilePathName
        {
            get
            {

                return this.FileFullName.Substring(this.FileFullName.LastIndexOf('/') + 1);
            }
        }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string FileName
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FileName);
            }
            set
            {
                string str = value;
                str = str.Replace("~", "-");
                str = str.Replace("'", "-");
                str = str.Replace("*", "-");

                this.SetValByKey(FrmAttachmentDBAttr.FileName, str);

                string fileExt = str.Substring(str.LastIndexOf('.') + 1);
                //后缀名.
                this.SetValByKey(FrmAttachmentDBAttr.FileExts, fileExt);
            }
        }
        /// <summary>
        /// 附件扩展名
        /// </summary>
        public string FileExts
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FileExts);
            }
            set
            {
                string val = value.Replace(".", "");
                string[] words = { "asp", "jsp", "do", "php", "msi", "bat", "exe", "sql" };
                val = val.ToLower();
                foreach (string item in words)
                {
                    if (val.Contains(item) && val.Length == item.Length)
                        throw new Exception("err@非法的文件格式.");

                }
            }
        }
        /// <summary>
        /// 相关附件
        /// </summary>
        public string FK_FrmAttachment
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FK_FrmAttachment);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FK_FrmAttachment, value);


                if (DataType.IsNullOrEmpty(this.FrmID) == true)
                    throw new Exception("err@错误:请首先给FK_MapData赋值..");

                //获取最后"_"的位置
                string val = value.Replace(this.FrmID + "_", "");
                this.SetValByKey(FrmAttachmentDBAttr.NoOfObj, val);
            }
        }
        /// <summary>
        /// 主键值
        /// </summary>
        public string RefPKVal
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.RefPKVal);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.RefPKVal, value);
            }
        }
        /// <summary>
        /// 工作ID.
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(FrmAttachmentDBAttr.FID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FID, value);
            }
        }
        /// <summary>
        /// MyNote
        /// </summary>
        public string MyNote
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.MyNote);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.MyNote, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.Rec);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.Rec, value);
            }
        }
        /// <summary>
        /// 记录人名称
        /// </summary>
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.RecName);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.RecName, value);
            }
        }

        /// <summary>
        /// 所在部门
        /// </summary>
        public string DeptNo
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 所在部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FK_DeptName);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FK_DeptName, value);
            }
        }
        /// <summary>
        /// 附件编号
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentDBAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 文件大小
        /// </summary>
        public float FileSize
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentDBAttr.FileSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.FileSize, value / 1024);
            }
        }
        /// <summary>
        /// 是否锁定行?
        /// </summary>
        public bool ItIsRowLock
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentDBAttr.IsRowLock);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.IsRowLock, value);
            }
        }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentDBAttr.Idx);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.Idx, value);
            }
        }
        /// <summary>
        /// 附件扩展名
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentDBAttr.NodeID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentDBAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 附件类型
        /// </summary>
        public AttachmentUploadType HisAttachmentUploadType
        {
            get
            {
                if (this.MyPK.Contains("_") && this.MyPK.Length < 32)
                    return AttachmentUploadType.Single;
                else
                    return AttachmentUploadType.Multi;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 附件数据存储
        /// </summary>
        public FrmAttachmentDB()
        {
        }
        /// <summary>
        /// 附件数据存储
        /// </summary>
        /// <param name="mypk"></param>
        public FrmAttachmentDB(string mypk)
        {
            this.setMyPK(mypk);
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
                Map map = new Map("Sys_FrmAttachmentDB", "附件数据存储");

                map.IndexField = FrmAttachmentDBAttr.RefPKVal;

                map.AddMyPK();

                map.AddTBString(FrmAttachmentDBAttr.FK_MapData, null, "FK_MapData", true, false, 1, 100, 20);
                map.AddTBString(FrmAttachmentDBAttr.FK_FrmAttachment, null, "附件主键", true, false, 1, 500, 20);
                map.AddTBString(FrmAttachmentDBAttr.NoOfObj, null, "附件标识", true, false, 0, 50, 20);

                map.AddTBString(FrmAttachmentDBAttr.RefPKVal, null, "实体主键", true, false, 0, 50, 20);
                map.AddTBInt(FrmAttachmentDBAttr.FID, 0, "FID", true, false);
                map.AddTBInt(FrmAttachmentDBAttr.NodeID, 0, "节点ID", true, false);


                map.AddTBString(FrmAttachmentDBAttr.Sort, null, "类别", true, false, 0, 200, 20);
                map.AddTBString(FrmAttachmentDBAttr.FileFullName, null, "文件路径", true, false, 0, 700, 20);
                map.AddTBString(FrmAttachmentDBAttr.FileName, null, "名称", true, false, 0, 500, 20);
                map.AddTBString(FrmAttachmentDBAttr.FileExts, null, "扩展", true, false, 0, 50, 20);
                map.AddTBFloat(FrmAttachmentDBAttr.FileSize, 0, "文件大小", true, false);

                map.AddTBDateTime(FrmAttachmentDBAttr.RDT, null, "记录日期", true, false);
                map.AddTBString(FrmAttachmentDBAttr.Rec, null, "记录人", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentDBAttr.RecName, null, "记录人名字", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentDBAttr.FK_Dept, null, "所在部门", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentDBAttr.FK_DeptName, null, "所在部门名称", true, false, 0, 50, 20);
                map.AddTBStringDoc(FrmAttachmentDBAttr.MyNote, null, "备注", true, false);

                map.AddTBInt(FrmAttachmentDBAttr.IsRowLock, 0, "是否锁定行", true, false);

                //顺序.
                map.AddTBInt(FrmAttachmentDBAttr.Idx, 0, "排序", true, false);

                //这个值在上传时候产生.
                map.AddTBString(FrmAttachmentDBAttr.UploadGUID, null, "上传GUID", true, false, 0, 500, 20);

                map.AddTBAtParas(3000); //增加参数属性.

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 生成文件.
        /// </summary>
        /// <returns></returns>
        private string MakeFullFileFromOSS()
        {
            string pathOfTemp = BP.Difference.SystemConfig.PathOfTemp;
            if (System.IO.Directory.Exists(pathOfTemp) == false)
                System.IO.Directory.CreateDirectory(pathOfTemp);

            string tempFile = pathOfTemp + DBAccess.GenerGUID() + "." + this.FileExts;


            //  string tempFile =  BP.Difference.SystemConfig.PathOfTemp + + this.FileName;
            try
            {
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);
            }
            catch
            {
            }

            //调用OSSUtil执行下载（下载到临时文件）
            OSSUtil.doDownload(this.FileFullName, tempFile);

            return tempFile;
        }

        /// <summary>
        /// 生成文件.
        /// </summary>
        /// <returns></returns>
        private string MakeFullFileFromFtp()
        {
            string pathOfTemp = BP.Difference.SystemConfig.PathOfTemp;
            if (System.IO.Directory.Exists(pathOfTemp) == false)
                System.IO.Directory.CreateDirectory(pathOfTemp);

            string tempFile = pathOfTemp + DBAccess.GenerGUID() + "." + this.FileExts;


            //  string tempFile =  BP.Difference.SystemConfig.PathOfTemp + + this.FileName;
            try
            {
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);
            }
            catch
            {
            }

            FtpConnection conn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);

            conn.GetFile(this.FileFullName, tempFile, false, System.IO.FileAttributes.Archive);

            return tempFile;
        }
        public string DoDown()
        {

            return "执行成功.";
        }

        /// <summary>
        /// 重写
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }

        protected override void afterDelete()
        {
            //判断删除excel数据提取的数据
            if (DataType.IsNullOrEmpty(this.FK_FrmAttachment))
                return;

            FrmAttachment ath = new FrmAttachment(this.FK_FrmAttachment);
            try
            {
                // @于庆海需要翻译.
                if (ath.AthSaveWay == BP.Sys.AthSaveWay.IISServer)
                    System.IO.File.Delete(this.FileFullName);

                if (ath.AthSaveWay == BP.Sys.AthSaveWay.FTPServer)
                {
                    FtpConnection ftpconn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                             SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);

                    string fullName = this.FileFullName;
                    ftpconn.DeleteFile(fullName);
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex.Message);
            }

            base.afterDelete();
        }


        #endregion

        /// <summary>
        /// 获得临时文件
        /// </summary>
        /// <returns></returns>
        public string GenerTempFile(AthSaveWay saveWay = BP.Sys.AthSaveWay.IISServer)
        {
            if (saveWay == BP.Sys.AthSaveWay.IISServer)
                return this.FileFullName;

            if (saveWay == BP.Sys.AthSaveWay.FTPServer)
                return this.MakeFullFileFromFtp();

            if (saveWay == BP.Sys.AthSaveWay.OSS)
                return this.MakeFullFileFromOSS();

            if (saveWay == BP.Sys.AthSaveWay.DB)
                throw new Exception("@尚未处理存储到db里面的文件.");

            throw new Exception("@尚未处理存储到db里面的文件.");
        }

        /// <summary>
        /// 向下移動
        /// </summary>
        public void DoDownTabIdx()
        {
            this.DoOrderDown(FrmAttachmentDBAttr.RefPKVal, this.RefPKVal,
                FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachment, FrmAttachmentDBAttr.Idx);
        }
        /// <summary>
        /// 向上移動
        /// </summary>
        public void DoUpTabIdx()
        {
            this.DoOrderUp(FrmAttachmentDBAttr.RefPKVal, this.RefPKVal,
               FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachment, FrmAttachmentDBAttr.Idx);

            //  this.DoOrderUp(FrmAttachmentDBAttr.FK_MapData, this.FrmID, FrmAttachmentDBAttr.Idx);
        }
    }
    /// <summary>
    /// 附件数据存储s
    /// </summary>
    public class FrmAttachmentDBs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 附件数据存储s
        /// </summary>
        public FrmAttachmentDBs()
        {
        }
        /// <summary>
        /// 附件数据存储s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmAttachmentDBs(string fk_mapdata, string pkval)
        {
            this.Retrieve(FrmAttachmentDBAttr.FK_MapData, fk_mapdata,
                FrmAttachmentDBAttr.RefPKVal, pkval);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmAttachmentDB();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmAttachmentDB> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmAttachmentDB>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmAttachmentDB> Tolist()
        {
            System.Collections.Generic.List<FrmAttachmentDB> list = new System.Collections.Generic.List<FrmAttachmentDB>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmAttachmentDB)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
