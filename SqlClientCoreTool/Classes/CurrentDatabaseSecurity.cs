using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlClientCoreTool
{
    /// <summary>
    /// Deals with some rules to prevent undesirables changes in current database. 
    /// Specifics changes in CurrentDatabaseSecurity will rollback after the accion is donne.
    /// </summary>
    public class CurrentDatabaseSecurity
    {
        /// <summary>
        /// Grant permission to restore database, once.
        /// </summary>
        public bool AllowRestore { get; set; }

        public string CheckAllowRestore()
        {
            return AllowRestore ? "" : "CurrentDatabaseSecurity.AllowRestore is set to false.";
        }
        
    }
}
