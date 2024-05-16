using SqlClientCoreTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    /// <summary>
    /// For testing purpose need to create two databases SQLCCTool and Copier
    /// </summary>
    public class TestCases
    {
        /// <summary>
        /// Connection string to SQLCCTool database with owner permissions.
        /// </summary>
        public static string ConnectionString { get { return "Data Source=ACERNUDA\\SQLEXPRESS;Initial Catalog=SQLCCTool ;Integrated Security=true"; } }

        /// <summary>
        /// For testing purpose need to create two databases SQLCCTool and Copier
        /// this method test database change on context
        /// </summary>
        /// <returns></returns>
        public static bool TestChangeDatabase()
        {
            try
            {
                DataGather dataGather = DataGather.GetInstance(ConnectionString, "Copier");
                CurrentDatabase currentDatabase = CurrentDatabase.Get(dataGather);
                return currentDatabase.Name == "Copier";
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a backup of database Copier.
        /// </summary>
        /// <param name="path">backup path. Root path must exist</param>
        /// <returns></returns>
        public static bool TestBackupDatabase(string path)
        {
            try
            {                
                DataGather dataGather = DataGather.GetInstance(ConnectionString, "Copier");
                CurrentDatabase currentDatabase = CurrentDatabase.Get(dataGather);
                currentDatabase.BackupWithCompression(path);                
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }


        public static string TestGetServerName()
        {
            try
            {
                DataGather dataGather = DataGather.GetInstance(ConnectionString, "Copier");
                return Check.GetCurrentServer(dataGather);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
