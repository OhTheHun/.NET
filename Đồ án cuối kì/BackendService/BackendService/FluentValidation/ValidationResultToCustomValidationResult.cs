using FluentValidation.Results;

namespace BackendService.Common
{
    public static class ValidationResultToCustomValidationResult
    {
        public static object Map(List<ValidationFailure> errors)
        {
            return new
            {
                errors = errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            };
        }
    }
}