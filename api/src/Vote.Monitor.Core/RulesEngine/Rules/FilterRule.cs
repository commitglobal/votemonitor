using PolyJson;

namespace Vote.Monitor.Core.RulesEngine.Rules;

/// <summary>One rule, by its <c>op</c>: a logical, field or answer rule.</summary>
[PolyJsonConverter(distriminatorPropertyName: "op")]
[PolyJsonConverter.SubType(typeof(AndFilter), FilterOps.And)]
[PolyJsonConverter.SubType(typeof(OrFilter), FilterOps.Or)]
[PolyJsonConverter.SubType(typeof(NotFilter), FilterOps.Not)]
[PolyJsonConverter.SubType(typeof(EqFilter), FilterOps.Eq)]
[PolyJsonConverter.SubType(typeof(NeFilter), FilterOps.Ne)]
[PolyJsonConverter.SubType(typeof(LtFilter), FilterOps.Lt)]
[PolyJsonConverter.SubType(typeof(LteFilter), FilterOps.Lte)]
[PolyJsonConverter.SubType(typeof(GtFilter), FilterOps.Gt)]
[PolyJsonConverter.SubType(typeof(GteFilter), FilterOps.Gte)]
[PolyJsonConverter.SubType(typeof(BetweenFilter), FilterOps.Between)]
[PolyJsonConverter.SubType(typeof(InFilter), FilterOps.In)]
[PolyJsonConverter.SubType(typeof(NotInFilter), FilterOps.NotIn)]
[PolyJsonConverter.SubType(typeof(ContainsFilter), FilterOps.Contains)]
[PolyJsonConverter.SubType(typeof(NotContainsFilter), FilterOps.NotContains)]
[PolyJsonConverter.SubType(typeof(IsEmptyFilter), FilterOps.IsEmpty)]
[PolyJsonConverter.SubType(typeof(IsNotEmptyFilter), FilterOps.IsNotEmpty)]
[PolyJsonConverter.SubType(typeof(AnswerEqFilter), FilterOps.AnswerEq)]
[PolyJsonConverter.SubType(typeof(AnswerNeFilter), FilterOps.AnswerNe)]
[PolyJsonConverter.SubType(typeof(AnswerLtFilter), FilterOps.AnswerLt)]
[PolyJsonConverter.SubType(typeof(AnswerLteFilter), FilterOps.AnswerLte)]
[PolyJsonConverter.SubType(typeof(AnswerGtFilter), FilterOps.AnswerGt)]
[PolyJsonConverter.SubType(typeof(AnswerGteFilter), FilterOps.AnswerGte)]
[PolyJsonConverter.SubType(typeof(AnswerBetweenFilter), FilterOps.AnswerBetween)]
[PolyJsonConverter.SubType(typeof(AnswerInFilter), FilterOps.AnswerIn)]
[PolyJsonConverter.SubType(typeof(AnswerNotInFilter), FilterOps.AnswerNotIn)]
[PolyJsonConverter.SubType(typeof(AnswerContainsFilter), FilterOps.AnswerContains)]
[PolyJsonConverter.SubType(typeof(AnswerNotContainsFilter), FilterOps.AnswerNotContains)]
[PolyJsonConverter.SubType(typeof(AnswerIsEmptyFilter), FilterOps.AnswerIsEmpty)]
[PolyJsonConverter.SubType(typeof(AnswerIsNotEmptyFilter), FilterOps.AnswerIsNotEmpty)]
public abstract record FilterRule
{
    [JsonPropertyName("op")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType())!;
}
