[![Getting Started](https://img.shields.io/badge/lang-en-red.svg)](https://github.com/alexarriete/SqlClientCoreTool/blob/master/README.md) [![Getting Started](https://img.shields.io/badge/lang-es-yellow.svg)](https://github.com/alexarriete/SqlClientCoreTool/blob/master/README.es.md)

# De qué trata este proyecto?

SqlClientCoreTool es una librería de clases implementada en c#. Contiene una serie de métodos para las interacciones más comunes entre SQL y las aplicaciones en .Net. Se puede utilizar en pequeños proyectos donde no se requiera la complejidad de los Frameworks actuales.
Además de las operaciones CRUD existen un grupo de métodos nacidos de la experiencia de trabajar con las bases de datos, como la creación de Backups, el guardado de datos de sesión o la conversión de imágenes.

# Dependencias

_En la versión actual_

- net7.0 (_Para proyectos en .net 6 use la version 6.00_)
- System.Data.SqlClient (>= 4.8.5)

# How it works?

Luego de instalar el [paquete de Nuget](https://www.nuget.org/packages/SqlClientCoreTool) seremos capaces de utilizar la clase DataGather y con ella las operaciones CRUD más comunes.

```csharp
using SqlClientCoreTool;
...

```

## Ejemplo de operaciones CRUD

##### Ejemplo de inserción múltiple

```csharp
  public async Task InsertListAsyn(List<User> users)
  {
        DataGather dg = DataGather.GetInstance(ConnectionString);
        await dg.InsertListAsync(users);
  }
```

##### Ejemplo de update

_Si el nombre de la tabla es el mismo que la del tipo, en este caso user, no necesitamos especificarlo. La tabla debe contener una clase primaria_

```csharp
  public async Task UpdateAsync(User user)
  {
        DataGather dg = DataGather.GetInstance(ConnectionString);
        await dg.UpdateAsync(user);
  }
```

##### Ejemplo de Delete range

_Elimina un rango de filas. La tabla debe tener una clave primaria_

```csharp
  public async Task UpdateAsync(List<User> user, string tableName)
  {
        DataGather dg = DataGather.GetInstance(ConnectionString);
        await dg.DeleteRangeAsync(users, tableName);
  }
```

## Otros ejemplos

##### Cambiar la base de datos en uso

_En el ejemplo cambiamos la base de datos en uso_

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

##### Ejemplo de crear una copia de la base de datos

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

Otros ejemplos pueden ser encontrados en [GitHub.](https://github.com/alexarriete/SqlClientCoreTool)
