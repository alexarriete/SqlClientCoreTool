using SqlClientCoreTool;
using SQLCLIENTUTEST.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester;
using Xunit;

namespace SQLCLIENTUTEST
{
    public class CRUD
    {
       
        private async Task CleanDBAsync()
        {
            DataGather dg = DataGather.GetInstance(ConnectionString);
            await dg.ChangeDbValuesAsync("TRUNCATE TABLE [USER]", false, 30, null);
        }
        private string ConnectionString { get { return TestCases.ConnectionString; } }
        [Fact]
        public async Task TestInsertAsyn()
        {
            await CleanDBAsync();
            DataGather dg = DataGather.GetInstance(ConnectionString);

            User user = new User(1);
            User user2 = new User(1);
            

            await dg.InsertAsync(user);
            int identityResult = await dg.InsertAsync(user2);

            User userResult = (await dg.GetAsync<User>()).FirstOrDefault();

            Assert.True(user.Name == userResult.Name && user.UserName == userResult.UserName);

            if(user.Name == userResult.Name && user.UserName == userResult.UserName)
            {

                userResult.Password = "OtherPassword";
                int updateResult = await dg.UpdateAsync(userResult);
                Assert.Equal(1, updateResult);
                User userResult2 = (await dg.GetAsync<User>()).FirstOrDefault(x=>x.Id == userResult.Id);

                Assert.True(userResult.Password == userResult2.Password);

                int deleteResult = await dg.DeleteAsync(userResult);
                Assert.Equal(1, deleteResult);
            }

            User lastUser = (await dg.GetAsync<User>()).LastOrDefault();
            Assert.True(lastUser.Id == identityResult);
        }
        [Fact]
        public async Task TestInsertListAsyn()
        {
            await CleanDBAsync();
            DataGather dg = DataGather.GetInstance(ConnectionString);

            List<User> users = new List<User>();
            users.AddRange(User.GetUserList(10));

            await dg.InsertListAsync(users);

            List<User> userResult = (await dg.GetAsync<User>()).ToList();
            Assert.True(users.Count() == userResult.Count());

            int deleteResult = await dg.DeleteRangeAsync(userResult);
            Assert.Equal(userResult.Count(), deleteResult);

            users.AddRange(User.GetUserList(10));
            await dg.InsertListAsync(users);
            userResult = (await dg.GetAsync<User>()).ToList();
            Assert.Equal(2000, await dg.DeleteRangeAsync(userResult));

            //Assert.Throws<ArgumentException>("Id", () => dg.DeleteRange(userResult));

        }

        [Fact]
        public async Task TestCurrentDatabaseAsyn()
        {
            DataGather dg = DataGather.GetInstance(ConnectionString);

            CurrentDatabase currentDatabase = await Task.Run(() => CurrentDatabase.Get(dg));
            Assert.True(!string.IsNullOrEmpty(currentDatabase.Name));

            string result = currentDatabase.BackupWithCompression(@$"c:\backups\{currentDatabase.Name}.bak");
            Assert.True(string.IsNullOrEmpty(result));
            Assert.True(((DateTime)currentDatabase.LastBackup).Date == DateTime.Today.Date);

            result = await Task.Run(()=> currentDatabase.RestoreWithMove(@$"c:\backups\{currentDatabase.Name}.bak"));
            Assert.True(!string.IsNullOrEmpty(result) && result == currentDatabase.CurrentDatabaseSecurity.CheckAllowRestore());

            currentDatabase.CurrentDatabaseSecurity.AllowRestore = true;
            result = await Task.Run(()=> currentDatabase.RestoreWithMove(@$"c:\backups\{currentDatabase.Name}.bak"));
            Assert.True(string.IsNullOrEmpty(result));
            Assert.True(!currentDatabase.CurrentDatabaseSecurity.AllowRestore);

        }
    }
}
