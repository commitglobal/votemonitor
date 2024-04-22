using FluentValidation;

namespace Vote.Monitor.Core.Validators;

public static class LanguageCodeValidatorExtension
{
    public static IRuleBuilderOptions<T, string> IsValidLanguageCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.SetValidator(new LanguageCodeValidator<T>());
}
