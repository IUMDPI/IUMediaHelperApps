using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Recorder
{
    public class BarcodeValidationRule: ValidationRule
    {
        private readonly Regex _barcodeExpression = new Regex(@"\d{14}");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = value as string;
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return new ValidationResult(false, "Invalid barcode");
            }

            return _barcodeExpression.IsMatch(stringValue) 
                ? new ValidationResult(true, null) 
                : new ValidationResult(false, $"{stringValue} is not a valid barcode value");
        }
    }
}
