using SqlClientCoreTool;
using SqlClientCoreTool.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLCLIENTUTEST.Data
{
    public class ValueTest : IAR
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
        public byte[] Photo { get; set; }
        public Guid GuidId { get; set; }
    }
}
