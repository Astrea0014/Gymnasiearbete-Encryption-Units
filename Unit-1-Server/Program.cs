using Unit_X_Common;
using Unit_X_Common.Unit1;

/* --- Unit 1 Server Side ---
 * The server side of a local area network accessible intraprocess communication pipeline.
 * For Unit 1, the server-client communication has no guarantee for data authenticity or privacy.
 */

new Unit1ServerRuntime().Start(Unit.PipeName);
class Unit1ServerRuntime : ServerRuntime<Message>
{
    public override Message GetGreetingResponseMessage(string message) => new()
    {
        MessageType = MessageType.ServerResponse,
        Payload = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretResponseMessage(Message incoming, string outgoing) => new()
    {
        MessageType = MessageType.ServerResponse,
        Payload = Unit.TextEncoding.GetBytes(outgoing)
    };

    public override void LogIncomingMessage(Message message) => Console.WriteLine("Incoming message: " + Unit.TextEncoding.GetString(message.Payload));
}

// Resolve warning CA1416:
// > This call site is reachable on all platforms. 'ServerRuntime<Message>.Start(string)' is supported on: 'windows'.
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static partial class Program { }