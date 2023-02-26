# What is this project about?

SqlClientCoreTool is a class library implemented in c#. It contains several methods for the most common interactions between SQL and .Net apps. It can be used in small projects where the complexity of current frameworks is not required.
In addition to CRUD operations, there are a group of methods born from the experience of working with databases, such as creating backups, saving session data or converting images.

# Dependencies

_In current version_

- net7.0 (_For projects in .net 6 use Version 6.00_)
- System.Data.SqlClient (>= 4.8.5)

# How it works?

After installing the [Nuget package](https://www.nuget.org/packages/SqlClientCoreTool) we will be able to use the DataGather class. Through this, the most common CRUD operations can be performed.

```csharp
using SqlClientCoreTool;
...

```

## CRUD examples

##### Multiple Insert example

```csharp
  public async Task InsertListAsyn(List<User> users)
  {
        DataGather dg = DataGather.GetInstance(ConnectionString);
        await dg.InsertListAsync(users);
  }
```

##### Simple update example
_We don't need to specify the name of the table if it matches the object type. Table must have a primary key_

```csharp
  public async Task UpdateAsync(User user)
  {
        DataGather dg = DataGather.GetInstance(ConnectionString);
        await dg.UpdateAsync(user);
  }
```

##### Delete range example
_Removes a ragne of rows. The table must have a primary key_

```csharp
  public async Task UpdateAsync(List<User> user, string tableName)
  {
        DataGather dg = DataGather.GetInstance(ConnectionString);
        await dg.DeleteRangeAsync(users, tableName);
  }
```

## Other examples

##### Change current database
_In the example we change the connection to a diferent database_

```csharp
    public static bool ChangeDatabase(string dbName)
        {
            try
            {
                DataGather dataGather = DataGather.GetInstance(ConnectionString, dbName);
                CurrentDatabase currentDatabase = CurrentDatabase.Get(dataGather);
                return currentDatabase.Name == dbName;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
```

##### Create backup
_Backup current dababase_

```csharp
     public static bool TestBackupDatabase(string path)
        {
            try
            {
                DataGather dataGather = DataGather.GetInstance(ConnectionString);
                CurrentDatabase currentDatabase = CurrentDatabase.Get(dataGather);
                currentDatabase.BackupWithCompression(path);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
```

## Summary
Other examples can be found on [GitHub.](https://github.com/alexarriete/SqlClientCoreTool)
