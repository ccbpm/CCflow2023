﻿function JiaoNaBanFei(BeiZhu,JinE){// 创建当前表单的实例, 当前以注销学籍为例来说明该操作的方法.
   var workID=GetQueryString('WorkID'); //获得传入的实体ID.
   var frmID=GetQueryString('FrmID'); //获得传入的frmID

// 创建当前表单的实例, 当前以注销学籍为例来说明该操作的方法.
   var en =new Entity(frmID,workID); //创建实例, 当前的en就是一个该实体的json.
   if (en.StudentState==0) {
    alert('当前学籍已经是注销状态,您不能在注销了.');
    return;
   }

// 执行状态的改变.
   en.StudentState=0; //设置为注销状态.
   en.Update(); //执行更新数据，该表状态标记.

   $("#Msg").html('该学生的学籍,已经注销.');

  //该实体有如下字段. @fields 
 }