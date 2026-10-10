namespace Vote.Monitor.Core.RulesEngine.Rules;
// The advanced filter rules the browser sends in the `where` parameter, as C# records.
// Contract: docs/advanced-filters-backend.md; the browser side is src/components/query-builder/query-json.ts.
//
//   Every item of the tree is a rule, discriminated by "op" — the logical operations (and / or /
//   not) are rules like the rest:
//     { "op": "and", "values": [ …rules… ] } | { "op": "or", "values": [ …rules… ] }
//     { "op": "not", "values": [ <a group rule> ] }
//   A rule's value keys follow how many values it takes:
//     "value"            one value       { "field": "hasFlaggedAnswers", "op": "eq", "value": true }
//     "values"           a list          { "field": "followUpStatus", "op": "in", "values": ["NeedsFollowUp"] }
//     "from" and "to"    a range         { "field": "submittedAt", "op": "between", "from": …, "to": … }
//     none                               { "field": "notes", "op": "isEmpty" }
//   Answer rules carry no "field", only "form" and "question": { "op": "answer-eq", "form": …, "question": …, "value": "2" }.
//
//   Field names a rule can carry (the full list, by report):
//     submissions, incidents, citizen:
//       followUpStatus (NeedsFollowUp | Resolved | NotApplicable)   submittedAt (UTC ISO 8601)
//       hasFlaggedAnswers | hasNotes | hasComments | hasAttachments   (boolean)
//       questionsAnswered (All | Some | None)
//       dataSource (Ngo | Coalition, coalition leaders only)
//       formId (the report's own forms: submissions every form, incidents only incident forms,
//               citizen only citizen-reporting forms)
//     submissions, incidents: monitoringObserverId, tags, level1..level5, pollingStationNumber
//     submissions only: formType (Opening | Voting | ClosingAndCounting | PSI | IncidentReporting | Other)
//     incidents only: locationType (PollingStation | OtherLocation)
//     citizen only: level1..level5 (from the locations tree)
//
// Uses the backend's PolyJson package (as BaseQuestion does).

/// <summary>Every <c>op</c> value the browser can send; also the polymorphic discriminator.</summary>
public static class FilterOps
{
    // Logical operations: rules like any other, their children under "values".
    public const string And = "and";
    public const string Or = "or";
    public const string Not = "not";

    // Rules about a field: { "field": …, "op": …, "value" | "values" | "from"/"to" }
    public const string Eq = "eq"; // is
    public const string Ne = "ne"; // is not
    public const string Lt = "lt"; // less than (number, or ISO 8601 UTC for dates)
    public const string Lte = "lte"; // at most
    public const string Gt = "gt"; // greater than
    public const string Gte = "gte"; // at least
    public const string Between = "between"; // from … to, both ends included; "from" and "to", an unset end is null
    public const string In = "in"; // is any of, "values" (array)
    public const string NotIn = "notIn"; // is none of, "values" (array)
    public const string Contains = "contains"; // text contains (matching is the server's choice)
    public const string NotContains = "notContains"; // text does not contain
    public const string IsEmpty = "isEmpty"; // has no value, no value keys
    public const string IsNotEmpty = "isNotEmpty"; // has a value, no value keys

    // Rules about an answer: { "op": …, "form": …, "question": …, "value" | "values" | "from"/"to" },
    // no field. The operators follow the question type (docs/advanced-filters-backend.md, section 2);
    // every question type also offers AnswerIsEmpty / AnswerIsNotEmpty — no question is mandatory.
    public const string AnswerEq = "answer-eq";
    public const string AnswerNe = "answer-ne";
    public const string AnswerLt = "answer-lt";
    public const string AnswerLte = "answer-lte";
    public const string AnswerGt = "answer-gt";
    public const string AnswerGte = "answer-gte";
    public const string AnswerBetween = "answer-between";
    public const string AnswerIn = "answer-in";
    public const string AnswerNotIn = "answer-notIn";
    public const string AnswerContains = "answer-contains";
    public const string AnswerNotContains = "answer-notContains";
    public const string AnswerIsEmpty = "answer-isEmpty";
    public const string AnswerIsNotEmpty = "answer-isNotEmpty";
}

// --- Logical operations -----------------------------------------------------------------------

// --- Rules about a field ----------------------------------------------------------------------

// --- Rules about an answer -------------------------------------------------------------------
// The value shapes match the question type: text takes strings, rating the numbers "1" to "N",
// number a number or numeric string, date "YYYY-MM-DD", single choice one option id,
// multi-select option ids ("eq" with an array means exactly that set, sent as "values").
// Every question type can also send AnswerIsEmptyFilter / AnswerIsNotEmptyFilter (no question is
// mandatory, so the answer can be missing); those two carry no value keys, like their field versions.

// --- Reading and writing ---------------------------------------------------------------------
