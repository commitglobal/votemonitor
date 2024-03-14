using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Vote.Monitor.Core.Validators;

public static class FileSizeValidatorExtension
{
    public static IRuleBuilderOptions<T, IFormFile> FileSmallerThan<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, uint max)
        => ruleBuilder.SetValidator(new FileSizeValidator<T>(max));
}
