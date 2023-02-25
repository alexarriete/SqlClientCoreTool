using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SqlClientCoreTool.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Collections;
using Parameter = SqlClientCoreTool.Classes.Parameter;

namespace SqlClientCoreTool
{
    /// <summary>
    /// Creates a connection to the database and contains methods for CRUD operations
    /// Examples: https://acernuda.com/sqlclientcoretool
    /// </summary>
    public sealed class DataGather
    {

        #region -- PROPERTIES, CONSTRUCTORS...

        internal string ConnectionString { get; set; }
        internal string CurrentDatabaseName { get; set; }
        internal string CurrentServerName { get; set; }
     
        internal static SqlConnectionStringBuilder Builder { get; set; }


        private static Lazy<DataGather> instance = new Lazy<DataGather>(() => new DataGather());


        /// <summary>
        /// Initializes a new connection to the database.
        /// </summary>
        /// <param name="connectionString">A Connectionstring obtained from config, setting, etc.</param>
        /// <param name="dataBaseName">Optional, dataBase name different to connectionstring DataBase.</param>
        /// <returns>Returns an instance of DataGather</returns>
        public static DataGather GetInstance(string connectionString, string dataBaseName = "")
        {
            bool haveOldInstanceWithDifferentConnection = Builder != null && (Builder.ConnectionString != connectionString || instance.Value.ConnectionString != connectionString);
            Builder = new SqlConnectionStringBuilder(connectionString);
            if (!string.IsNullOrWhiteSpace(dataBaseName) || haveOldInstanceWithDifferentConnection)
            {
                Builder.InitialCatalog = !string.IsNullOrWhiteSpace(dataBaseName) ? dataBaseName: Builder.InitialCatalog;
                return new DataGather();
            }           
            
            return instance.Value;
        }


        /// <summary>
        /// Creates a new connection to the database. If the username and password are not specified, it will try to connect through trusted Windows authentication.
        /// </summary>
        /// <param name="dataSource">Server instance name</param>
        /// <param name="dataBaseName">Inicial catalog name</param>
        /// <param name="userName">Database login uername</param>
        /// <param name="password">Database login password</param>
        /// <returns></returns>
        public static DataGather GetBuilderInstance(string dataSource, string dataBaseName, string userName = "", string password = "")
        {
            Builder = new SqlConnectionStringBuilder();
            Builder["Connect Timeout"] = 1000;
            Builder.DataSource = dataSource;
            Builder.InitialCatalog = dataBaseName;

            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(userName))
            {
                Builder.UserID = userName;
                Builder.Password = password;

            }
            else if (!string.IsNullOrWhiteSpace(password) || !string.IsNullOrWhiteSpace(userName))
            {
                throw new Exception("You must specify username and password or both empty fields");
            }
            else
            {
                Builder["Trusted_Connection"] = true;
            }
            return instance.Value;
        }

        internal static DataGather GetInstance(SqlConnectionStringBuilder builder)
        {
            Builder = builder;
            return instance.Value;
        }

