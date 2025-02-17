﻿
/*
1. 该页面旨在解决各个项目所需功能显示问题，定义各个‘是否显示’变量.
2. 为了不与其他字段冲突,命名加前缀'Hide_'
3. 根据项目差异，自行定义，自行调用. 
*/
var CommonConfig = {

    ToolbarPos: 0, //0=顶部，1=底部 发送工具栏的位置.
    Hide_IsRead: true, //轨迹中是否显示已阅读: true 显示
    Hide_IsOpenFrm: true, //时间轴中是否显示查看表单:true 显示
    Hide_HastenWork: true, //在途是否显示催办按钮:true 显示
    Hide_IsTodoList: true, //待办列表中是否显示查看授权:true 显示

    UserICon: "/DataUser/Siganture/", //默认的用户签名地址
    UserIConExt: ".jpg",
    IsButtonShowSignature: false,

    ReturnWin_IsBackTracking_Selected: false,  //是否强制设置退回并原路返回?
    ReturnWin_IsKillEtcThread_Show: false, //是否显示:全部子线程退回.
    FrmDevelop_IsShowStar: true, //开发者表单解析的时候，是否显示 star .

    //是否记录用户登陆，发送日志,admin的路程设计日志.
    IsRecordUserLog: false,

    //审核组件是否显示常用短语
    IsShowWorkCheckUsefulExpres: true,
    //是否显示已审核完成的图标，已办结
    IsShowComplteCheckIcon: false,

    //附件信息配置
    IsOnlinePreviewOfAth: true, //是否在线预览
    PreviewPathOfAth: "http://101.43.55.81:8012",//附件预览服务器地址，在上传附件后预览时可配置此处的预览服务器地址
    IsHideMobileBack: false,//是否隐藏移动端返回标签
    RichTextType: 'tinymce',
    IsClearSearchCond: true,//是否清空查询条件

    Hide_TodoListSearchWay:"FlowIdx",//待办列表按照时间查询
}



