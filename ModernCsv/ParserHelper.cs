using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernCsv
{
    public class ParserHelper
    {
        public static string[] SplitLine(string line, char spliter)
        {
            return line.Split(spliter);
        }

        public static object ConvertFromString(PropertyDescriptor propertyDescriptor, string value)
        {
            return propertyDescriptor.Converter.ConvertFromString(value);
        }

        public static bool IsValidValue(PropertyDescriptor propertyDescriptor, string value)
        {
            var result = true;
            try
            {
                ConvertFromString(propertyDescriptor, value);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static bool CanCastIntToEnum(Type enumType, int value)
        {
            bool success;
            try
            {
                success = Enum.GetValues(enumType).Cast<int>().Contains(value);
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public static ValidationResult IntToEnumValidation<T>(Type enumType, int? value, ValidationContext validationContext) where T : class
        {
            var propertyName = validationContext.MemberName;

            // Nullなら判定できませんね。一応成功にするから必須ならRequiredにしてね。
            if (!value.HasValue)
                return ValidationResult.Success;

            bool success;
            try
            {
                success = ParserHelper.CanCastIntToEnum(enumType, int.Parse(value.ToString()));
            }
            catch
            {
                success = false;
            }
            return success
                ? ValidationResult.Success
                : new ValidationResult(propertyName + " に未登録の値が指定されています。", new List<string> { propertyName });
        }

        public static string[] BoolTrueValues = new[] { "true", "1", "on", "good", "○" };
        public static string[] BoolFalseValues = new[] { "false", "0", "off", "bad", "×" };
        public static string NormalizeString(PropertyDescriptor propertyDescriptor, string value)
        {
            // boolならいろいろ自動変換してあげる
            if (propertyDescriptor.PropertyType == typeof(bool) ||
                propertyDescriptor.PropertyType == typeof(bool?))
            {
                if (BoolTrueValues.Contains(value.ToLower()))
                    return "true";
                if (BoolFalseValues.Contains(value.ToLower()))
                    return "false";
            }

            return value;
        }

    }
}
