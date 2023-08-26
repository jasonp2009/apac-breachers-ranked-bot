namespace ApacBreachersRanked.Domain.Helpers
{
    public static class EnumExtensions
    {
        public static string GetName<TEnum>(this TEnum enumVal)
        {
            return Enum.GetName(typeof(TEnum), enumVal)!;
        }
    }
}
