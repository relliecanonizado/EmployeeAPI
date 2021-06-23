# EmployeeAPI


**Create employee request:**

Method: POST
URL: http://[address]/api/employee/create

Sample json request:

{
  "firstName": "test",
  "middleName": "test1",
  "lastName" : "test2"
}

**Update employee request:**

Method: PUT
URL: http://[address]/api/employee/update/1

Sample json request:

{
  "firstName": "test",
  "middleName": "test1",
  "lastName" : "test2"
}

**Get employee list request:**

Method: GET
URL: http://[address]/api/employee/list

**Delete employee request:**

Method: DELETE
URL: http://<[address]/api/employee/delete?id=2
  
NOTE:

- To update connection string, go to Web.config of EmployeeApi and find the entry <connectionStrings>
- SQL Script for creating database is in SQL Script folder: Create DB and Table.sql

