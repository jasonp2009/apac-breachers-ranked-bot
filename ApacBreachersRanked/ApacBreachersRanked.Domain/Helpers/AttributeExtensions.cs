namespace ApacBreachersRanked.Domain.Helpers
{
    public static class AttributeExtensions
    {
        public static TAttribute GetAttributeOfType<TAttribute>(this object val)
            where TAttribute : Attribute
        {
            var type = val.GetType();
            var memInfo = type.GetMember(val.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(TAttribute), false);
            return (attributes.Length > 0) ? (TAttribute)attributes[0] : null;
        }

        public static Dictionary<TEnum, TAttribute> GetEnumValuesWithAttribute<TEnum, TAttribute>()
            where TEnum : Enum
            where TAttribute : Attribute
        {
            Dictionary<TEnum, TAttribute> result = new();
            foreach (TEnum enumVal in Enum.GetValues(typeof(TEnum)))
            {
                result.Add(enumVal, enumVal.GetAttributeOfType<TAttribute>());
            }
            return result;
        }
    }
}
