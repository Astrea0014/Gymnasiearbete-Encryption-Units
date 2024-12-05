using Unit_X_Common;
using Unit_X_Common.Unit2;

/* --- Unit 2 Client Side ---
 * The client side of a local area network accessible intraprocess communication pipeline.
 * For Unit 2, the server-client communication is ciphered using a basic Caesar-cipher but features no guarantee for data authenticity.
 */

new Unit2ClientRuntime().Start(Unit.ServerName, Unit.PipeName);
class Unit2ClientRuntime : ClientRuntime<Message>
{
    public override Message GetGreetingMessage(string message) => new()
    {
        MessageType = MessageType.Greeting,
        Payload = Unit.TextEncoding.GetBytes(message)
    };

    public override Message GetSecretMessage(string message) => new()
    {
        MessageType = MessageType.Secret,
        Payload = Unit.TextEncoding.GetBytes(Unit.Cipher(message, Unit.CipherShift))
    };

    public override void ProcessGreetingResponseMessage(Message message) => Console.WriteLine("Response: " + Unit.TextEncoding.GetString(message.Payload));

    public override void ProcessSecretResponseMessage(Message message)
    {
        string decoded = Unit.TextEncoding.GetString(message.Payload);
        Console.WriteLine("Response (ciphered): " + decoded);
        Console.WriteLine("Response (deciphered): " + Unit.Decipher(decoded, Unit.CipherShift));
    }
}