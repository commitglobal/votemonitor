using SubmissionsFaker.Clients.Models.Questions;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.PlatformAdmin.Models;

namespace SubmissionsFaker.Forms;

public class PSIFormData
{
    public static UpsertPSIFormRequest PSIForm = new()
    {
        DefaultLanguage = "RO",
        Languages = new List<string> { "RO", "EN" },
        Questions = [
        new NumberQuestionRequest
        {
            Id = Guid.NewGuid(),
            Code = "A1",
            Text = new TranslatedString
            {
                { "EN", "How many PEC members have been appointed" },
                { "RO", "Câți membri PEC au fost numiți" }
            },
            Helptext = new TranslatedString
            {
                { "EN", "Please enter a number" },
                { "RO", "Vă rugăm să introduceți numărul dvs" }
            },
            InputPlaceholder = new TranslatedString
            {
                { "EN", "number" },
                { "RO", "numar" }
            }
        },
            new TextQuestionRequest
            {
                Id = Guid.NewGuid(),
                Code = "A2",
                Text = new TranslatedString
            {
                { "EN", "How are you today" },
                { "RO", "Cum te simți azi" }
            },
                Helptext = new TranslatedString
            {
                { "EN", "Please enter how are you" },
                { "RO", "Vă rugăm să introduceți cum sunteți" }
            },
                InputPlaceholder = new TranslatedString
            {
                { "EN", "mood" },
                { "RO", "dispozitie" }
            }
            },
            new DateQuestionRequest
            {
                Id = Guid.NewGuid(),
                Code = "A3",
                Text = new TranslatedString
                {
                    { "EN", "Time of arrival" },
                    { "RO", "Timpul sosirii" }
                },
                Helptext = new TranslatedString
                {
                    { "EN", "Please enter exact hour when did you arrive" },
                    { "RO", "Vă rugăm să introduceți ora exactă când ați sosit" }
                }
            },
            new RatingQuestionRequest
            {
                Id = Guid.NewGuid(),
                Code = "C1",
                Text = new TranslatedString
                {
                    { "EN", "Please rate this form" },
                    { "RO", "Vă rugăm să evaluați acest formular" }
                },
                Helptext = new TranslatedString
                {
                    { "EN", "Please give us a rating" },
                    { "RO", "Vă rugăm să ne dați o evaluare" }
                },
                Scale = "OneTo10"
            },
            new SingleSelectQuestionRequest
            {
                Id = Guid.NewGuid(),
                Code = "B1",
                Text = new TranslatedString
                {
                    { "EN", "The overall conduct of the opening of this PS was:" },
                    { "RO", "Conducerea generală a deschiderii acestui PS a fost:" }
                },
                Helptext = new TranslatedString
                {
                    { "EN", "Please select a single option" },
                    { "RO", "Vă rugăm să selectați o singură opțiune" }
                },
                Options = new List<SelectOptionRequest>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Very good" },
                            { "RO", "Foarte bun" }
                        },
                        IsFreeText = false,
                        IsFlagged = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Good" },
                            { "RO", "bun" }
                        },
                        IsFreeText = false,
                        IsFlagged = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Bad" },
                            { "RO", "Rea" }
                        },
                        IsFreeText = false,
                        IsFlagged = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Very bad" },
                            { "RO", "Foarte rea" }
                        },
                        IsFreeText = false,
                        IsFlagged = true
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Other" },
                            { "RO", "Alta" }
                        },
                        IsFreeText = true,
                        IsFlagged = true
                    },
                    }

            },
            new MultiSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "B2",
                Text = new TranslatedString
                {
                    { "EN", "What party/bloc proxies were present at the opening of this PS" },
                    { "RO", "Ce împuterniciri de partid/bloc au fost prezenți la deschiderea acestui PS" }
                },
                Helptext = new TranslatedString
                {
                    { "EN", "Please select as many you want" },
                    { "RO", "Vă rugăm să selectați câte doriți" }
                },
                Options = new List<SelectOptionRequest>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Bloc 1" },
                            { "RO", "Bloc 1" }
                        },
                        IsFreeText = false,
                        IsFlagged = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Bloc 2" },
                            { "RO", "Bloc 2" }
                        },
                        IsFreeText = false,
                        IsFlagged = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Bloc 3" },
                            { "RO", "Bloc 3" }
                        },
                        IsFreeText = false,
                        IsFlagged = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Party 1" },
                            { "RO", "Party 1" }
                        },
                        IsFreeText = false,
                        IsFlagged = true
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text = new TranslatedString
                        {
                            { "EN", "Other" },
                            { "RO", "Other" }
                        },
                        IsFreeText = true,
                        IsFlagged = true
                    }
                }
            }
        ]
    };
}