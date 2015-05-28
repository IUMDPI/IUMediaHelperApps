using System;
using System.Data;
using System.Reflection;
using Packager.Attributes;

namespace Packager.Utilities
{
    public class ExcelImporter<T> : IExcelImporter where T : new()
    {
        public object Import(DataRow row)
        {
            var result = new T();

            SlurpFromRow(result, row);
            return result;
        }

        private static void SlurpFromRow(T instance, DataRow row)
        {
            foreach (var info in instance.GetType().GetProperties())
            {
                SetStringValue(instance, info, row);
                SetObjectValue(instance, info, row);
            }
        }

        private static void SetStringValue(T instance, PropertyInfo info, DataRow row)
        {
            var fieldAttribute = info.GetCustomAttribute<ExcelFieldAttribute>();
            if (fieldAttribute == null)
            {
                return;
            }

            var value = row[fieldAttribute.Name].ToString();
            if (string.IsNullOrWhiteSpace(value) && fieldAttribute.Required)
            {
                throw new Exception(string.Format("The field {0} is required", fieldAttribute.Name));
            }

            info.SetValue(instance, value);
        }

        private static void SetObjectValue(T instance, PropertyInfo info, DataRow row)
        {
            var objectAttribute = info.GetCustomAttribute<ExcelObjectAttribute>();
            if (objectAttribute == null)
            {
                return;
            }

            var importer = GetImporterForType(info.PropertyType);
            var value = importer.Import(row);
            info.SetValue(instance, value);
        }

        private static IExcelImporter GetImporterForType(Type type)
        {
            var importerType = typeof (ExcelImporter<>);
            var typeArgs = new[] {type};
            var maker = importerType.MakeGenericType(typeArgs);
            return (IExcelImporter) Activator.CreateInstance(maker);
        }
    }
}