using System.ComponentModel.DataAnnotations;

namespace backend_api.Services
{
    //Static class to validate all DTO response Data Annotations
    public static class ValidationDataAnnotations
    {
        //Function to validate all parameters of Data Annotations, if have error return the specific error in message
        public static IEnumerable<ValidationResult> Validate<T>(this T obj)
        {
            var context = new ValidationContext(obj!);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(obj!, context, results, validateAllProperties: true);

            return results;
        }

        //Function to validate all parameters of Data Annotation, and return true or false
        public static bool IsValid<T>(this T obj)
        {
            return !obj.Validate().Any();
        }
    }
}