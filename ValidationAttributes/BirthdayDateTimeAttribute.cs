using System;
using System.ComponentModel.DataAnnotations;

namespace ToqueToqueApi.ValidationAttributes
{
    public class BirthdayDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            var date = ((DateTime) value).Date;
            var now = DateTime.Now;
            if (DateTime.Compare(date, now) < 0 || date.Year < 1900)
                return ValidationResult.Success;

            return new ValidationResult("The date should be between 1900/01/01 and right now.");
        }
    }
}