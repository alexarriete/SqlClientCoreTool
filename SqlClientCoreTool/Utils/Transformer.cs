using SqlClientCoreTool.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlClientCoreTool.Utils
{
    /// <summary>
    /// Object transformer.
    /// </summary>
    public class Transformer
    {
        /// <summary>
        /// Gets a List<typeparamref name="T"/> from a Datatable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns>List<typeparamref name="T"/></returns>
        public static ICollection<T> GetListFromDataTable<T>(DataTable dt)
        {
            CheckType<T>(dt.Columns.Count);

            ICollection<T> list =new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T obj = GetObject<T>(row);
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Gets a List<typeparamref name="T"/> from a Datatable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns>List<typeparamref name="T"/></returns>
        public static async Task<ICollection<T>> GetListFromDataTableAsync<T>(DataTable dt)
        {
            return await Task.Run(() => GetListFromDataTable<T>(dt));
        }

        /// <summary>
        /// Converts any images, documents, etc, to varbinary(Max) to store in Database
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>A BlobFile Class. The property FileData stores a byte[] that can be saved in Database as varbinary(max) type.</returns>
        public static BlobFile CreateBlobFile(string path)
        {            
            BlobFile blobFile = new BlobFile();
            return blobFile.GetInstance(path);
        }

        /// <summary>
        /// Converts any images, documents, etc, to varbinary(Max) to store in Database
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>A BlobFile Class. The property FileData stores a byte[] that can be saved in Database as varbinary(max) type.</returns>
        public async static Task<BlobFile> CreateBlobFileAsync(string path)
        {
            return await Task.Run(() => CreateBlobFile(path));
        }



        private static T GetObject<T>(DataRow row)
        {
            List<string> frameworkTypesName = GetSimpleTypes();
            var typeName = typeof(T).Name;
            if (frameworkTypesName.Any(x => x == typeName))
            {
                return (T)row[0];
            }
            T obj = (T)Activator.CreateInstance(typeof(T));
            foreach (PropertyInfo ob in obj.GetType().GetProperties().Where(x => !x.GetCustomAttributes(typeof(NotMappedAttribute)).Any()))
            {
                try
                {
                    object value = row[ob.Name];
                    if (value != DBNull.Value)
                    {
                        ob.SetValue(obj, value);
                    }                    
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in field {ob.Name} {ex.Message}");
                }
            }
            return obj;
        }

        internal static void CheckType<T>(int columnNumber)
        {
            List<string> frameworkTypesName = GetSimpleTypes();
            var typeName = typeof(T).Name;
            if(frameworkTypesName.Any(x=>x== typeName) && columnNumber > 1)
            {
                throw new Exception("Error: Number of columns is diferent to number of expected parameters");
            }

        }

        private static List<string> GetSimpleTypes()
        {
            List<string> frameworkTypesName = typeof(Type).Assembly.GetTypes().Where(x => x.IsPrimitive).Select(x => x.Name).ToList();
            frameworkTypesName.Add("String");
            frameworkTypesName.Add("Object");
            return frameworkTypesName;
        }
    }
}
