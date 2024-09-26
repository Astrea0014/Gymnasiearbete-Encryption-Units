﻿using System.IO.Pipes;
using Unit_X_Common.Unit1;

/* --- Unit 1 Server Side ---
 * The server side of a local area network accessible intraprocess communication pipeline.
 * For Unit 1, the server-client communication has no guarantee for data authenticity or privacy.
 */

using (NamedPipeServerStream pipe = new(Unit.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
{
    Console.WriteLine($"Created instance of named pipe '{Unit.PipeName}'");

    pipe.WaitForConnection();                           // Waiting for a client to connect.

    Console.WriteLine($"Connected to pipe client. Awaiting contact...");

    CancellationTokenSource cts = new();
    CancellationToken ct = cts.Token;

    Task readTask = Task.Run(async () =>
    {
        ct.ThrowIfCancellationRequested();

        while (true)
        {
            byte[] bytes = new byte[Message.MaxSize];
            int size = await pipe.ReadAsync(bytes, ct);     // Call to PipeStream.ReadAsync is awaited until 
                                                            // either a message is sent through the pipe, or
                                                            // the operation is cancelled.

            Message message = Message.FromBytes(bytes, size);

            Console.WriteLine("Incoming message: " + Unit.TextEncoding.GetString(message.Payload));

            switch (message.MessageType)
            {
                case MessageType.Greeting:
                    Console.WriteLine("Greeting the client back...");
                    await pipe.WriteAsync(new Message()
                    {
                        MessageType = MessageType.ServerResponse,
                        Payload = Unit.TextEncoding.GetBytes("Hello from Server!")
                    }.ToBytes(), ct);
                    break;
                case MessageType.Secret:
                    Console.WriteLine("Updating the client about their requested task...");
                    await pipe.WriteAsync(new Message()
                    {
                        MessageType = MessageType.ServerResponse,
                        Payload = Unit.TextEncoding.GetBytes("Widthdrew $10 from your bank account with password: 1234")
                    }.ToBytes(), ct);
                    break;
            }
        }
    });

    while (pipe.IsConnected && !readTask.IsCompleted) ;     // Wait for the pipe client to disconnect or for the task to stop.

    cts.Cancel();                                           // Cancel read task in case the pipe for redundancy.

    try { await readTask; }                                 // Propagate any pending exceptions thrown in the task by awaiting it.
    catch { }
    
    cts.Dispose();
    pipe.Disconnect();
}

// Resolve warning CA1416:
// > This call site is reachable on all platforms. 'PipeTransmissionMode.Message' is supported on: 'windows'.
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static partial class Program { }