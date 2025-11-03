
using FastEndpoints;
using FluentValidation;

namespace Feature.Form.Submission.SMS.Submit;
public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(r => r.PhoneNumber).NotEmpty();
        RuleFor(r => r.SmsMessage).NotEmpty();
    }
}