        internal static SqlConnectionStringBuilder CheckInstance()
        {
            return Builder;
        }
        private DataGather()
        {
            try
            {
                ConnectionString = Builder.ConnectionString;
                CurrentDatabaseName = Builder.InitialCatalog;
                CurrentServerName = Builder.DataSource;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private SqlCommand GetCommand(string query, List<Parameter> parameters, SqlConnection sqlConnection)
        {
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
          
            if (parameters != null)
            {
                foreach (Parameter param in parameters)
                {
                    sqlCommand.Parameters.AddWithValue(param.Name, param.Value??DBNull.Value);
                }
            }

            return sqlCommand;
        }
        

        #endregion

        #region --GET METHODS--       
        /// <summary>
        /// Returns a DataTable object resulting from the query to the database.
        /// </summary>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <param name="parameters">Parameters list</param>
        /// <returns>System.Data.DataTable</returns>
        public DataTable GetDataTable(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            string message = "";
            using(SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {                
                parameters = Parameter.CheckAtForProcedure(parameters, isProcedure);                
                try
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = GetCommand(query, parameters, sqlConnection);
                    sqlCommand.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    sqlCommand.CommandTimeout = timeout;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    DataSet ds = new DataSet();
                    sqlDataAdapter.Fill(ds);
                    return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            
            CheckForException(message);
            return null;
        }


        /// <summary>
        /// Returns a DataTable object resulting from the query to the database.
        /// </summary>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <param name="parameters">Parameters list</param>
        /// <returns>System.Data.DataTable</returns>
        public async Task<DataTable> GetDataTableAsync(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            return await Task.Run(() => GetDataTable(query, isProcedure, timeout, parameters));
        }


        /// <summary>
        /// Gets Table of T 
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="limit">Number of rows. Empty or (0) for all rows</param>
        /// <param name="timeout">Remote query timeout </param>
        /// <returns>System.Data.DataTable</returns>
        public DataTable GetDataTable<T>(int limit = 0, int timeout = 30)
        {
            string tableName = Check.GetTableName<T>(Builder);
            string top = limit > 0 ? $" TOP {limit}" : "";
            string query = $"SELECT {top} * FROM [{tableName}]";
            return GetDataTable(query, false, timeout);
        }


        /// <summary>
        /// Gets Table of T 
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="limit">Number of rows. Empty or (0) for all rows</param>
        /// <param name="timeout">Remote query timeout </param>
        /// <returns>System.Data.DataTable</returns>
        public async Task<DataTable> GetDataTableAsync<T>(int limit = 0, int timeout = 30)
        {
            return await Task.Run(() => GetDataTable<T>(limit, timeout));
        }

        private static void CheckForException(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                throw new Exception($"Error: {message}");
            }
        }



        /// <summary>
        /// Gets a IEnumerable with the values ​​of the table generated by the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <param name="parameters">Parameters list</param>
        /// <returns><typeparamref name="T"/> IEnumerable</returns>
        public ICollection<T> Get<T>(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            DataTable dt = GetDataTable(query, isProcedure, timeout, parameters);
            return Transformer.GetListFromDataTable<T>(dt);
        }


        /// <summary>
        /// Gets an IEnumerable with the values ​​of the table generated by the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <param name="parameters">Parameters list</param>
        /// <returns><typeparamref name="T"/> IEnumerable</returns>
        public async Task<ICollection<T>> GetAsync<T>(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            return await Task.Run(() => Get<T>(query, isProcedure, timeout, parameters));
        }


        /// <summary>   
        /// <typeparam name="T"> Gets a subset of T</typeparam>
        /// </summary>
        /// <param name="numberOfRows">Number of rows</param>
        /// <param name="startRow">First row</param>
        /// <param name="field">you can sort by one field or by several separated by commas</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <returns>Returns an ICollection of T </returns>
        public ICollection<T> GetSubset<T>(int numberOfRows, int startRow = 1, string field = "Id", int timeout = 30)
        {
            string query = $"SELECT * FROM [{Check.GetTableName<T>(Builder)}] ORDER BY {field} OFFSET {startRow} ROWS FETCH NEXT {numberOfRows} ROWS ONLY";
            DataTable dt = GetDataTable(query, false, timeout, null);
            return Transformer.GetListFromDataTable<T>(dt);
        }


        /// <summary>
        /// <typeparam name="T"> Gets a subset of T</typeparam>
        /// </summary>
        /// <param name="numberOfRows">Number of rows</param>
        /// <param name="startRow">First row</param>
        /// <param name="field">you can sort by one field or by several separated by commas</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <returns><typeparamref name="T"/>IEnumerable</returns>
        /// 


        public async Task<IEnumerable<T>> GetSubsetAsync<T>(int numberOfRows, int startRow = 1, string field = "Id", int timeout = 30)
        {
            return await Task.Run(() => GetSubset<T>(numberOfRows, startRow, field, timeout));
        }


        /// <summary>
        /// Gets all or limited records from a table named equal to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">Number of rows. Empty or (0) for all rows</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <returns><typeparamref name="T"/> IEnumerable</returns>
        public IEnumerable<T> Get<T>(int limit = 0, int timeout = 60)
        {
            string objectName = Check.GetTableName<T>(Builder);
            string top = limit > 0 ? $" TOP {limit}" : "";
            string query = $"SELECT {top} * FROM [{objectName}]";


            DataTable dt = GetDataTable(query, false, timeout, null);
            return Transformer.GetListFromDataTable<T>(dt);
        }


        /// <summary>
        /// Gets all or limited records from a table named equal to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">Number of rows. Empty or (0) for all rows</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <returns><typeparamref name="T"/> IEnumerable</returns>
        public async Task<IEnumerable<T>> GetAsync<T>(int limit = 0, int timeout = 60)
        {
            return await Task.Run(() => Get<T>(limit, timeout));
        }


        /// <summary>
        /// Gets a single value from a cell in a table.
        /// </summary>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <param name="parameters">Parameters list</param>
        /// <returns>Returns a single value, of type object, the product of a query to the database.</returns>
        public object GetSingleValue(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            string message = "";
            using(SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                sqlConnection.Open();
                parameters = Parameter.CheckAtForProcedure(parameters, isProcedure);
                SqlCommand sqlCommand = GetCommand(query, parameters, sqlConnection);
                sqlCommand.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                sqlCommand.CommandTimeout = timeout;
                try
                {
                    return sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }            
            CheckForException(message);
            return null;
        }

        /// <summary>
        /// Gets a single value from a cell in a table.
        /// </summary>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>
        /// <param name="timeout">Remote query timeout</param>
        /// <param name="parameters">Parameters list</param>
        /// <returns>Returns a single value, of type object, the product of a query to the database.</returns>
        public async Task<object> GetSingleValueAsync(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            return await Task.Run(() => GetSingleValue(query, isProcedure, timeout, parameters));
        }

        #endregion

        #region -- INSERT, UPDATE, DELETE--

        #region -- CRUD GENERAL METHODS


        /// <summary>
        /// Update, delete, insert operations
        /// </summary>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>        
        /// <param name="timeout">Remote query timeout</param>
        /// <param name="parameters"></param>
        /// <returns>Returns number of rows</returns>
        public int ChangeDbValues(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            string message = "";
            int result = 0;
            using(SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                
                try
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = GetCommand(query, parameters, sqlConnection);
                    parameters = Parameter.CheckAtForProcedure(parameters, isProcedure);
                    sqlCommand.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    sqlCommand.CommandTimeout = timeout;
                    string queryResult = sqlCommand.ExecuteNonQuery().ToString();
                    result = GetIntResult(queryResult);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
           
            CheckForException(message);
            return result;
        }

        internal int InsertDbValues(string tableName, List<List<Classes.Parameter>> parameters, bool returnIdentityInsert)
        {
            string message = "";
            int result = 0;
            string returnIdentityQuery = returnIdentityInsert ? " ;SELECT SCOPE_IDENTITY()" : "";
            using (SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                sqlConnection.Open();
                string parametersNameList = string.Join(",", parameters.FirstOrDefault().Select(x =>$"[{x.Name}]" ));
                foreach (List<Parameter> parameterList in parameters)
                {
                    List<Parameter> auxParameterList = Parameter.CheckAtForProcedure(parameterList, true);
                    string query = $"INSERT INTO [{tableName}] ({parametersNameList}) VALUES({string.Join(",", auxParameterList.Select(x => x.Name))}) {returnIdentityQuery}";
                    SqlCommand sqlCommand = GetCommand(query, parameterList, sqlConnection);
                    TryExecuteCommand(ref message, ref result, sqlConnection, sqlCommand, returnIdentityInsert);
                }

                CheckForException(message);
            }
            return result;
        }

        private static void TryExecuteCommand(ref string message, ref int result, SqlConnection sqlConnection, SqlCommand sqlCommand, bool returnIdentityInsert = false)
        {
            try
            {
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = new SqlConnection(sqlConnection.ConnectionString);               
                sqlCommand.Connection.Open();

                string queryResult = returnIdentityInsert ? sqlCommand.ExecuteScalar().ToString(): sqlCommand.ExecuteNonQuery().ToString();
                result = GetIntResult(queryResult);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                sqlCommand.Connection.Close();
                sqlConnection.Close();
            }
        }

        internal int UpdateDbValues(string tableName, List<Classes.Parameter> parameters, Parameter primaryKeyParameter)
        {
            string message = "";
            int result = 0;
            using(SqlConnection sqlConnection = new SqlConnection(this.ConnectionString))
            {
                sqlConnection.Open();
                //string parametersNameList = string.Join(",", parameters.Select(x => x.Name));

                List<string> sets = new List<string>();
                foreach (Parameter parameter in parameters)
                {
                    sets.Add($"[{parameter.Name}] = @{parameter.Name}");
                }
                string setClause = string.Join(",", sets);
                string query = $"UPDATE [{tableName}] SET {setClause} WHERE {primaryKeyParameter.Name} = {primaryKeyParameter.Value}";
                SqlCommand sqlCommand = GetCommand(query, parameters, sqlConnection);
                TryExecuteCommand(ref message, ref result, sqlConnection, sqlCommand);
               
            }
            
            CheckForException(message);

            return result;
        }


        /// <summary>
        /// Update, delete, insert operations
        /// </summary>
        /// <param name="query">T-SQL query</param>
        /// <param name="isProcedure">Specifies whether the query is a stored procedure or not</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// /// <param name="timeout">Operation tiemout</param>
        /// <returns>Returns number of rows affected</returns>
        public async Task<int> ChangeDbValuesAsync(string query, bool isProcedure, int timeout = 30, List<Parameter> parameters = null)
        {
            return await Task.Run(() => ChangeDbValues(query, isProcedure, timeout, parameters));
        }


        #endregion

        #region -- UDATE

        /// <summary>
        /// Updates a record in the database. The table must have a primary key.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Returns the number of rows updated.</returns>
        public int Update(object obj, string tableName = "")
        {
            int result = 0;
            tableName = string.IsNullOrWhiteSpace(tableName) ? Check.FindPosibleTableName(this, obj.GetType().Name): tableName;
            string primaryKey = Check.GetColumnPrimaryKey(this, tableName);
            if (!string.IsNullOrEmpty(primaryKey))
            {
                if (obj.GetType().GetProperties().Select(s => s.Name.ToLower()).Any(n => n == primaryKey.ToLower()))
                {                  

                    string identityColumn = Check.GetColumnIdentity(this, tableName);
                    string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'"
                          + $" AND COLUMN_NAME not like '{identityColumn}' ORDER BY ORDINAL_POSITION";

                    IEnumerable<string> columns = Get<string>(query, false);

                    List<PropertyInfo> propertyObj = obj.GetType().GetProperties()
                                                      .Where(x => !x.GetCustomAttributes(typeof(NotMappedAttribute)).Any()).ToList();

                    Parameter primaryKeyParameter = new Parameter() { Name = primaryKey, Value = GetValue(obj.GetType().GetProperties().FirstOrDefault(s => s.Name.ToLower() == primaryKey.ToLower()), obj) };

                    result = UpdateDbValues(tableName, CreateValueModule(obj, propertyObj, columns), primaryKeyParameter);
                }
                else
                {
                    throw new Exception($"Error: The object of type {obj.GetType().Name} does not have a matching field with the primary key {primaryKey} in table {tableName}.");
                }
            }
            else
            {
                throw new Exception($"Error: Table {tableName} does not have a primary key.");
            }
            return result;
        }


        /// <summary>
        /// Updates a record in the database. The table must have a primary key.
        /// </summary>
        /// <param name="obj">Object that contains fields to update. Its type has to match the name of the table to update</param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Returns the number of rows updated</returns>
        public async Task<int> UpdateAsync(object obj, string tableName = "")
        {
            return await Task.Run(() => Update(obj, tableName));
        }


        /// <summary>
        /// Updates all the record in the database that matchs filters name value pairs.
        /// </summary>
        /// <param name="obj">Object that contains fields to update. Its type has to match the name of the table to update</param>
        /// <param name="filter">Name value pair of fields to filter (where clause)</param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Number of files updated</returns>
        public int UpdateMany(object obj, List<Parameter> filter, string tableName)
        {
            int result = 0;
            tableName = string.IsNullOrWhiteSpace(tableName) ? Check.FindPosibleTableName(this, obj.GetType().Name): tableName;
            string primaryKey = Check.GetColumnPrimaryKey(this, tableName);
            if (filter.Any())
            {
                string query = $"UPDATE {tableName} {CreateUpdateSet(obj, primaryKey)} WHERE {CreateWhereClause(filter)} ";
                result = ChangeDbValues(query, false, 30, null);
            }
            else
            {
                throw new Exception($"Error: filter can't be empty.");
            }
            return result;
        }


        /// <summary>
        /// Updates all the record in the database that matchs filters name value pairs.
        /// </summary>
        /// <param name="obj">Object that contains fields to update. Its type has to match the name of the table to update</param>
        /// <param name="filter">Name value pair of fields to filter (where clause)</param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Number of files updated</returns>
        public async Task<int> UpdateManyAsync(object obj, List<Parameter> filter, string tableName = "")
        {
            return await Task.Run(() => UpdateMany(obj, filter, tableName));
        }


        private string CreateWhereClause(List<Parameter> filters)
        {
            string result = "";
            foreach (Parameter param in filters)
            {
                result = result == "" ? "" : " and " + $"{param.Name} = {param.Value} ";
            }
            return result;
        }


        private string CreateUpdateSet(object obj, string primaryKey)
        {
            List<string> sets = new List<string>();
            foreach (PropertyInfo ob in obj.GetType().GetProperties().Where(x => !x.GetCustomAttributes(typeof(NotMappedAttribute)).Any() && x.Name.ToLower() != primaryKey.ToLower()))
            {                
                sets.Add($"{ob.Name} = {GetValue(ob, obj)}");
            }
            if (sets.Any())
            {
                return $" SET {string.Join(", ", sets)}";
            }
            return "";
        }

        private object GetValue(PropertyInfo prop, object mainObject)
        {
            object valueObject = prop.GetValue(mainObject);
            if (valueObject != null)
            {
                return valueObject;             
            }
            return null;

        }


        #endregion

        #region -- INSERT


        /// <summary>
        /// Inserts an object into the Table named as object type.
        /// </summary>
        /// <param name="obj">Its type has to match with the name of the table in the database</param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns> if Table identity returns identity value else the number of inserted rows</returns>
        public int Insert(object obj, string tableName = "")
        {
            List<object> objs = new List<object>() { obj };
            return InsertList(objs, tableName);
        }


        /// <summary>
        /// Inserts an object into the Table named as object type.
        /// </summary>
        /// <param name="obj">Its type has to match the name of the table in the database</param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns> if Table identity returns identity value else the number of inserted rows</returns>
        public async Task<int> InsertAsync(object obj, string tableName = "")
        {
            return await Task.Run(() => Insert(obj, tableName));
        }


        /// <summary>
        /// Inserts a list of object into the Table named as object type.
        /// </summary>
        /// <param name="items">items to insert. Its type has to match with the name of the table in the database</param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns> If Table identity and items list has only one object returns identity value else the number of inserted rows</returns>
        public int InsertList(object items, string tableName = "")
        {
            var collection = ((IEnumerable)items).Cast<object>().ToList();

            tableName = string.IsNullOrWhiteSpace(tableName) ? Check.FindPosibleTableName(this, collection.FirstOrDefault().GetType().Name) : tableName;
            string identityColumn = Check.GetColumnIdentity(this, tableName);

            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'"
                           + $" AND COLUMN_NAME not like '{identityColumn}' ORDER BY ORDINAL_POSITION";

            IEnumerable<string> columns = Get<string>(query, false);

            List<PropertyInfo> propertyObj = collection.FirstOrDefault().GetType().GetProperties()
                                              .Where(x => !x.GetCustomAttributes(typeof(NotMappedAttribute)).Any()).ToList();

            int counter = 0;
            int total = collection.Count();
            bool returnIdentityValue = !string.IsNullOrWhiteSpace(identityColumn) && collection.Count() == 1;
            int result = 0;
            do
            {
                List<List<Classes.Parameter>> valueModules = collection.Skip(counter * 1000).Take(1000)?.Select(x => CreateValueModule(x, propertyObj, columns)).ToList();
                if (valueModules != null && valueModules.Any())
                {
                    result = InsertDbValues(tableName, valueModules, returnIdentityValue);
                }
                

                counter++;
            } while (counter <= total / 1000);

            return returnIdentityValue ? result:  total;
        }


        /// <summary>
        /// Inserts a list of object into the Table named as object type.
        /// </summary>
        /// <param name="items">items to insert. Its type has to match with the name of the table in the database</param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns> If Table identity and items list has only one object returns identity value else the number of inserted rows</returns>
        public async Task<int> InsertListAsync(object items, string tableName = "")
        {
            return await Task.Run(() => InsertList(items, tableName));
        }


        private List<Classes.Parameter> CreateValueModule(object obj, List<PropertyInfo> propertyInfos, IEnumerable<string> columns)
        {
            List<Classes.Parameter> parameters = new List<Classes.Parameter>();
            foreach (string col in columns)
            {
                Classes.Parameter parameter = new Classes.Parameter();
                parameter.Name = col;
                PropertyInfo prop = propertyInfos.FirstOrDefault(x => x.Name.ToLower() == col.ToLower());
                parameter.Value = GetValue(prop, obj);
                parameters.Add(parameter);
            }
            return parameters;
        }



        #endregion

        #region -- DELETE


        /// <summary>
        /// Deletes a row from the table named as the object type. The table must have a primary key.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Returns the number of rows deleted</returns>
        public int Delete(object obj, string tableName = "")
        {
            tableName = string.IsNullOrWhiteSpace(tableName) ? Check.FindPosibleTableName(this, obj.GetType().Name) : tableName;
            string primaryKey = Check.GetColumnPrimaryKey(this, tableName);
            return Delete(obj, tableName, primaryKey);
        }


        /// <summary>
        /// Deletes a row from the table named equal to the object type. The table must have a primary key.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Returns the number of rows deleted</returns>
        public async Task<int> DeleteAsync(object obj, string tableName= "")
        {
            return await Task.Run(() => Delete(obj, tableName));
        }


        private int Delete(object obj, string tableName, string primaryKey)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(primaryKey))
            {
                var value = obj.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == primaryKey.ToLower()).GetValue(obj);
                if (obj.GetType().GetProperties().Select(s => s.Name.ToLower()).Any(n => n == primaryKey.ToLower()))
                {
                    string query = $"DELETE [{tableName}] WHERE {primaryKey} = {value}";
                    result = ChangeDbValues(query, false,30, null);
                }
                else
                {
                    throw new Exception($"Error: The object of type {obj.GetType().Name} does not have a matching field with the primary key {primaryKey} in table {tableName}.");
                }
            }
            else
            {
                throw new Exception($"Error: Table {tableName} does not have a primary key.");
            }
            return result;
        }

        private static int GetIntResult(string result)
        {
            int valueResult = 0;
            if (Int32.TryParse(result, out valueResult))
            {
                return valueResult;
            }
            else
            {
                throw new Exception(result);
            }
        }


        /// <summary>
        /// Removes a range of objects from the table named as object type. The table must have primary key.
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Number of files deleted</returns>
        public int DeleteRange(IEnumerable<object> objs, string tableName = "")
        {
            int result = 0;
            if (objs != null && objs.FirstOrDefault() !=null)
            {

                tableName = string.IsNullOrWhiteSpace(tableName) ? Check.FindPosibleTableName(this, objs.FirstOrDefault().GetType().Name) : tableName;
                string primaryKey = Check.GetColumnPrimaryKey(this, tableName);

                int counter = 0;
                int total = objs.Count();
                do
                {
                    List<string> primaryKeys = new List<string>();
                    List<object> objToDelete = objs.Skip(counter * 1000).Take(1000).ToList();
                    foreach (var obj in objs)
                    {
                        var value = obj.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == primaryKey.ToLower()).GetValue(obj);
                        primaryKeys.Add(value.ToString());
                    }

                    string query = $"DELETE [{tableName}] WHERE {primaryKey} in ({string.Join(",", primaryKeys)})";
                    result = ChangeDbValues(query, false, 30, null);

                    counter++;
                } while (counter <= total / 1000);

                return total;
            }
            return result;
        }

        /// <summary>
        /// Removes a range of objects from the table named as object type. The table must have primary key
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="tableName">Fill it in if tableName is diferent to obj type name</param>
        /// <returns>Number of files deleted</returns>
        public async Task<int> DeleteRangeAsync(IEnumerable<object> objs, string tableName= "")
        {
            return await Task.Run(() => DeleteRange(objs, tableName));
        }

        #endregion

        #endregion

        #region -- OTHERS ---

        internal void SetExclusiveMode(string databaseName)
        {
            try
            {
                string query = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                ChangeDbValues(query, false);
            }catch 
            {
                throw new Exception($"Something went wrong traying tu get single_user acces to database {databaseName}");
            }            
        }

        /// <summary>
        /// Execute script in single user mode.
        /// </summary>
        /// <param name="databaseName">Database where query should be excecuted</param>
        /// <param name="script">Scritp to execute</param>
        /// <param name="timeOut">Operation timeout</param>
        /// <returns>Error message</returns>
        /// <exception cref="Exception"></exception>
        public string ExecuteScriptExclusiveMode(string databaseName, string script, int timeOut)
        {
            try
            {
                string currentDbName = CurrentDatabaseName;
                GetInstance(ConnectionString, "master");
                string sub = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                ChangeDbValues(sub, false);
                try
                {
                    GetInstance(ConnectionString, currentDbName);
                    ChangeDbValues(script, false, timeOut);

                    GetInstance(ConnectionString, "master");
                    List<int> spids = GetSP_WHO(databaseName);
                    foreach (int spid in spids)
                    {
                        ChangeDbValues($"kill {spid}", false);
                    }

                    ChangeDbValues($"ALTER DATABASE [{databaseName}] SET MULTI_USER; ", false);
                    GetInstance(ConnectionString, databaseName);

                    return "";
                }
                catch (Exception ex)
                {
                    return ex.Message;                    
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong traying tu get single_user acces to database {databaseName} Error {e.Message}");
            }
        }

        private List<int> GetSP_WHO(string dbName)
        {
            DataTable dt = GetDataTable("sp_Who", true);
            List<int> list = new List<int>();
            if(dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["dbname"].ToString().ToLower() == dbName.ToLower())
                        list.Add(Int32.Parse(dr["spid"].ToString()));
                }
            }
            return list;
        }

        #endregion

    }
}
