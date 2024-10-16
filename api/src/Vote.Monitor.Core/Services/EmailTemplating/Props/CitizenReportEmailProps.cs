namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public abstract record BaseAnswerFragmentProps(
    string Text,
    string NotesListTitle,
    IEnumerable<NoteFragmentProps> Notes,
    string AttachmentsListTitle,
    IEnumerable<AttachmentFragmentProps> Attachments);

public record NoteFragmentProps(string Index, string Text);
public record AttachmentFragmentProps(string Title, string Url);

public record InputAnswerFragmentProps(
    string Text,
    string NotesListTitle,
    IEnumerable<NoteFragmentProps> Notes,
    string AttachmentsListTitle,
    IEnumerable<AttachmentFragmentProps> Attachments,
    string Answer) : BaseAnswerFragmentProps(Text, NotesListTitle, Notes, AttachmentsListTitle, Attachments);

public record RatingAnswerFragmentProps(
    string Text,
    string NotesListTitle,
    IEnumerable<NoteFragmentProps> Notes,
    string AttachmentsListTitle,
    IEnumerable<AttachmentFragmentProps> Attachments,
    int Scale,
    int Value)
    : BaseAnswerFragmentProps(Text, NotesListTitle, Notes, AttachmentsListTitle, Attachments);

public record SelectAnswerOptionFragmentProps(
    string Text,
    bool IsSelected);

public record SelectAnswerFragmentProps(
    string Text,
    string NotesListTitle,
    IEnumerable<NoteFragmentProps> Notes,
    string AttachmentsListTitle,
    IEnumerable<AttachmentFragmentProps> Attachments,
    IEnumerable<SelectAnswerOptionFragmentProps> Options)
    : BaseAnswerFragmentProps(Text, NotesListTitle, Notes, AttachmentsListTitle, Attachments);

public record CitizenReportEmailProps(
    string Title,
    string Heading,
    string Preview,
    IEnumerable<BaseAnswerFragmentProps> Answers,
    string CdnUrl)
    : BaseEmailProps(CdnUrl);