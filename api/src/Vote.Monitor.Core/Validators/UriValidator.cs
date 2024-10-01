using FluentValidation.Validators;

namespace Vote.Monitor.Core.Validators;

public class UriValidator<T> : PropertyValidator<T, string>
{
    public override bool IsValid(FluentValidation.ValidationContext<T> context, string link)
    {
        if (string.IsNullOrWhiteSpace(link)) return false;
      
        //Courtesy of @Pure.Krome's comment and https://stackoverflow.com/a/25654227/563532
        return Uri.TryCreate(link, UriKind.Absolute, out var outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }

    public override string Name => "UriValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "Provided string is not a valid URI";
}