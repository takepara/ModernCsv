using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernCsv
{
    public class ModelValidator
    {
        public static void Validation(object model, List<ValidationResult> invalids)
        {
            Validation(null, model, invalids, false);
        }

        public static void Validation(string modelName, object model, List<ValidationResult> invalids, bool hasModelPrefix = true)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);

            invalids.AddRange(
                validationResults.Select(_ =>
                    new ValidationResult(
                        hasModelPrefix
                        ? string.Format("{0}.", modelName) + string.Join(".", _.MemberNames) + "." + _.ErrorMessage
                        : _.ErrorMessage,
                        _.MemberNames)
            ));
        }
    }
}
