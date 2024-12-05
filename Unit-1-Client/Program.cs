using Unit_X_Common;
using Unit_X_Common.Unit1;

/* --- Unit 1 Client Side ---
 * The client side of a local area network accessible intraprocess communication pipeline.
 * For Unit 1, the server-client communication has no guarantee for data authenticity or privacy.
 */

new Unit1ClientRuntime().Start(Unit.ServerName, Unit.PipeName);
class Unit1ClientRuntime : ClientRuntime<Message>
{
    public override Message GetGreetingMessage(string message) => new()
    {
        MessageType = MessageType.Greeting,
        Payload = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretMessage(string message) => new()
    {
        MessageType = MessageType.Secret,
        Payload = Unit.TextEncoding.GetBytes(message)
    };

    public override void ProcessGreetingResponseMessage(Message message) => Console.WriteLine("Response: " + Unit.TextEncoding.GetString(message.Payload));

    public override void ProcessSecretResponseMessage(Message message) => Console.WriteLine("Response: " + Unit.TextEncoding.GetString(message.Payload));
}