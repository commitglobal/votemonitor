# Forms
## Question types
Questions are represented using polymorphic json and the type is specified in the `$questionType` property.

### Numeric input question (`$questionType: "numberInputQuestion"`)
Allows users to input numbers (integer only ?)

### Text input question (`$questionType: "textInputQuestion"`)
Allows users to input texts (what is the limit of 2048 chars?)

### Date input question (`$questionType: "dateInputQuestion"`)
Allows users to input date and time

### Single select questions (`$questionType: "singleSelectQuestion"`)
Question with a list of options where the user can select only a single option (can the user select none ?). There can be options where users can provide their input (what is the limit of 2048 chars?)

### Multi-select questions (`$questionType: "multiSelectQuestion"`)
Question with a list of options where the user can select multiple (can the user select none ?). There can be options where users can provide their input (what is the limit of 2048 chars?)

### Rating questions (`$questionType: "ratingQuestion"`)
Question where the user can rate something on a scale.

Rating scale options:
* from 1 to 3
* from 1 to 4
* from 1 to 5
* from 1 to 6
* from 1 to 7
* from 1 to 8
* from 1 to 9
* from 1 to 10

## Handling multi-language forms
Each form states a list of ISO 639-1 code of supported languages.

```json
  {
  "id": "1d965966-8349-4d38-b2ab-9459c2ea1ded",
  "code": "A-Form",
  "languages": [
    "RO",
    "EN"
  ]
}
```

## Form statuses
A form can be in one of the following statuses:
* **Drafted** - the form was just created and its content is being edited
* **Published** - the form is in use.
* **Retired** - the form was in use and it has submitted answers but a new version of it was being published.

