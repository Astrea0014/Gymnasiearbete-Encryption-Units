using Unit_X_Common;
using Unit_X_Common.Unit3;

/* --- Unit 3 Client Side ---
 * The client side of a local area network accessible intraprocess communication pipeline.
 * For Unit 3, the server-client communication is encrypted using a symmetric encryption algorithm (AES) but features no guarantee for data authenticity.
 */

new Unit3ClientRuntime().Start(Unit.ServerName, Unit.PipeName);
class Unit3ClientRuntime : ClientRuntime<Message>
{
    public override Message GetGreetingMessage(string message) => new()
    {
        MessageType = MessageType.Greeting,
        EncryptedLength = 0,
        Encrypted = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretMessage(string message)
    {
        byte[] encrypted = Unit.EncryptString(message);

        return new Message()
        {
            MessageType = MessageType.Secret,
            EncryptedLength = encrypted.Length,
            Encrypted = encrypted
        };
    }

    public override void ProcessGreetingResponseMessage(Message message) => Console.WriteLine("Response: " + Unit.TextEncoding.GetString(message.Encrypted));

    public override void ProcessSecretResponseMessage(Message message)
    {
        Console.WriteLine("Response (encrypted): " + UnitX.FormatByteArray(message.Encrypted, 0, message.EncryptedLength));
        Console.WriteLine("Response (decrypted): " + Unit.DecryptString(message.Encrypted, message.EncryptedLength));
    }
}