using System;
using System.Collections.Generic;
using System.Text;

namespace SqlClientCoreTool.Classes
{
    /// <summary>
    /// When inheriting from it, any class in the database will have fields normally used in a sql Table; as well as its initialization.
    /// </summary>
    public abstract class IAR
    {
        public int Id { get; set; }
        /// <summary>
        /// This property will be initialized as true.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// This property will be initialized with the current date
        /// </summary>
        public DateTime RowUpdateDate { get; set; }
        
        public IAR() 
        {
            Active = true;
            RowUpdateDate = DateTime.Now;
        }
        
    }
}
