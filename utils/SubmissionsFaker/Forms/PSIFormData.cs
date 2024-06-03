using SubmissionsFaker.Clients.Models.Questions;
using SubmissionsFaker.Clients.PlatformAdmin.Models;

namespace SubmissionsFaker.Forms;

public class PSIFormData
{
    public static UpsertPSIFormRequest PSIForm = new()
    {
        DefaultLanguage = "SR",
        Languages = new List<string> { "SR" },
        Questions =
        [
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "1",
                Text = new()
                {
                    { "SR", "Biračko mesto je:" }
                },
                Options =
                [
                    new SelectOptionRequest()
                    {
                        Id = Guid.NewGuid(),
                        Text = new()
                        {
                            { "SR", "Urbano" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new SelectOptionRequest
                    {
                        Id = Guid.NewGuid(),
                        Text = new()
                        {
                            { "SR", "Ruralno" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext = new()
                {
                    { "SR", "" }
                },
            },
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "2",
                Text =new()
                {
                    { "SR", "Biračko mesto je:" }
                },
                Options =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Osnovna/srednja škola" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Mesna zajednica" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Gerontološki centar/zdravstvena ustanova" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Javna ustanova" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext = new()
                {
                    { "SR", "" }
                }
            },
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "3",
                Text =new()
                {
                    { "SR", "Predsednik biračkog odbora je kog pola:" }
                },
                Options =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Muški" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ženski" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext =new()
                {
                    { "SR", "" }
                },
            },
            new TextQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "4",
                Text =new()
                {
                    { "SR", "Koliko je članova stalnog sastava biračkog odbora:" }
                },
                Helptext =new()
                {
                    { "SR", "" }
                },

                InputPlaceholder =new()
                {
                    {
                        "SR", "Unesite broj"
                    }
                }
            },
            new NumberQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "5",
                Text =new()
                {
                    { "SR", "Koliko članova stalnog sastava biračkog odbora su žene:" }
                },
                Helptext =new()
                {
                    { "SR", "" }
                },


                InputPlaceholder =new()
                {
                    {
                        "SR", "Unesite broj"
                    }
                }
            },
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "6",
                Text =new()
                {
                    {
                        "SR",
                        "Da li su predstavnici svih izbornih lista bili prisutni tokom otvaranja ovog biračkog mesta?"
                    }
                },
                Options =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Da" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ne" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ne znam" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext =new()
                {
                    { "SR", "" }
                },
            },
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "7",
                Text =new()
                {
                    { "SR", "Da li su tokom otvaranja ovog biračkog mesta bila prisutna neovlašćena lica:" }
                },
                Options =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Da" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ne" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ne znam" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext =new()
                {
                    { "SR", "" }
                },
            },
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "8",
                Text =new()
                {
                    {
                        "SR",
                        "Da li su ima drugih domaći nestranačkih posmatrača koji prate glasanje na ovom biračkom mestu?"
                    }
                },
                Options =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            {
                                "SR", "Da"
                            }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ne" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ne znam" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext =new()
                {
                    { "SR", "" }
                },
            },
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "9",
                Text =new()
                {
                    { "SR", "Da li ste na bilo koji način sprečeni da posmatrate?" }
                },
                Options =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Da" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Ne" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext =new()
                {
                    { "SR", "" }
                },
            },
            new SingleSelectQuestionRequest()
            {
                Id = Guid.NewGuid(),
                Code = "10",
                Text =new()
                {
                    { "SR", "Celokupno sprovođenje glasanja ne biračkom mestu je:" }
                },
                Options =
                [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Veoma dobro" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", " Dobro" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", " Dobro" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Text =new()
                        {
                            { "SR", "Veoma loše" }
                        },
                        IsFlagged = false,
                        IsFreeText = false
                    }
                ],
                Helptext =new()
                {
                    { "SR", "" }
                },
            }
        ]
    };
}