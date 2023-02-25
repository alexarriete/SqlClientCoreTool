using SqlClientCoreTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public class ServerName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ServerIp { get; set; }
        public bool PreventCopyTo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string ConnectionString { get; set; }


      
    }
}
