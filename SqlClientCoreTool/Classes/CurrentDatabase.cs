using SqlClientCoreTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlClientCoreTool
{
    /// <summary>
    /// Current working database
    /// </summary>
    public class CurrentDatabase
    {
        /// <summary>
        /// Deals with some rules to prevent undesirables changes in current database. 
        /// Specifics changes in CurrentDatabaseSecurity will rollback after the accion is donne.
        /// </summary>
        [NotMapped]
        public CurrentDatabaseSecurity CurrentDatabaseSecurity { get; set; }
        /// <summary>
        /// Database name
        /// </summary>
        public string Name { get; set; } 
        /// <summary>
        /// Server name.
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// Logical mdf Name
        /// </summary>
        public string MdfName { get; set; }
        /// <summary>
        /// Logical log name
        /// </summary>
        public string LogName { get; set; }
        /// <summary>
        /// Mdf file full path
        /// </summary>
        public string FullPathMdf { get; set; }
        /// <summary>
        /// Log file full path
        /// </summary>
        public string FullPathLog { get; set; }
        /// <summary>
        /// Database size (mb)
        /// </summary>
        public decimal Size { get; set; }
        /// <summary>
        /// Database last backup
        /// </summary>
        public DateTime? LastBackup { get; set; }
        private string ConnectionString { get; set; }

        public CurrentDatabase() { CurrentDatabaseSecurity = new CurrentDatabaseSecurity(); }
        /// <summary>
        /// Get Current Database
        /// </summary>
        /// <param name="dg">Datagather</param>
        /// <returns>A CurrentDatabase object</returns>
        public static CurrentDatabase Get(DataGather dg)
        {
            CurrentDatabase currentDatabase = new CurrentDatabase();
            string query = GetQuery();
            currentDatabase = dg.Get<CurrentDatabase>(query, false).FirstOrDefault();
            currentDatabase.LastBackup = DataBaseBackup.GetLastDatabaseBackup(dg);
            currentDatabase.ConnectionString = dg.ConnectionString;
            return currentDatabase;
        }

        private static string GetQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"select @@SERVERNAME AS ServerName, DB_NAME() AS Name, df.name as MdfName, df.physical_name as FullPathMdf, df1.name as LogName, df1.physical_name as FullPathLog ");
            sb.Append($", CAST(SUM(mf.size) * 8. / 1024 AS DECIMAL(8,2)) as Size, null as LastBackup from sys.database_files df ");
            sb.Append($"join sys.database_files df1 on df.file_id <> df1.file_id and df.physical_name like '%.mdf%' and df1.physical_name like '%.ldf%' ");
            sb.Append($"left join sys.master_files mf on mf.database_id = DB_ID() ");
            sb.Append($"group by df.name, df.physical_name, df1.name, df1.physical_name ");            

            return sb.ToString();
        }

        /// <summary>
        /// Restore current database with MOVE. Change files other location desired. 
        /// You must set CurrentDatabaseSecurity.AllowRestore to true, in every restore.
        /// </summary>
        /// <param name="targetPath">Copy to path</param>
        /// <param name="timeOut">Operation timeout</param>
        /// <returns>Message if error.</returns>
        public string RestoreWithMove(string targetPath, int timeOut = 1800)
        {
            string check = CurrentDatabaseSecurity.CheckAllowRestore();
            if (!string.IsNullOrEmpty(check))
                return check;                
            else
                CurrentDatabaseSecurity.AllowRestore = false;

            return DataBaseBackup.RestoreDatabaseWithMove(ConnectionString, Name, targetPath, MdfName, FullPathMdf, LogName, FullPathLog, timeOut);
        }

        /// <summary>
        /// Backup current database to Disk WITH COMPRESSION. If opearation success property LastBackup will be updated.
        /// </summary>
        /// <param name="sourcePath">Source path of .back copy</param>
        /// <returns>Message if error.</returns>
        public string BackupWithCompression(string sourcePath)
        {
            var result = DataBaseBackup.BackUpDatabaseWithCompression(ConnectionString, Name, sourcePath);
            if (string.IsNullOrEmpty(result))
                this.LastBackup = DataBaseBackup.GetLastDatabaseBackup(ConnectionString);

            return result;
        }
    }
}
