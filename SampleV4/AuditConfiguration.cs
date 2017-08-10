namespace SampleV4
{
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;

    public class AuditConfiguration : IProvideConfiguration<AuditConfig>
    {
        public AuditConfig GetConfiguration()
        {
            return new AuditConfig{ QueueName = "audit" };
        }
    }
}