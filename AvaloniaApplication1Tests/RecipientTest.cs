using CommunityToolkit.Mvvm.Messaging;
using Xunit;
using Xunit.Abstractions;

namespace AvaloniaApplication1Tests;

public class RecipientTest(ITestOutputHelper testOutput)
{
    [Fact]
    public void SendTest()
    {
        var recipient = "Hiep";
        IMessenger messenger = WeakReferenceMessenger.Default;
        messenger.Register(recipient, new MessageHandler<object, string>((recip, message) =>
        {
            testOutput.WriteLine($"Recipient '{recip}' received message: '{message}'");
        }));
        messenger.Register(new StringRecipient(testOutput), "min");
        messenger.Send("hi");
        messenger.Send("toto", "min");
    }

    internal class StringRecipient(ITestOutputHelper testOutput) : IRecipient<string>
    {
        public void Receive(string message)
        {
            testOutput.WriteLine($"'String Recipient' received message: '{message}'");
        }
    }
}
