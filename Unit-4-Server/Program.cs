using Unit_X_Common;
using Unit_X_Common.Unit4;

using System.Security.Cryptography;

/* --- Unit 4 Server Side ---
 * The server side of a local area network accessible intraprocess communication pipeline.
 * For Unit 4, the server-client communication is unencrypted and unprotected against unauthorized access,
 * but features strong data authenticity by using digital signatures created using an asymmetric algorithm such that
 * the sender can sign its payload using their private key, and the receiver can verify the authenticity of the data using
 * the corresponding public key.
 */

new Unit4ServerRuntime().Start(Unit.PipeName);
class Unit4ServerRuntime : ServerRuntime<Message>
{
    public RsaKeypair Keypair { get; }
    public RsaKeypair ClientKeypair { get; set; }

    public Unit4ServerRuntime()
    {
        using RSA rsa = RSA.Create();

        Keypair = new()
        {
            Public = rsa.ExportRSAPublicKey(),
            Private = rsa.ExportRSAPrivateKey()
        };

        ClientKeypair = new RsaKeypair();
    }

    public override Message GetGreetingResponseMessage(string message) => new()
    {
        MessageType = MessageType.ServerResponse,
        RSAPublicKeyLength = Keypair.Public.Length,
        Security = Keypair.Public,
        Encoded = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretResponseMessage(Message incoming, string outgoing)
    {
        byte[] encoded = Unit.TextEncoding.GetBytes(outgoing);

        return new Message()
        {
            MessageType = MessageType.ServerResponse,
            RSAPublicKeyLength = 0,
            Security = Unit.SignString(encoded, Keypair),
            Encoded = encoded
        };
    }

    public override void LogIncomingMessage(Message message)
    {
        if (message.MessageType == MessageType.Greeting)
        {
            Console.WriteLine("Incoming message: " + Unit.TextEncoding.GetString(message.Encoded));
            Console.WriteLine("Client public key reported as: " + UnitX.FormatByteArray(message.Security, 0, message.RSAPublicKeyLength));
            ClientKeypair = new() { Public = message.Security };
        }

        else if (message.MessageType == MessageType.Secret)
        {
            Console.WriteLine("Incoming message: " + Unit.TextEncoding.GetString(message.Encoded));

            if (Unit.VerifyString(message.Encoded, message.Security, ClientKeypair))
                Console.WriteLine("Signature verified using client's reported public key.");
            else
                Console.WriteLine("Signature could not be verified. Aborting...");
        }

        else
            Console.WriteLine("Incoming message's message type is invalid...");
    }
}

// Resolve warning CA1416:
// > This call site is reachable on all platforms. 'ServerRuntime<Message>.Start(string)' is supported on: 'windows'.
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static partial class Program { }