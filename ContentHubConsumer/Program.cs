using Microsoft.Azure.ServiceBus;
using System.Text;

var connectionString = "Connection string can be taken from Share Access Policy of the topic in Azure Service bus";
var subscriptionName = "the name of the subscription";

var subscriptionClient = new SubscriptionClient(new ServiceBusConnectionStringBuilder(connectionString), subscriptionName);

Task MessageHandler(Message message)
{
    string body = Encoding.UTF8.GetString(message.Body);
    Console.WriteLine($"Received: {body}");

    return Task.CompletedTask;
}

Task ErrorHandler(ExceptionReceivedEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

try
{
    subscriptionClient.RegisterMessageHandler((message, cancellationToken) => MessageHandler(message), new MessageHandlerOptions(eventArgs => ErrorHandler(eventArgs)));
    
    Console.WriteLine("Listening");
    Console.ReadKey();
} finally
{
    await subscriptionClient.CloseAsync();
}