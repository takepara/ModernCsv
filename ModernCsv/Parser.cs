using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernCsv
{
    public class Parser
    {
        public static char SplitChar = '\t';

        /// <summary>
        /// 読み込みループ
        /// </summary>
        /// <param name="tsv"></param>
        /// <returns></returns>
        public static IEnumerable<string> LoadLine(string tsv)
        {
            using (var reader = new StringReader(tsv))
            {
                var line = default(string);
                while ((line = reader.ReadLine()) != null)
                {
                    // 空行無視
                    if (string.IsNullOrEmpty(line))
                        continue;

                    yield return line;
                }
            }
        }

        /// <summary>
        /// ヘッダ行からカラムマッピング生成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static ColumnMapping[] GenerateColmunMappings<T>(string[] columns)
        {
            var mappings = new List<ColumnMapping>();
            var modelProperties = ModelProperties.GetCachedProperties(typeof(T)).OfType<PropertyDescriptor>();
            for (var index = 0; index < columns.Length; index++)
            {
                // プロパティ名は勝手に解決するからね
                var map = new ColumnMapping
                {
                    Index = index,
                    ModelType = typeof(T),
                    HeaderName = columns[index]
                };

                // プロパティの取得(無きゃないでいいす)
                map.PropertyDescriptor = modelProperties.FirstOrDefault(pd => pd.Name.Equals(map.PropertyName, StringComparison.InvariantCultureIgnoreCase));

                // なくてもNull入れとく。マーキング。
                mappings.Add(map);
            }

            return mappings.ToArray();
        }

        public static IList<T> Parse<T>(Stream stream) where T : ModelBase, new()
        {
            // デフォShift_JISね。
            // テストの時は指定するやつ使おうね
            return Parse<T>(stream, Encoding.GetEncoding("Shift_JIS"));
        }

        public static IList<T> Parse<T>(Stream stream, Encoding encoding, Action<T> action = null) where T : ModelBase, new()
        {
            string text;

            using (var reader = new StreamReader(stream, encoding))
            {
                text = reader.ReadToEnd();
            }

            return Parse<T>(text, action);
        }
        
        public static IList<T> Parse<T>(string text, Action<T> action = null) where T : ModelBase, new()
        {

            var lineNumber = 0;
            var mappings = new ColumnMapping[] { };
            var result = new List<T>();
            foreach (var line in LoadLine(text))
            {
                var rowErrors = new List<string>();
                var columns = ParserHelper.SplitLine(line, SplitChar);
                lineNumber++;

                if (lineNumber == 1)
                {
                    // 1行目のヘッダからマッピングカラム判定
                    mappings = GenerateColmunMappings<T>(columns);
                    continue;
                }

                var model = new T() { IsValid = false, LineNumber = lineNumber - 1 };
                result.Add(model);

                // マッピングと数が一致してない行はエラー
                if (columns.Length != mappings.Length)
                {
                    rowErrors.Add("ヘッダのカラム数と一致しません");
                    continue;
                }

                // カラム値をマッピング
                foreach (var map in mappings)
                {
                    var value = columns[map.Index];
                    // 無効カラムはスキップ
                    if (!map.IsValid)
                        continue;

                    // 値の正規化
                    value = ParserHelper.NormalizeString(map.PropertyDescriptor, value);

                    // 値の型変換可能かチェック
                    if (!ParserHelper.IsValidValue(map.PropertyDescriptor, value))
                    {
                        rowErrors.Add("カラムの型が適切ではありません(" + map.HeaderName + ")");
                    }
                    else
                    {
                        map.PropertyDescriptor.SetValue(model, ParserHelper.ConvertFromString(map.PropertyDescriptor, value));
                    }
                }

                // DataAnnotationでモデル検証
                var validationResults = new List<ValidationResult>();
                ModelValidator.Validation(model, validationResults);
                rowErrors.AddRange(validationResults.Select(vr => vr.ErrorMessage));

                model.IsValid = !validationResults.Any();
                if (!model.IsValid)
                {
                    model.ErrorMessage = string.Join("\n", validationResults.Select(vr => vr.ErrorMessage));
                }

                // なんかやりたかったらどうぞ
                if (action != null)
                {
                    action(model);
                }
            }

            return result;
        }
    }
}
