using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validators;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Form.Module.Validators;

public class MatrixRowRequestValidator: Validator<MatrixRowRequest>
{
    public MatrixRowRequestValidator(List<string> languages)
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Text)
            .SetValidator(new PartiallyTranslatedStringValidator(languages));
    }
}
