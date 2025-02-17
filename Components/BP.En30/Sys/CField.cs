﻿using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
	/// <summary>
	/// 属性
	/// </summary>
	public class CFieldAttr
	{
		/// <summary>
		/// 属性Key
		/// </summary>
		public const string EnsName="EnsName";
		/// <summary>
		/// 工作人员
		/// </summary>
		public const string EmpNo = "EmpNo";
		/// <summary>
		/// 列选择
		/// </summary>
		public const string Attrs="Attrs";
	}
	/// <summary>
	/// 列选择
	/// </summary>
	public class CField: EntityMyPK
	{
		#region 基本属性
		/// <summary>
		/// 列选择
		/// </summary>
		public string Attrs
		{
			get
			{
				return this.GetValStringByKey(CFieldAttr.Attrs ) ; 
			}
			set
			{
				this.SetValByKey(CFieldAttr.Attrs,value) ; 
			}
		}
		/// <summary>
		/// 操作员ID
		/// </summary>
		public string EmpNo
		{
			get
			{
				return this.GetValStringByKey(CFieldAttr.EmpNo) ; 
			}
			set
			{
				this.SetValByKey(CFieldAttr.EmpNo, value) ; 
			}
		}
		/// <summary>
		/// 属性
		/// </summary>
		public string EnsName
		{
			get
			{
				return this.GetValStringByKey(CFieldAttr.EnsName ) ; 
			}
			set
			{
				this.SetValByKey(CFieldAttr.EnsName,value) ; 
			}
		}
		#endregion

		#region 构造方法
		/// <summary>
		/// 列选择
		/// </summary>
		public CField()
		{
		}
		/// <summary>
		/// map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null) 
                    return this._enMap;
                Map map = new Map("Sys_UserRegedit","列选择");

                map.AddMyPK();
                map.AddTBString(CFieldAttr.EnsName, null, "实体类名称", false, true, 0, 100, 10);
                map.AddTBString(CFieldAttr.EmpNo, null, "工作人员", false, true, 0, 100, 10);
                map.AddTBStringDoc(CFieldAttr.Attrs, null, "属性s", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

        protected override bool beforeUpdateInsertAction()
        {
			if(DataType.IsNullOrEmpty(this.MyPK)==true)
				this.setMyPK(this.EnsName + "_" + this.EmpNo);
            return base.beforeUpdateInsertAction();
        }
	
        public static Attrs GetMyAttrs(Entities ens, Map map)
        {
            string vals =  BP.Difference.SystemConfig.GetConfigXmlEns("ListAttrs", ens.ToString());
            if (vals == null)
                return map.Attrs;
            Attrs attrs=new Attrs();
            foreach (Attr attr in map.Attrs)
            {
                if ( vals.Contains(","+attr.Key+",") )
                    attrs.Add(attr);
            }
            return attrs;
        }
	}
	/// <summary>
	/// 列选择s
	/// </summary>
	public class CFields : EntitiesMyPK
	{
		/// <summary>
		/// 列选择s
		/// </summary>
		public CFields()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CField();
			}
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CField> ToJavaList()
        {
            return (System.Collections.Generic.IList<CField>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CField> Tolist()
        {
            System.Collections.Generic.List<CField> list = new System.Collections.Generic.List<CField>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CField)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
