using System.Globalization;
using Microsoft.Recognizers.Text.DateTime;

namespace ApacBreachersRanked.Application.Common.Services;

public class DateTimeParser
{
    public async IAsyncEnumerable<DateTime> Parse(string dateTimeString, TimeZoneInfo timeZone)
    {
        var refTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        var results = DateTimeRecognizer.RecognizeDateTime(dateTimeString, CultureInfo.CurrentCulture.ToString(), refTime: refTime);
        List<DateTime> moments = new List<DateTime>();
        var result = results.FirstOrDefault();
        if (result == null) yield break;
        var resolutionValues = (IList<Dictionary<string, string>>)result.Resolution["values"];
        var resultTimes = resolutionValues.Select(v => DateTime.Parse(v["value"])).ToList();
        foreach (var moment in resultTimes)
        {
            var convertedMoment = TimeZoneInfo.ConvertTimeToUtc(moment, timeZone);
            if (moment > DateTime.UtcNow)
            {
                yield return convertedMoment;
            }
        }
    } 
}
