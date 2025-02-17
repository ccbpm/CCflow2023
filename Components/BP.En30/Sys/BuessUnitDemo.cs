﻿using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;

namespace BP.Sys
{
    /// <summary>
    /// 业务单元基类
    /// 1. 重写该类为业务单元子类.
    /// 2. 每个业务单元子类可以在流程事件节点时间设置.
    /// 3. 被继承的子类的必须在BP.*.DLL 里面,才能确保设置时候被映射到.
    /// 4. 子类在DoIt方法中根据WorkID 的书写业务逻辑.
    /// </summary>
    public class BuessUnitDemo : BuessUnitBase
    {
        public override string Title
        {
            get {
                return "测试BuessUnitDemo";
            }
        }
        /// <summary>
        /// 执行的方法
        /// </summary>
        public  string DoIt()
        {
            return "测试成功WorkID:" + this.WorkID;
        }
    }
}
