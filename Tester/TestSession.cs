using SqlClientCoreTool.Classes;
using SqlClientCoreTool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    internal class TestSession
    {
        private static string ConnectionString { get { return "Data Source=(local);Initial Catalog=SQLCCTool ;Integrated Security=true"; } }
        
        //key = "bookedRoom", value = "JohnDoeId"
        private async Task<int> InsertSessionLogAsync(string key, string value)
        {
            SessionLog sessionLog = SessionLog.GetInstance(ConnectionString);
            // The sessionLog will be saved in a table named SCCT_SessionLog.
            // If table doesn't exists we will attempt to create.
            return await sessionLog.InsertAsync(key, value, Period.One_Week);
        }


        //key = "bookedRoom"
        private async Task<IEnumerable<SCCT_SessionLog>> GetSessionLogAsync(string key)
        {
            SessionLog sessionLog = SessionLog.GetInstance(ConnectionString);

            // Every get an insert delete rows where expriationDate < now

            return await sessionLog.GetAsync(key);
        }
    }
}
