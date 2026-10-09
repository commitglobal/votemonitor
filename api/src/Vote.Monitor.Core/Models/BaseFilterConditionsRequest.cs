using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.Models;

public class BaseFilterConditionsRequest
{
    public FilterRule? Filter { get; set; }
}
