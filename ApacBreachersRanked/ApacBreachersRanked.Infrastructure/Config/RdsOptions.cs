namespace ApacBreachersRanked.Infrastructure.Config
{
    public class RdsOptions
    {
        public static string Key = "RdsOptions";
        public DatabaseEngine DatabaseEngine { get; set; } = DatabaseEngine.SqlServer;
        public string UserName { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string HostName { get; init; } = null!;
        public string DbName { get; init; } = null!;
        internal string ConnectionString =>
            DatabaseEngine switch
            {
                DatabaseEngine.SqlServer => $"Data Source={HostName};Database={DbName};User ID={UserName};Password={Password};",
                DatabaseEngine.Postgress => $"Host={HostName};Database={DbName};Username={UserName};Password={Password}",
                _ => throw new NotImplementedException($"Database engine {DatabaseEngine} has not been configured")
            };
    }
}
