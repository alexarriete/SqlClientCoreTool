using System;
using System.Collections.Generic;
using System.Text;

namespace SqlClientCoreTool.Classes
{
    /// <summary>
    /// A string object value pair.
    /// </summary>
    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        /// <summary>
        /// Creates up to five parameters in a simple way.
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="value1"></param>
        /// <param name="name2"></param>
        /// <param name="value2"></param>
        /// <param name="name3"></param>
        /// <param name="value3"></param>
        /// <param name="name4"></param>
        /// <param name="value4"></param>
        /// <param name="name5"></param>
        /// <param name="value5"></param>
        /// <returns></returns>
        public static List<Parameter> ParameterList(string name1, object value1, string name2= "", object value2 = null, string name3 = ""
            , object value3 = null, string name4 = "", object value4 = null, string name5 = "", object value5 = null)
        {
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter() { Name = name1, Value = value1 });

            if(!string.IsNullOrEmpty(name2) && value2 != null)
            {
                parameters.Add(new Parameter() { Name = name2, Value = value2 });

                if (!string.IsNullOrEmpty(name3) && value3 != null)
                {
                    parameters.Add(new Parameter() { Name = name3, Value = value3 });

                    if (!string.IsNullOrEmpty(name4) && value4 != null)
                    {
                        parameters.Add(new Parameter() { Name = name4, Value = value4 });

                        if (!string.IsNullOrEmpty(name5) && value5 != null)
                        {
                            parameters.Add(new Parameter() { Name = name5, Value = value5 });
                        }
                    }
                }
            }
            return parameters;
        }

        internal static List<Parameter> CheckAtForProcedure(List<Parameter> parameters, bool isProcedure)
        {
            if (isProcedure && parameters !=null)
            {
                foreach (Parameter parameter in parameters)
                {
                    if (!parameter.Name.StartsWith("@"))
                    {
                        parameter.Name = $"@{parameter.Name}";
                    }
                }
            }
            return parameters;
        }
    }
}