### Example
An example of a form with all the question types
```json
{
    "id": "1d965966-8349-4d38-b2ab-9459c2ea1ded",
    "code": "A-Form",
    "name": {
        "EN": "test form",
        "RO": "formular de test"
    },
    "status": "Published",
    "createdOn": "2024-03-07T08:57:49.225314Z",
    "lastModifiedOn": "2024-03-07T08:58:27.048574Z",
    "sections": [
        {
            "id": "2e22dcf4-b1c6-4acf-a885-366a27b54c0c",
            "code": "A",
            "title": {
                "EN": "a section",
                "RO": "o sectiune"
            },
            "questions": [
                {
                    "id": "3f38e125-8b63-4224-8962-e3200547ddeb",
                    "code": "A1",
                    "inputPlaceholder": {
                        "EN": "number",
                        "RO": "numar"
                    },
                    "$questionType": "numberInputQuestion",
                    "text": {
                        "EN": "How many PEC members have been appointed",
                        "RO": "Câți membri PEC au fost numiți"
                    },
                    "helptext": {
                        "EN": "Please enter a number",
                        "RO": "Vă rugăm să introduceți numărul dvs"
                    }
                },
                {
                    "id": "505f5aac-8052-4c6a-b22b-ad0719695122",
                    "code": "A2",
                    "inputPlaceholder": {
                        "EN": "mood",
                        "RO": "dispozitie"
                    },
                    "$questionType": "textInputQuestion",
                    "text": {
                        "EN": "How are you today",
                        "RO": "Cum te simți azi"
                    },
                    "helptext": {
                        "EN": "Please enter how are you",
                        "RO": "Vă rugăm să introduceți cum sunteți"
                    }
                },
                {
                    "id": "7472b57c-6180-44c4-bb83-afed17cea6da",
                    "code": "A3",
                    "$questionType": "dateInputQuestion",
                    "text": {
                        "EN": "Time of arrival",
                        "RO": "Timpul sosirii"
                    },
                    "helptext": {
                        "EN": "Please enter exact hour when did you arrive",
                        "RO": "Vă rugăm să introduceți ora exactă când ați sosit"
                    }
                }
            ]
        },
        {
            "id": "467e9b3e-3d00-4c55-92ba-026213656b41",
            "code": "B",
            "title": {
                "EN": "another section",
                "RO": "inca o sectiune"
            },
            "questions": [
                {
                    "id": "fe206677-0a2c-412c-850c-6947aff932b4",
                    "code": "B1",
                    "options": [
                        {
                            "id": "b1520a73-44ee-41be-96ce-b4f0f73e1bef",
                            "text": {
                                "EN": "Very good",
                                "RO": "Foarte bun"
                            },
                            "isFlagged": false,
                            "isFreeText": false
                        },
                        {
                            "id": "6792bd15-20e6-4854-a364-bd3b35d495bd",
                            "text": {
                                "EN": "Good",
                                "RO": "bun"
                            },
                            "isFlagged": false,
                            "isFreeText": false
                        },
                        {
                            "id": "2ea802b6-8367-4fe6-beae-024be27e9efc",
                            "text": {
                                "EN": "Bad",
                                "RO": "Rea"
                            },
                            "isFlagged": false,
                            "isFreeText": false
                        },
                        {
                            "id": "c98077db-947f-4bd5-965c-88bc4b3c9a6d",
                            "text": {
                                "EN": "Very bad",
                                "RO": "Foarte rea"
                            },
                            "isFlagged": true,
                            "isFreeText": false
                        },
                        {
                            "id": "a2b0d4d5-595d-454a-9b83-207a66cb5886",
                            "text": {
                                "EN": "Other",
                                "RO": "Alta"
                            },
                            "isFlagged": true,
                            "isFreeText": true
                        }
                    ],
                    "$questionType": "singleSelectQuestion",
                    "text": {
                        "EN": "The overall conduct of the opening of this PS was:",
                        "RO": "Conducerea generală a deschiderii acestui PS a fost:"
                    },
                    "helptext": {
                        "EN": "Please select a single option",
                        "RO": "Vă rugăm să selectați o singură opțiune"
                    }
                },
                {
                    "id": "4e8207bf-d428-4901-a7f2-22483ec1bf54",
                    "code": "B2",
                    "options": [
                        {
                            "id": "5db7b450-5d26-4983-9a42-e9cf23e36e38",
                            "text": {
                                "EN": "Bloc 1",
                                "RO": "Bloc 1"
                            },
                            "isFlagged": false,
                            "isFreeText": false
                        },
                        {
                            "id": "d80664a1-9f17-4378-ac95-f2550442ff38",
                            "text": {
                                "EN": "Bloc 2",
                                "RO": "Bloc 2"
                            },
                            "isFlagged": false,
                            "isFreeText": false
                        },
                        {
                            "id": "c681399b-24b3-4664-8dd1-fc916c3f4025",
                            "text": {
                                "EN": "Bloc 3",
                                "RO": "Bloc 3"
                            },
                            "isFlagged": false,
                            "isFreeText": false
                        },
                        {
                            "id": "9ef6560b-0555-4ad8-aedc-d3da55b5da86",
                            "text": {
                                "EN": "Party 1",
                                "RO": "Party 1"
                            },
                            "isFlagged": true,
                            "isFreeText": false
                        },
                        {
                            "id": "d8af023b-71ec-4fcb-baca-235fdd86e0e4",
                            "text": {
                                "EN": "Other",
                                "RO": "Other"
                            },
                            "isFlagged": true,
                            "isFreeText": true
                        }
                    ],
                    "$questionType": "multiSelectQuestion",
                    "text": {
                        "EN": "What party/bloc proxies were present at the opening of this PS",
                        "RO": "Ce împuterniciri de partid/bloc au fost prezenți la deschiderea acestui PS"
                    },
                    "helptext": {
                        "EN": "Please select as many you want",
                        "RO": "Vă rugăm să selectați câte doriți"
                    }
                }
            ]
        },
        {
            "id": "e8786365-3345-4d1e-9ba1-1a5a3332aafc",
            "code": "C",
            "title": {
                "EN": "The last section",
                "RO": "Ultima sectiune"
            },
            "questions": [
                {
                    "id": "838450ce-e088-4dd1-b76d-4c0951269f89",
                    "code": "C1",
                    "scale": "OneTo10",
                    "$questionType": "ratingQuestion",
                    "text": {
                        "EN": "Please rate this form",
                        "RO": "Vă rugăm să evaluați acest formular"
                    },
                    "helptext": {
                        "EN": "Please give us a rating",
                        "RO": "Vă rugăm să ne dați o evaluare"
                    }
                }
            ]
        }
    ],
    "languages": [
        "RO",
        "EN"
    ]
}
```