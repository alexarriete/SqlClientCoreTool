

using SqlClientCoreTool;
using Tester;

//A few test cases here.
// CRUD operation could be found in SQLCLIENTUTEST project.
// Script.sql must be previously executed in Sql management studio or any sql base editor *************************

bool resultchangeDatabase = TestCases.TestChangeDatabase();

bool resultBackupDatabase = TestCases.TestBackupDatabase(@"c:\backups\copier.bak");

string serverName = TestCases.TestGetServerName();
