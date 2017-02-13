CREATE TABLE if not exists  receipts(ID INTEGER PRIMARY KEY AUTOINCREMENT,date datetime,receipt_number char(31),name char(31),content char(31),amount INTEGER,note text);

CREATE TABLE if not exists  receipts_content(ID INTEGER PRIMARY KEY AUTOINCREMENT, content char(31));

CREATE TABLE if not exists  group_name(ID INTEGER PRIMARY KEY AUTOINCREMENT, name char(31));

CREATE TABLE if not exists  building(ID INTEGER PRIMARY KEY AUTOINCREMENT, name char(31));

CREATE TABLE if not exists internal_payment(ID INTEGER PRIMARY KEY AUTOINCREMENT,date datetime,payment_number char(31),name char(31),content text,group_name char(31),advance_payment INTEGER,reimbursement INTEGER,actually_spent INTEGER,note text);

CREATE TABLE if not exists external_payment(ID INTEGER PRIMARY KEY AUTOINCREMENT,date datetime,payment_number char(31),name char(31),content text,building char(31),group_name char(31),spent INTEGER,note text);

CREATE TABLE if not exists salary(ID INTEGER PRIMARY KEY AUTOINCREMENT,month INTEGER,date datetime,payment_number char(31),name char(31),group_name char(31),content text,salary INTEGER,note text);

CREATE VIEW if not exists v_receipts
as
select content, amount, cast(strftime('%Y', date) as integer) as year, (strftime('%m', date) + 2) / 3 as qtr 
from receipts
where strftime('%Y','now') - strftime('%Y',date) between 0 and 4;

CREATE VIEW if not exists v_internal_payment
as
select group_name, actually_spent, cast(strftime('%Y', date) as integer) as year, (strftime('%m', date) + 2) / 3 as qtr 
from internal_payment
where strftime('%Y','now') - strftime('%Y',date) between 0 and 4;

CREATE VIEW if not exists v_external_payment
as
select group_name, spent, cast(strftime('%Y', date) as integer) as year, (strftime('%m', date) + 2) / 3 as qtr 
from external_payment
where strftime('%Y','now') - strftime('%Y',date) between 0 and 4;

CREATE VIEW if not exists v_salary
as
select group_name, salary, cast(strftime('%Y', date) as integer) as year, (strftime('%m', date) + 2) / 3 as qtr 
from salary
where strftime('%Y','now') - strftime('%Y',date) between 0 and 4;

CREATE INDEX IF NOT EXISTS idx_receipts_date ON receipts(date);
CREATE INDEX IF NOT EXISTS idx_receipts_content ON receipts(content);

insert into receipts(date, content, amount) values('2015-01-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2015-04-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2015-09-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2015-10-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2014-01-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2014-04-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2014-09-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2014-10-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2013-01-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2013-04-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2013-09-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2013-10-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2012-01-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2012-04-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2012-09-12','content 1', 10000000);
insert into receipts(date, content, amount) values('2012-10-12','content 1', 10000000);


insert into internal_payment(date, group_name, actually_spent) values('2016-01-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2016-04-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2016-09-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2016-10-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2015-01-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2015-04-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2015-09-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2015-10-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2014-01-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2014-04-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2014-09-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2014-10-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2013-01-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2013-04-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2013-09-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2013-10-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2012-01-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2012-04-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2012-09-12','group 2', 10000000);
insert into internal_payment(date, group_name, actually_spent) values('2012-10-12','group 2', 10000000);

insert into external_payment(date, group_name, spent) values('2016-01-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2016-04-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2016-09-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2016-10-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2015-01-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2015-04-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2015-09-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2015-10-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2014-01-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2014-04-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2014-09-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2014-10-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2013-01-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2013-04-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2013-09-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2013-10-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2012-01-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2012-04-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2012-09-12','group 2', 10000000);
insert into external_payment(date, group_name, spent) values('2012-10-12','group 2', 10000000);

insert into salary(date, group_name, salary) values('2016-01-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2016-04-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2016-09-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2016-10-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2015-01-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2015-04-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2015-09-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2015-10-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2014-01-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2014-04-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2014-09-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2014-10-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2013-01-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2013-04-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2013-09-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2013-10-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2012-01-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2012-04-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2012-09-12','group 1', 10000000);
insert into salary(date, group_name, salary) values('2012-10-12','group 1', 10000000);

insert into receipts_content(content) values('Nguồn thu khác');
insert into receipts_content(content) values('Đổ hòm công đức');

insert into group_name(name) values('Ban tri sự');
insert into group_name(name) values('Ban tri khách');
insert into group_name(name) values('Ban tri Khố');


select *, (receipt - inter_pay - exter_pay - salary) as remain from 
(select date, sum(actually_spent) as inter_pay from internal_payment where date between '2016-12-01' and '2016-12-02' group by date) as t1
NATURAL JOIN
(select date, sum(spent) as exter_pay from external_payment where date between '2016-12-01' and '2016-12-02' group by date) as t2
NATURAL JOIN
(select date, sum(salary) as salary from salary where date between '2016-12-01' and '2016-12-02' group by date) as t3
NATURAL JOIN
(select date, sum(amount) as receipt from receipts where date between '2016-12-01' and '2016-12-02' group by date) as t4