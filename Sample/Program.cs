using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using NServiceBus;
using NServiceBus.MessageInterfaces;
using NServiceBus.MessageMutator;
using NServiceBus.Serialization;
using NServiceBus.Settings;
using JsonSerializer = NServiceBus.JsonSerializer;

class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.Msmq.Simple";

        var endpointConfiguration = new EndpointConfiguration("Samples.Msmq.Simple");

        endpointConfiguration.Conventions().DefiningCommandsAs(x => x != null && x.Name == "MyMessage");
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        endpointConfiguration.UseSerialization<CustomJsonSerializer>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        var myMessage = new MyMessage { Property = "Foo" };
        await endpointInstance.Send("SampleV4", myMessage)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    public class CustomJsonSerializer : SerializationDefinition
    {
        public override Func<IMessageMapper, IMessageSerializer> Configure(ReadOnlySettings settings)
        {
            var xmlSerializerDefinition = new XmlSerializer();
            var xmlSerializerFactory = xmlSerializerDefinition.Configure(settings);

            var jsonSerializerDefinition = new JsonSerializer();
            var jsonSerializerFactory = jsonSerializerDefinition.Configure(settings);
            return mapper => new DecoratorSerializer(xmlSerializerFactory(mapper), jsonSerializerFactory(mapper));
        }
    }

    class DecoratorSerializer : IMessageSerializer
    {
        IMessageSerializer xmlSerializer;
        IMessageSerializer jsonSerializer;

        public DecoratorSerializer(IMessageSerializer xmlSerializer, IMessageSerializer jsonSerializer)
        {
            this.xmlSerializer = xmlSerializer;
            this.jsonSerializer = jsonSerializer;
        }

        public void Serialize(object message, Stream stream)
        {
            if (message.GetType() == typeof(MyMessage))
            {
                xmlSerializer.Serialize(message, stream);
            }
            else
            {
                jsonSerializer.Serialize(message, stream);
            }
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes = null)
        {
            return jsonSerializer.Deserialize(stream, messageTypes);
        }

        public string ContentType
        {
            get { return jsonSerializer.ContentType; }
        }
    }
}