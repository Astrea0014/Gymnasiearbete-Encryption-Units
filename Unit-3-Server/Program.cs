using Unit_X_Common;
using Unit_X_Common.Unit3;

/* --- Unit 3 Server Side ---
 * The server side of a local area network accessible intraprocess communication pipeline.
 * For Unit 3, the server-client communication is encrypted using a symmetric encryption algorithm (AES) but features no guarantee for data authenticity.
 */

new Unit3ServerRuntime().Start(Unit.PipeName);
class Unit3ServerRuntime : ServerRuntime<Message>
{
    public override Message GetGreetingResponseMessage(string message) => new()
    {
        MessageType = MessageType.ServerResponse,
        EncryptedLength = 0,
        Encrypted = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretResponseMessage(Message incoming, string outgoing)
    {
        Console.WriteLine("Incoming message (decrypted): " + Unit.DecryptString(incoming.Encrypted, incoming.EncryptedLength));
        byte[] encrypted = Unit.EncryptString(outgoing);

        return new Message()
        {
            MessageType = MessageType.ServerResponse,
            EncryptedLength = encrypted.Length,
            Encrypted = encrypted
        };
    }

    public override void LogIncomingMessage(Message message)
    {
        string msg;

        if (message.EncryptedLength == 0)
            msg = Unit.TextEncoding.GetString(message.Encrypted);
        else
            msg = UnitX.FormatByteArray(message.Encrypted, 0, message.EncryptedLength);

        Console.WriteLine("Incoming message: " + msg);
    }
}

// Resolve warning CA1416:
// > This call site is reachable on all platforms. 'ServerRuntime<Message>.Start(string)' is supported on: 'windows'.
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static partial class Program { }