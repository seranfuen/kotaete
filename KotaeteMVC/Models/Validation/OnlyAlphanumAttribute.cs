﻿using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KotaeteMVC.Models.Validation
{
    public class OnlyAlphanumAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valueStr = value as string;
            if (value != null)
            {
                var regex = new Regex("^[a-zA-Z0-9]*$");
                if (regex.IsMatch(valueStr) == false)
                {
                    return new ValidationResult("The field must be alphanumeric (only contain letters and numbers)");
                }
            }
            return ValidationResult.Success;
        }
    }
}