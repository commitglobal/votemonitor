using FluentValidation.Validators;
using Humanizer;
using Microsoft.AspNetCore.Http;

namespace Vote.Monitor.Core.Validation;

public class FileSizeValidator<T> : PropertyValidator<T, IFormFile>
{
    public uint MaxFileSizeInBytes { get; }

    public FileSizeValidator(uint maxFileSizeInBytes) => MaxFileSizeInBytes = maxFileSizeInBytes;

    public override bool IsValid(FluentValidation.ValidationContext<T> context, IFormFile value)
    {
        if (value is null) return true;
        if (value.Length <= MaxFileSizeInBytes) return true;
        context.MessageFormatter
            .AppendArgument("MaxFilesize", MaxFileSizeInBytes.Megabytes());

        return false;
    }

    public override string Name => "FileSizeValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "Maximum file size is {MaxFilesize}.";
}
