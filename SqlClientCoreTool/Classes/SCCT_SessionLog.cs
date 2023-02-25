using SqlClientCoreTool.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlClientCoreTool.Classes
{
    /// <summary>
    /// Class to create a log table to store temporal values.
    /// </summary>
    public class SCCT_SessionLog:IAR
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        
    }
}
