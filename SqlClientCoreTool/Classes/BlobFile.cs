using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SqlClientCoreTool.Classes
{
    /// <summary>
    /// Any class inheriting from it will contains a property to store documents, images etc, in a sql table.
    /// See Transformer.CreateBlobFile(...)
    /// </summary>
    public class BlobFile: IAR
    {
       /// <summary>
       /// Filename
       /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Property to store documents, images, etc.
        /// </summary>
        public byte[] FileData { get; set; }
        /// <summary>
        /// File extension
        /// </summary>
        public string Extension { get; set; }

        internal BlobFile GetInstance(string path)
        {
           // DataGather dg = DataGather.GetInstance(connectionString);
           // FileData = (byte[])dg.GetSingleValue($"SELECT BulkColumn FROM OPENROWSET (Bulk '{path}', SINGLE_BLOB) AS varBinaryData;", false);
            FileData = File.ReadAllBytes(path);
            FileInfo file = new FileInfo(path);
            Name = file.Name;
            Extension = file.Extension;
            return this;
        }
      
    }
}
