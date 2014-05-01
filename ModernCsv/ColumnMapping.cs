using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernCsv
{
    public class ColumnMapping
    {
        public static string AppSettingsPrefix = "ColumnMap.";

        public int Index { get; set; }
        public Type ModelType { get; set; }
        public string HeaderName { get; set; }
        public PropertyDescriptor PropertyDescriptor { get; set; }

        public static Func<Type, string, string> PropertyNameSolver = (type, headerName) =>
        {
            var typeName = type.Name;
            var keyPrefix = AppSettingsPrefix + typeName + ".";

            // ちょっと効率わるいけど、キー(プロパティ名)から値(ヘッダ名)を逆引き
            var candidateKeys = ConfigurationManager.AppSettings.AllKeys.Where(key => key.StartsWith(keyPrefix));

            // 値に指定ヘッダ名が入ってるのがあるなら、それをプロパティ名にする。
            // ないならヘッダ名そのまま信じる。
            var propertyKey = candidateKeys.FirstOrDefault(
                key => headerName.Equals(ConfigurationManager.AppSettings[key], StringComparison.InvariantCultureIgnoreCase)
            );

            if (string.IsNullOrEmpty(propertyKey))
                return headerName;

            return propertyKey.Replace(keyPrefix, "");
        };

        public bool IsValid
        {
            get
            {
                return PropertyDescriptor != null;
            }
        }
        /// <summary>
        /// ヘッダ名を対象モデル型のプロパティ名に変換するよ
        /// </summary>
        public string PropertyName
        {
            get
            {
                return PropertyNameSolver(ModelType, HeaderName);
            }
        }
    }
}
