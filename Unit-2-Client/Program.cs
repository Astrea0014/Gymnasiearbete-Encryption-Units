using System.IO.Pipes;
using Unit_X_Common.Unit2;

/* --- Unit 2 Client Side ---
 * The client side of a local area network accessible intraprocess communication pipeline.
 * For Unit 2, the server-client communication is ciphered using a basic Caesar-cipher but features no guarantee for data authenticity.
 */

using (NamedPipeClientStream pipe = new(Unit.ServerName, Unit.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
{
    pipe.Connect();

    Console.WriteLine("Connected to pipe server.");

    // Greeting the server and looking for a response.
    {
        Console.WriteLine("Sending greeting...");

        await pipe.WriteAsync(new Message()
        {
            MessageType = MessageType.Greeting,
            Payload = Unit.TextEncoding.GetBytes("Hello from Client!")
        }.ToBytes());

        byte[] bytes = new byte[Message.MaxSize];
        int size = await pipe.ReadAsync(bytes);

        Message message = Message.FromBytes(bytes, size);
        Console.WriteLine("Response: " + Unit.TextEncoding.GetString(message.Payload));
    }

    // Sending the client's secret message and looking for a response.
    {
        Console.WriteLine("Sending secret...");

        // Obfuscating the message using the Caesar-cypher.
        string secret = Unit.Cipher("Widthdraw $10 from my bank account. Bank password: 1234", Unit.CipherShift);

        await pipe.WriteAsync(new Message()
        {
            MessageType = MessageType.Secret,
            Payload = Unit.TextEncoding.GetBytes(secret)
        }.ToBytes());

        byte[] bytes = new byte[Message.MaxSize];
        int size = await pipe.ReadAsync(bytes);

        Message message = Message.FromBytes(bytes, size);
        string responseString = Unit.TextEncoding.GetString(message.Payload);
        Console.WriteLine("Response (ciphered): " + responseString);
        Console.WriteLine("Response (deciphered): " + Unit.Decipher(responseString, Unit.CipherShift));
    }

    Console.WriteLine("Press any key to disconnect pipe...");
    Console.ReadKey(); // Waiting for a key to be pressed.
}

// Once the client stream runs out of scope, its Dispose()
// method will be called and the connection between the
// pipe and the server will be severed.