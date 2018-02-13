
--数据中心

--doc_supplier
  --增加列是否代购 doc_supplier.isdg char(1) 'N'
  alter table doc_supplier add (isdg char(1));
  alter table doc_supplier modify isdg default('N');
  comment on column DOC_SUPPLIER.isdg
  is '是否代购';
  update doc_supplier set isdg = 'N' where isdg is null;
--接口定义
  insert into INF_TABLE_DEF (infkey, infvalue, flag, str1, str2, str3, str4, num1, num2, num3, num4, memo)
values ('DOC_SUPPLIER_DG', 'select supid, supname, supename, supsimname, supsimid, flag, subject, supcat, regid, corpkid, corptype, yyzzno, loginrq, loginlabel, loginfund, jygm, jyfw, taxpayer,taxrate,' || chr(10) || '       taxno, bank,accntno,loginaddr, leader, leaderidcard, tel, fax, telservice, zip, email,url,isgathering, gatfundcorp,gatfundbank, gataccntno, zzaddr,linkman,    linkmanduty,linktel, linkfax, linkemail, cwlinkman, cwlinkduty, cwlinktel,  cwlinkfax,cwlinkemail,buyerid,applydept, manager,crerq,zzrq,delrq,memo,str1,str2, str3,num1,     num2, num3,ISSUPPLIER,' || chr(10) || 'ISPRODUCER,' || chr(10) || 'ISKEHU,' || chr(10) || 'FLAG1,' || chr(10) || 'FLAG2,' || chr(10) || 'FLAG3,' || chr(10) || 'isdg' || chr(10) || ' from doc_supplier where upttime >to_date(''{2}'',''yyyy/mm/dd hh24:mi:ss'')  and flag = ''Y'' AND ISDG = ''Y''' || chr(10) || ' and (supid like ''%{3}%'' or supname like ''%{3}%''  or supsimid like ''%{3}%'')', 'Y', null, null, null, null, null, null, null, null, null);
commit;