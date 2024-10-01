using FluentValidation;

namespace Vote.Monitor.Core.Validators;

public static class UriValidatorExtension
{
    public static IRuleBuilderOptions<T, string> IsValidUri<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new UriValidator<T>());
}