using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PluggableWorkers
{
    public class SettingsFactory
    {
        private static object ConvertArray(string rawValue, PropertyInfo prop)
        {
            var elementType = prop.PropertyType.GetElementType();
            var convertedValues =
                rawValue.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => elementType == typeof (DateTime)
                                         ? ConvertDateTime(v)
                                         : ConvertPrimitiveType(elementType, v))
                        .ToArray();

            var targetArray = Array.CreateInstance(elementType, convertedValues.Length);

            for (var i = 0; i < convertedValues.Length; i++)
            {
                targetArray.SetValue(convertedValues[i], i);
            }

            return targetArray;
        }

        private static object ConvertPrimitiveType(Type targetType, string rawValue)
        {
            if (targetType != typeof (string) && targetType != typeof(char))
                rawValue = rawValue.Replace(",", "");

            return Convert.ChangeType(rawValue, targetType);
        }

        private static object ConvertDateTime(string rawValue)
        {
            switch (rawValue.ToUpper())
            {
                case "TODAY":
                    return DateTime.Today;
                case "YESTERDAY":
                    return DateTime.Today.AddDays(-1);
                case "TOMORROW":
                    return DateTime.Today.AddDays(1);
                default:
                    return DateTime.Parse(rawValue);
            }
        }

        public object GetSettingsFor(Type settingsType, Dictionary<string, string> parameters)
        {
            var settings = Activator.CreateInstance(settingsType);

            foreach (var prop in settingsType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite))
            {
                var settingName = prop.Name;

                string rawValue = null;
                if (parameters.ContainsKey(prop.Name))
                    rawValue = parameters[prop.Name];

                if (rawValue == null)
                    throw new ApplicationException("Setting not found in config file: " + settingName);

                object targetValue;

                //NOTE: This only works for primitive, DateTime, and non-nullable types and arrays.
                if (prop.PropertyType.IsArray)
                    targetValue = ConvertArray(rawValue, prop);
                else if (prop.PropertyType.Name == "DateTime")
                    targetValue = ConvertDateTime(rawValue);
                else
                    targetValue = ConvertPrimitiveType(prop.PropertyType, rawValue);

                prop.SetValue(settings, targetValue, null);
            }

            return settings;
        }

    }
}
