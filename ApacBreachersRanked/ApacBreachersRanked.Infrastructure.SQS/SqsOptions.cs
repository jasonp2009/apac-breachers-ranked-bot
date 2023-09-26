namespace ApacBreachersRanked.Infrastructure.SQS
{
    internal class SqsOptions
    {

        public static string Key = "SqsOptions";
        public string AccessKey { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public string Region { get; set; } = null!;
        public string QueueUrl { get; set; } = null!;
        public int MaxMessages { get; set; } = 20;
        public int WaitTime { get; set; } = 20000;
    }
}
