namespace SampleV4
{
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;

    public class ErrorConfiguration : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    {
        public MessageForwardingInCaseOfFaultConfig GetConfiguration()
        {
            return new MessageForwardingInCaseOfFaultConfig { ErrorQueue = "error" };
        }
    }
}