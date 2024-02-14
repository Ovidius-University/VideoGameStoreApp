using System.ComponentModel.DataAnnotations;
namespace AppUI.Validators;
public class YearAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is short year)
        {
            if (year <= 0)
            {
                return new ValidationResult("Release year can not be zero or negative!");
            }
            else if (year > DateTime.Now.Year)
            {
                return new ValidationResult("Release year can not be from the future!");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Year is not valid!");
    }
}