﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.CT;

namespace BP.CT.Model
{
    /// <summary>
    /// 发送对象
    /// </summary>
    public class SendObjs : TestBase
    {
        /// <summary>
        /// 发送对象
        /// </summary>
        public SendObjs()
        {
            this.Title = "发送对象";
            this.DescIt = "流程: 023 执行发送,产看发送对象是否符合要求.";
            this.EditState = CT.EditState.Passed;
        }

        #region 全局变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workID = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        #endregion 变量

        /// <summary>
        /// 测试案例说明:
        /// 1, .
        /// 2, .
        /// 3，.
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "023";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建一个工作。
            this.workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, userNo, null);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, this.workID, null, null, 0, null);


            #region 检查发送结果是否符合预期.
            string msgText = objs.ToMsgOfText();
            if (msgText.Contains("<"))
                throw new Exception("text信息, 具有html标记.");


             
            #endregion 检查发送结果是否符合预期.

        }
    }
}
