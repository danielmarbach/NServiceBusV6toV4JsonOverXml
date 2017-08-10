namespace SampleV4
{
    using System;
    using NServiceBus;
    using NServiceBus.Installation.Environments;

    class Program
    {
        static void Main(string[] args)
        {
            Configure.Serialization.Xml();

            var configure = Configure.With()
                .DefaultBuilder()
                .DefiningMessagesAs(x => x != null && x.Name == "MyMessage")
                .DefineEndpointName("SampleV4")
                .UseTransport<Msmq>();

            var bus = configure
                .UnicastBus()
                .CreateBus()
                .Start(() => Configure.Instance.ForInstallationOn<Windows>().Install());

            Console.WriteLine("Bus started");
            Console.ReadLine();
        }
    }

    public class Handler : IHandleMessages<MyMessage>
    {
        public void Handle(MyMessage message)
        {
            Console.WriteLine(message.Property);
        }
    }
}
