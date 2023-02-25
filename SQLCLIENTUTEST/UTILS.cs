using SqlClientCoreTool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SqlClientCoreTool.Utils;
using System.Linq;
using Tester;

namespace SQLCLIENTUTEST
{
    public class UTILS
    {
        private string ConnectionString { get { return TestCases.ConnectionString; } }
        [Fact]
        public async Task TestGetLastDatabaseBackup()
        {
            DateTime? date = DataBaseBackup.GetLastDatabaseBackup(ConnectionString, "Copier");

            Assert.True(date == null || ((DateTime)date) < DateTime.Now);

        }
        [Fact]
        public async Task TestSessionLog()
        {
            SessionLog sessionLog = SessionLog.GetInstance(ConnectionString);
            int resultInsert = await sessionLog.InsertAsync("key", "value", Period.One_Minute);
            Assert.True(resultInsert>0);

            var resultGet = await sessionLog.GetAsync("key");
            Assert.True(resultGet.Count() > 0);
        }
       
    }
}
