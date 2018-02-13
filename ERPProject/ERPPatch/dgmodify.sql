
--ERP

--doc_supplier
  --�������Ƿ���� doc_supplier.isdg char(1) 'N'
  alter table doc_supplier add (isdg char(1));
  alter table doc_supplier modify isdg default('N');
  comment on column DOC_SUPPLIER.isdg
  is '�Ƿ����';
  update doc_supplier set isdg = 'N' where isdg is null;

  alter table dat_xs_doc add(supid varchar2(20));
   comment on column dat_xs_doc.supid
	  is '��Ӧ��id';
   update dat_xs_doc set supid='00002' where supid is null;
  commit;
--���Ӳ˵�

  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9', '���ܹ���', '00', 'BS', null, 'FUN', 'Y', null, 'N', '#', null, 'Y', 9, 1, null, null);
--���ܻ������Ϲ����˵�
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('91', '���ܻ������Ϲ���','9', 'BS', null, 'FUN', 'Y', null, 'N', '#', null, 'Y', 91, 2, null, null);
     
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9101', '���ܹ�Ӧ�̹���', '91', 'BS', null, 'FUN', 'Y', null, 'S', '/ERPEntrust/SupplierEntrust.aspx', null, 'Y', 9101, 3, null, null);
--���ܿⷿ�����˵�
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('92', '���ܿⷿ����','9', 'BS', null, 'FUN', 'Y', null, 'N', '#', null, 'Y', 92, 2, null, null);

  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9201', '���ܲ�������','92', 'BS', null, 'FUN', 'Y', null, 'S', '/ERPStorage/GoodsOrder.aspx?oper=input&dg=1', null, 'Y', 9201, 3, null, null);

  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9202', '���ܲ������','92', 'BS', null, 'FUN', 'Y', null, 'S', '/ERPStorage/GoodsOrder.aspx?oper=audit&dg=1', null, 'Y', 9202, 3, null, null);

  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9203', '����������','92', 'BS', null, 'FUN', 'Y', null, 'S', '/ERPStorage/GoodsStorage.aspx?oper=input&dg=1', null, 'Y', 9203, 3, null, null);
  
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9204', '����������','92', 'BS', null, 'FUN', 'Y', null, 'S', '/ERPStorage/GoodsStorage.aspx?oper=audit&dg=1', null, 'Y', 9204, 3, null, null);

  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9205', '�����˻�����','92', 'BS', null, 'FUN', 'Y', null, 'S', '/ERPStorage/SalesReturn.aspx?oper=input&dg=1', null, 'Y', 9205, 3, null, null);
  
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9206', '�����˻����','92', 'BS', null, 'FUN', 'Y', null, 'S', '/ERPStorage/SalesReturn.aspx?oper=audit&dg=1', null, 'Y', 9206, 3, null, null);
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9207', '�����������','92', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9207, 3, null, null);
--���ܿ��ҹ���(ʹ��)�˵�
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('93', '����ʹ�ù���','9', 'BS', null, 'FUN', 'Y', null, 'N', '#', null, 'Y', 93, 2, null, null);
  
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9301', '����ʹ������','93', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9301, 3, null, null);

  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9302', '����ʹ��ȷ��','93', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9302, 3, null, null);

  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9303', '����ʹ���˻�����','93', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9303, 3, null, null);
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9304', '����ʹ���˻�����','93', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9304, 3, null, null);
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9305', '����ʹ���˻����','93', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9305, 3, null, null);
--���ܱ����˵�
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('94', '���ܱ�������','9', 'BS', null, 'FUN', 'Y', null, 'N', '#', null, 'Y', 94, 2, null, null);
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9401', '������Ʒ��������ϸ','94', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9401, 2, null, null);
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9402', '������Ʒ������ϸ','94', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9402, 2, null, null); 
  insert into SYS_FUNCTION
    (funcid,funcname, treeid, system, funico, funtype, isvisible, micohelp, runhow, runwhat, runpara, isfuncarea, itemsort, treelevel, ipadname, method)
  values
    ('9403', '���ܿ���ѯ','94', 'BS', null, 'FUN', 'Y', null, 'S', '#', null, 'Y', 9403, 2, null, null); 
  commit;


--update sys_report
update sys_report set ( type, name, flag, colkey, colsort, colsum, colnum, condnum, pagesize, str1, str2, str3, selectsql)=
 (select  'DDLLIST', 'DDL_DOC_SUPPLIER', 'Y', null, null, null, 0, 0, 0, null, null, '0', 'select  CODE,NAME from (' || chr(10) || 'SELECT ''--��ѡ��--'' NAME,'''' CODE   FROM dual' || chr(10) || 'union all' || chr(10) || 'SELECT ''[''||SUPID||'']''||SUPNAME NAME,SUPID CODE FROM DOC_SUPPLIER where isdg = ''N'' ' || chr(10) || ')' || chr(10) || 'ORDER BY DECODE(CODE,'''',99,0) DESC ,CODE ASC' from dual)
where seqno = 'DDL_DOC_SUPPLIERNULL';


insert into sys_report  ( seqno,type, name, flag, colkey, colsort, colsum, colnum, condnum, pagesize, str1, str2, str3, selectsql)
values ( 'DDL_DOC_SUPPLIER_DG' ,'DDLLIST', 'DDL_DOC_SUPPLIER_DG', 'Y', null, null, null, 0, 0, 0, null, null, '0', 'select  CODE,NAME from (' || chr(10) || 'SELECT ''--��ѡ��--'' NAME,'''' CODE   FROM dual' || chr(10) || 'union all' || chr(10) || 'SELECT ''[''||SUPID||'']''||SUPNAME NAME,SUPID CODE FROM DOC_SUPPLIER where isdg = ''Y'' ' || chr(10) || ')' || chr(10) || 'ORDER BY DECODE(CODE,'''',99,0) DESC ,CODE ASC' );
commit;
