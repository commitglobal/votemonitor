# questions display logic
Some questions will be displayed based on user answers on previous questions.
Questions without display logic are always displayed.
Form question count increases when child question display logic is satisfied decreases otherwise.

# terminology:
- Parent question: question which answer will determine if a child question will be displayed
- Child question: question with display logic
- Display logic: condition based on which a question is displayed

# V1.0 
- Parent questions can be SingleSelectQuestion or MultiSelectQuestion
- Condition is only includes
- Child questions can have parent questions which are child question for another one 

# near future
- add support for `number` and `rating question`
- add support for other conditions: `less`, `less or equal`, `equals`, `greater or equal`, `greater`

# examples:
# case 1 
No display logic
```json
[
    {
        "Id": "uuid-question-1",
        "$questionType": "singleSelectQuestion",
        ...
        "Options": [
            {
                "Id": "uuid-option-1",
                ...
            }
            ...
        ],
        ...
    },
    {
        "Id": "uuid-question-2",
        "$questionType": "numberQuestion",
        "DisplayLogic": {
            "ParentQuestionId": "uuid-question-1",
            "Condition" : "includes",
            "Value": "uuid-option-1"
        }
        ...
    },
    ...
]
```
Form question flow:
 > **uuid-question-1** -> **uuid-question-2**-> ... **uuid-question-n**


# case 2
Display logic when parent question is a single select question
```json
[
    {
        "Id": "uuid-question-1",
        "$questionType": "singleSelectQuestion",
        ...
        "Options": [
            {
                "Id": "uuid-option-1",
                ...
            },
            {
                "Id": "uuid-option-2",
                ...
            },
            ...
        ],
        ...
    },
    {
        "Id": "uuid-question-2",
        "$questionType": "numberQuestion",
        "DisplayLogic": {
            "ParentQuestionId": "uuid-question-1",
            "Condition" : "includes",
            "Value": "uuid-option-1"
        }
        ...
    },
    ...
]
```

Form question flows:
* `uuid-option-2` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-3**-> ... **uuid-question-n**
* `uuid-option-1` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-2**-> **uuid-question-3**-> ... **uuid-question-n**

# case 3
Display logic when parent question is a multi select question
```json
[
    {
        "Id": "uuid-question-1",
        "$questionType": "multiSelectQuestion",
        ...
        "Options": [
            {
                "Id": "uuid-option-1",
                ...
            },
            {
                "Id": "uuid-option-2",
                ...
            },
            {
                "Id": "uuid-option-3",
                ...
            },
            ...
        ],
        ...
    },
    {
        "Id": "uuid-question-2",
        "$questionType": "numberQuestion",
        "DisplayLogic": {
            "ParentQuestionId": "uuid-question-1",
            "Condition" : "includes",
            "Value": "uuid-option-1"
        }
        ...
    },
    ...
]
```

Form question flows:
* `uuid-option-2` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-3**-> ... **uuid-question-n**
* `uuid-option-1` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-2**-> **uuid-question-3**-> ... **uuid-question-n**
* `uuid-option-1`, `uuid-option-3` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-2**-> **uuid-question-3**-> ... **uuid-question-n**


# case 3.1
Multiple questions with display logic with a multi select question as parent question
```json
[
    {
        "Id": "uuid-question-1",
        "$questionType": "multiSelectQuestion",
        ...
        "Options": [
            {
                "Id": "uuid-option-1",
                ...
            },
            {
                "Id": "uuid-option-2",
                ...
            },
            {
                "Id": "uuid-option-3",
                ...
            },
            ...
        ],
        ...
    },
    {
        "Id": "uuid-question-2",
        "$questionType": "numberQuestion",
        "DisplayLogic": {
            "ParentQuestionId": "uuid-question-1",
            "Condition" : "includes",
            "Value": "uuid-option-1"
        }
        ...
    },
    {
        "Id": "uuid-question-3",
        "$questionType": "numberQuestion",
        "DisplayLogic": {
            "ParentQuestionId": "uuid-question-1",
            "Condition" : "includes",
            "Value": "uuid-option-2"
        }
        ...
    },
    
    ...
]
```

Form question flows:
* `uuid-option-3` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-4**-> ... **uuid-question-n**
* `uuid-option-1` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-2**-> **uuid-question-3**-> ... **uuid-question-n**
* `uuid-option-2` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-3**-> ... **uuid-question-n**
* `uuid-option-1`, `uuid-option2` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-2**-> **uuid-question-3**-> ... **uuid-question-n**


# case 4
Display logic based on a child question value
```json
[
    {
        "Id": "uuid-question-1",
        "$questionType": "singleSelectQuestion",
        ...
        "Options": [
            {
                "Id": "uuid-option-1",
                ...
            },
            {
                "Id": "uuid-option-2",
                ...
            },
            ...
        ],
        ...
    },
    {
        "Id": "uuid-question-2",
        "$questionType": "singleSelectQuestion",
        "DisplayLogic": {
            "ParentQuestionId": "uuid-question-1",
            "Condition" : "includes",
            "Value": "uuid-option-1"
        }
        ...
        "Options": [
            {
                "Id": "uuid-option-1",
                ...
            },
            {
                "Id": "uuid-option-2",
                ...
            },
            ...
        ],
        ...
    },
    {
        "Id": "uuid-question-3",
        "$questionType": "numberQuestion",
        "DisplayLogic": {
            "ParentQuestionId": "uuid-question-2",
            "Condition" : "includes",
            "Value": "uuid-option-1"
        }
        ...
    },
    ...
]
```

Form question flows:
* `uuid-option-2` selected on `uuid-question-1` - **uuid-question-1** -> **uuid-question-4**-> ... **uuid-question-n**
* `uuid-option-1` selected on `uuid-question-1` and `uuid-option-2` selected on `uuid-question-2` - **uuid-question-1** -> **uuid-question-2**-> **uuid-question-4**-> ... **uuid-question-n**
* `uuid-option-1` selected on `uuid-question-1` and `uuid-option-1` selected on `uuid-question-2` - **uuid-question-1** -> **uuid-question-2**-> **uuid-question-3**-> ... **uuid-question-n**

