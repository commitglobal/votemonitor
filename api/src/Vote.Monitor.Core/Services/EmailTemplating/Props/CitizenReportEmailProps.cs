namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public abstract record BaseAnswerFragmentProps(string Text);

public record InputAnswerFragmentProps(string Text, string Answer) : BaseAnswerFragmentProps(Text);

public record RatingAnswerFragmentProps(string Text, int Scale, int Value)
    : BaseAnswerFragmentProps(Text);

public record SelectAnswerOptionFragmentProps(string Text, bool IsSelected);

public record SelectAnswerFragmentProps(string Text, IEnumerable<SelectAnswerOptionFragmentProps> Options)
    : BaseAnswerFragmentProps(Text);

public record CitizenReportEmailProps(
    string Title,
    string Heading,
    string Preview,
    IEnumerable<BaseAnswerFragmentProps> Answers,
    string CdnUrl)
    : BaseEmailProps(CdnUrl);