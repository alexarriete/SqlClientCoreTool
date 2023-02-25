using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlClientCoreTool.Classes;

namespace SqlClientCoreTool.Utils
{
    /// <summary>
    /// Database backup util.
    /// </summary>
    public class DataBaseBackup
    {
        /// <summary>
        /// Gets last Database backupt date.
        /// </summary>
        /// <param name="dbname">Database name</param>
        /// <param name="connectionString"></param>
        /// <returns>DateTime</returns>
        public static DateTime? GetLastDatabaseBackup(string connectionString, string dbname  = "")
        {
            DataGather dataGather = DataGather.GetInstance(connectionString, dbname);
            return GetLastDatabaseBackup(dataGather);
        }

        /// <summary>
        /// Gets last Database backupt date.
        /// </summary>
        /// <param name="dataGather">Datagather</param>
        /// <returns>Last database date. Null if never a backup has been created</returns>
        public static DateTime? GetLastDatabaseBackup(DataGather dataGather)
        {
            string query = $"SELECT TOP 1 backup_start_date FROM msdb.dbo.backupset WHERE database_name = '{dataGather.CurrentDatabaseName}' ORDER BY backup_start_date desc";
            DateTime? result = (DateTime?)dataGather.GetSingleValue(query, false, 30, null);
            return result;
        }

        /// <summary>
        /// Restore Database with MOVE RECOVERY REPLACE
        /// </summary>
        /// <param name="connectionString">Target connectionstring</param>
        /// <param name="databaseName">Database name to restore</param>
        /// <param name="sourcePath">Full path .bak</param>
        /// <param name="mdfName">Mdf Name</param>
        /// <param name="mdfFile">Full path .mdf</param>
        /// <param name="logName">Log Name</param>
        /// <param name="logFile">Full path .ldf</param>
        /// <param name="timeOut">Operation timeout</param>
        /// <returns>If error returns exception message.</returns>
        internal static string RestoreDatabaseWithMove(string connectionString, string databaseName, string sourcePath, string mdfName, string mdfFile, string logName, string logFile, int timeOut = 1800)
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append($"RESTORE DATABASE [{databaseName}] ");
            sb.Append($"FROM DISK = '{sourcePath}' ");
            sb.Append($"WITH MOVE '{mdfName}' ");
            sb.Append($"TO '{mdfFile}' ");
            sb.Append($",MOVE '{logName}' ");
            sb.Append($"TO '{logFile}' ");
            sb.Append($",RECOVERY, REPLACE, STATS = 10; ");
            

            try
            {
                DataGather dg = DataGather.GetInstance(connectionString, "master");
                var result = dg.ExecuteScriptExclusiveMode(databaseName, sb.ToString(), timeOut);
                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        /// <summary>
        /// Backup database with COMPRESSION.
        /// </summary>
        /// <param name="connectionString">Source connectionstring</param>
        /// <param name="databaseName">Database to backup</param>
        /// <param name="targetPath">Full target path /{filename}.bak</param>
        /// <param name="timeOut">Operation timeout</param>
        /// <returns>If error returns exception message.</returns>
        internal static string BackUpDatabaseWithCompression(string connectionString, string databaseName, string targetPath, int timeOut = 1800)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"BACKUP DATABASE [{databaseName}] TO  DISK = '{targetPath}' WITH FORMAT, COMPRESSION,  STATS = 10 ");
            
            try
            {
                DataGather dg = DataGather.GetInstance(connectionString, databaseName);
                var result = dg.ChangeDbValues(sb.ToString(), false, timeOut, null);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
}
