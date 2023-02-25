using Microsoft.VisualBasic.CompilerServices;
using SqlClientCoreTool;
using SqlClientCoreTool.Classes;
using SqlClientCoreTool.Utils;
using SQLCLIENTUTEST.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester;
using Xunit;

namespace SQLCLIENTUTEST
{
    public class TRANSFORMER
    {
       
        private async Task CleanDBAsync()
        {
            DataGather dg = DataGather.GetInstance(ConnectionString);
            await dg.ChangeDbValuesAsync("TRUNCATE TABLE [USER]", false, 30, null);
        }
        private string ConnectionString { get { return TestCases.ConnectionString; } }
        [Fact]
        public async Task TestGetListFromDataTableAsync()
        {            
            DataGather dg = DataGather.GetInstance(ConnectionString);
            DataTable dt = await dg.GetDataTableAsync("SELECT * FROM VALUETEST", false);
            ICollection<ValueTest> valueTests = await SqlClientCoreTool.Utils.Transformer.GetListFromDataTableAsync<ValueTest>(dt);          

                       
            Assert.True(valueTests.Count() >0);           
          
        }
        [Fact]
        public async Task TestCreateBlobFileAsync()
        {
            BlobFile blobFile = await GetDoomieBlobFile();
            Assert.True(blobFile.Extension == ".jpg");

            ValueTest valueTest = new ValueTest();
            valueTest.Name = blobFile.Name;
            valueTest.GuidId = new Guid();
            valueTest.Photo = blobFile.FileData;
            valueTest.Total = -1;

            DataGather dg = DataGather.GetInstance(ConnectionString);

            int result = await dg.InsertAsync(valueTest);
            Assert.True(result >0);

            List<ValueTest> values = (await dg.GetAsync<ValueTest>()).Where(x => x.Total == -1).ToList();
            result = await dg.DeleteRangeAsync(values);
            Assert.True(result > 0);


        }

        public async Task<BlobFile> GetDoomieBlobFile()
        {
            string path = $"{new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName}/img/magazine.jpg";
            BlobFile blobFile = await Transformer.CreateBlobFileAsync(path);
            return blobFile;
        }
    }
}
