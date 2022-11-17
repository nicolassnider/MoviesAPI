using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class FileWeightValidation : ValidationAttribute
    {
        private int maxWeightInMB;
        public FileWeightValidation(int MaxWeightInMB)
        {
            maxWeightInMB = MaxWeightInMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            IFormFile formFile = value as IFormFile;
            if (formFile == null) return ValidationResult.Success;
            if (formFile.Length > maxWeightInMB * 1024 * 1024)
                return new ValidationResult($"The image weight can't be greater than {maxWeightInMB} MB");
            return ValidationResult.Success;
        }
    }
}
