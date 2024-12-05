using Unit_X_Common;
using Unit_X_Common.Unit2;

/* --- Unit 2 Server Side ---
 * The server side of a local area network accessible intraprocess communication pipeline.
 * For Unit 2, the server-client communication is ciphered using a basic Caesar-cipher but features no guarantee for data authenticity.
 */

new Unit2ServerRuntime().Start(Unit.PipeName);
class Unit2ServerRuntime : ServerRuntime<Message>
{
    public override Message GetGreetingResponseMessage(string message) => new()
    {
        MessageType = MessageType.ServerResponse,
        Payload = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretResponseMessage(Message incoming, string outgoing)
    {
        Console.WriteLine("Incoming message (deciphered): " + Unit.Decipher(Unit.TextEncoding.GetString(incoming.Payload), Unit.CipherShift));

        return new Message()
        {
            MessageType = MessageType.ServerResponse,
            Payload = Unit.TextEncoding.GetBytes(Unit.Cipher(outgoing, Unit.CipherShift))
        };
    }

    public override void LogIncomingMessage(Message message) => Console.WriteLine("Incoming message: " + Unit.TextEncoding.GetString(message.Payload));
}

// Resolve warning CA1416:
// > This call site is reachable on all platforms. 'ServerRuntime<Message>.Start(string)' is supported on: 'windows'.
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static partial class Program { }