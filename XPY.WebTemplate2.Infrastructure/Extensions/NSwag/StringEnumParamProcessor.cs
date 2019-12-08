using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XPY.WebTemplate2.Infrastructure.Extensions.NSwag
{
    /// <summary>
    /// Swagger 可選參數操作過濾器
    /// </summary>
    public class StringEnumParamProcessor : IOperationProcessor
    {
        /// <summary>
        /// 所有列舉值
        /// </summary>
        /// <returns>列舉值陣列</returns>
        private static string[] GetAllValues(Type t)
        {
            // 取得自身類型中所有靜態欄位
            var tStaticFields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            // 取得字串常數欄位
            var tConstFields = tStaticFields.Where(x => x.IsLiteral && !x.IsInitOnly && x.FieldType == typeof(string));

            // 取得欄位值
            var tConstFieldValues = tConstFields.Select(x => x.GetValue(null) as string);

            return tConstFieldValues.ToArray();
        }

        /// <summary>
        /// 所有列舉值名稱
        /// </summary>
        /// <returns>列舉值陣列</returns>
        private static string[] GetAllValueNames(Type t)
        {
            // 取得自身類型中所有靜態欄位
            var tStaticFields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            // 取得字串常數欄位
            var tConstFields = tStaticFields.Where(x => x.IsLiteral && !x.IsInitOnly && x.FieldType == typeof(string));

            // 取得欄位值
            var tConstFieldNames = tConstFields.Select(x => x.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? x.Name);

            return tConstFieldNames.ToArray();
        }

        /// <summary>
        /// 所有列舉值名稱
        /// </summary>
        /// <returns>列舉值陣列</returns>
        private static string[] GetAllValueDescriptions(Type t)
        {
            // 取得自身類型中所有靜態欄位
            var tStaticFields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            // 取得字串常數欄位
            var tConstFields = tStaticFields.Where(x => x.IsLiteral && !x.IsInitOnly && x.FieldType == typeof(string));

            // 取得欄位值
            var tConstFieldDescriptions = tConstFields.Select(x => x.GetCustomAttribute<DescriptionAttribute>()?.Description);

            return tConstFieldDescriptions.ToArray();
        }

        /// <summary>
        /// 取得棄用的參數
        /// </summary>
        /// <returns>列舉值陣列</returns>
        private static bool[] GetAllValueObsolete(Type t)
        {
            // 取得自身類型中所有靜態欄位
            var tStaticFields = t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            // 取得字串常數欄位
            var tConstFields = tStaticFields.Where(x => x.IsLiteral && !x.IsInitOnly && x.FieldType == typeof(string));

            // 取得欄位值
            var tConstFieldObsoletes = tConstFields.Select(x => x.GetCustomAttribute<ObsoleteAttribute>() != null);

            return tConstFieldObsoletes.ToArray();
        }


        public bool Process(OperationProcessorContext context)
        {
            foreach (var param in context.Parameters.Values.ToList())
            {
                var paramInfo = context.MethodInfo.GetParameters().Single(x => x.Position == param.Position - 1);

                var stringEnum = paramInfo.GetCustomAttribute<StringEnumDataTypeAttribute>();
                if (stringEnum != null)
                {
                    List<(string value, string name)> mapping = new List<(string value, string name)>();

                    var values = GetAllValues(stringEnum.EnumType);
                    var names = GetAllValueNames(stringEnum.EnumType);
                    var descriptions = GetAllValueDescriptions(stringEnum.EnumType);
                    var obsoletes = GetAllValueObsolete(stringEnum.EnumType);

                    var table = "<table><thead><tr><th>值</th><th>名稱</th><th>說明</th></tr></thead><tbody>";

                    for (int i = 0; i < values.Length; i++)
                    {
                        param.Enumeration.Add(values[i]);
                        mapping.Add((values[i], names[i]));
                        if (obsoletes[i])
                        {
                            table += $"<tr><td><i>{values[i]}(棄用)</i></td><td><i>{names[i]}</i></td><td><i>{descriptions[i]}</i></td></tr>";
                        }
                        else
                        {
                            table += $"<tr><td>{values[i]}</td><td>{names[i]}</td><td>{descriptions[i]}</td></tr>";
                        }
                    }

                    table += "</tbody></table>";

                    param.Description += "\r\n" + table;
                }
            }
            return true;
        }
    }
}
