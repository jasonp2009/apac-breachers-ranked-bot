using Discord;
using Discord.Interactions;
using TimeZoneConverter;

namespace ApacBreachersRanked.AutoCompleteHandlers;

public class TimeZoneAutoCompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        return AutocompletionResult.FromSuccess(
            TZConvert.KnownIanaTimeZoneNames
                .Where(x => x.Contains(autocompleteInteraction.Data.Current.Value as string, StringComparison.OrdinalIgnoreCase))
                .Select(x => new AutocompleteResult(x, x))
                .Take(25));
    }
}