using SqlClientCoreTool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tester;
using Xunit;

namespace SQLCLIENTUTEST
{
    public class CONSTRUCTOR
    {
        private string ConnectionString { get { return TestCases.ConnectionString; } }
        [Fact]
        public void TestChangeServer()
        {
            DataGather dg = DataGather.GetInstance(ConnectionString);

            string serverName = Check.GetCurrentServer(dg);

            dg = DataGather.GetInstance(ConnectionString.Replace("local", "local1"));

            string serverName2 = Check.GetCurrentServer(dg);

            Assert.False(serverName == serverName2);

            dg = DataGather.GetInstance(ConnectionString.Replace("local1", "local"));

        }
        }
    }

