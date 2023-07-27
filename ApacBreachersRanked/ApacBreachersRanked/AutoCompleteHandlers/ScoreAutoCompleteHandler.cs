using Discord;
using Discord.Interactions;

namespace ApacBreachersRanked.AutoCompleteHandlers
{
    public class ScoreAutoCompleteHandler : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            long? otherScore = autocompleteInteraction.Data.Options
                .SingleOrDefault(x => x.Type == ApplicationCommandOptionType.Integer && !x.Focused)?.Value as long?;

            string? curValueString = autocompleteInteraction.Data.Options.Single(x => x.Focused).Value as string;
            long? curValue = string.IsNullOrWhiteSpace(curValueString) ? null : Convert.ToInt64(curValueString);

            IEnumerable<AutocompleteResult>? validOptions = null;

            if (otherScore == null)
            {
                validOptions = Enumerable.Range(0, 8).Select(x => new AutocompleteResult(x.ToString(), x));
            } else if (0 <= otherScore && otherScore < 7)
            {
                validOptions = new List<AutocompleteResult> { new AutocompleteResult("7", 7) };
            } else if (otherScore == 7)
            {
                validOptions = Enumerable.Range(0, 7).Select(x => new AutocompleteResult(x.ToString(), x));
            }

            if (validOptions == null ||
                (curValue != null && !validOptions.Select(x => Convert.ToInt64(x.Value)).ToList().Contains(curValue.Value)))
            {
                return AutocompletionResult.FromError(InteractionCommandError.BadArgs, "Invalid score");
            }

            if (curValue != null)
            {
                return AutocompletionResult.FromSuccess(new List<AutocompleteResult> { new AutocompleteResult(curValue.ToString(), curValue) });
            }

            return AutocompletionResult.FromSuccess(validOptions.ToList());
        }
    }
}
