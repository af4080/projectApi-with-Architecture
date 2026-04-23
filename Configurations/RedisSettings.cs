namespace projectApiAngular.Configurations
{
    public class RedisSettings
    {
        public required string ConnectionString { get; set; }
        public int GiftCacheTtlSeconds { get; set; }
    }
}
