using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SqlClientCoreTool.Classes;

namespace SqlClientCoreTool
{
    /// <summary>
    /// Groups several methods to check database, tables, connection, etc.
    /// </summary>
    public class Check
    {
        /// <summary>
        /// Checks connectionstring
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cancellationToken"></param>     
        /// <returns>Database name if exists</returns>
        public static async Task<string> CheckConnectionAsync(string connectionString, CancellationToken cancellationToken)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync(cancellationToken);                    
                    return conn.Database;
                }
            }
            catch (System.Exception)
            {                
                return "";
            }
        }
        /// <summary>
        /// Checks connectionstring.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static async Task<string> CheckConnectionAsync(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    return conn.Database;
                }
            }
            catch (System.Exception)
            {
                return "";
            }
        }

        ///// <summary>
        ///// Check connectionstring in the configuration file.
        ///// </summary>
        ///// <param name="connectionString">A Connectionstring obtained from config, setting, etc.</param>
        ///// <param name="databaseName">Optional: database name</param>
        ///// <returns>Database name if exists</returns>
        //public async static Task<string> CheckConnectionAsync(string connectionString, CancellationToken cancellationToken, string databaseName = "")
        //{
        //    return await Task.Run(() => CheckConnection(connectionString, cancellationToken, databaseName));
        //}


        /// <summary>
        /// Gets the current connected database.
        /// </summary>
        /// <param name="dg"></param>
        /// <returns>Current Database name</returns>
        public static string GetCurrentDataBase(DataGather dg)
        {
            return dg.CurrentDatabaseName;
        }

        /// <summary>
        /// Gets the current connected server.
        /// </summary>
        /// <param name="dg"></param>
        /// <returns>Current server name</returns>
        public static string GetCurrentServer(DataGather dg)
        {
            return dg.CurrentServerName;
        }


        /// <summary>
        /// Check id there is a connection to a linked servers.
        /// </summary>
        /// <param name="connectionString">A Connectionstring obtained from config, setting, etc.</param>
        /// <param name="serverName"></param>
        /// <returns>Linked server name if exists</returns>
        public static string CheckLinkedServers(string connectionString, string serverName)
        {
            List<Parameter> sqlParams = new List<Parameter>();
            sqlParams.Add(new Parameter() { Name = "@serverName", Value = serverName });
            DataGather dg = DataGather.GetInstance(connectionString);

            var result = dg.GetSingleValue("sp_testlinkedserver", true, 15, sqlParams);

            if (result != null)
            {
                if (result.ToString().ToLower().Contains("timeout"))
                {
                    throw new Exception($"Error: Timeout while trying to connect to the server { serverName}");
                }
                else if (result.ToString().ToLower().Contains("error"))
                {
                    throw new Exception(result.ToString());
                }
            }
            return serverName;
        }


        /// <summary>
        /// Check if table exist.
        /// </summary>
        /// <param name="connectionString">A Connectionstring obtained from config, setting, etc.</param>
        /// <param name="tableName"></param>
        /// <returns>Table name</returns>
        public static string CheckTableExist(string connectionString, string tableName)
        {
            DataGather dg = DataGather.GetInstance(connectionString);
            string result = CheckTableExist(dg, tableName);
            if(!string.IsNullOrEmpty(result))
            {
                return "";
            }
            return tableName;
        }

        private static string CheckTableExist(DataGather dg, string tableName)
        {
            int number = 0;
            var result = dg.GetSingleValue($"SELECT count(*) FROM sys.tables WHERE name LIKE '{tableName}'", false);
            if (result == null || (Int32.TryParse(result.ToString(), out number) && number == 0))
            {
                return $"Error: The table {tableName} was not found in current Database";
            }
            return "";
        }

        internal static string FindPosibleTableName(DataGather dg, string objectName)
        {            
            string result = Check.CheckTableExist(dg, objectName);
            if (!string.IsNullOrEmpty(result))
            {
                result = Check.CheckTableExist(dg, objectName + "s");
                if (!string.IsNullOrEmpty(result))
                {
                    result = Check.CheckTableExist(dg, objectName + "es");
                    if (string.IsNullOrEmpty(result))
                    {
                        return objectName + "es";
                    }
                    return objectName;
                }
                return objectName + "s";
            }
            return objectName;
        }

        internal static string GetColumnPrimaryKey(DataGather dg, string TableName)
        {
            string query = $"SELECT TOP 1 COLUMN_NAME FROM {dg.CurrentDatabaseName}.INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME LIKE '{TableName}' AND CONSTRAINT_NAME LIKE 'PK%'";
            var result = dg.GetSingleValue(query, false);
            return result != null ? result.ToString() : "";
        }

        internal static string GetColumnIdentity(DataGather dg, string TableName)
        {
            
            string query = $"SELECT top 1 name FROM [{dg.CurrentDatabaseName}].sys.columns WHERE  is_identity = 1 and object_id = (select object_id from [{dg.CurrentDatabaseName}].sys.objects where name like '{TableName}')";
            var result = dg.GetSingleValue(query, false);
            return result != null ? result.ToString() : "";
        }

        internal static string GetTableName<T>(SqlConnectionStringBuilder builder)
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            DataGather dg = DataGather.GetInstance(builder.ConnectionString);
            string objectName = Check.FindPosibleTableName(dg, obj.GetType().Name);
            return objectName;

        }
    }
}
