//��������: ����һ��ʵ�岢�༭.
function DemoTest1() {
    var webUser = new WebUser();
    if (webUser.No == null) {
        alert('�û�����ʧ');
        return;
    }

    //д��һ������.
    var en = new Entity("Dict_Student");
    en.Name = "����";
    en.Insert(); //���뵽���ݿ�.

    alert('�����Ѿ�д��:OID=' + en.OID + '���:' + en.BillNo);

    //���²���.
    en.Name = '��������';
    en.Update(); //���µ�����.


    //��������OIDʵ����ʵ��. 
    var en = new Entity("Dict_Student", 123);
    alert('ѧ������:' + en.Title + " ѧ�����:" + en.BillNo);

    //����ѧ�����ʵ����ʵ��. 
    var en = new Entity("Dict_Student", "0001");
    alert('ѧ������:' + en.Title + " ѧ�����:" + en.BillNo);

    //ɾ��ʵ��.
    var en = new Entity("Dict_Student", "0001");
    var i = en.Delete();
    if (i == 1)
        alert("ѧ��[" + en.Title + "]ɾ���ɹ�");
    else
        alert("ѧ��[" + en.Title + "]ɾ��ʧ��");

    //�����ʵ�岢�༭��ʽ1.
    var url = '/WF/Entity/MyDict.htm?FrmID=Student&OID=' + oid;
    window.localhost.href = url;

    //�����ʵ�岢�༭��ʽ2.
    var url = '/WF/Entity/MyDict.htm?FrmID=Student&BillNo=0001';
    window.localhost.href = url;
}


function TestFrmAPI() {

    var frmID = "Frm_ChuKuDan";

    //����һ���¼�¼. ����һ��oid ���͵�����.
    var oid = CCForm_CreateBlankOID(frmID);


    //���������
    var url = CCFrom_FrmViewUrl(frmID, oid); //���url.
    window.open(url); //�����url.


    //ɾ��ʵ���¼.
    CCFrom_DeleteFrmEntityByOID(frmID, oid);

}