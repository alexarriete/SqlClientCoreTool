[![Getting Started](https://img.shields.io/badge/lang-en-red.svg)](https://github.com/alexarriete/SqlClientCoreTool/blob/master/README.md) [![Getting Started](https://img.shields.io/badge/lang-es-yellow.svg)](https://github.com/alexarriete/SqlClientCoreTool/blob/master/Readme.es.md)

# De qué trata este proyecto?

TextDatabase es una librería de clases implementada en c#. Contiene una serie de métodos para las interacciones más comunes entre una base de datos y las aplicaciones en .Net. Se puede utilizar en pequeños proyectos de consola y escritorio.

# Dependencias

_En la versión actual_

- net7.0
- Newtonsoft.Json 13.03

# ¿Cómo funciona?

Luego de instalar el [paquete de Nuget](https://www.nuget.org/packages/TextDatabase) seremos capaces de utilizar la clase FileHandler y con ella las operaciones CRUD más comunes.

El producto es fuertemente tipado y necesita seguir una serie de reglas para su correcto funcionamiento.

- La base de datos se creará automáticamente al llamar el método FileHandler.GetInstance(appName). Su ubicación es C:\Users\{username}\AppData\Local\{appName}
- Las tablas se crearán con el primer Insert y llevarán el nombre de la clase que se inserta.
- Todos los objetos a insertar deben pertenecer a clases que hereden de TextDatabase.IAR y llevarán un Id entero que será la clave primaria y por tanto debe ser único.

```csharp
using TextDataBase;
...

```

## Ejemplo de operaciones CRUD

##### Ejemplo de inserción múltiple

```csharp
  public async Task InsertListAsyn(List<User> users)
  {
        FileHandler fh = FileHandler.GetInstance("MyApp");
        await fh.InsertListAsync<User>(users);
  }
```

##### Ejemplo de Get

```csharp
  public async Task GetUserList()
  {
        FileHandler fh = FileHandler.GetInstance("MyApp");
        await fh.Get<User>();
  }
```

##### Ejemplo de Update

```csharp
  public async Task UpdateAsync(User user)
  {
       FileHandler fh = FileHandler.GetInstance("MyApp");
        await fh.UpdateAsync<User>(user);
  }
```

##### Ejemplo de Delete

```csharp
  public async Task DeleteAsync(List<User> user)
  {
         FileHandler fh = FileHandler.GetInstance("MyApp");
        await fh.Delete<USser>(users, tableName);
  }
```

## Otros ejemplos

##### Ejemplo de crear una copia de la base de datos

```csharp
     public static bool BackupDatabase(string path)
        {
            try
            {
                DataBaseHandler.CreateBackup("MyApp", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), true);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
```

## Conclusión

Aunque se han realizado pruebas con miles de registros, no lo consideramos sustitutivo de las bases de datos. TextDatabase puede funcionar bien con una estructura de poca complejidad y un pequeño volumen de datos. En específico, cuando se requiera almacenar datos temporales o propios de la configuración, etc.
