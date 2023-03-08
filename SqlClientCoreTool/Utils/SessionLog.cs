using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlClientCoreTool.Utils
{
    public enum Period
    {
        One_Minute,
        Five_Minutes,
        Ten_Minutes,
        Fifteen_Minutes,
        Half_An_Hour,
        One_Hour,
        One_Day,
        One_Week,
        One_Month,
        One_Year,
        Forever
    }


    /// <summary>
    /// Methods to manage SCCT_SessionLog table.
    /// Examples: https://sqlcct.acernuda.com
    /// </summary>
    public sealed class SessionLog
    {

        #region -- PROPERTIES, CONSTRUCTORS...

        private const string SCCT_SessionLog = "SCCT_SessionLog";
        private static DataGather DataGather { get; set; }

        private static Lazy<SessionLog> instance = new Lazy<SessionLog>(() => new SessionLog());
        /// <summary>
        /// Creates an instance of SessionLog
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SessionLog GetInstance(string connectionString)
        {
            DataGather = DataGather.GetInstance(connectionString);

            return instance.Value;

        }
        private SessionLog()
        {

        }

        private string GetCreateQueryTable()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($" CREATE TABLE [{SCCT_SessionLog}] ");
            stringBuilder.Append($" ( ");

            stringBuilder.Append($" [id]int not null primary key identity(1,1), ");
            stringBuilder.Append($" [key] varchar(50) not null, ");
            stringBuilder.Append($" [value] varchar(500) not null , ");
            stringBuilder.Append($" [recordDate] datetime default(getdate()) not null , ");
            stringBuilder.Append($" [expirationDate] datetime  null,  ");

            stringBuilder.Append($" [active] bit not  null,  ");
            stringBuilder.Append($" [rowUpdateDate] datetime not  null  ");

            stringBuilder.Append($" ) ");

            return stringBuilder.ToString();
        }


        /// <summary>
        /// If SCCT_SessionLog doesn't exist creates it and insert a record.
        /// Special permission are needed on database to create a table. 
        /// If user in ConnectionString doesn't have it you can previously create a table.
        /// Script to create the table, available in https://sqlcct.acernuda.com
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationDate"></param>
        /// <returns>returns the number of rows inserted</returns>
        public int Insert(string key, string value, DateTime? expirationDate)
        {
            string exist = Check.CheckTableExist(DataGather.ConnectionString, SCCT_SessionLog);
            if (string.IsNullOrEmpty(exist))
            {
                DataGather.ChangeDbValues(GetCreateQueryTable(), false,30, null);
            }
            else
            {
                CheckOldValues();
            }

            Classes.SCCT_SessionLog sCCT_Session = new Classes.SCCT_SessionLog();
            sCCT_Session.Key = key;
            sCCT_Session.Value = value;
            sCCT_Session.RecordDate = DateTime.Now;
            sCCT_Session.ExpirationDate = expirationDate;

            return DataGather.Insert(sCCT_Session);
        }

        /// <summary>
        /// If SCCT_SessionLog doesn't exist creates it and insert a record.
        /// Special permission are needed on database to create a table. 
        /// If user in ConnectionString doesn't have it you can previously create a table.
        /// Script to create the table, available in https://sqlcct.acernuda.com
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationDate"></param>
        /// <returns>returns the number of rows inserted</returns>
        public async Task<int> InsertAsync(string key, string value, DateTime? expirationDate)
        {
            return await Task.Run(() => Insert(key, value, expirationDate));
        }

        /// <summary>
        /// If SCCT_SessionLog doesn't exist creates it and insert a record.
        /// Special permission are needed on database to create a table. 
        /// If user in ConnectionString doesn't have it you can previously create a table.
        /// Script to create the table, available in https://sqlcct.acernuda.com
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="period">Period is a enum with several preset periods</param>
        /// <returns>returns the number of rows inserted</returns>
        public int Insert(string key, string value, Period period)
        {
            DateTime? expirationDate = ConvertPeriod(period);
            return Insert(key, value, expirationDate);
            
        }

        /// <summary>
        /// If SCCT_SessionLog doesn't exist creates it and insert a record.
        /// Special permission are needed on database to create a table. 
        /// If user in ConnectionString doesn't have it you can previously create a table.
        /// Script to create the table, available in https://sqlcct.acernuda.com
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="period">Period is a enum with several preset periods</param>
        /// <returns>returns the number of rows inserted</returns>
        public async Task<int> InsertAsync(string key, string value, Period period)
        {
            return await Task.Run(() => Insert(key, value, period));
        }

        /// <summary>
        /// Gets all rows for a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>IEnumerable of Classes.SCCT_SessionLog</returns>
        public IEnumerable<Classes.SCCT_SessionLog> Get(string key)
        {
            CheckOldValues();
            return DataGather.Get<Classes.SCCT_SessionLog>().OrderBy(x => x.ExpirationDate);
        }

        /// <summary>
        /// Gets all rows for a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>IEnumerable of Classes.SCCT_SessionLog</returns>
        public async Task<IEnumerable<Classes.SCCT_SessionLog>> GetAsync(string key)
        {
            return await Task.Run(() => Get(key));
        }

        private void CheckOldValues()
        {
            IEnumerable<Classes.SCCT_SessionLog> sCCT_Sessions = 
                    DataGather.Get<Classes.SCCT_SessionLog>().Where(x => x.ExpirationDate < DateTime.Now);
            
            DataGather.DeleteRange(sCCT_Sessions);
        }

        private DateTime? ConvertPeriod(Period period)
        {
            switch (period)
            {
                case Period.One_Minute:
                    return DateTime.Now.AddMinutes(1);
                case Period.Five_Minutes:
                    return DateTime.Now.AddMinutes(5);
                case Period.Ten_Minutes:
                    return DateTime.Now.AddMinutes(10);
                case Period.Fifteen_Minutes:
                    return DateTime.Now.AddMinutes(15);
                case Period.Half_An_Hour:
                    return DateTime.Now.AddMinutes(30);
                case Period.One_Hour:
                    return DateTime.Now.AddHours(1);
                case Period.One_Day:
                    return DateTime.Now.AddDays(1);
                case Period.One_Week:
                    return DateTime.Now.AddDays(7);
                case Period.One_Month:
                    return DateTime.Now.AddMonths(1);
                case Period.One_Year:
                    return DateTime.Now.AddYears(1);
                default:
                    return null;
            }
        }

        #endregion
    }
}
