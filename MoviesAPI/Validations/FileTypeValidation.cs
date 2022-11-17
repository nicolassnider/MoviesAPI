using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class FileTypeValidation:ValidationAttribute
    {
        private readonly string[] validTypes;
        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }
        public FileTypeValidation(FileTypeGroup fileTypeGroup)
        {
            if (fileTypeGroup==FileTypeGroup.Image)
            {
                validTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            IFormFile formFile = value as IFormFile;
            if (formFile == null) return ValidationResult.Success;
            if (!validTypes.Contains(formFile.ContentType))
                return new ValidationResult($"File type is not valid. Only {string.Join(", ", validTypes)} are allowed");
            return ValidationResult.Success;
        }
    }
}
