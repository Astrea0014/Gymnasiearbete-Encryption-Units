using Unit_X_Common;
using Unit_X_Common.Unit4;

using System.Security.Cryptography;

new Unit4ClientRuntime().Start(Unit.ServerName, Unit.PipeName);
class Unit4ClientRuntime : ClientRuntime<Message>
{
    public RsaKeypair Keypair { get; }
    public RsaKeypair ServerKeypair { get; set; }

    public Unit4ClientRuntime()
    {
        using RSA rsa = RSA.Create();

        Keypair = new()
        {
            Public = rsa.ExportRSAPublicKey(),
            Private = rsa.ExportRSAPrivateKey()
        };

        ServerKeypair = new RsaKeypair();
    }

    public override Message GetGreetingMessage(string message) => new()
    {
        MessageType = MessageType.Greeting,
        RSAPublicKeyLength = Keypair.Public.Length,
        Security = Keypair.Public,
        Encoded = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretMessage(string message)
    {
        byte[] encoded = Unit.TextEncoding.GetBytes(message);

        return new Message()
        {
            MessageType = MessageType.Secret,
            RSAPublicKeyLength = 0,
            Security = Unit.SignString(encoded, Keypair),
            Encoded = encoded
        };
    }

    public override void ProcessGreetingResponseMessage(Message message)
    {
        Console.WriteLine("Response: " + Unit.TextEncoding.GetString(message.Encoded));

        if (message.RSAPublicKeyLength != 0)
        {
            Console.WriteLine("Server reported RSA public key. Storing key...");
            ServerKeypair.Public = message.Security;
        }
        else Console.WriteLine("Server reported no RSA public key, this is not intended behavior");
    }

    public override void ProcessSecretResponseMessage(Message message)
    {
        Console.WriteLine("Response: " + Unit.TextEncoding.GetString(message.Encoded));

        if (message.RSAPublicKeyLength == 0)
        {
            Console.WriteLine($"Response message signed.");

            if (Unit.VerifyString(message.Encoded, message.Security, ServerKeypair))
                Console.WriteLine("Signature VALID.");
            else
                Console.WriteLine("Signature NOT VALID.");
        }
        else Console.WriteLine("Server response contains an RSA public key, this is not intended behavior");
    }
}